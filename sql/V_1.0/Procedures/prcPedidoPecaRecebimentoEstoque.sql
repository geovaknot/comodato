GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaRecebimentoEstoque]    Script Date: 07/10/2021 10:35:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Edgar Coutinho
-- Create date: 29/07/2021
-- Description:	Atualização estoque recebimento de peças
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoPecaRecebimentoEstoque]
	@p_ID_ITEM_PEDIDO			NUMERIC(9,0)	= NULL,
	@p_ID_PEDIDO				NUMERIC(9,0)	= NULL,
	@p_QTD_RECEBIDA				NUMERIC(15,3)	= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_status_item				char(1)			= NULL,
	@p_Mensagem					VARCHAR(8000)	OUTPUT
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage		NVARCHAR(4000),
			@nidErrorSeverity		INT,
			@nidErrorState			INT,
			@nidLog					NUMERIC(18,0),
			@TP_TIPO_PEDIDO			CHAR(1),
			@CD_TECNICO				VARCHAR(06),
			@CD_PECA				VARCHAR(15),
			@QTD_APROVADA			NUMERIC(15,3),
			@QTD_APROVADA_3M1		NUMERIC(15,3),
			@QTD_APROVADA_3M2		NUMERIC(15,3),
			@ID_ESTOQUE_DEBITO_3M1	BIGINT,
			@ID_ESTOQUE_DEBITO_3M2	BIGINT,
			@ID_ESTOQUE_PECA		BIGINT,
			@ID_ESTOQUE_CREDITO		BIGINT,
			@QT_PECA_ATUAL			NUMERIC(15,3),
			@QTD_RECEBIDA_ATUAL     NUMERIC(15,3),
			@QTD_RECEBIDA_TOTAL     NUMERIC(15,3),
			@QTD_ULTIMO_RECEBIMENTO NUMERIC(15,3),
			@DT_MOVIMENTACAO		DATETIME,
			@ID_ESTOQUE_MOVI		BIGINT,
			@ID_ITEM_PEDIDO			BIGINT,
			@CD_CLIENTE				NUMERIC(6,0),
			@TIPO_PECA				TINYINT,

			@STATUS_ITEM_RECEBIDO TINYINT = 5,
			@STATUS_ITEM_RECEBIDO_PENDENCIA TINYINT = 7


	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	
	BEGIN TRY
	
		-- Busca os Itens do Pedido para processar
		-- OBS: Se uma quantidade aprovada for 0(ZERO), não gera nenhum lançamento 
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT	dbo.TB_PEDIDO.TP_TIPO_PEDIDO,
				dbo.TB_PEDIDO.CD_CLIENTE,
				dbo.TB_PEDIDO.CD_TECNICO,
				dbo.TB_PEDIDO_PECA.CD_PECA,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA,
				dbo.TB_PEDIDO_PECA.QTD_RECEBIDA,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M1,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M2,
				dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO,
				dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2,
				dbo.TB_PEDIDO_PECA.TIPO_PECA,
				dbo.TB_PEDIDO_PECA.QTD_ULTIMO_RECEBIMENTO,
				dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO
  		  FROM dbo.TB_PEDIDO_PECA 
		 INNER JOIN dbo.TB_PEDIDO
			ON dbo.TB_PEDIDO_PECA.ID_PEDIDO = dbo.TB_PEDIDO.ID_PEDIDO
		 WHERE dbo.TB_PEDIDO_PECA.ID_PEDIDO = @p_ID_PEDIDO
		   AND QTD_APROVADA > 0
           AND (dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO) OR (@p_ID_ITEM_PEDIDO IS NULL AND dbo.TB_PEDIDO_PECA.QTD_ULTIMO_RECEBIMENTO > 0)

		OPEN C1
		FETCH NEXT FROM C1
			INTO @TP_TIPO_PEDIDO, @CD_CLIENTE, @CD_TECNICO, @CD_PECA, @QTD_APROVADA, @QTD_RECEBIDA_ATUAL, @QTD_APROVADA_3M1, @QTD_APROVADA_3M2, @ID_ESTOQUE_DEBITO_3M1, @ID_ESTOQUE_DEBITO_3M2, @TIPO_PECA, @QTD_ULTIMO_RECEBIMENTO, @ID_ITEM_PEDIDO

		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF (ISNULL(@QTD_APROVADA, 0) = 0)
			BEGIN
				SET @p_Mensagem = 'Enviado recebimento de peça sem quantidade APROVADA - Peça' + @CD_PECA
				RETURN; 
			END


			IF (@QTD_ULTIMO_RECEBIMENTO > 0)
			BEGIN
				SET @p_QTD_RECEBIDA = @QTD_ULTIMO_RECEBIMENTO;
			END

			SET @QTD_RECEBIDA_TOTAL = ISNULL(@p_QTD_RECEBIDA, 0);

			UPDATE TB_PEDIDO_PECA
			   SET QTD_RECEBIDA = case 
					when ISNULL(QTD_RECEBIDA, 0) = 0
						then @QTD_RECEBIDA_TOTAL
					when @QTD_ULTIMO_RECEBIMENTO > 0
						then @QTD_RECEBIDA_ATUAL
					when QTD_RECEBIDA > 0 AND ISNULL(@QTD_ULTIMO_RECEBIMENTO, 0) = 0
						then QTD_RECEBIDA + @QTD_RECEBIDA_TOTAL
				   end,
				   QTD_ULTIMO_RECEBIMENTO = NULL,
				   ST_STATUS_ITEM = CASE 
										WHEN ISNULL(@p_status_item, 0) > 0 then @p_status_item
										WHEN (@QTD_APROVADA > 0 AND (@QTD_RECEBIDA_TOTAL IS NULL OR @QTD_RECEBIDA_TOTAL >= @QTD_APROVADA) AND ISNULL(@p_status_item, 0) = 0) THEN @STATUS_ITEM_RECEBIDO
										WHEN (@QTD_APROVADA > 0 AND @QTD_RECEBIDA_TOTAL > 0 AND @QTD_RECEBIDA_TOTAL < @QTD_APROVADA AND @QTD_RECEBIDA_ATUAL < QTD_APROVADA AND ISNULL(@p_status_item, 0) = 0) THEN @STATUS_ITEM_RECEBIDO_PENDENCIA
										WHEN (@QTD_APROVADA > 0 AND QTD_RECEBIDA >= QTD_APROVADA AND ISNULL(@p_status_item, 0) = 0) THEN @STATUS_ITEM_RECEBIDO
										WHEN (@QTD_RECEBIDA_ATUAL > 0 AND @QTD_RECEBIDA_ATUAL >= QTD_APROVADA) THEN @STATUS_ITEM_RECEBIDO
									END
			 WHERE ID_ITEM_PEDIDO = @ID_ITEM_PEDIDO

			EXECUTE dbo.prcLogGravar 
					@p_nidLog					= @nidLog,
					@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
					@p_ccdAcao					= 'U',
					@p_cnmTabela				= 'TB_PEDIDO_PECA',
					@p_nidPK					= @p_ID_ITEM_PEDIDO,
					@p_nidLogReturn				= @nidLog OUTPUT
		
			IF (@TIPO_PECA = 2)
			BEGIN
				RETURN
			END

				SET @ID_ESTOQUE_PECA		= NULL;
				SET @ID_ESTOQUE_CREDITO		= NULL;
				SET @QT_PECA_ATUAL			= NULL;
				SET @DT_MOVIMENTACAO		= GETDATE();
				SET @ID_ESTOQUE_MOVI		= NULL;

				-- **************************************************************************************************************************
				-- ************************************************* LANÇAMENTO DE CRÉDITO **************************************************
				-- **************************************************************************************************************************

				IF(@TP_TIPO_PEDIDO = 'T' OR @TP_TIPO_PEDIDO = 'A') -- PEDIDO TÉCNICO E AVULSO
				BEGIN 
					SET @ID_ESTOQUE_PECA = NULL;
					SET @QT_PECA_ATUAL = NULL;

					-- Busca o ID_ESTOQUE do Técnico
					SELECT @ID_ESTOQUE_CREDITO = ID_ESTOQUE 
			  			FROM dbo.tbEstoque 
						WHERE dbo.tbEstoque.CD_TECNICO = @CD_TECNICO 
						AND dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'TEC' 
						AND dbo.tbEstoque.FL_ATIVO	= 'S'

					IF (@ID_ESTOQUE_CREDITO IS NULL)
					BEGIN
						SET @p_Mensagem = 'Não encontrado estoque para o técnico -' + @CD_TECNICO
						RETURN; 
					END

					-- Busca o ID_ESTOQUE_PECA e QT_PECA_ATUAL em tbEstoquePeca para crédito (entrada) do Técnico
					SELECT @ID_ESTOQUE_PECA = dbo.tbEstoquePeca.ID_ESTOQUE_PECA,
							@QT_PECA_ATUAL = dbo.tbEstoquePeca.QT_PECA_ATUAL
						FROM dbo.tbEstoquePeca 
						INNER JOIN dbo.tbEstoque
						ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
						WHERE dbo.tbEstoquePeca.CD_PECA = @CD_PECA
						AND dbo.tbEstoque.CD_TECNICO	= @CD_TECNICO
						AND dbo.tbEstoque.ID_ESTOQUE	= @ID_ESTOQUE_CREDITO

					IF (@QTD_APROVADA_3M1 > 0 or @QTD_APROVADA_3M2 > 0) -- Estoque 3M-1
					BEGIN
						-- Faz o cálculo a creditar
						if (@QTD_ULTIMO_RECEBIMENTO > 0)
						begin
							SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) + @QTD_ULTIMO_RECEBIMENTO;
						end
						if (ISNULL(@QTD_ULTIMO_RECEBIMENTO,0) <= 0 AND ISNULL(@p_QTD_RECEBIDA,0) = 0)
						begin
							SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) + @QTD_RECEBIDA_ATUAL;
						end
						if ((ISNULL(@QTD_ULTIMO_RECEBIMENTO,0) <= 0 AND ISNULL(@p_QTD_RECEBIDA,0) > 0))
						begin
							SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) + @p_QTD_RECEBIDA;
						end
						-- Inclui ou Atualiza a tabela tbEstoquePeca com o lançamento de crédito (entrada) do Técnico
						IF (@ID_ESTOQUE_PECA IS NULL)
						BEGIN
							EXEC dbo.prcEstoquePecaInsert 
									@p_CD_PECA = @CD_PECA,
									@p_QT_PECA_ATUAL = @QT_PECA_ATUAL,
									@p_QT_PECA_MIN = 0,
									@p_DT_ULT_MOVIM = @DT_MOVIMENTACAO,
									@p_ID_ESTOQUE = @ID_ESTOQUE_CREDITO,
									@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
									@p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA OUTPUT
						END          
						ELSE 
						BEGIN
							EXEC dbo.prcEstoquePecaUpdate 
									@p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA,
									@p_CD_PECA = @CD_PECA,
									@p_QT_PECA_ATUAL = @QT_PECA_ATUAL,
									@p_QT_PECA_MIN = 0,
									@p_DT_ULT_MOVIM = @DT_MOVIMENTACAO,
									@p_ID_ESTOQUE = @ID_ESTOQUE_CREDITO,
									@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao
						END 

						-- Atualiza a tabela tbEstoqueMovi (LOG) com o lançamento de crédito (entrada) do Técnico
						EXEC dbo.prcEstoqueMoviInsert 
								@p_CD_TP_MOVIMENTACAO = '6', --Remessa 3M p/ est int.
								@p_ID_OS = NULL,
								@p_DT_MOVIMENTACAO = @DT_MOVIMENTACAO,
								@p_ID_ESTOQUE = @ID_ESTOQUE_CREDITO,
								@p_CD_PECA = @CD_PECA,
								@p_QT_PECA = @QTD_APROVADA,
								@p_ID_USU_MOVI = @p_nidUsuarioAtualizacao,
								@p_ID_ESTOQUE_ORIGEM = @ID_ESTOQUE_DEBITO_3M1,
								@p_TP_ENTRADA_SAIDA = 'E',
								@p_CD_CLIENTE = @CD_CLIENTE,
								@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
								@p_ID_ESTOQUE_MOVI = @ID_ESTOQUE_MOVI OUTPUT
					END

				--	IF (@QTD_APROVADA_3M2 > 0) -- Estoque 3M-2
				--	BEGIN
				--		-- Faz o cálculo a creditar
				--		SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) + @QTD_APROVADA_3M2;
				
				--		-- Inclui ou Atualiza a tabela tbEstoquePeca com o lançamento de crédito (entrada) do Técnico
				--		IF (@ID_ESTOQUE_PECA IS NULL)
				--		BEGIN
				--			EXEC dbo.prcEstoquePecaInsert 
				--					@p_CD_PECA = @CD_PECA,
				--					@p_QT_PECA_ATUAL = @QT_PECA_ATUAL,
				--					@p_QT_PECA_MIN = 0,
				--					@p_DT_ULT_MOVIM = @DT_MOVIMENTACAO,
				--					@p_ID_ESTOQUE = @ID_ESTOQUE_CREDITO,
				--					@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
				--					@p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA OUTPUT
				--		END          
				--		ELSE 
				--		BEGIN
				--			EXEC dbo.prcEstoquePecaUpdate 
				--					@p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA,
				--					@p_CD_PECA = @CD_PECA,
				--					@p_QT_PECA_ATUAL = @QT_PECA_ATUAL,
				--					@p_QT_PECA_MIN = 0,
				--					@p_DT_ULT_MOVIM = @DT_MOVIMENTACAO,
				--					@p_ID_ESTOQUE = @ID_ESTOQUE_CREDITO,
				--					@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao
				--		END 

				--		-- Atualiza a tabela tbEstoqueMovi (LOG) com o lançamento de crédito (entrada) do Técnico
				--		EXEC dbo.prcEstoqueMoviInsert 
				--				@p_CD_TP_MOVIMENTACAO = '6', --Remessa 3M p/ est int.
				--				@p_ID_OS = NULL,
				--				@p_DT_MOVIMENTACAO = @DT_MOVIMENTACAO,
				--				@p_ID_ESTOQUE = @ID_ESTOQUE_CREDITO,
				--				@p_CD_PECA = @CD_PECA,
				--				@p_QT_PECA = @QTD_APROVADA_3M2,
				--				@p_ID_USU_MOVI = @p_nidUsuarioAtualizacao,
				--				@p_ID_ESTOQUE_ORIGEM = @ID_ESTOQUE_DEBITO_3M2,
				--				@p_TP_ENTRADA_SAIDA = 'E',
				--				@p_CD_CLIENTE = @CD_CLIENTE,
				--				@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
				--				@p_ID_ESTOQUE_MOVI = @ID_ESTOQUE_MOVI OUTPUT
				--	END
				END

				FETCH NEXT FROM C1
					INTO @TP_TIPO_PEDIDO, @CD_CLIENTE, @CD_TECNICO, @CD_PECA, @QTD_APROVADA, @QTD_RECEBIDA_ATUAL, @QTD_APROVADA_3M1, @QTD_APROVADA_3M2, @ID_ESTOQUE_DEBITO_3M1, @ID_ESTOQUE_DEBITO_3M2, @TIPO_PECA, @QTD_ULTIMO_RECEBIMENTO, @ID_ITEM_PEDIDO
		END;
		CLOSE C1;
		DEALLOCATE C1;

		SET @p_Mensagem = '';
	
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
