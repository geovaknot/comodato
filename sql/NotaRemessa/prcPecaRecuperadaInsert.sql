GO
/****** Object:  StoredProcedure [dbo].[prcPecaRecuperadaInsert]    Script Date: 20/07/2022 15:38:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_PECA
-- =============================================
CREATE PROCEDURE [dbo].[prcPecaRecuperadaInsert]
	@p_CD_PECA							VARCHAR(15) = null,
	@p_CD_PECA_RECUPERADA				VARCHAR(15) = null
	
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
		
		UPDATE tbPecaRecuperada
            SET CD_PECA_RECUPERADA = @p_CD_PECA_RECUPERADA
            WHERE CD_PECA = @p_CD_PECA
        IF @@ROWCOUNT = 0 
        BEGIN
            INSERT INTO tbPecaRecuperada 
                    ( CD_PECA, CD_PECA_RECUPERADA
                    )
            VALUES ( @p_CD_PECA, @p_CD_PECA_RECUPERADA
                    )
        END 
		
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


