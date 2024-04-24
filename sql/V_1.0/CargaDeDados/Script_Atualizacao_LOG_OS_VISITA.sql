--select * from tbLogStatusOsPadrao
--select * from tbLogStatusOs

--select * from tbLogStatusVisitaPadrao
--select * from tbLogStatusVisita

--delete from tbLogStatusVisitaPadrao
--delete from tbLogStatusOsPadrao

----------------------------------------------------Att log Visita Padrao--------------------------------------------------

SET IDENTITY_INSERT dbo.tbLogStatusVisitaPadrao ON; 
insert into tbLogStatusVisitaPadrao (ID_LOG_STATUS_VISITA, ID_VISITA, DT_DATA_LOG_VISITA, ST_STATUS_VISITA, nidUsuarioAtualizacao, dtmDataHoraAtualizacao)
select VisitaAntiga.ID_LOG_STATUS_VISITA,
	   VisitaAntiga.ID_VISITA, 
	   VisitaAntiga.DT_DATA_LOG_VISITA, 
	   (case 
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 1
			then 2
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 2
			then 3
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 3
			then 4
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 4
			then 6
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 5
			then 3
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 6
			then 4
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 7
			then 5
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 8
			then 3
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 9
			then 3
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 10
			then 3
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 11
			then 3
			
	   end), 
	   VisitaAntiga.nidUsuarioAtualizacao,
	   VisitaAntiga.dtmDataHoraAtualizacao
from tbLogStatusVisita VisitaAntiga

----------------------------------------------------------------------------------------------------------------------------

---------------------------------------------------Att Log OS PADRAO--------------------------------------------------------
SET IDENTITY_INSERT dbo.tbLogStatusOsPadrao ON; 
insert into tbLogStatusOsPadrao (ID_LOG_STATUS_OS, ID_OS, DT_DATA_LOG_OS, ST_STATUS_OS, nidUsuarioAtualizacao, dtmDataHoraAtualizacao)
select OsAntiga.ID_LOG_STATUS_OS, OsAntiga.ID_OS, OsAntiga.DT_DATA_LOG_OS, 
	   (case 
			when OsAntiga.ST_TP_STATUS_VISITA_OS = 1
			then 1
			when OsAntiga.ST_TP_STATUS_VISITA_OS = 2
			then 2
			when OsAntiga.ST_TP_STATUS_VISITA_OS = 3
			then 2
			when OsAntiga.ST_TP_STATUS_VISITA_OS = 4
			then 3
			when OsAntiga.ST_TP_STATUS_VISITA_OS = 5
			then 3
			when OsAntiga.ST_TP_STATUS_VISITA_OS = 6
			then 4
			when OsAntiga.ST_TP_STATUS_VISITA_OS = 7
			then 5
			
			
	   end),
	    
	   OsAntiga.nidUsuarioAtualizacao,
	   OsAntiga.dtmDataHoraAtualizacao
from tbLogStatusOs OsAntiga

----------------------------------------------------------------------------------------------------------------------------

