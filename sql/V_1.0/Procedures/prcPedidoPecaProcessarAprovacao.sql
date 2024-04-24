GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 03/07/2018
-- Description:	Update das QTD_APROVADA na tabela
--               TB_PEDIDO_PECA que ficaram vazias
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoPecaProcessarAprovacao]
	@p_ID_PEDIDO				NUMERIC(9,0)	= NULL,
	@p_PecasLote				NVARCHAR(MAX)	= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
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
			@QTD_APROVADA_NEGATIVO	NUMERIC(15,3),
			@ID_ESTOQUE_DEBITO_3M1	BIGINT,
			@ID_ESTOQUE_DEBITO_3M2	BIGINT,
			@ID_ESTOQUE_PECA		BIGINT,
			@ID_ESTOQUE_CREDITO		BIGINT,
			@QT_PECA_ATUAL			NUMERIC(15,3),
			@DT_MOVIMENTACAO		DATETIME,
			@ID_ESTOQUE_MOVI		BIGINT,
			@CD_CLIENTE				NUMERIC(6,0)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET XACT_ABORT ON;
	
	BEGIN TRY
		
		-- Busca os Itens do Pedido para processar
		-- OBS: Se uma quantidade aprovada for 0(ZERO), não gera nenhum lançamento 
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT	dbo.TB_PEDIDO.TP_TIPO_PEDIDO
				,dbo.TB_PEDIDO.CD_CLIENTE
				,dbo.TB_PEDIDO.CD_TECNICO
				,dbo.TB_PEDIDO_PECA.CD_PECA
				,dbo.TB_PEDIDO_PECA.QTD_APROVADA
				,dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M1
				,dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M2
				,dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO
				,dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2

		 FROM dbo.TB_PEDIDO_PECA 
		INNER JOIN dbo.TB_PEDIDO
    	   ON dbo.TB_PEDIDO_PECA.ID_PEDIDO = dbo.TB_PEDIDO.ID_PEDIDO
		WHERE dbo.TB_PEDIDO_PECA.ID_PEDIDO = @p_ID_PEDIDO
		  AND QTD_APROVADA > 0
		  AND dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_PecasLote, ',')) --Add para aprov. parc. lotes
		
		OPEN C1
		FETCH NEXT FROM C1
			INTO @TP_TIPO_PEDIDO, @CD_CLIENTE, @CD_TECNICO, @CD_PECA, @QTD_APROVADA, @QTD_APROVADA_3M1, @QTD_APROVADA_3M2, @ID_ESTOQUE_DEBITO_3M1, @ID_ESTOQUE_DEBITO_3M2--, @VL_PECA

		WHILE @@FETCH_STATUS = 0
		BEGIN
			SET @QTD_APROVADA_NEGATIVO	= NULL;
			SET @ID_ESTOQUE_PECA		= NULL;
			SET @ID_ESTOQUE_CREDITO		= NULL;
			SET @QT_PECA_ATUAL			= NULL;
			SET @DT_MOVIMENTACAO		= NULL;
			SET @ID_ESTOQUE_MOVI		= NULL;

			-- **************************************************************************************************************************
			-- ************************************************** LANÇAMENTO DE DÉBITO **************************************************
			-- **************************************************************************************************************************
			
			-- Lançamento de débito no Estoque 3M1
			IF (@QTD_APROVADA_3M1 > 0)
			BEGIN 
				-- Se usuário não escolheu o Estoque a Debitar (ID_ESTOQUE_DEBITO = NULL), utilizar o ID_ESTOQUE do Estoque 3M1 ativo (padrão)
				IF (@ID_ESTOQUE_DEBITO_3M1 IS NULL)
				BEGIN
					SELECT TOP 1 @ID_ESTOQUE_DEBITO_3M1 = ID_ESTOQUE FROM dbo.tbEstoque WHERE TP_ESTOQUE_TEC_3M = '3M1' AND FL_ATIVO = 'S'
				END          

				IF (@ID_ESTOQUE_DEBITO_3M1 IS NULL)
				BEGIN
					SET @p_Mensagem = 'Não encontrado estoque interno 3M1!'
					RETURN; 
				END

				-- Busca o ID_ESTOQUE_PECA e QT_PECA_ATUAL em tbEstoquePeca para débito (saída) do Estoque a Debitar
				SELECT @ID_ESTOQUE_PECA = ID_ESTOQUE_PECA,
					   @QT_PECA_ATUAL = QT_PECA_ATUAL
				  FROM dbo.tbEstoquePeca
				 WHERE CD_PECA = @CD_PECA
				   AND ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M1

				IF (@ID_ESTOQUE_PECA IS NULL)
				BEGIN
					DECLARE @DataAtual DATETIME;
					SET @DataAtual = GETDATE();

					EXEC [dbo].[prcEstoquePecaInsert] 
							@p_CD_PECA = @CD_PECA,
							@p_QT_PECA_ATUAL = 0,
							@p_QT_PECA_MIN = 0,
							@p_DT_ULT_MOVIM = @DataAtual,
							@p_ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M1,
							@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
							@p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA OUTPUT
				END

				-- Faz o cálculo a debitar
				SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) - @QTD_APROVADA_3M1;
				SET @DT_MOVIMENTACAO = GETDATE();
				SET @QTD_APROVADA_NEGATIVO = (-1 * @QTD_APROVADA_3M1);

				IF (@QT_PECA_ATUAL<0)
				BEGIN
					SET @p_Mensagem = 'O estoque 3M1-F4, não possui peça suficiente para aprovação!'
					RETURN;
				END

				-- Atualiza a tabela tbEstoquePeca com o lançamento de débito (saída) do Estoque escolhido pelo usuário ou do Estoque padrão 3M1
				EXEC dbo.prcEstoquePecaUpdate 
						@p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA,
						@p_CD_PECA = @CD_PECA,
						@p_QT_PECA_ATUAL = @QT_PECA_ATUAL,
						@p_QT_PECA_MIN = NULL,
						@p_DT_ULT_MOVIM = @DT_MOVIMENTACAO,
						@p_ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M1,
						@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao
			
				-- Atualiza a tabela tbEstoqueMovi (LOG) com o lançamento de débito (saída) do Estoque escolhido pelo usuário ou do Estoque padrão 3M1
				EXEC dbo.prcEstoqueMoviInsert 
						@p_CD_TP_MOVIMENTACAO = '6', --Remessa 3M p/ est int.
						@p_ID_OS = NULL,
						@p_DT_MOVIMENTACAO = @DT_MOVIMENTACAO,
						@p_ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M1,
						@p_CD_PECA = @CD_PECA,
						@p_QT_PECA = @QTD_APROVADA_NEGATIVO,
						@p_ID_USU_MOVI = @p_nidUsuarioAtualizacao,
						@p_ID_ESTOQUE_ORIGEM = NULL,
						@p_TP_ENTRADA_SAIDA = 'S',
						@p_CD_CLIENTE = NULL,
						@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
						@p_ID_ESTOQUE_MOVI = @ID_ESTOQUE_MOVI OUTPUT
			END

			-- Lançamento de débito no Estoque 3M2
			IF (@QTD_APROVADA_3M2 > 0)
			BEGIN 
				IF (@ID_ESTOQUE_DEBITO_3M2 IS NULL)
				BEGIN
					SELECT TOP 1 @ID_ESTOQUE_DEBITO_3M2 = ID_ESTOQUE FROM dbo.tbEstoque WHERE TP_ESTOQUE_TEC_3M = '3M2' AND FL_ATIVO = 'S'
				END          

				IF (@ID_ESTOQUE_DEBITO_3M2 IS NULL)
				BEGIN
					SET @p_Mensagem = 'Não encontrado estoque interno 3M2!'
					RETURN; 
				END

				-- Busca o ID_ESTOQUE_PECA e QT_PECA_ATUAL em tbEstoquePeca para débito (saída) do Estoque a Debitar
				SELECT @ID_ESTOQUE_PECA	= ID_ESTOQUE_PECA,
					   @QT_PECA_ATUAL = QT_PECA_ATUAL
  				  FROM dbo.tbEstoquePeca
				 WHERE CD_PECA = @CD_PECA
				   AND ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M2

				IF (@ID_ESTOQUE_PECA IS NULL)
				BEGIN
					DECLARE @DataAtual2 DATETIME;
					SET @DataAtual2 = GETDATE();

					EXEC [dbo].[prcEstoquePecaInsert] 
							@p_CD_PECA = @CD_PECA,
							@p_QT_PECA_ATUAL = 0,
							@p_QT_PECA_MIN = 0,
							@p_DT_ULT_MOVIM = @DataAtual2,
							@p_ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M2,
							@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
							@p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA OUTPUT
				END

				-- Faz o cálculo a debitar
				SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) - @QTD_APROVADA_3M2;
				SET @DT_MOVIMENTACAO = GETDATE();
				SET @QTD_APROVADA_NEGATIVO = (-1 * @QTD_APROVADA_3M2);
				
				IF (@QT_PECA_ATUAL<0)
				BEGIN
					SET @p_Mensagem = 'O estoque 3M2-RECUP, não possui peça suficiente para aprovação!'
					RETURN; 
				END

				-- Atualiza a tabela tbEstoquePeca com o lançamento de débito (saída) do Estoque escolhido pelo usuário ou do Estoque padrão 3M1
				EXEC dbo.prcEstoquePecaUpdate 
						@p_ID_ESTOQUE_PECA = @ID_ESTOQUE_PECA,
						@p_CD_PECA = @CD_PECA,
						@p_QT_PECA_ATUAL = @QT_PECA_ATUAL,
						@p_QT_PECA_MIN = NULL,
						@p_DT_ULT_MOVIM = @DT_MOVIMENTACAO,
						@p_ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M2,
						@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao
			
				-- Atualiza a tabela tbEstoqueMovi (LOG) com o lançamento de débito (saída) do Estoque escolhido pelo usuário ou do Estoque padrão 3M1
				EXEC dbo.prcEstoqueMoviInsert 
						@p_CD_TP_MOVIMENTACAO = '6', --Remessa 3M p/ est int.
						@p_ID_OS = NULL,
						@p_DT_MOVIMENTACAO = @DT_MOVIMENTACAO,
						@p_ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M2,
						@p_CD_PECA = @CD_PECA,
						@p_QT_PECA = @QTD_APROVADA_NEGATIVO,
						@p_ID_USU_MOVI = @p_nidUsuarioAtualizacao,
						@p_ID_ESTOQUE_ORIGEM = NULL,
						@p_TP_ENTRADA_SAIDA = 'S',
						@p_CD_CLIENTE = NULL,
						@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
						@p_ID_ESTOQUE_MOVI = @ID_ESTOQUE_MOVI OUTPUT
			END 

			-- **************************************************************************************************************************
			-- ************************************************* LANÇAMENTO DE CRÉDITO **************************************************
			-- **************************************************************************************************************************
			IF (@TP_TIPO_PEDIDO = 'C') -- Pedido para o CLIENTE
			BEGIN
				SET @ID_ESTOQUE_PECA = NULL;
				SET @QT_PECA_ATUAL = NULL;

				-- Busca o ID_ESTOQUE do Cliente
				SELECT @ID_ESTOQUE_CREDITO = ID_ESTOQUE 
				  FROM dbo.tbEstoque 
				 WHERE dbo.tbEstoque.CD_CLIENTE =  @CD_CLIENTE
				   AND dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'CLI' 
				   AND dbo.tbEstoque.FL_ATIVO = 'S'

				IF (@ID_ESTOQUE_CREDITO IS NULL)
				BEGIN
					SET @p_Mensagem = 'Não encontrado estoque para o cliente - ' + CAST(@CD_CLIENTE AS VARCHAR)
					RETURN; 
				END

				-- Busca o ID_ESTOQUE_PECA e QT_PECA_ATUAL em tbEstoquePeca para crédito (entrada) do Cliente
				SELECT @ID_ESTOQUE_PECA	= dbo.tbEstoquePeca.ID_ESTOQUE_PECA,
					   @QT_PECA_ATUAL = dbo.tbEstoquePeca.QT_PECA_ATUAL
				  FROM dbo.tbEstoquePeca INNER JOIN dbo.tbEstoque
					ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
				 WHERE dbo.tbEstoquePeca.CD_PECA = @CD_PECA
				   AND dbo.tbEstoque.CD_CLIENTE	= @CD_CLIENTE
				   AND dbo.tbEstoque.ID_ESTOQUE	= @ID_ESTOQUE_CREDITO

				IF (@QTD_APROVADA_3M1 > 0)
				BEGIN
					-- Faz o cálculo a creditar
					SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) + @QTD_APROVADA_3M1;
				
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
							@p_QT_PECA = @QTD_APROVADA_3M1,
							@p_ID_USU_MOVI = @p_nidUsuarioAtualizacao,
							@p_ID_ESTOQUE_ORIGEM = @ID_ESTOQUE_DEBITO_3M1,
							@p_TP_ENTRADA_SAIDA = 'E',
							@p_CD_CLIENTE = @CD_CLIENTE,
							@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
							@p_ID_ESTOQUE_MOVI = @ID_ESTOQUE_MOVI OUTPUT
				END

				IF (@QTD_APROVADA_3M2 > 0)
				BEGIN
					-- Faz o cálculo a creditar
					SET @QT_PECA_ATUAL = ISNULL(@QT_PECA_ATUAL, 0) + @QTD_APROVADA_3M2;
				
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
							@p_QT_PECA = @QTD_APROVADA_3M2,
							@p_ID_USU_MOVI = @p_nidUsuarioAtualizacao,
							@p_ID_ESTOQUE_ORIGEM = @ID_ESTOQUE_DEBITO_3M2,
							@p_TP_ENTRADA_SAIDA = 'E',
							@p_CD_CLIENTE = @CD_CLIENTE,
							@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
							@p_ID_ESTOQUE_MOVI = @ID_ESTOQUE_MOVI OUTPUT
				END
			END

			FETCH NEXT FROM C1
				INTO @TP_TIPO_PEDIDO, @CD_CLIENTE, @CD_TECNICO, @CD_PECA, @QTD_APROVADA, @QTD_APROVADA_3M1, @QTD_APROVADA_3M2, @ID_ESTOQUE_DEBITO_3M1, @ID_ESTOQUE_DEBITO_3M2--, @VL_PECA
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
GO


