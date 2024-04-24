GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 18/06/2018
-- Description:	Exclusão dos dados na tabela tbPecaOS
-- =============================================
ALTER PROCEDURE [dbo].[prcPecaOSDelete]	
	@p_ID_OS					BIGINT,
	@p_CD_PECA					VARCHAR(15),
	@p_QT_PECA					NUMERIC(15,3),
	@p_CD_TP_ESTOQUE_CLI_TEC	CHAR(1),
	@p_CD_TECNICO				VARCHAR(06),
	@p_CD_CLIENTE				NUMERIC(06),
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_ID_PECA_OS				BIGINT
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

	    EXECUTE dbo.prcLogGravar 
					@p_nidLog					= @nidLog,
					@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
					@p_ccdAcao					= 'D',
					@p_cnmTabela				= 'tbPecaOS',
					@p_nidPK					= @p_ID_PECA_OS,
					@p_nidLogReturn				= @nidLog OUTPUT

		-- Exclui na tbPecaOS
		DELETE FROM dbo.tbPecaOS
		WHERE ID_PECA_OS = @p_ID_PECA_OS

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

GO