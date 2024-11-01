GO
/****** Object:  StoredProcedure [dbo].[prcTB_PONDERACAODelete]    Script Date: 25/04/2023 15:17:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino 
-- Create date: 21/03/2018
-- Description:	Exclusão de Dados na Tabela
--              TB_Empresa
-- =============================================
CREATE PROCEDURE [dbo].[prcTB_PONDERACAODelete]
	@p_ID				NUMERIC(8,0)
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

	    
		DELETE	FROM TB_PONDERACAO_pz
		WHERE	ID	= @p_ID

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

