GO
/****** Object:  StoredProcedure [dbo].[prcOSPadraoInsert]    Script Date: 26/11/2021 08:41:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcOSPadraoInsert]
	@p_DT_DATA_OS				DATETIME	= NULL,	
	@p_ST_STATUS_OS				INT		= NULL,
	@p_CD_TIPO_OS				INT		= NULL,
	@p_CD_CLIENTE				INT		= NULL,
	@p_CD_ATIVO_FIXO			VARCHAR(06)	= NULL,
	@p_CD_TECNICO				VARCHAR(06)	= NULL,
	@p_HR_INICIO				VARCHAR(5)	= NULL,
	@p_HR_FIM					VARCHAR(5)	= NULL,
	@p_DS_OBSERVACAO			VARCHAR(max)	= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0) = NULL,
	@p_TOKEN    				BIGINT		= NULL,
	@p_NOME_LINHA				VARCHAR(50)	= NULL,
	@p_Email					VARCHAR(50)	= NULL,
	@p_DS_RESPONSAVEL			VARCHAR(60)	= NULL,
	@p_TOKEN_GERADO    		    BIGINT	    OUTPUT,
	@p_ID_OS					BIGINT		OUTPUT

AS
BEGIN

	DECLARE @cdsErrorMessage		NVARCHAR(4000),
			@nidErrorSeverity		INT,
			@nidErrorState			INT,
			@nidLog					NUMERIC(18,0),
			@ID_LOG_STATUS_OS	    BIGINT,
			@TOKEN_REGISTRO_INCLUSAO BIGINT,
			@APLICACAO_ORIGEM_TOKEN BIGINT -- 1 = APP 2 = WEB

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION

		INSERT INTO dbo.tbOSPadrao
		        ( DT_DATA_OS,
		          ST_STATUS_OS,
		          CD_TIPO_OS,
		          CD_CLIENTE,
		          CD_TECNICO,
				  CD_ATIVO_FIXO,
				  HR_INICIO,
				  HR_FIM,
				  DS_OBSERVACAO,
				  nidUsuarioAtualizacao,
		          dtmDataHoraAtualizacao,
				  TOKEN,
				  NOME_LINHA,
				  Email,
				  DS_RESPONSAVEL
				)
		VALUES
		        ( @p_DT_DATA_OS, 
		          @p_ST_STATUS_OS,
		          @p_CD_TIPO_OS,
		          @p_CD_CLIENTE,
		          @p_CD_TECNICO,
				  @p_CD_ATIVO_FIXO,
				  @p_HR_INICIO,
				  @p_HR_FIM,
				  @p_DS_OBSERVACAO,
				  @p_nidUsuarioAtualizacao,
		          GETDATE(),
				  IIF(LEFT(@p_TOKEN, 1) = 1, @p_TOKEN, 0),
				  @p_NOME_LINHA,
				  @p_Email,
				  @p_DS_RESPONSAVEL
		          )

		SET @p_ID_OS = @@IDENTITY

		SET @APLICACAO_ORIGEM_TOKEN = LEFT(@p_TOKEN, 1);

		IF (@p_ID_OS > 0 AND @APLICACAO_ORIGEM_TOKEN = 2) -- 2 = ORIGEM Applicação WEB
		BEGIN
			SET @TOKEN_REGISTRO_INCLUSAO = CAST((CAST(@p_TOKEN AS nvarchar(MAX)) + CAST(@p_ID_OS AS nvarchar(MAX))) AS BIGINT);
			
			UPDATE dbo.tbOSPadrao
			   SET TOKEN = @TOKEN_REGISTRO_INCLUSAO
			 WHERE ID_OS = @p_ID_OS

			SET @p_TOKEN_GERADO = @TOKEN_REGISTRO_INCLUSAO;
		END
		ELSE
		BEGIN
			SET @p_TOKEN_GERADO = @p_TOKEN;
		END

		EXECUTE dbo.prcLogGravar 
					@p_nidLog			= @nidLog,
					@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
					@p_ccdAcao			= 'I',
					@p_cnmTabela			= 'tbOSPadrao',
					@p_nidPK			= @p_ID_OS,
					@p_nidLogReturn			= @nidLog OUTPUT
	
		EXECUTE dbo.prcLogStatusOSPadraoInsert 
		    @p_ID_OS				= @p_ID_OS,
		    @p_DT_DATA_LOG_OS		= @p_DT_DATA_OS,
		    @p_ST_STATUS_OS			= @p_ST_STATUS_OS,
		    @p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
		    @p_ID_LOG_STATUS_OS		= @ID_LOG_STATUS_OS OUTPUT


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

