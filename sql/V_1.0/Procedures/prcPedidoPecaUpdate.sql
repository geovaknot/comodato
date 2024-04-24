GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Update dos dados na tabela
--               TB_PEDIDO_PECA
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoPecaUpdate]
	@p_ID_ITEM_PEDIDO					NUMERIC(9,0)	= NULL,
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
	@p_TIPO_PECA						TINYINT			= NULL,
	@p_DESCRICAO_PECA					VARCHAR(150)	= NULL,
	@p_QTD_ULTIMO_RECEBIMENTO   		NUMERIC(15,3)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		
		EXECUTE dbo.prcLogGravar 
		    @p_nidLog = @nidLog,
		    @p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
		    @p_ccdAcao = 'U',
		    @p_cnmTabela = 'TB_PEDIDO_PECA',
		    @p_nidPK = @p_ID_ITEM_PEDIDO,
		    @p_nidLogReturn = @nidLog OUTPUT
		 
		UPDATE	TB_PEDIDO_PECA
		SET				
			ID_PEDIDO			= ISNULL(@p_ID_PEDIDO, ID_PEDIDO),
			CD_PECA				= ISNULL(@p_CD_PECA, CD_PECA),
			QTD_SOLICITADA		= ISNULL(@p_QTD_SOLICITADA, QTD_SOLICITADA),
			QTD_APROVADA		= ISNULL(@p_QTD_APROVADA, QTD_APROVADA),
			QTD_RECEBIDA		= ISNULL(@p_QTD_RECEBIDA, QTD_RECEBIDA),
			TX_APROVADO			= ISNULL(@p_TX_APROVADO, TX_APROVADO),
			NR_DOC_ORI			= ISNULL(@p_NR_DOC_ORI, NR_DOC_ORI),
			ST_STATUS_ITEM		= ISNULL(@p_ST_STATUS_ITEM, ST_STATUS_ITEM),
			DS_OBSERVACAO		= ISNULL(@p_DS_OBSERVACAO, DS_OBSERVACAO),
			DS_DIR_FOTO			= @p_DS_DIR_FOTO,
			ID_ESTOQUE_DEBITO	= @p_ID_ESTOQUE_DEBITO,
			ID_ESTOQUE_DEBITO_3M2 = @p_ID_ESTOQUE_DEBITO_3M2,
			QTD_APROVADA_3M1 = @p_QTD_APROVADA_3M1,
			QTD_APROVADA_3M2 = @p_QTD_APROVADA_3M2,
			TIPO_PECA = ISNULL(@p_TIPO_PECA, TIPO_PECA),
			DESCRICAO_PECA = @p_DESCRICAO_PECA,
			QTD_ULTIMO_RECEBIMENTO = ISNULL(@p_QTD_ULTIMO_RECEBIMENTO, QTD_ULTIMO_RECEBIMENTO)
		WHERE ID_ITEM_PEDIDO	= @p_ID_ITEM_PEDIDO
		         	
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
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


GO
