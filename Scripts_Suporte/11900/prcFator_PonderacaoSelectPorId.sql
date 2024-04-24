GO
/****** Object:  StoredProcedure [dbo].[prcFator_PonderacaoSelectPorId]    Script Date: 25/04/2023 09:23:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex natalino
-- Create date: 21/03/2018
-- Description:	Seleção de dados na tabela 
--              tbEmpresa
-- =============================================
CREATE PROCEDURE [dbo].[prcFator_PonderacaoSelectPorId]
	@p_Id				numeric(8,0)		= NULL
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
		FROM	TB_PONDERACAO_pz
			Where ID = @p_Id
		ORDER BY
			MIN_CLIENTES			
		
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


