GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		André Farinelli
-- Create date: 19/03/2018
-- Description:	Atualiza o status de um pedido
-- =============================================
ALTER PROCEDURE [dbo].[prcAtualizaStatusPedido]
	@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0)


            /*STATUS PEDIDO:										|	STATUS ITEM:
            1	Novo/Rascunho			-							|	1-Novo/Rascunho
            2	Solicitado				Solicitar à 3M				|	2-Pendente
            3	Aprovado				Aprovar Itens				|	3-Aprovado
            4	Recebido				Confirmar Recebimento		|	4-Cancelado
            5	Pendente				Registrar Pendências		|	5-Recebido
            6	Recebido com Pendência	Registrar Peças Pendentes   |   6-Solicitado
            7	Cancelado				Cancelar Itens              |   7-Recebido com Pendência
            */

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		--BEGIN TRANSACTION
		
		EXECUTE dbo.prcLogGravar 
		    @p_nidLog = @nidLog,
		    @p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
		    @p_ccdAcao = 'U',
		    @p_cnmTabela = 'TB_PEDIDO',
		    @p_nidPK = @p_ID_PEDIDO,
		    @p_nidLogReturn = @nidLog OUTPUT
		 
		 DECLARE @AguardandoOuPendente INT,
				@Aprovado INT,
				@Cancelado INT,
				@Recebido INT,
				@NovoStatusPedido INT,
				@Checar INT,
				@RecebidoPendecia INT

		SET @NovoStatusPedido = 0

		SET @AguardandoOuPendente = 
			(SELECT TOP 1 COUNT(ST_STATUS_ITEM) 
			   FROM TB_PEDIDO_PECA 
			  WHERE ID_PEDIDO = @p_ID_PEDIDO
			    AND (ST_STATUS_ITEM = 1 OR ST_STATUS_ITEM = 2))

		SET @Aprovado = 
			(SELECT TOP 1 COUNT(ST_STATUS_ITEM)
			   FROM TB_PEDIDO_PECA 
	   		  WHERE ID_PEDIDO = @p_ID_PEDIDO
				AND (ST_STATUS_ITEM = 3))

		SET @Cancelado = 
			(SELECT TOP 1 COUNT(ST_STATUS_ITEM) 
				FROM TB_PEDIDO_PECA 
			   WHERE ID_PEDIDO = @p_ID_PEDIDO
				 AND (ST_STATUS_ITEM = 4))

		SET @Recebido = 
			(SELECT TOP 1 (ST_STATUS_ITEM) 
			   FROM TB_PEDIDO_PECA 
			  WHERE ID_PEDIDO = @p_ID_PEDIDO
				AND (ST_STATUS_ITEM = 5))

		SET @RecebidoPendecia = 
			(SELECT TOP 1 (ST_STATUS_ITEM) 
	    	   FROM TB_PEDIDO_PECA 
			  WHERE ID_PEDIDO = @p_ID_PEDIDO
			    AND (ST_STATUS_ITEM = 7))

		SET @Checar = 
		(SELECT TOP 1 ID_LOTE_APROVACAO
		FROM TB_PEDIDO_PECA 
		WHERE ID_PEDIDO = @p_ID_PEDIDO AND ID_LOTE_APROVACAO IS NOT NULL)
		
		--Já será um pedido pendente
		IF (@checar > 0)
		BEGIN
			SET @NovoStatusPedido = 5 --Pendente 
		END

		--se ( ap > N && c = N && ag = N && p = N && r = N ) { APROVADO
		IF (@Aprovado > 0 
			AND (@Cancelado < 1 or @Cancelado is null)
			AND (@AguardandoOuPendente < 1 or @AguardandoOuPendente is null)
			AND (@Recebido < 1 or @Recebido is null))
			BEGIN
				SET @NovoStatusPedido = 3
			END

			ELSE
			BEGIN

		--se ( ap > N && c > N && ag = N && p = N && r = N ) { APROVADO
		IF (@Aprovado > 0 
			AND @Cancelado > 0 
			AND (@AguardandoOuPendente < 1 or @AguardandoOuPendente is null)
			AND (@Recebido < 1 or @Recebido is null))
			BEGIN
				SET @NovoStatusPedido = 3
			END

			ELSE
			BEGIN

		--se ( ap > N && c > N && ag = N && p = N && r = N ) { APROVADO
		IF (@Aprovado > 0 
			AND (@Cancelado < 1 or @Cancelado is null)
			AND (@AguardandoOuPendente < 1 or @AguardandoOuPendente is null)
			AND @Recebido > 0)
			BEGIN
				SET @NovoStatusPedido = 3
			END

			ELSE
			BEGIN

		--se ( ap = N && c > N && ag = N && p = N && r = N ) { CANCELADO
		IF ((@Aprovado < 1 or @Aprovado is null)
			AND @Cancelado > 0 
			AND (@AguardandoOuPendente < 1 or @AguardandoOuPendente is null)
			AND (@Recebido < 1 or @Recebido is null))
			BEGIN
				SET @NovoStatusPedido = 7
			END

			ELSE
			BEGIN

		--se ( ap = N && c = N && ag = N && p = N && r > N ) { RECEBIDO
		--se ( ap = N && c > N && ag = N && p = N && r > N ) { RECEBIDO
		IF ((@Aprovado < 1 or @Aprovado is null) 
			AND (@AguardandoOuPendente < 1 or @AguardandoOuPendente is null)
			AND @Recebido > 0)
			BEGIN
				SET @checar =
				(SELECT 
				SUM(ISNULL(QTD_APROVADA,0) - ISNULL(QTD_RECEBIDA,0)) 
				FROM TB_PEDIDO_PECA WHERE ID_PEDIDO = @p_ID_PEDIDO)
				
				IF (@checar > 0)
				BEGIN
					SET @NovoStatusPedido = 6 --Recebido com pendencia 
				END
				ELSE
				BEGIN 
					SET @NovoStatusPedido = 4 --Recebido
				END
			END

			ELSE
			BEGIN

		IF (@RecebidoPendecia > 0)
			BEGIN
				SET @NovoStatusPedido = 6 --Recebido com pendencia 
			END
		END
				
		END
		END
		END
		END

		IF (@NovoStatusPedido > 0)
		BEGIN

			UPDATE	TB_PEDIDO
			SET				
				ID_STATUS_PEDIDO = @NovoStatusPedido
			WHERE ID_PEDIDO	= @p_ID_PEDIDO

		END
		         	
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela = 'TB_PEDIDO',
				@p_nidPK = @p_ID_PEDIDO,
				@p_nidLogReturn				= @nidLog OUTPUT
	
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
GO