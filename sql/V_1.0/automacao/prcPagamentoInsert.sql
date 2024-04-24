USE [COMODATODEV]
GO
/****** Object:  StoredProcedure [dbo].[prcPagamentoInsert]    Script Date: 22/11/2021 08:39:23 ******/
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
CREATE PROCEDURE [dbo].[prcPagamentoInsert]
	@p_CD_CLIENTE					VARCHAR(8)		= NULL,
	@p_NRAtivo						VARCHAR(15)		= NULL,
	@p_NRSolicitacaoSESM			VARCHAR(10)		= NULL,
	@p_DT_Solicitacao				DATETIME		= NULL,
	@p_NR_NotaFiscal				VARCHAR(2)		= NULL,
	@p_SerieNF						VARCHAR(3)		= NULL,
	@p_NRLinhaNF					NUMERIC(8,0)	= NULL,
	@p_DataEmissaoNF				DATETIME		= NULL,
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
		
		INSERT INTO dbo.TB_DadosPagamento
		(CD_Cliente, NRAtivo, NRSolicitacaoSESM, DT_Solicitacao, NR_NotaFiscal,SerieNF,NRLinhaNF, DataEmissaoNF, ID_ATIVO_CLIENTE)
		VALUES
		(@p_CD_CLIENTE, @p_NRAtivo, @p_NRSolicitacaoSESM, @p_DT_Solicitacao, @p_NR_NotaFiscal,@p_SerieNF, @p_NRLinhaNF, @p_DataEmissaoNF, @p_ID_ATIVO_CLIENTE)
		
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


