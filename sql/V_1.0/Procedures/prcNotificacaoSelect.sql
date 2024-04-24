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
CREATE PROCEDURE [dbo].[prcNotificacaoSelect]
	@p_Lida					bit	= NULL,
	@p_DataInclusao  		DATETIME = NULL,
	@p_IdUsuario			bigint = NULL,
	@p_IdNotificacao    	bigint = NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	SET NOCOUNT ON;

	BEGIN TRY
		
		SELECT	*
  		  FROM	tbNotificacao
		WHERE (	tbNotificacao.ID_NOTIFICACAO = @p_IdNotificacao OR @p_IdNotificacao IS NULL )
		AND	  (	tbNotificacao.DATA_INCLUSAO >= @p_DataInclusao OR @p_DataInclusao IS NULL )
		AND	  (	tbNotificacao.ID_USUARIO = @p_IdUsuario OR @p_IdUsuario IS NULL )
		AND	  (	tbNotificacao.LIDA = @p_Lida OR @p_Lida IS NULL )
		ORDER BY
				tbNotificacao.DATA_INCLUSAO
		
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


GO