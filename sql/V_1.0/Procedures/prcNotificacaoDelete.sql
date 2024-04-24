GO
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- ========================================================
-- Author:		Edgar Coutinho
-- Create date: 23/06/2021
-- Description:	Inclusão de dados na tabela tbNotificacao
-- ========================================================
CREATE PROCEDURE [dbo].[prcNotificacaoDelete]
	@p_IdNotificacao bigint,
	@p_IdUsuario bigint = NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0)

	SET NOCOUNT ON;

	BEGIN TRY

	    EXECUTE dbo.prcLogGravar 
					@p_nidLog					= @nidLog,
					@p_nidUsuarioAtualizacao	= @p_IdUsuario,
					@p_ccdAcao					= 'D',
					@p_cnmTabela				= 'tbNotificacao',
					@p_nidPK					= @p_IdNotificacao,
					@p_nidLogReturn				= @nidLog OUTPUT

		DELETE FROM tbNotificacao
		 WHERE ID_NOTIFICACAO = @p_IdNotificacao

	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH

END

GO


