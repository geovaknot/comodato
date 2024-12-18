GO
/****** Object:  StoredProcedure [dbo].[prcFator_PonderacaoUpdate]    Script Date: 25/04/2023 09:27:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex natalino
-- Create date: 21/03/2018
-- Description:	CREATEação de Dados na Tabela de
--              TB_Empresa
-- =============================================
CREATE PROCEDURE [dbo].[prcFator_PonderacaoUpdate]
	@p_Id						numeric(8,0)		= null,
	@p_MIN_CLIENTES				numeric(6,0)		= NULL,
	@p_MAX_CLIENTES				numeric(6,0)		= NULL,
	@p_FATOR					numeric(3,0)		= NULL,
	@p_DATAINCLUSAO				DateTime			= NULL,
	@p_nidUsuario				numeric(8,0)		= NULL,
	@p_Ativo					bit					= null
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

		BEGIN TRANSACTION
		
		
				
		UPDATE	TB_PONDERACAO_pz 
		SET		
				Min_Clientes					= ISNULL(@p_MIN_CLIENTES, MIN_CLIENTES),
				MAX_CLIENTES					= ISNULL(@p_MAX_CLIENTES, MAX_CLIENTES),
				FATOR							= ISNULL(@p_FATOR, FATOR),
				DataInclusao					= ISNULL(@p_DATAINCLUSAO, DataInclusao),
				nidUsuario						= ISNULL(@p_nidUsuario, nidUsuario),
				Ativo							= ISNULL(@p_Ativo, Ativo)
		WHERE	ID				= @p_Id
		         	
		
	
		COMMIT TRANSACTION
	
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

