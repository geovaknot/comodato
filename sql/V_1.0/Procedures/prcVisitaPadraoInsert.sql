GO
/****** Object:  StoredProcedure [dbo].[prcVisitaPadraoInsert]    Script Date: 26/11/2021 08:51:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcVisitaPadraoInsert]
	@p_DT_DATA_VISITA			DATETIME		= NULL,	
	@p_ST_STATUS_VISITA			INT				= NULL,
	@p_CD_CLIENTE				INT				= NULL,
	@p_CD_TECNICO				VARCHAR(06)		= NULL,
	@p_DS_OBSERVACAO			VARCHAR(MAX)	= NULL,
	@p_HR_INICIO				VARCHAR(5)		= NULL,
	@p_HR_FIM					VARCHAR(5)		= NULL,
	@p_CD_MOTIVO_VISITA			INT				= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_TOKEN    				BIGINT			= NULL,
	@p_Email					VARCHAR(50)		= NULL,
	@p_DS_RESPONSAVEL			VARCHAR(60) 	= NULL,
	@p_TOKEN_GERADO    		    BIGINT		    OUTPUT,
	@p_ID_VISITA				BIGINT			OUTPUT

AS
BEGIN

	DECLARE @cdsErrorMessage		NVARCHAR(4000),
			@nidErrorSeverity		INT,
			@nidErrorState			INT,
			@nidLog					NUMERIC(18,0),
			@ID_LOG_STATUS_VISITA	BIGINT,
			@TOKEN_REGISTRO_INCLUSAO BIGINT,
			@APLICACAO_ORIGEM_TOKEN BIGINT -- 1 = APP 2 = WEB
			
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		
		INSERT INTO dbo.tbVisitaPadrao
		        ( DT_DATA_VISITA,
		          ST_STATUS_VISITA,
		          CD_CLIENTE,
		          CD_TECNICO,
		          DS_OBSERVACAO,
				  HR_INICIO,
				  HR_FIM,
				  CD_MOTIVO_VISITA,
				  nidUsuarioAtualizacao,
		          dtmDataHoraAtualizacao,
				  TOKEN,
				  EMAIL,
				  DS_RESPONSAVEL
				)
		VALUES
		        ( @p_DT_DATA_VISITA, 
		          @p_ST_STATUS_VISITA,
		          @p_CD_CLIENTE,
		          @p_CD_TECNICO,
		          @p_DS_OBSERVACAO,
				  @p_HR_INICIO,
				  @p_HR_FIM,
				  @p_CD_MOTIVO_VISITA,
				  @p_nidUsuarioAtualizacao,
		          GETDATE(),
				  IIF(LEFT(@p_TOKEN, 1) = 1, @p_TOKEN, 0),
				  @p_Email,
				  @p_DS_RESPONSAVEL
		          )

		SET @p_ID_VISITA = @@IDENTITY

		SET @APLICACAO_ORIGEM_TOKEN = LEFT(@p_TOKEN, 1);

		IF (@p_ID_VISITA > 0 AND @APLICACAO_ORIGEM_TOKEN = 2) -- 2 = ORIGEM Applicação WEB
		BEGIN
			SET @TOKEN_REGISTRO_INCLUSAO = CAST((CAST(@p_TOKEN AS nvarchar(MAX)) + CAST(@p_ID_VISITA AS nvarchar(MAX))) AS BIGINT);
			
			UPDATE dbo.tbVisitaPadrao
			   SET TOKEN = @TOKEN_REGISTRO_INCLUSAO
			 WHERE ID_VISITA = @p_ID_VISITA

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
					@p_cnmTabela				= 'tbVisitaPadrao',
					@p_nidPK					= @p_ID_VISITA,
					@p_nidLogReturn				= @nidLog OUTPUT
	
		EXECUTE dbo.prcLogStatusVisitaPadraoInsert 
		    @p_ID_VISITA				= @p_ID_VISITA,
		    @p_DT_DATA_LOG_VISITA		= @p_DT_DATA_VISITA,
		    @p_ST_STATUS_VISITA			= @p_ST_STATUS_VISITA,
		    @p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
		    @p_ID_LOG_STATUS_VISITA		= @ID_LOG_STATUS_VISITA OUTPUT


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

