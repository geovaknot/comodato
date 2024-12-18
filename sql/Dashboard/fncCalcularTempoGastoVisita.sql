GO
/****** Object:  UserDefinedFunction [dbo].[fncCalcularTempoGastoVisita]    Script Date: 13/05/2022 14:39:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[fncCalcularTempoGastoVisita]
	(@pID_VISITA	bigint)
RETURNS FLOAT
AS
BEGIN
	declare @tempoGasto float, @tempoTotalGasto float
	declare @pDT_DATA_LOG_VISITA	DATETIME,
			@pST_TP_STATUS_VISITA_OS	int

	declare @start_date DATETIME, @end_date DATETIME

	declare crLogVisita cursor for
		select DISTINCT DT_DATA_LOG_OS AS DT_DATA_LOG_VISITA, 
						tbLogStatusOSPadrao.ST_STATUS_OS AS ST_TP_STATUS_VISITA_OS 
		from tbLogStatusOSPadrao 
			--INNER JOIN dbo.tbTpStatusVisitaOS
			--ON dbo.tbLogStatusVisita.ST_TP_STATUS_VISITA_OS = select * from dbo.tbTpStatusVisitaOS.ST_TP_STATUS_VISITA_OS
			--AND dbo.tbTpStatusVisitaOS.FL_STATUS_OS = 'N'		select * from dbo.tbLogStatusOSPadrao select * from dbo.tbtpStatusOSPadrao
		where ID_OS=@pID_VISITA 
		and tbLogStatusOSPadrao.ST_STATUS_OS != 1 
		and tbLogStatusOSPadrao.ST_STATUS_OS != 4 
		and tbLogStatusOSPadrao.ST_STATUS_OS != 5

	open crLogVisita

	fetch next from crLogVisita into @pDT_DATA_LOG_VISITA, @pST_TP_STATUS_VISITA_OS

	while (@@FETCH_STATUS = 0)
	BEGIN
		if (@pST_TP_STATUS_VISITA_OS=2) --Aberta
			begin
				set @start_date = @pDT_DATA_LOG_VISITA
				set @end_date = null
			end
		else
			begin
				set @end_date = @pDT_DATA_LOG_VISITA
			end

		if (@start_date is not null and @end_date is not null) 
		begin
			set @tempoGasto = DATEDIFF(minute, @start_date, @end_date)
			set @tempoTotalGasto = isnull(@tempoTotalGasto,0) + @tempoGasto
			
			set @start_date = null
			set @end_date = null
		end


		fetch next from crLogVisita into @pDT_DATA_LOG_VISITA, @pST_TP_STATUS_VISITA_OS
	END

	close crLogVisita
	deallocate crLogVisita

	return ISNULL(@tempoTotalGasto/60,0)
END
