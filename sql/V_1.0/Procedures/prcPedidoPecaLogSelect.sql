GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaLogSelect]    Script Date: 23/07/2021 10:10:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_PEDIDO_PECA_LOG
-- =============================================
CREATE PROCEDURE [dbo].[prcPedidoPecaLogSelect]
	@p_ID_ITEM_PEDIDO						NUMERIC(9,0)
	

AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@TP_TIPO_PEDIDO		CHAR(1),
			@CD_TECNICO			VARCHAR(06)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		select * from TB_PEDIDO_PECA_LOG (NOLOCK) where ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO

		
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



