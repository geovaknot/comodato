GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Inclusão dos dados na tabela
--              tbPendenciaOS
-- =============================================
ALTER PROCEDURE [dbo].[prcPendenciaOSInsert]	
	@p_ID_OS							BIGINT			= NULL,
	@p_DT_ABERTURA						DATETIME		= NULL,
	@p_DS_DESCRICAO						VARCHAR(MAX)	= NULL,
	@p_CD_PECA							VARCHAR(15)		= NULL,
	@p_CD_TECNICO						VARCHAR(06)		= NULL,
	@p_QT_PECA							NUMERIC(15,3)	= NULL,
	@p_ST_STATUS_PENDENCIA				CHAR(1)			= NULL,
	@p_CD_TP_ESTOQUE_CLI_TEC			CHAR(1)			= NULL,
	@p_ST_TP_PENDENCIA					CHAR(1)			= NULL,
	@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL,
	@p_TOKEN    						BIGINT			= NULL,
	@p_TOKEN_GERADO    					BIGINT		    OUTPUT,
	@p_ID_PENDENCIA_OS					BIGINT			OUTPUT
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog				NUMERIC(18,0),
			@TOKEN_REGISTRO_INCLUSAO BIGINT,
			@APLICACAO_ORIGEM_TOKEN BIGINT -- 1 = APP 2 = WEB

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		INSERT INTO dbo.tbPendenciaOS
		        ( ID_OS,
		          DT_ABERTURA,
		          DS_DESCRICAO,
		          CD_PECA,
		          CD_TECNICO,
		          QT_PECA,
		          ST_STATUS_PENDENCIA,
				  CD_TP_ESTOQUE_CLI_TEC,
				  ST_TP_PENDENCIA,
		          nidUsuarioAtualizacao,
		          dtmDataHoraAtualizacao,
				  TOKEN
				)
		VALUES
		        ( @p_ID_OS,
		          @p_DT_ABERTURA,
		          @p_DS_DESCRICAO,
		          @p_CD_PECA,
		          @p_CD_TECNICO,
		          @p_QT_PECA,
		          @p_ST_STATUS_PENDENCIA,
				  @p_CD_TP_ESTOQUE_CLI_TEC,
				  @p_ST_TP_PENDENCIA,
		          @p_nidUsuarioAtualizacao,
		          GETDATE(),
				  IIF(LEFT(@p_TOKEN, 1) = 1, @p_TOKEN, 0)
		          )

		SET @p_ID_PENDENCIA_OS = @@IDENTITY
		
		SET @APLICACAO_ORIGEM_TOKEN = LEFT(@p_TOKEN, 1);

		IF (@p_ID_PENDENCIA_OS > 0 AND @APLICACAO_ORIGEM_TOKEN = 2) -- 2 = ORIGEM Applicação WEB
		BEGIN
			SET @TOKEN_REGISTRO_INCLUSAO = CAST((CAST(@p_TOKEN AS nvarchar(MAX)) + CAST(@p_ID_PENDENCIA_OS AS nvarchar(MAX))) AS BIGINT);
			
			UPDATE dbo.tbPendenciaOS
			   SET TOKEN = @TOKEN_REGISTRO_INCLUSAO
			 WHERE ID_PENDENCIA_OS = @p_ID_PENDENCIA_OS

			SET @p_TOKEN_GERADO = @TOKEN_REGISTRO_INCLUSAO;
		END
		ELSE
		BEGIN
			SET @p_TOKEN_GERADO = @p_TOKEN;
		END
	
		EXECUTE dbo.prcLogGravar 
					@p_nidLog					= @nidLog,
					@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
					@p_ccdAcao					= 'I',
					@p_cnmTabela				= 'tbPendenciaOS',
					@p_nidPK					= @p_ID_PENDENCIA_OS,
					@p_nidLogReturn				= @nidLog OUTPUT
	
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