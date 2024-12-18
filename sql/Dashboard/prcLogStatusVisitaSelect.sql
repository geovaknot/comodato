GO
/****** Object:  StoredProcedure [dbo].[prcLogStatusVisitaSelect]    Script Date: 13/05/2022 11:59:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Seleção de dados na tabela 
--              tbLogStatusOSPadrao
-- =============================================
ALTER PROCEDURE [dbo].[prcLogStatusVisitaSelect]
	@p_ID_LOG_STATUS_VISITA			BIGINT			= NULL,
	@p_ID_VISITA					BIGINT			= NULL,
	@p_DT_DATA_LOG_VISITA			DATETIME		= NULL,	
	@p_ST_TP_STATUS_VISITA_OS		INT				= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		SELECT	dbo.tbLogStatusOSPadrao.*,
				dbo.tbTpStatusOSPadrao.DS_STATUS_OS,
				dbo.tbUsuario.cnmNome
		FROM	dbo.tbLogStatusOSPadrao
		INNER JOIN dbo.tbTpStatusOSPadrao
			ON dbo.tbLogStatusOSPadrao.ST_STATUS_OS = dbo.tbTpStatusOSPadrao.ST_STATUS_OS
		LEFT OUTER JOIN dbo.tbUsuario
			ON dbo.tbLogStatusOSPadrao.nidUsuarioAtualizacao = tbUsuario.nidUsuario
		WHERE (	dbo.tbLogStatusOSPadrao.ID_LOG_STATUS_OS		= @p_ID_LOG_STATUS_VISITA		OR @p_ID_LOG_STATUS_VISITA		IS NULL )
		AND	  (	dbo.tbLogStatusOSPadrao.ID_OS					= @p_ID_VISITA					OR @p_ID_VISITA					IS NULL )
		AND	  (	dbo.tbLogStatusOSPadrao.DT_DATA_LOG_OS		= @p_DT_DATA_LOG_VISITA			OR @p_DT_DATA_LOG_VISITA		IS NULL )
		AND   ( dbo.tbLogStatusOSPadrao.ST_STATUS_OS	= @p_ST_TP_STATUS_VISITA_OS		OR @p_ST_TP_STATUS_VISITA_OS	IS NULL )
		ORDER BY
				dbo.tbLogStatusOSPadrao.DT_DATA_LOG_OS			
		
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


--select * from tbLogStatusOSPadrao
--select * from tbTpStatusOSPadrao
--select * from tbTpStatusVisitaOS