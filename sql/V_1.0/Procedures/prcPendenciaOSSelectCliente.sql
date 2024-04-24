GO
/****** Object:  StoredProcedure [dbo].[prcPendenciaOSSelectCliente]    Script Date: 19/07/2021 13:35:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Trazer todas as Pendências do Cliente com Status "1-Pendente"
-- junto com todas as Pendências da OS com qualquer Status
-- =============================================
ALTER PROCEDURE [dbo].[prcPendenciaOSSelectCliente]
	@p_CD_CLIENTE					NUMERIC(6,0),
	@p_ID_OS						BIGINT,
	@p_CD_TECNICO					BIGINT

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
		
		SELECT 
			dbo.tbPendenciaOS.*,
			dbo.tbPendenciaOS.ID_OS [PENDENCIA_OS],
			dbo.TB_PECA.DS_PECA,
			dbo.TB_PECA.TX_UNIDADE,
			dbo.tbOSPadrao.CD_CLIENTE
			
		FROM dbo.tbPendenciaOS
			INNER JOIN tbOSPadrao ON tbPendenciaOS.ID_OS = tbOSPadrao.ID_OS
			--INNER JOIN tbVisita ON tbOS.ID_VISITA = tbVisita.ID_VISITA
			INNER JOIN dbo.TB_TECNICO_CLIENTE ON dbo.tbOSPadrao.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
			LEFT OUTER JOIN dbo.TB_PECA ON dbo.tbPendenciaOS.CD_PECA = dbo.TB_PECA.CD_PECA
		WHERE
			TB_TECNICO_CLIENTE.CD_CLIENTE			= @p_CD_CLIENTE
		AND dbo.tbPendenciaOS.ST_STATUS_PENDENCIA	= 1 --Pendente
		AND dbo.TB_TECNICO_CLIENTE.CD_CLIENTE = @p_CD_CLIENTE
		AND dbo.tbPendenciaOS.CD_TECNICO = @p_CD_TECNICO
		UNION 
		SELECT 
			dbo.tbPendenciaOS.*,
			dbo.tbPendenciaOS.ID_OS [PENDENCIA_OS],
			dbo.TB_PECA.DS_PECA,
			dbo.TB_PECA.TX_UNIDADE,
			dbo.tbOSPadrao.CD_CLIENTE
			
		FROM dbo.tbPendenciaOS
			left JOIN tbOSPadrao ON tbPendenciaOS.ID_OS = tbOSPadrao.ID_OS
			--left JOIN tbVisita ON tbOS.ID_VISITA = tbVisita.ID_VISITA
			left JOIN dbo.TB_TECNICO_CLIENTE ON dbo.tbOSPadrao.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
			LEFT OUTER JOIN dbo.TB_PECA ON dbo.tbPendenciaOS.CD_PECA = dbo.TB_PECA.CD_PECA
		WHERE 
			dbo.tbOSPadrao.CD_CLIENTE = @p_CD_CLIENTE
			and dbo.tbPendenciaOS.CD_TECNICO = @p_CD_TECNICO
			--AND dbo.tbPendenciaOS.ST_STATUS_PENDENCIA	= 1 --Pendente
		ORDER BY
			ID_OS DESC,      
			ID_PENDENCIA_OS			 
		
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


