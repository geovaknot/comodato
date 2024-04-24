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
CREATE PROCEDURE [dbo].[prcNotificacaoUpdate]
	@p_Mensagem				NVARCHAR(MAX) = NULL,
	@p_Titulo				VARCHAR(150) = NULL,
	@p_Lida					bit	= NULL,
	@p_DataInclusao  		DATETIME = NULL,
	@p_IdUsuario			bigint = NULL,
	@p_IdNotificacao    	bigint = NULL
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
		
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_IdUsuario,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'tbNotificacao',
				@p_nidPK					= @p_IdNotificacao,
				@p_nidLogReturn				= @nidLog OUTPUT
				
		UPDATE	tbNotificacao
		SET		TITULO = @p_Titulo,
				MENSAGEM = @p_Mensagem,
				LIDA = @p_Lida
		WHERE	ID_NOTIFICACAO = @p_IdNotificacao
		         	
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_IdUsuario,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'tbNotificacao',
				@p_nidPK					= @p_IdNotificacao,
				@p_nidLogReturn				= @nidLog OUTPUT
	
		COMMIT TRANSACTION
	
	END TRY

	BEGIN CATCH

		SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
				@nidErrorSeverity	= ERROR_SEVERITY(),
				@nidErrorState		= ERROR_STATE();

		ROLLBACK TRANSACTION

		RAISERROR (@cdsErrorMessage, -- Message text.
				   @nidErrorSeverity, -- Severity.
				   @nidErrorState -- State.
				   )

	END CATCH

END

GO


