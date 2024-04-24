GO
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
ALTER PROCEDURE [dbo].[prcPedidoPecaInsert]	
	@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_CD_PECA							VARCHAR(15)		= NULL,
	@p_QTD_SOLICITADA					NUMERIC(15,3)	= NULL,
	@p_QTD_APROVADA						NUMERIC(15,3)	= NULL,
	@p_QTD_RECEBIDA						NUMERIC(15,3)	= NULL,
	@p_TX_APROVADO						VARCHAR(1)		= NULL,
	@p_NR_DOC_ORI						NUMERIC(18,0)	= NULL,
	@p_ST_STATUS_ITEM					CHAR(1)			= NULL,
	@p_DS_OBSERVACAO					VARCHAR(MAX)	= NULL,
	@p_DS_DIR_FOTO						VARCHAR(255)	= NULL,
	@p_ID_ESTOQUE_DEBITO				BIGINT			= NULL,
	@p_ID_ESTOQUE_DEBITO_3M2			BIGINT			= NULL,
	@p_QTD_APROVADA_3M1					NUMERIC(15,3)	= NULL,
	@p_QTD_APROVADA_3M2					NUMERIC(15,3)	= NULL,
	@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL,
	@p_VL_PECA							NUMERIC(14,2)	= NULL,
	@p_TIPO_PECA						TINYINT			= NULL,
	@p_DESCRICAO_PECA					VARCHAR(150)	= NULL,
	@p_TOKEN    						BIGINT			= NULL,
	@p_TOKEN_GERADO    					BIGINT		    OUTPUT,
	@p_ID_ITEM_PEDIDO					NUMERIC(9,0)	OUTPUT
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

		INSERT INTO dbo.TB_PEDIDO_PECA
		        ( ID_PEDIDO,
		          CD_PECA,
		          QTD_SOLICITADA,
		          QTD_APROVADA,
		          TX_APROVADO,
		          NR_DOC_ORI,
		          QTD_RECEBIDA,
		          ST_STATUS_ITEM,
		          DS_OBSERVACAO,
		          DS_DIR_FOTO,
		          ID_ESTOQUE_DEBITO,
				  ID_ESTOQUE_DEBITO_3M2,
				  QTD_APROVADA_3M1,
				  QTD_APROVADA_3M2,
				  VL_PECA,
				  TIPO_PECA,
				  DESCRICAO_PECA,
				  TOKEN
				)
		VALUES
		        ( @p_ID_PEDIDO,
		          @p_CD_PECA,
		          @p_QTD_SOLICITADA,
		          @p_QTD_APROVADA,
		          @p_TX_APROVADO,
		          @p_NR_DOC_ORI,
		          @p_QTD_RECEBIDA,
		          @p_ST_STATUS_ITEM,
		          @p_DS_OBSERVACAO,
		          @p_DS_DIR_FOTO,
		          @p_ID_ESTOQUE_DEBITO,
				  @p_ID_ESTOQUE_DEBITO_3M2,
				  @p_QTD_APROVADA_3M1,
				  @p_QTD_APROVADA_3M2,
				  @p_VL_PECA,
				  CASE WHEN (@p_TIPO_PECA IS NULL) AND (@p_DESCRICAO_PECA = '' OR @p_DESCRICAO_PECA IS NULL) THEN @TIPO_PECA_COM_ITEM ELSE @p_TIPO_PECA END,
				  @p_DESCRICAO_PECA,
				  IIF(LEFT(@p_TOKEN, 1) = 1, @p_TOKEN, 0)
		          )
		
		SET @p_ID_ITEM_PEDIDO = @@IDENTITY

		SET @APLICACAO_ORIGEM_TOKEN = LEFT(@p_TOKEN, 1);

		IF (@p_ID_ITEM_PEDIDO > 0 AND @APLICACAO_ORIGEM_TOKEN = 2) -- 2 = ORIGEM Applicação WEB
		BEGIN
			SET @TOKEN_REGISTRO_INCLUSAO = CAST((CAST(@p_TOKEN AS nvarchar(MAX)) + CAST(@p_ID_ITEM_PEDIDO AS nvarchar(MAX))) AS BIGINT);
			
			UPDATE dbo.TB_PEDIDO_PECA
			   SET TOKEN = @TOKEN_REGISTRO_INCLUSAO
			 WHERE ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO

			SET @p_TOKEN_GERADO = @TOKEN_REGISTRO_INCLUSAO;
		END
		ELSE
		BEGIN
			SET @p_TOKEN_GERADO = @p_TOKEN;
		END
	
		EXECUTE dbo.prcLogGravar 
					@p_nidLog					= @nidLog,
					@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
					@p_ccdAcao					= 'I',
					@p_cnmTabela				= 'TB_PEDIDO_PECA',
					@p_nidPK					= @p_ID_ITEM_PEDIDO,
					@p_nidLogReturn				= @nidLog OUTPUT
	
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


