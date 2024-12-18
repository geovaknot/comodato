GO
/****** Object:  StoredProcedure [dbo].[prcRptAgendaVisitaSelectAtendimento]    Script Date: 29/11/2021 14:42:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcRptAgendaVisitaSelectAtendimento]
(
	@p_CDS_TECNICO				VARCHAR(MAX)	=NULL,-- '347818,347805,347801',
	@p_STS_TP_STATUS_VISITA_OS	VARCHAR(MAX)	= NULL,-- '3,4',
	@p_nidUsuario	numeric		(18),
	@pDtInicial DAtetime = NULL,--'2021-08-22 12:11:37',
	@pDtFinal DAtetime = NULL--'2021-10-29 12:11:37'
)
AS
BEGIN

	SET NOCOUNT ON;
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
		@nidErrorSeverity	INT,
		@nidErrorState		INT,
		@ID_AGENDA			BIGINT,
		@v_CD_TECNICO		VARCHAR(6),
		@v_NM_TECNICO		VARCHAR(50)

	BEGIN TRY

		Select VS.*, Cli.*, Tec.*, Stts.*, motivo.* from tbVisitaPadrao VS
		inner Join TB_CLIENTE Cli on
			VS.CD_CLIENTE = Cli.CD_CLIENTE
		inner Join TB_TECNICO Tec on
			VS.CD_TECNICO = Tec.CD_TECNICO
		inner Join tbTpStatusVisitaPadrao Stts on
			VS.ST_STATUS_VISITA = Stts.ST_STATUS_VISITA
		inner Join tbTpMotivoVisitaPadrao motivo on
			VS.CD_MOTIVO_VISITA = motivo.CD_MOTIVO_VISITA

		where 
			(@p_CDS_TECNICO IS NULL OR VS.CD_Tecnico COLLATE Latin1_General_CI_AS IN (SELECT CAST(cdsString AS VARCHAR(06)) FROM fncGetValuesByString(@p_CDS_TECNICO,',')) )
			AND (@p_STS_TP_STATUS_VISITA_OS IS NULL OR VS.ST_STATUS_VISITA IN (SELECT CAST(cdsString AS VARCHAR(06)) FROM fncGetValuesByString(@p_STS_TP_STATUS_VISITA_OS,',')) )
			AND	  (	convert(date, VS.DT_DATA_VISITA, 23) >= convert(date, @pDtInicial, 23)	OR @pDtInicial IS NULL )
			AND	  (	convert(date, VS.DT_DATA_VISITA, 23) <= convert(date, @pDtFinal, 23) OR @pDtFinal IS NULL )
			ORDER BY
				VS.DT_DATA_VISITA DESC 


	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH
END
