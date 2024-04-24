--'CLI:147330'

declare @idestoqueatual numeric(6) = 678,
		@idestoqueantigo numeric(6) = 671

SELECT DISTINCT T2.*
INTO #TempAntiga
FROM tbEstoquePeca T2
	JOIN tbEstoquePeca as T1 
		ON T1.CD_PECA = T2.CD_PECA
where T2.ID_ESTOQUE = @idestoqueantigo 
	AND T1.ID_ESTOQUE = @idestoqueatual

UPDATE tAntigo
  SET tAntigo.QT_PECA_ATUAL = tAntigo.QT_PECA_ATUAL + t2.QT_PECA_ATUAL
  FROM dbo.tbEstoquePeca AS tAntigo
  INNER JOIN dbo.tbEstoquePeca AS t2
  ON tAntigo.CD_PECA = t2.CD_PECA
  WHERE tAntigo.ID_ESTOQUE = @idestoqueatual and t2.ID_ESTOQUE = @idestoqueantigo


delete from tbEstoqueMovi where ID_ESTOQUE = @idestoqueantigo

DELETE w
FROM tbEstoquePeca w
INNER JOIN #TempAntiga e
  ON e.ID_ESTOQUE = w.ID_ESTOQUE
WHERE w.CD_PECA = e.CD_PECA AND e.ID_ESTOQUE = @idestoqueantigo


insert into tbEstoquePeca
SELECT CD_PECA, QT_PECA_ATUAL, QT_PECA_MIN, DT_ULT_MOVIM, @idestoqueatual
FROM   tbestoquePeca where id_estoque = @idestoqueantigo


delete from tbEstoquePeca where ID_ESTOQUE = @idestoqueantigo

delete from tbEstoque where ID_ESTOQUE = @idestoqueantigo
drop table #TempAntiga


--#####################################################################################################################################