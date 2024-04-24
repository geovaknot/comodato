SET IDENTITY_INSERT dbo.tbOSPadrao ON;  
GO 
insert into tbOSPadrao
	(ID_OS, DT_DATA_OS, ST_STATUS_OS, CD_TIPO_OS, CD_CLIENTE, CD_TECNICO, HR_INICIO, HR_FIM, nidUsuarioAtualizacao, dtmDataHoraAtualizacao, CD_ATIVO_FIXO, token)
select OsAntiga.ID_OS, 
	   (case
			when (select top 1 LogOS.DT_DATA_LOG_OS from tbLogStatusOS LogOS where 
				LogOs.ID_OS = OsAntiga.Id_Os 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 2 or LogOS.ID_LOG_STATUS_OS = 3) 
				and LogOS.DT_DATA_LOG_OS is not null
				order by LogOS.DT_DATA_LOG_OS desc) is not null
				then 
					(select top 1 LogOS.DT_DATA_LOG_OS from tbLogStatusOS LogOS where 
				LogOs.ID_OS = OsAntiga.Id_Os 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 2 or LogOS.ID_LOG_STATUS_OS = 3) 
				and LogOS.DT_DATA_LOG_OS is not null
				order by LogOS.DT_DATA_LOG_OS desc)
			when (select top 1 LogOS.DT_DATA_LOG_OS from tbLogStatusOS LogOS where 
				LogOs.ID_OS = OsAntiga.Id_Os 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 2 or LogOS.ID_LOG_STATUS_OS = 3) 
				and LogOS.DT_DATA_LOG_OS is not null
				order by LogOS.DT_DATA_LOG_OS desc) is null
				then 
					OsAntiga.DT_DATA_ABERTURA
	   end), 
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
	   (case when OsAntiga.TP_MANUTENCAO = 'P' 
				then 1
			 when OsAntiga.TP_MANUTENCAO = 'C'
				then 2
			 when OsAntiga.TP_MANUTENCAO = 'I'
				then 3
			 when OsAntiga.TP_MANUTENCAO = 'O'
				then 4
			 when OsAntiga.TP_MANUTENCAO = 'T'
				then 4
		end), 
	   (select CD_CLIENTE from tbVisita Visita where OsAntiga.ID_VISITA = Visita.ID_VISITA), OsAntiga.CD_TECNICO, 
	   (select top 1 Convert(varchar(5), 
			FORMAT(LogOS.DT_DATA_LOG_OS,'HH:mm')) from tbLogStatusOS LogOS where 
				LogOs.ID_OS = OsAntiga.Id_Os 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 2 or LogOS.ID_LOG_STATUS_OS = 3) 
				and LogOS.DT_DATA_LOG_OS is not null
				order by LogOS.DT_DATA_LOG_OS desc),
	   (select top 1 Convert(varchar(5), 
			FORMAT(LogOS.DT_DATA_LOG_OS,'HH:mm')) from tbLogStatusOS LogOS where 
				LogOs.ID_OS = OsAntiga.Id_Os 
				and (LogOS.ST_TP_STATUS_VISITA_OS = 4 or LogOS.ID_LOG_STATUS_OS = 5)
				and LogOS.DT_DATA_LOG_OS is not null
				order by LogOS.DT_DATA_LOG_OS desc),
	   OsAntiga.nidUsuarioAtualizacao, OsAntiga.dtmDataHoraAtualizacao, OsAntiga.CD_ATIVO_FIXO,
	   OsAntiga.ID_OS
from tbOS OsAntiga


--delete from tbOSPadrao

--select * from tbOSPadrao
