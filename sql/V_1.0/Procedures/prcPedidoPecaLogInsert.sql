GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaLogInsert]    Script Date: 23/07/2021 09:24:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Inclusão dos dados na tabela
--              TB_PEDIDO_PECA_ÇOG
-- =============================================
CREATE PROCEDURE [dbo].[prcPedidoPecaLogInsert]	
	@p_ID_ITEM_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_QTD_PECA_RECEBIDA					NUMERIC(15,0)	= NULL,
	@p_DATA_RECEBIMENTO						DATETIME    	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0)
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		INSERT INTO dbo.TB_PEDIDO_PECA_LOG
		        ( ID_ITEM_PEDIDO,
		          QTD_PECA_RECEBIDA,
		          DATA_RECEBIMENTO
				)
		VALUES
		        ( @p_ID_ITEM_PEDIDO,
		          @p_QTD_PECA_RECEBIDA,
		          @p_DATA_RECEBIMENTO
		          )
		
		COMMIT
		
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


