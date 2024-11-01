GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaInsertDuplicada]    Script Date: 16/09/2022 10:08:56 ******/
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
ALTER PROCEDURE [dbo].[prcPedidoPecaInsertDuplicada]	
	@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_CD_PECA							VARCHAR(15)		= NULL,
	@p_QTD_SOLICITADA					NUMERIC(15,3)	= NULL,
	@p_QTD_APROVADA						NUMERIC(15,3)	= NULL,
	@p_QTD_RECEBIDA						NUMERIC(15,3)	= NULL,
	@p_TX_APROVADO						VARCHAR(1)		= NULL,
	@p_NR_DOC_ORI						NUMERIC(18,0)	= NULL,
	@p_ST_STATUS_ITEM					CHAR(1)			= NULL,
	@p_ID_ESTOQUE_DEBITO				BIGINT			= NULL,
	@p_ID_ESTOQUE_DEBITO_3M2			BIGINT			= NULL,
	@p_QTD_APROVADA_3M1					NUMERIC(15,3)	= NULL,
	@p_QTD_APROVADA_3M2					NUMERIC(15,3)	= NULL,
	@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL,
	@p_VL_PECA							NUMERIC(14,2)	= NULL,
	@p_TIPO_PECA						TINYINT			= NULL,
	@p_DESCRICAO_PECA					VARCHAR(150)	= NULL,
	@p_CD_PECA_REFERENCIA				VARCHAR(15)		= NULL,
	@p_TOKEN    						BIGINT			= NULL
AS
BEGIN

	-- Declaração de Variáveis
	
	SET NOCOUNT ON;

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
		          ID_ESTOQUE_DEBITO,
				  ID_ESTOQUE_DEBITO_3M2,
				  QTD_APROVADA_3M1,
				  QTD_APROVADA_3M2,
				  VL_PECA,
				  TIPO_PECA,
				  DESCRICAO_PECA,
				  EnviadoBPCS,
				  CD_PECA_REFERENCIA,
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
		          @p_ID_ESTOQUE_DEBITO,
				  @p_ID_ESTOQUE_DEBITO_3M2,
				  @p_QTD_APROVADA_3M1,
				  @p_QTD_APROVADA_3M2,
				  @p_VL_PECA,
				  1,
				  @p_DESCRICAO_PECA,
				  'N',
				  @p_CD_PECA_REFERENCIA,
				  @p_TOKEN
		          )
		
		
		COMMIT TRANSACTION
	
END


