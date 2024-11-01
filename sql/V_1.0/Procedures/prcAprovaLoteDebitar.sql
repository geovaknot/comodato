GO
/****** Object:  StoredProcedure [dbo].[prcAprovaLoteDebitar]    Script Date: 01/10/2021 16:15:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Andre Farinelli
-- Create date: 06/11/2018
-- Description:	Gerar lote de aprovação de peças
-- de uma solicitação
-- =============================================
ALTER PROCEDURE [dbo].[prcAprovaLoteDebitar]
	@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_PecasLote						NVARCHAR(MAX)	= NULL,
	@p_PecasLoteAP						NVARCHAR(MAX)	= NULL,
	@p_FlagAtualizaUn					CHAR(1)			= NULL,
	@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL,
	@p_Mensagem							VARCHAR(8000)	OUTPUT
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0),
			@ID_ITEM_PEDIDO		BIGINT,
			@ID_ESTOQUE_DEBITO_3M1	BIGINT,
			@ID_ESTOQUE_DEBITO_3M2	BIGINT,
			@QTD_SOLICITADA		NUMERIC(15,3),
			@CD_PECA			VARCHAR(15),
			@QTD_PECA_3M1		NUMERIC(15,3),
			@QTD_PECA_3M2		NUMERIC(15,3),
			
			@TP_TIPO_PEDIDO 	CHAR(1),
			@ID_ESTOQUE_PECA 	INT,
			@QT_PECA_ATUAL		INT,
			@ID_ESTOQUE_CREDITO INT,
			@CD_CLIENTE 		INT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--SET FMTONLY OFF;
	--SET XACT_ABORT ON;

	BEGIN TRY

		--BEGIN TRANSACTION
  
  		
  		SET @TP_TIPO_PEDIDO = (SELECT TOP 1 TP_TIPO_PEDIDO FROM TB_PEDIDO WHERE ID_PEDIDO = @p_ID_PEDIDO);
  
  		IF (@TP_TIPO_PEDIDO = 'C')
		BEGIN
			
			SET @CD_CLIENTE = (SELECT TOP 1 CD_CLIENTE FROM TB_PEDIDO WHERE ID_PEDIDO = @p_ID_PEDIDO);
		
			SET @ID_ESTOQUE_PECA = NULL;
			SET @QT_PECA_ATUAL = NULL;

			-- Busca o ID_ESTOQUE do Cliente
			SELECT @ID_ESTOQUE_CREDITO = ID_ESTOQUE 
			FROM dbo.tbEstoque 
			WHERE	dbo.tbEstoque.CD_CLIENTE =  @CD_CLIENTE
			AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'CLI' 
			AND		dbo.tbEstoque.FL_ATIVO			= 'S'

			IF (@ID_ESTOQUE_CREDITO IS NULL)
			BEGIN
				SET @p_Mensagem = 'Não encontrado estoque para o cliente - ' + CAST(@CD_CLIENTE AS VARCHAR)
				--ROLLBACK TRANSACTION 
				RETURN; 
			END
  		END
  
		--Iniciar variáveis:
		SET @ID_ITEM_PEDIDO			= 0;
		SET @ID_ESTOQUE_DEBITO_3M1	= 0;
		SET @ID_ESTOQUE_DEBITO_3M2	= 0;
		SET @QTD_SOLICITADA			= 0;
		SET @CD_PECA				= 0;
		SET @QTD_PECA_3M1			= 0;
		SET @QTD_PECA_3M2			= 0;

		-- Cria lote na tabela tbLoteAprovacao
		DECLARE	@p_ID_LOTE_APROVACAO bigint,
				@p_DATA_ATUAL DATETIME

		SET @p_DATA_ATUAL = DATEADD(HH,-3,GETUTCDATE());

		IF (@p_FlagAtualizaUn is null)
		BEGIN			
			EXEC	[dbo].[prcLoteInsert]
					@p_ID_USUARIO = @p_nidUsuarioAtualizacao,
					@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
					@p_dtmDataHoraAtualizacao = @p_DATA_ATUAL,
					@p_ID_LOTE_APROVACAO = @p_ID_LOTE_APROVACAO OUTPUT


			UPDATE	TB_PEDIDO_PECA
			SET				
				ID_LOTE_APROVACAO	= @p_ID_LOTE_APROVACAO
			WHERE dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_PecasLoteAP, ','))
				AND dbo.TB_PEDIDO_PECA.ID_PEDIDO = @p_ID_PEDIDO		
		END

		-- Tratar aprovações automaticamente
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT	dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO,
				dbo.TB_PEDIDO_PECA.QTD_SOLICITADA, 
				dbo.TB_PEDIDO_PECA.CD_PECA
		FROM dbo.TB_PEDIDO_PECA (nolock)
		WHERE dbo.TB_PEDIDO_PECA.ID_PEDIDO = @p_ID_PEDIDO
		--AND dbo.TB_PEDIDO_PECA.QTD_APROVADA IS NULL
		AND dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO IS NULL
		AND dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2 IS NULL
		AND dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M1 IS NULL
		AND dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M2 IS NULL
		AND dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_PecasLote, ','))
		
		OPEN C1
		FETCH NEXT FROM C1
			INTO @ID_ITEM_PEDIDO, @QTD_SOLICITADA, @CD_PECA

		WHILE @@FETCH_STATUS = 0
		BEGIN
			
			EXECUTE dbo.prcLogGravar 
				@p_nidLog = @nidLog,
				@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
				@p_ccdAcao = 'U',
				@p_cnmTabela = 'TB_PEDIDO_PECA',
				@p_nidPK = @ID_ITEM_PEDIDO,
				@p_nidLogReturn = @nidLog OUTPUT
		 
		 --Existe qtd nos estoques 3M?

			SELECT TOP 1 @ID_ESTOQUE_DEBITO_3M1 = ID_ESTOQUE 
			FROM tbEstoque (nolock) 
			WHERE TP_ESTOQUE_TEC_3M = '3M1' AND FL_ATIVO = 'S'

			SELECT TOP 1 @QTD_PECA_3M1 = QT_PECA_ATUAL 
			FROM tbEstoquePeca (nolock)
			WHERE ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M1 AND CD_PECA = @CD_PECA

			IF (@QTD_PECA_3M1 >= @QTD_SOLICITADA)
			BEGIN
				UPDATE TB_PEDIDO_PECA SET
					ID_ESTOQUE_DEBITO	= ISNULL(@ID_ESTOQUE_DEBITO_3M1,0),
					QTD_APROVADA		= ISNULL(@QTD_SOLICITADA,0),
					QTD_APROVADA_3M1	= ISNULL(@QTD_SOLICITADA,0),
					TX_APROVADO			= 'S',
					ST_STATUS_ITEM		= 3 --Aprovado
				WHERE ID_ITEM_PEDIDO	= @ID_ITEM_PEDIDO
			END
			ELSE
			BEGIN
				--Caso não tenha qtd suficiente em 3M1 para qlqr item solicitado, abortar lote:

				SET @p_Mensagem = 'O estoque 3M1-F4, não possui peças suficientes para aprovação!';
					--ROLLBACK TRANSACTION 
					RETURN;


				--CASO QUEIRA PEGAR O MAXIMO DE PEÇAS POSSÍVEIS EM AMBOS ESTOQUES PARA ATINGIR SOLICITAÇÃO HABILITAR LINHAS:
				--IF (@QTD_PECA_3M1 > 0)
				--BEGIN
				--	UPDATE TB_PEDIDO_PECA SET
				--		ID_ESTOQUE_DEBITO	= ISNULL(@ID_ESTOQUE_DEBITO_3M1,0),
				--		QTD_APROVADA		= ISNULL(@QTD_PECA_3M1,0),
				--		QTD_APROVADA_3M1	= ISNULL(@QTD_PECA_3M1,0),
				--		TX_APROVADO			= 'S',						
				--		ST_STATUS_ITEM		= 3 --Aprovado
				--	WHERE ID_ITEM_PEDIDO	= @ID_ITEM_PEDIDO
				--END
				
				--SET @QTD_SOLICITADA = @QTD_SOLICITADA - ISNULL(@QTD_PECA_3M1,0);

				--SELECT TOP 1 @ID_ESTOQUE_DEBITO_3M2 = ID_ESTOQUE 
				--FROM tbEstoque (nolock) 
				--WHERE TP_ESTOQUE_TEC_3M = '3M2' AND FL_ATIVO = 'S'

				--SELECT TOP 1 @QTD_PECA_3M2 = tbEstoquePeca.QT_PECA_ATUAL 
				--FROM tbEstoquePeca (nolock)
				--WHERE tbEstoquePeca.ID_ESTOQUE = @ID_ESTOQUE_DEBITO_3M2 AND CD_PECA = @CD_PECA
				
				--IF (@QTD_PECA_3M2 >= @QTD_SOLICITADA)
				--BEGIN
				--	UPDATE TB_PEDIDO_PECA SET
				--		ID_ESTOQUE_DEBITO_3M2	= ISNULL(@ID_ESTOQUE_DEBITO_3M2,0),
				--		QTD_APROVADA		= (ISNULL(@QTD_PECA_3M1,0) + ISNULL(@QTD_SOLICITADA,0)),
				--		QTD_APROVADA_3M2	= ISNULL(@QTD_SOLICITADA,0),						
				--		TX_APROVADO			= 'S',
				--		ST_STATUS_ITEM		= 3
				--	WHERE ID_ITEM_PEDIDO	= @ID_ITEM_PEDIDO
				--END
				--ELSE
				--BEGIN
				--	IF (@QTD_PECA_3M2 > 0)
				--	BEGIN
				--		UPDATE TB_PEDIDO_PECA SET
				--			ID_ESTOQUE_DEBITO_3M2	= ISNULL(@ID_ESTOQUE_DEBITO_3M2,0),
				--			QTD_APROVADA		= (ISNULL(@QTD_PECA_3M1,0) + ISNULL(@QTD_PECA_3M2,0)),
				--			QTD_APROVADA_3M2	= ISNULL(@QTD_PECA_3M2,0),							
				--			TX_APROVADO			= 'S',
				--			ST_STATUS_ITEM		= 3
				--		WHERE ID_ITEM_PEDIDO	= @ID_ITEM_PEDIDO
				--	END
				--END
			END

			DECLARE @checarQtd INT

			SET @checarQtd =
			(SELECT	dbo.TB_PEDIDO_PECA.QTD_APROVADA
			FROM dbo.TB_PEDIDO_PECA (nolock)
			WHERE dbo.TB_PEDIDO_PECA.ID_PEDIDO = @p_ID_PEDIDO
				AND dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO = @ID_ITEM_PEDIDO);

			IF (@checarQtd is null OR @checarQtd < 1)
			BEGIN
				UPDATE TB_PEDIDO_PECA SET
							ST_STATUS_ITEM		= 4, --Cancelado
							QTD_APROVADA = 0,
							QTD_RECEBIDA = 0,
							TX_APROVADO = 'N'
						WHERE ID_ITEM_PEDIDO	= @ID_ITEM_PEDIDO
			END
		         	
			EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'TB_PEDIDO_PECA',
				@p_nidPK					= @ID_ITEM_PEDIDO,
				@p_nidLogReturn				= @nidLog OUTPUT
				
			FETCH NEXT FROM C1
				INTO @ID_ITEM_PEDIDO, @QTD_SOLICITADA, @CD_PECA;
		

		--Correção 8/4/19 erro na aprov avulsa e status aprov manual:
		--UPDATE dbo.TB_PEDIDO_PECA 
		--SET ST_STATUS_ITEM = 3,
		--	TX_APROVADO = 'S'
		--WHERE dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_PecasLote, ','))
		--		AND dbo.TB_PEDIDO_PECA.ID_PEDIDO = @p_ID_PEDIDO 
		--		AND dbo.TB_PEDIDO_PECA.ST_STATUS_ITEM IN ('1', '2', '3') --Atualiza p aprov os status aguard ou pend
		--		AND QTD_APROVADA > 0


		END;
		CLOSE C1;
		DEALLOCATE C1;
		SET @p_Mensagem = '';
    
	
		--Correção 8/4/19 erro na aprov avulsa e status aprov manual:
		UPDATE dbo.TB_PEDIDO_PECA 
		SET ST_STATUS_ITEM = 3,
			TX_APROVADO = 'S'
		WHERE dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_PecasLote, ','))
				AND dbo.TB_PEDIDO_PECA.ID_PEDIDO = @p_ID_PEDIDO 
				AND dbo.TB_PEDIDO_PECA.ST_STATUS_ITEM IN ('1', '2', '3', 1, 2, 3) --Atualiza p aprov os status aguard ou pend
				AND QTD_APROVADA > 0
	    
		--COMMIT TRANSACTION
	
	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		--ROLLBACK TRANSACTION

		-- Use RAISERROR inside the CATCH block to return error
		-- information about the original error that caused
		-- execution to jump to the CATCH block.
		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				  @nidErrorState -- State.
				   )

	END CATCH

END
