GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Create date: 24/01/2019
-- Description:	Inclusão de Dados na Tabela 
--              TbRRRelatorioReclamacao
-- =============================================
ALTER PROCEDURE [dbo].[prcRRRelatorioReclamacaoInsert]		
	@p_ST_RR_STATUS bigint,
	@p_CD_TECNICO VARCHAR(6) ,
	@p_CD_CLIENTE VARCHAR(6) ,
	@p_CD_ATIVO_FIXO VARCHAR(6) ,
	@p_CD_PECA VARCHAR(15) ,
	@p_CD_TIPO_ATENDIMENTO BIGINT,
	@p_CD_TIPO_RECLAMACAO BIGINT,
	@p_DS_MOTIVO VARCHAR(100)  = NULL,
	@p_DS_DESCRICAO VARCHAR(100)  = NULL,
	@p_TEMPO_ATENDIMENTO int,
	@p_DS_ARQUIVO_FOTO VARCHAR(MAX)  = NULL,
	@p_DS_TIPO_FOTO CHAR(4)  = NULL,
	@p_VL_CUSTO_PECA  decimal = null,
	@p_CD_GRUPO_RESPONS nchar(20) = null,
	@p_ID_OS bigint = null,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)   = NULL,
	@p_TOKEN    				BIGINT			= NULL,
	@p_TOKEN_GERADO    		    BIGINT		    OUTPUT,
	@p_nidRRRelatorioReclamacao	NUMERIC(18,0)	OUTPUT
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

		
INSERT INTO [dbo].[TbRRRelatorioReclamacao]
           (
		    [ST_STATUS_RR]
           ,[CD_TECNICO]
           ,[CD_CLIENTE]
           ,[CD_ATIVO_FIXO]
           ,[CD_PECA]
           ,[CD_TIPO_ATENDIMENTO]
           ,[CD_TIPO_RECLAMACAO]
           ,[DS_MOTIVO]
           ,[DS_DESCRICAO]
           ,[VL_TEMPO_ATENDIMENTO]
           ,[DS_ARQUIVO_FOTO]
           ,[DS_TIPO_FOTO]
           ,[nidUsuarioAtualizacao]
           ,[dtmDataHoraAtualizacao]
		   ,[VL_CUSTO_PECA]
		   ,[CD_GRUPO_RESPONS]
		   ,[ID_OS]
		   ,[Dt_Criacao]
		   ,TOKEN
		   )
		VALUES
			(
				@p_ST_RR_STATUS, 
				@p_CD_TECNICO, 
				@p_CD_CLIENTE, 
				@p_CD_ATIVO_FIXO, 
				@p_CD_PECA, 
				@p_CD_TIPO_ATENDIMENTO, 
				@p_CD_TIPO_RECLAMACAO, 
				@p_DS_MOTIVO, 
				@p_DS_DESCRICAO, 
				@p_TEMPO_ATENDIMENTO, 
				@p_DS_ARQUIVO_FOTO, 
				@p_DS_TIPO_FOTO, 
				@p_nidUsuarioAtualizacao, 
				getdate(),
				@p_VL_CUSTO_PECA,
				@p_CD_GRUPO_RESPONS,
				@p_ID_OS,
				getdate(),
				IIF(LEFT(@p_TOKEN, 1) = 1, @p_TOKEN, 0)
			)

		SET @p_nidRRRelatorioReclamacao = @@IDENTITY

		SET @APLICACAO_ORIGEM_TOKEN = LEFT(@p_TOKEN, 1);

		IF (@p_nidRRRelatorioReclamacao > 0 AND @APLICACAO_ORIGEM_TOKEN = 2) -- 2 = ORIGEM Applicação WEB
		BEGIN
			SET @TOKEN_REGISTRO_INCLUSAO = CAST((CAST(@p_TOKEN AS nvarchar(MAX)) + CAST(@p_nidRRRelatorioReclamacao AS nvarchar(MAX))) AS BIGINT);
			
			UPDATE dbo.TbRRRelatorioReclamacao
			   SET TOKEN = @TOKEN_REGISTRO_INCLUSAO
			 WHERE ID_RELATORIO_RECLAMACAO = @p_nidRRRelatorioReclamacao

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
				@p_cnmTabela				= 'TbRRRelatorioReclamacao',
				@p_nidPK					= @p_nidRRRelatorioReclamacao,
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