GO
/****** Object:  StoredProcedure [dbo].[prcNotasPedidoSelect]    Script Date: 19/07/2022 09:28:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 02/03/2011
-- Description:	Seleção de dados na tabela 
--              tbMensagem
-- =============================================
CREATE PROCEDURE [dbo].[prcNotasPedidoSelect]
	@p_ID_PEDIDO				NUMERIC(9,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		SELECT	*
		FROM	TB_PEDIDO_BPCS_NF
		
		WHERE (	TB_PEDIDO_BPCS_NF.ID_PEDIDO				= @p_ID_PEDIDO				OR @p_ID_PEDIDO				IS NULL )
				
		ORDER BY
				NR_CONTROL ASC			
		
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


