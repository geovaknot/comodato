GO
/****** Object:  StoredProcedure [dbo].[prcFaturamentoSelect]    Script Date: 20/11/2021 12:20:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Consulta dos dados na tabela
--              tbPecaOS
-- =============================================
CREATE PROCEDURE [dbo].[prcFaturamentoSelect]
	@p_ID_ATIVO_CLIENTE				BIGINT			= NULL
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
		
		SELECT dbo.TB_DadosFaturamento.* FROM dbo.TB_DadosFaturamento
		WHERE ID_ATIVO_CLIENTE = @p_ID_ATIVO_CLIENTE
		ORDER BY DT_UltimoFaturamento DESC
		
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


