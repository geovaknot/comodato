GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaUpdateEstTecnico]    Script Date: 24/05/2022 15:24:09 ******/
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
ALTER PROCEDURE [dbo].[prcPedidoPecaUpdateEstTecnico]
	@p_ID_ITEM_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_QTD_PECA								NUMERIC(9,0)	= NULL
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
			@CD_PECA			varchar(15)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		update TB_PEDIDO_PECA
			set Estoque_Tec_Aprov = @p_QTD_PECA
		where ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO
	
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


