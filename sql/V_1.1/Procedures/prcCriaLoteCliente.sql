GO
/****** Object:  StoredProcedure [dbo].[prcCriaLoteCliente]    Script Date: 26/01/2022 11:41:07 ******/
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
CREATE PROCEDURE [dbo].[prcCriaLoteCliente]
	@p_ID_PEDIDO						NUMERIC(9,0)	= null,
	@p_PecasLote						NVARCHAR(MAX)	= null,
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
