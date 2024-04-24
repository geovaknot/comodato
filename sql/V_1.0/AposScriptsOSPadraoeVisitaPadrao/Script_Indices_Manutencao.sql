DECLARE @tableName nvarchar(500) 
DECLARE @indexName nvarchar(500) 
DECLARE @percentFragment decimal(11,2) 
DECLARE @page_count int
 
DECLARE FragmentedTableList cursor for 
SELECT dbtables.[name] AS 'Table',
 dbindexes.[name] AS 'Index',
 indexstats.avg_fragmentation_in_percent,
  indexstats.page_count
 FROM sys.dm_db_index_physical_stats (DB_ID(), NULL, NULL, NULL, NULL) AS indexstats
 INNER JOIN sys.tables dbtables 
 ON dbtables.[object_id] = indexstats.[object_id]
 INNER JOIN sys.schemas dbschemas 
 ON dbtables.[schema_id] = dbschemas.[schema_id]
 INNER JOIN sys.indexes dbindexes 
 ON dbindexes.[object_id] = indexstats.[object_id]
  AND indexstats.index_id = dbindexes.index_id
  AND dbindexes.[name] IS NOT NULL
 WHERE indexstats.database_id = DB_ID()
  AND indexstats.avg_fragmentation_in_percent > 05
  AND indexstats.page_count > 10 
 ORDER BY indexstats.page_count DESC, indexstats.avg_fragmentation_in_percent DESC
  
 OPEN FragmentedTableList 
 FETCH NEXT FROM FragmentedTableList 
 INTO @tableName, @indexName, @percentFragment, @page_count
  
 WHILE @@FETCH_STATUS = 0 
 BEGIN 
 PRINT 'Processando ' + @indexName + ' na tabela ' + @tableName + ' com ' + CAST(@percentFragment AS NVARCHAR(50)) + '% fragmentado' 
 
 IF(@percentFragment BETWEEN 05 AND 30) 
 BEGIN 
 
 EXEC( 'ALTER INDEX ' + @indexName + ' ON ' + @tableName + ' REORGANIZE;') 
 PRINT 'Concluindo a reorganização do índice ' + @indexName + ' da tabela ' + @tableName 
 END 
 ELSE IF (@percentFragment > 30)
  BEGIN 
 EXEC( 'ALTER INDEX ' + @indexName + ' ON ' + @tableName + ' REBUILD; ') 
 PRINT 'Concluindo a recriação do índice ' + @indexName + 'da tabela ' + @tableName 
 END 
 FETCH NEXT FROM FragmentedTableList 
 INTO @tableName, @indexName, @percentFragment,@page_count
  END 
 CLOSE FragmentedTableList 
DEALLOCATE FragmentedTableList