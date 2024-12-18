GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaInsertDados]    Script Date: 23/08/2022 15:06:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Inclusão dos dados na tabela
--              TB_PEDIDO_PECA
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoPecaInsertDados]	
	@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_CD_PECA							VARCHAR(15)		= NULL,
	@p_QTD_SOLICITADA					NUMERIC(15,3)	= NULL,
	@p_QTD_APROVADA						NUMERIC(15,3)	= NULL,
	@p_ID_ESTOQUE_DEBITO				BIGINT			= NULL,
	@p_ID_ESTOQUE_DEBITO_3M2			BIGINT			= NULL,
	@p_QTD_APROVADA_3M1					NUMERIC(15,3)	= NULL,
	@p_QTD_APROVADA_3M2					NUMERIC(15,3)	= NULL,
	@p_ID_ITEM_PEDIDO					NUMERIC(9,0)	= NULL,
	@p_RAMAL							NUMERIC(9,0)	= NULL,
	@p_VOLUME							NUMERIC(9,0)	= NULL,
	@p_PESOBRUTO						NUMERIC(9,0)	= NULL,
	@p_PESOLIQUIDO						NUMERIC(9,0)	= NULL,
	@p_TELEFONE							VARCHAR(12)		= NULL,
	@p_DS_APROVADOR						VARCHAR(50)		= NULL,
	@p_DS_CLIENTE						VARCHAR(50)		= NULL,
	@p_NR_REMESSA						NUMERIC(3,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0),
			@TOKEN_REGISTRO_INCLUSAO BIGINT,
			@APLICACAO_ORIGEM_TOKEN BIGINT, -- 1 = APP 2 = WEB
			@TIPO_PECA_COM_ITEM TINYINT = 1

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		INSERT INTO dbo.TB_PEDIDO_AP_DADOS
		        ( ID_PEDIDO,
		          CD_PECA,
		          QTD_SOLICITADA,
		          QTD_APROVADA,
		          ID_ESTOQUE_DEBITO,
				  ID_ESTOQUE_DEBITO_3M2,
				  QTD_APROVADA_3M1,
				  QTD_APROVADA_3M2,
				  VOLUME,
				  RAMAL,
				  ID_ITEM_PEDIDO,
				  PesoLiquido,
				  PesoBruto,
				  DS_TELEFONE,
				  DS_APROVADOR,
				  RESP_CLIENTE,
				  NR_REMESSA
				)
		VALUES
		        ( @p_ID_PEDIDO,
		          @p_CD_PECA,
		          @p_QTD_SOLICITADA,
		          @p_QTD_APROVADA,
		          @p_ID_ESTOQUE_DEBITO,
				  @p_ID_ESTOQUE_DEBITO_3M2,
				  @p_QTD_APROVADA_3M1,
				  @p_QTD_APROVADA_3M2,
				  @p_VOLUME,
				  @p_RAMAL,
				  @p_ID_ITEM_PEDIDO,
				  @p_PESOLIQUIDO,
				  @p_PESOBRUTO,
				  @p_TELEFONE,
				  @p_DS_APROVADOR,
				  @p_DS_CLIENTE,
				  @p_NR_REMESSA
		          )
	
		COMMIT TRANSACTION
	
	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		ROLLBACK TRANSACTION

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH

END


