GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Inclusão dos dados na tabela
--              TB_PEDIDO
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoInsert]	
		@p_CD_TECNICO						VARCHAR(6)		= NULL,
		@p_NR_DOCUMENTO						NUMERIC(7,0)	= NULL,
		@p_DT_CRIACAO						DATETIME		= NULL,
		@p_DT_ENVIO							DATETIME		= NULL,
		@p_DT_RECEBIMENTO					DATETIME		= NULL,
		@p_TX_OBS							VARCHAR(255)	= NULL,
		@p_PENDENTE							VARCHAR(1)		= NULL,
		@p_NR_DOC_ORI						NUMERIC(18,0)	= NULL,
		@p_ID_STATUS_PEDIDO					BIGINT			= NULL,
		@p_TP_TIPO_PEDIDO					CHAR(1)			= NULL,
		@p_CD_CLIENTE						NUMERIC(6,0)	= NULL,
		--@p_CD_PEDIDO						BIGINT			= NULL,
		@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL,
		@p_TOKEN    						BIGINT			= NULL,
		@p_TOKEN_GERADO    					BIGINT		    OUTPUT,
		@p_ID_PEDIDO						NUMERIC(9,0)	OUTPUT
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0),
			@ID_ITEM_PEDIDO		BIGINT,
			@CD_PEDIDO			BIGINT,
			@CD_PECA			VARCHAR(15),
			@QT_ESTOQUE_MINIMO	NUMERIC(15,3),
			@QT_SUGERIDA_PZ		NUMERIC(15,3),
			@QT_SOLICITADA		NUMERIC(15,3),
			@QUANTIDADE			INT,
			@VL_PECA			NUMERIC(14,2),
			@TOKEN_REGISTRO_INCLUSAO BIGINT,
			@APLICACAO_ORIGEM_TOKEN BIGINT -- 1 = APP 2 = WEB
            
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		SELECT @p_ID_PEDIDO = ISNULL(MAX(ID_PEDIDO), 0) + 1 FROM TB_PEDIDO
		SELECT @CD_PEDIDO = ISNULL(MAX(CD_PEDIDO), 0) + 1 FROM TB_PEDIDO

		IF @p_NR_DOCUMENTO = 0
		BEGIN
			SET @p_NR_DOCUMENTO = @p_ID_PEDIDO
		END      

		INSERT INTO TB_PEDIDO
		(
			ID_PEDIDO,
			CD_TECNICO,
			NR_DOCUMENTO,	
			DT_CRIACAO,
			DT_ENVIO,	
			DT_RECEBIMENTO,
			TX_OBS,
			PENDENTE,
			NR_DOC_ORI,
			ID_STATUS_PEDIDO,
			TP_TIPO_PEDIDO,
			CD_CLIENTE,
			CD_PEDIDO,
			TOKEN
		)
		VALUES
		(
			@p_ID_PEDIDO,	
			@p_CD_TECNICO,	
			@p_NR_DOCUMENTO,	
			@p_DT_CRIACAO,	
			@p_DT_ENVIO	,	
			@p_DT_RECEBIMENTO,	
			@p_TX_OBS,
			@p_PENDENTE,
			@p_NR_DOC_ORI,
			@p_ID_STATUS_PEDIDO,
			@p_TP_TIPO_PEDIDO,
			@p_CD_CLIENTE,
			@CD_PEDIDO,
			IIF(LEFT(@p_TOKEN, 1) = 1, @p_TOKEN, 0)
		)

		IF(@p_TP_TIPO_PEDIDO = 'T') 
		BEGIN

			CREATE TABLE #tbPZ (
				CD_PECA VARCHAR(12) COLLATE SQL_Latin1_General_CP1_CI_AI,
				DS_PECA VARCHAR(250),
				TX_UNIDADE VARCHAR(5),
				CD_CRITICIDADE_ABC CHAR(1),
				QT_PECA_NO_MOD INT,
				QT_SUGERIDA_PZ INT				
			)
			INSERT #tbPZ 
			EXEC [dbo].[prcPlanoZeroPedidoTecnico] @p_CD_TECNICO;


			-- Para o tipo Técnico (Cria Itens em TB_PEDIDO_PECA baseados no Plano Zero)
			DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
			SELECT 
				tbPZEst.CD_PECA,
				dbo.TB_PECA.VL_PECA,
				IIF((QT_SUGERIDA_PZ - QT_PECA_ATUAL) < 0, 0, (QT_SUGERIDA_PZ - QT_PECA_ATUAL)) AS QT_SOLICITADA 
			FROM
				(SELECT 
					#tbPZ.CD_PECA,
					#tbPZ.DS_PECA,
					QT_SUGERIDA_PZ ,
					ISNULL((SELECT TOP 1 
						dbo.tbEstoquePeca.QT_PECA_ATUAL 
					FROM dbo.tbEstoquePeca 
					INNER JOIN dbo.tbEstoque 
						ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
					WHERE tbEstoque.FL_ATIVO		= 'S'
					AND dbo.tbEstoquePeca.CD_PECA	= #tbPZ.CD_PECA 
					AND dbo.tbEstoque.CD_TECNICO	= @p_CD_TECNICO
					),0) AS QT_PECA_ATUAL
				FROM #tbPZ) tbPZEst
				INNER JOIN dbo.TB_PECA ON TB_PECA.CD_PECA = tbPZEst.CD_PECA

			OPEN C1
			FETCH NEXT FROM C1
				INTO @CD_PECA, @VL_PECA, @QT_SOLICITADA

			WHILE @@FETCH_STATUS = 0
			BEGIN
				EXEC dbo.prcPedidoPecaInsert 
				    @p_ID_PEDIDO = @p_ID_PEDIDO,
				    @p_CD_PECA = @CD_PECA,
				    @p_QTD_SOLICITADA = @QT_SOLICITADA,
				    @p_QTD_APROVADA = NULL,
				    @p_QTD_RECEBIDA = NULL,
				    @p_TX_APROVADO = NULL,
				    @p_NR_DOC_ORI = NULL,
				    @p_ST_STATUS_ITEM = '1',
				    @p_DS_OBSERVACAO = NULL,
				    @p_DS_DIR_FOTO = NULL,
				    @p_ID_ESTOQUE_DEBITO = NULL,
				    @p_nidUsuarioAtualizacao = NULL,
					@p_VL_PECA = @VL_PECA,
					@p_TOKEN = @p_TOKEN,
					@p_TOKEN_GERADO = @p_TOKEN_GERADO OUTPUT,
				    @p_ID_ITEM_PEDIDO = @ID_ITEM_PEDIDO OUTPUT
				
				FETCH NEXT FROM C1
					INTO @CD_PECA, @VL_PECA, @QT_SOLICITADA;
			END;
			CLOSE C1;
			DEALLOCATE C1;

			If(OBJECT_ID('tempdb..#tbPZ') Is Not Null)
			BEGIN
				DROP TABLE #tbPZ
			END 

		END      
	
		SET @p_TOKEN_GERADO = 0;
		SET @APLICACAO_ORIGEM_TOKEN = LEFT(@p_TOKEN, 1);

		IF (@p_ID_PEDIDO > 0 AND @APLICACAO_ORIGEM_TOKEN = 2) -- 2 = ORIGEM Applicação WEB
		BEGIN
			SET @TOKEN_REGISTRO_INCLUSAO = CAST((CAST(@p_TOKEN AS nvarchar(MAX)) + CAST(@p_ID_PEDIDO AS nvarchar(9))) AS BIGINT);
			
			UPDATE dbo.TB_PEDIDO
			   SET TOKEN = @TOKEN_REGISTRO_INCLUSAO
			 WHERE ID_PEDIDO = @p_ID_PEDIDO

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
					@p_cnmTabela				= 'TB_PEDIDO',
					@p_nidPK					= @p_ID_PEDIDO,
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