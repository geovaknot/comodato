GO
/****** Object:  StoredProcedure [dbo].[prcOSPadraoUpdate]    Script Date: 26/11/2021 08:47:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcOSPadraoUpdate]
	@p_ID_OS					BIGINT		= NULL, 
	@p_DT_DATA_OS				DATETIME	= NULL,	
	@p_ST_STATUS_OS				INT		= NULL,
	@p_CD_TIPO_OS				INT		= NULL,
	@p_CD_CLIENTE				INT		= NULL,
	@p_CD_TECNICO				VARCHAR(06)	= NULL,
	@p_CD_ATIVO_FIXO			VARCHAR(06)	= NULL,
	@p_HR_INICIO				VARCHAR(5)	= NULL,
	@p_HR_FIM					VARCHAR(5)	= NULL,
	@p_DS_OBSERVACAO			VARCHAR(max) = NULL,
	@p_NOME_LINHA				VARCHAR(50) = NULL,
	@p_Email					VARCHAR(50) = NULL,
	@p_DS_RESPONSAVEL			VARCHAR(60) = NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage		NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@nidLog			NUMERIC(18,0),
			@ST_STATUS_OS 	INT,
			@ID_LOG_STATUS_OS	BIGINT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'tbOSPadrao',
				@p_nidPK					= @p_ID_OS,
				@p_nidLogReturn				= @nidLog OUTPUT
  
		-- Busca o ST_STATUS_OS original
		SELECT @ST_STATUS_OS = ST_STATUS_OS
		FROM tbOSPadrao
		WHERE ID_OS = @p_ID_OS
				
		UPDATE	tbOSPadrao
		SET		DT_DATA_OS			= ISNULL(@p_DT_DATA_OS, DT_DATA_OS),
				ST_STATUS_OS			= ISNULL(@p_ST_STATUS_OS, ST_STATUS_OS),
				CD_TIPO_OS			= ISNULL(@p_CD_TIPO_OS, CD_TIPO_OS),
				CD_CLIENTE			= ISNULL(@p_CD_CLIENTE, CD_CLIENTE),
				CD_TECNICO			= ISNULL(@p_CD_TECNICO, CD_TECNICO),
				CD_ATIVO_FIXO		= ISNULL(@p_CD_ATIVO_FIXO, CD_ATIVO_FIXO),
				HR_INICIO			= @p_HR_INICIO,
				HR_FIM				= @p_HR_FIM,
				DS_OBSERVACAO       = @p_DS_OBSERVACAO,
				NOME_LINHA			= @p_NOME_LINHA,
				Email				= @p_Email,
				DS_RESPONSAVEL		= @p_DS_RESPONSAVEL,
				dtmDataHoraAtualizacao		= GETDATE()
		WHERE	ID_OS					= @p_ID_OS
		         	
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'tbOSPadrao',
				@p_nidPK					= @p_ID_OS,
				@p_nidLogReturn				= @nidLog OUTPUT
  
		-- Somente atualiza o LogStatusOS se ocorreu mudança no ST_TP_STATUS_OS
		IF @ST_STATUS_OS <> @p_ST_STATUS_OS
		BEGIN 
			EXECUTE dbo.prcLogStatusOSPadraoInsert 
				@p_ID_OS				= @p_ID_OS,
				@p_DT_DATA_LOG_OS		= @p_DT_DATA_OS,
				@p_ST_STATUS_OS			= @p_ST_STATUS_OS,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ID_LOG_STATUS_OS		= @ID_LOG_STATUS_OS OUTPUT
		END              
	
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


