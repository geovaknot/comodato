GO
/****** Object:  StoredProcedure [dbo].[prcVisitaPadraoRespostaInsert]    Script Date: 19/05/2022 13:54:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[prcVisitaPadraoRespostaInsert]
	@p_DT_DATA					DATETIME		= NULL,	
	@p_ID_PESQUISA_SATISF		INT				= NULL,
	@p_ID_OS					BIGINT			= NULL,
	@p_ID_VISITA				BIGINT			= NULL,
	@p_Justificativa			VARCHAR(MAX)	= NULL,
	@p_NotaPesquisa				DECIMAL(18,2)	= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_DS_NOME_RESPONDEDOR    	varchar(50)		= NULL,
	@p_Email					VARCHAR(50)		= NULL,
	@p_DS_RESPONSAVEL			VARCHAR(60) 	= NULL

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

	INSERT INTO dbo.tbSatisfResposta
		        ( DT_DATA_RESPOSTA,
		          ID_OS,
		          ID_VISITA,
		          DS_NOME_RESPONDEDOR,
		          NM_NOTA_PESQ,
				  DS_JUSTIFICATIVA,
				  dtmDataHoraAtualizacao,
				  nidUsuarioAtualizacao,
				  ID_PESQUISA_SATISF
				)
		VALUES
		        ( @p_DT_DATA,
				  @p_ID_OS,
				  @p_ID_VISITA,
				  @p_DS_NOME_RESPONDEDOR,
				  @p_NotaPesquisa,
				  @p_Justificativa,
				  getdate(),
				  @p_nidUsuarioAtualizacao,
				  0
		          )
END