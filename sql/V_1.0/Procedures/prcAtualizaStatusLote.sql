GO
/****** Object:  StoredProcedure [dbo].[prcAtualizaStatusLote]    Script Date: 05/10/2021 14:53:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		André Farinelli
-- Create date: 08/11/2018
-- Description:	Update dos ST_STATUS_ITEM na tabela
--               TB_PEDIDO_PECA por lote
-- =============================================
ALTER PROCEDURE [dbo].[prcAtualizaStatusLote]
	@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_ST_STATUS						INT				= NULL,
	@p_LotePecas						NVARCHAR(MAX)	= NULL,
	@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0),
			@ID_ITEM_PEDIDO		BIGINT,
			@QTD_APROVADA		NUMERIC(15,3),
			@QTD_RECEBIDA		NUMERIC(15,3),

			@STATUS_ITEM_PENDENTE TINYINT = 2,
			@STATUS_ITEM_CANCELADO TINYINT = 4,
			@STATUS_ITEM_RECEBIDO TINYINT = 5,
			@STATUS_ITEM_SOLICITADO TINYINT = 6,
			@STATUS_ITEM_RECEBIDO_PENDENCIA TINYINT = 7

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
  
			--EXECUTE dbo.prcLogGravar 
			--	@p_nidLog = @nidLog,
			--	@p_nidUsuarioAtualizacao = @p_nidUsuarioAtualizacao,
			--	@p_ccdAcao = 'U',
			--	@p_cnmTabela = 'TB_PEDIDO_PECA',
			--	@p_nidPK = @ID_ITEM_PEDIDO,
			--	@p_nidLogReturn = @nidLog OUTPUT
			
			IF ( @p_ST_STATUS = 2 ) --Solicitar
			BEGIN
				UPDATE TB_PEDIDO_PECA 
				   SET ST_STATUS_ITEM = CASE WHEN (QTD_SOLICITADA = 0 OR QTD_SOLICITADA IS NULL)
				                              THEN @STATUS_ITEM_CANCELADO ELSE @STATUS_ITEM_SOLICITADO 
										 END
				 WHERE ID_PEDIDO = @p_ID_PEDIDO
				   AND (@p_LotePecas IS NULL OR dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_LotePecas, ',')))
			END	
			ELSE
			BEGIN
				IF ( @p_ST_STATUS = 4 ) --Receber
				BEGIN
					UPDATE TB_PEDIDO_PECA 
					   SET ST_STATUS_ITEM = CASE 
												WHEN (QTD_APROVADA > 0 AND QTD_RECEBIDA >= QTD_APROVADA) THEN @STATUS_ITEM_RECEBIDO
												WHEN (QTD_APROVADA > 0 AND QTD_RECEBIDA > 0 AND QTD_RECEBIDA < QTD_APROVADA) THEN @STATUS_ITEM_RECEBIDO_PENDENCIA
											 END
					 WHERE ID_PEDIDO = @p_ID_PEDIDO
					   AND dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_LotePecas, ','))
				END	
				ELSE
				BEGIN
					IF ( @p_ST_STATUS = 6 ) --Receber com pendência
					BEGIN
						UPDATE TB_PEDIDO_PECA 
						   SET ST_STATUS_ITEM = CASE WHEN (QTD_APROVADA > 0 AND QTD_RECEBIDA > 0 AND QTD_RECEBIDA < QTD_APROVADA) 
												      THEN @STATUS_ITEM_RECEBIDO_PENDENCIA
													  WHEN (QTD_APROVADA > 0 AND QTD_RECEBIDA >= QTD_APROVADA) THEN @STATUS_ITEM_RECEBIDO
												 END
						 WHERE ID_PEDIDO = @p_ID_PEDIDO
						   AND dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_LotePecas, ','))
					END
					ELSE
					BEGIN
						IF ( @p_ST_STATUS = 7 ) --Cancelar
						BEGIN
							UPDATE TB_PEDIDO_PECA 
							   SET ST_STATUS_ITEM = @STATUS_ITEM_CANCELADO,
								   QTD_APROVADA = 0
							 WHERE ID_PEDIDO = @p_ID_PEDIDO
							   AND dbo.TB_PEDIDO_PECA.CD_PECA IN (SELECT cdsString FROM dbo.fncGetValuesByString(@p_LotePecas, ','))
						END
					END
				END	
			END

			--EXECUTE dbo.prcLogGravar 
			--	@p_nidLog					= @nidLog,
			--	@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
			--	@p_ccdAcao					= 'U',
			--	@p_cnmTabela				= 'TB_PEDIDO_PECA',
			--	@p_nidPK					= @ID_ITEM_PEDIDO,
			--	@p_nidLogReturn				= @nidLog OUTPUT
        
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

