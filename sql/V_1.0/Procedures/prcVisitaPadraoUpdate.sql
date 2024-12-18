GO
/****** Object:  StoredProcedure [dbo].[prcVisitaPadraoUpdate]    Script Date: 26/11/2021 08:58:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcVisitaPadraoUpdate]
	@p_ID_VISITA				BIGINT			= NULL, 
	@p_DT_DATA_VISITA			DATETIME		= NULL,	
	@p_ST_STATUS_VISITA			INT				= NULL,
	@p_CD_CLIENTE				INT				= NULL,
	@p_CD_TECNICO				VARCHAR(06)		= NULL,
	@p_DS_OBSERVACAO			VARCHAR(MAX)	= NULL,
	@p_HR_INICIO				VARCHAR(5)		= NULL,
	@p_HR_FIM					VARCHAR(5)		= NULL,
	@p_CD_MOTIVO_VISITA			INT				= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_Email					VARCHAR(50)		= NULL,
	@p_DS_RESPONSAVEL			VARCHAR(60) 	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage		NVARCHAR(4000),
			@nidErrorSeverity		INT,
			@nidErrorState			INT,
			@nidLog					NUMERIC(18,0),
			@ST_STATUS_VISITA		INT,
			@ID_LOG_STATUS_VISITA	BIGINT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		BEGIN TRANSACTION
		
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'tbVisitaPadrao',
				@p_nidPK					= @p_ID_VISITA,
				@p_nidLogReturn				= @nidLog OUTPUT
  
		-- Busca o ST_TP_STATUS_VISITA_OS original
		SELECT @ST_STATUS_VISITA = ST_STATUS_VISITA
		FROM tbVisitaPadrao
		WHERE ID_VISITA = @p_ID_VISITA              
				
		UPDATE	tbVisitaPadrao
		SET		DT_DATA_VISITA			= ISNULL(@p_DT_DATA_VISITA, DT_DATA_VISITA),
				ST_STATUS_VISITA		= ISNULL(@p_ST_STATUS_VISITA, ST_STATUS_VISITA),
				CD_CLIENTE				= ISNULL(@p_CD_CLIENTE, CD_CLIENTE),
				CD_TECNICO				= ISNULL(@p_CD_TECNICO, CD_TECNICO),
				DS_OBSERVACAO			= @p_DS_OBSERVACAO,
				HR_INICIO				= @p_HR_INICIO,
				HR_FIM					= @p_HR_FIM,
				CD_MOTIVO_VISITA		= @p_CD_MOTIVO_VISITA,
				dtmDataHoraAtualizacao	= GETDATE(),
				Email					= @p_Email,
				DS_RESPONSAVEL			= @p_DS_RESPONSAVEL
		WHERE	ID_VISITA				= @p_ID_VISITA
		         	
		EXECUTE dbo.prcLogGravar 
				@p_nidLog					= @nidLog,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ccdAcao					= 'U',
				@p_cnmTabela				= 'tbVisitaPadrao',
				@p_nidPK					= @p_ID_VISITA,
				@p_nidLogReturn				= @nidLog OUTPUT
  
		-- Somente atualiza o LogStatusVisita se ocorreu mudança no ST_TP_STATUS_VISITA_OS
		IF @ST_STATUS_VISITA <> @p_ST_STATUS_VISITA
		BEGIN 
			EXECUTE dbo.prcLogStatusVisitaPadraoInsert 
				@p_ID_VISITA				= @p_ID_VISITA,
				@p_DT_DATA_LOG_VISITA		= @p_DT_DATA_VISITA,
				@p_ST_STATUS_VISITA			= @p_ST_STATUS_VISITA,
				@p_nidUsuarioAtualizacao	= @p_nidUsuarioAtualizacao,
				@p_ID_LOG_STATUS_VISITA		= @ID_LOG_STATUS_VISITA OUTPUT
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


