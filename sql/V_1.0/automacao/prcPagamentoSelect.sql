GO
/****** Object:  StoredProcedure [dbo].[prcPagamentoSelect]    Script Date: 16/12/2021 10:35:53 ******/
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
CREATE PROCEDURE [dbo].[prcPagamentoSelect]
	@p_ID_DADOS_FATURAMENTO				varchar(50)			= null
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
		
		SELECT dbo.TB_DadosPagamento.* FROM dbo.TB_DadosPagamento
		WHERE ID_DADOS_FATURAMENTO IN (SELECT CAST(cdsString AS VARCHAR(06)) FROM fncGetValuesByString(@p_ID_DADOS_FATURAMENTO,';'))
		ORDER BY DataEmissaoNF DESC
		
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


