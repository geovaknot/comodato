SET IDENTITY_INSERT dbo.tbVisitaPadrao ON;  
GO 
insert into tbVisitaPadrao
	(ID_VISITA, DT_DATA_VISITA, ST_STATUS_VISITA, CD_CLIENTE, CD_TECNICO, DS_OBSERVACAO, HR_INICIO, HR_FIM, CD_MOTIVO_VISITA, nidUsuarioAtualizacao, dtmDataHoraAtualizacao, TOKEN)
select VisitaAntiga.ID_VISITA, 
	   (case
			when (select top 1 LogOS.DT_DATA_LOG_VISITA from tbLogStatusVisita LogOS where 
				LogOs.ID_VISITA = VisitaAntiga.ID_VISITA 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 1 or LogOS.ID_LOG_STATUS_VISITA = 2 or LogOS.ID_LOG_STATUS_VISITA = 5) 
				and LogOS.DT_DATA_LOG_VISITA is not null
				order by LogOS.DT_DATA_LOG_VISITA desc) is not null
				then 
					(select top 1 LogOS.DT_DATA_LOG_VISITA from tbLogStatusVisita LogOS where 
				LogOs.ID_VISITA = VisitaAntiga.ID_VISITA 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 1 or LogOS.ID_LOG_STATUS_VISITA = 2 or LogOS.ID_LOG_STATUS_VISITA = 5) 
				and LogOS.DT_DATA_LOG_VISITA is not null
				order by LogOS.DT_DATA_LOG_VISITA desc)
			when (select top 1 LogOS.DT_DATA_LOG_VISITA from tbLogStatusVisita LogOS where 
				LogOs.ID_VISITA = VisitaAntiga.ID_VISITA 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 1 or LogOS.ID_LOG_STATUS_VISITA = 2 or LogOS.ID_LOG_STATUS_VISITA = 5) 
				and LogOS.DT_DATA_LOG_VISITA is not null
				order by LogOS.DT_DATA_LOG_VISITA desc) is null
				then 
					VisitaAntiga.DT_DATA_VISITA
	   end), 
	   (case 
			when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 1
				then 3
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
		VisitaAntiga.CD_CLIENTE, VisitaAntiga.CD_TECNICO, VisitaAntiga.DS_OBSERVACAO, 
		(select top 1 Convert(varchar(5), 
			FORMAT(LogOS.DT_DATA_LOG_VISITA,'HH:mm')) from tbLogStatusVisita LogOS where 
				LogOs.ID_VISITA = VisitaAntiga.ID_VISITA 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 1 or LogOS.ID_LOG_STATUS_VISITA = 2 or LogOS.ID_LOG_STATUS_VISITA = 5) 
				and LogOS.DT_DATA_LOG_VISITA is not null
				order by LogOS.DT_DATA_LOG_VISITA desc),
		(select top 1 Convert(varchar(5), 
			FORMAT(LogOS.DT_DATA_LOG_VISITA,'HH:mm')) from tbLogStatusVisita LogOS where 
				LogOs.ID_VISITA = VisitaAntiga.ID_VISITA 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 3 or LogOS.ID_LOG_STATUS_VISITA = 6)
				and LogOS.DT_DATA_LOG_VISITA is not null
				order by LogOS.DT_DATA_LOG_VISITA desc),
	   (case when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 8 
				then 1
			 when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 9
				then 1
			 when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 10
				then 2
			 when VisitaAntiga.ST_TP_STATUS_VISITA_OS = 11
				then 3
			 when VisitaAntiga.ST_TP_STATUS_VISITA_OS not in (8, 9, 10, 11)
				then 4
		end), 
	   
	   VisitaAntiga.nidUsuarioAtualizacao, VisitaAntiga.dtmDataHoraAtualizacao,
	   VisitaAntiga.ID_VISITA
from tbVisita VisitaAntiga

--delete from tbVisitaPadrao

