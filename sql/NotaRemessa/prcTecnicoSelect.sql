GO
/****** Object:  StoredProcedure [dbo].[prcTecnicoSelect]    Script Date: 10/06/2022 10:33:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_Tecnico
-- =============================================
ALTER PROCEDURE [dbo].[prcTecnicoSelect]
	@p_CD_Tecnico				VARCHAR(06)		= NULL,	
	@p_NM_Tecnico				VARCHAR(50)		= NULL,
	@p_EN_Endereco				VARCHAR(100)	= NULL, 
	@p_EN_Bairro				VARCHAR(30)		= NULL,
	@p_EN_Cidade				VARCHAR(30)		= NULL,
	@p_EN_Estado				VARCHAR(02)		= NULL,
	@p_EN_CEP					VARCHAR(09)		= NULL,
	@p_TX_Telefone				VARCHAR(20)		= NULL,
	@p_TX_FAX					VARCHAR(20)		= NULL,
	@P_TX_Email					VARCHAR(255)	= NULL,
	@p_TP_Tecnico				VARCHAR(01)		= NULL,
	@p_VL_Custo_Hora			NUMERIC(14,2)	= NULL,
	@p_FL_Ativo					VARCHAR(01)		= NULL,
	@p_ID_USUARIO_COORDENADOR	NUMERIC(18,0)	= NULL,
	@p_ID_USUARIO_SUPERVISOR	NUMERIC(18,0)	= NULL,
	@p_ID_USUARIO				NUMERIC(18,0)	= NULL,
	@p_CD_EMPRESA				NUMERIC(18,0)	= NULL,
	@p_FL_Ferias				VARCHAR(01)		= NULL, 
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_NM_REDUZIDO				VARCHAR(20)		= NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		SELECT	
			dbo.TB_Tecnico.CD_TECNICO, 
			dbo.TB_Tecnico.NM_TECNICO,
			dbo.TB_TECNICO.NM_REDUZIDO,
			dbo.TB_Tecnico.EN_ENDERECO,
			dbo.TB_Tecnico.EN_BAIRRO,
			dbo.TB_Tecnico.EN_CIDADE,
			dbo.TB_Tecnico.EN_ESTADO,
			dbo.TB_Tecnico.EN_CEP,
			dbo.TB_Tecnico.TX_TELEFONE,
			dbo.TB_Tecnico.TX_FAX,
			dbo.TB_Tecnico.TX_EMAIL,
			dbo.TB_Tecnico.TP_TECNICO,
			dbo.TB_Tecnico.VL_CUSTO_HORA,
			dbo.TB_Tecnico.FL_ATIVO,
			dbo.TB_Tecnico.ID_USUARIO_COORDENADOR,
			dbo.TB_Tecnico.ID_USUARIO_TECNICOREGIONAL,
			dbo.TB_Tecnico.ID_USUARIO,
			dbo.TB_Tecnico.CD_EMPRESA,
			dbo.TB_Tecnico.CD_BCPS,
			ISNULL(dbo.TB_Tecnico.FL_FERIAS, 'N') AS FL_FERIAS,
			tbUsuarioCoordenador.cnmNome AS cnmNomeCoordenador,
			tbUsuarioCoordenador.cdsLogin AS cdsLoginCoordenador,
			tbUsuarioCoordenador.cdsEmail AS cdsEmailCoordenador,
			tbUsuarioSupervidor.cnmNome As cnmNomeTecRegional,
			tbUsuarioSupervidor.cdsLogin AS cdsLoginTecRegional,
			tbUsuarioSupervidor.cdsEmail AS cdsEmailTecRegional,

			tbUsuario.cnmNome,
			tbUsuario.cdsLogin,
			tbUsuario.cdsEmail,

			dbo.TB_Empresa.Nm_Empresa
		FROM	TB_Tecnico
		LEFT OUTER JOIN dbo.tbUsuario AS tbUsuarioCoordenador
		ON tbUsuarioCoordenador.nidUsuario = dbo.TB_TECNICO.ID_USUARIO_COORDENADOR
		LEFT OUTER JOIN dbo.tbUsuario AS tbUsuarioSupervidor
		ON tbUsuarioSupervidor.nidUsuario = dbo.TB_TECNICO.ID_USUARIO_TECNICOREGIONAL
	
		LEFT OUTER JOIN dbo.tbUsuario
		ON dbo.tbUsuario.nidUsuario = dbo.TB_TECNICO.ID_USUARIO
		LEFT OUTER JOIN dbo.TB_Empresa
		ON dbo.TB_TECNICO.CD_EMPRESA = dbo.TB_Empresa.CD_Empresa
		WHERE (	TB_Tecnico.CD_Tecnico				= @p_CD_Tecnico				OR @p_CD_Tecnico				IS NULL )
		AND	  (	TB_Tecnico.NM_Tecnico				LIKE @p_NM_Tecnico			OR @p_NM_Tecnico				IS NULL )
		AND	  (	TB_Tecnico.EN_Endereco				LIKE @p_EN_Endereco			OR @p_EN_Endereco				IS NULL )
		AND	  (	TB_Tecnico.EN_Bairro				LIKE @p_EN_Bairro			OR @p_EN_Bairro					IS NULL )
		AND	  (	TB_Tecnico.EN_Cidade				LIKE @p_EN_Cidade			OR @p_EN_Cidade					IS NULL )
		AND	  (	TB_Tecnico.EN_Estado				LIKE @p_EN_Estado			OR @p_EN_Estado					IS NULL )
		AND	  (	TB_Tecnico.EN_CEP					LIKE @p_EN_CEP				OR @p_EN_CEP					IS NULL )
		AND	  (	TB_Tecnico.TX_Telefone				LIKE @p_TX_Telefone			OR @p_TX_Telefone				IS NULL )
		AND	  (	TB_Tecnico.TX_FAX					LIKE @p_TX_FAX				OR @p_TX_FAX					IS NULL )
		AND	  (	TB_Tecnico.TX_Email					LIKE @P_TX_Email			OR @P_TX_Email					IS NULL )
		AND	  (	TB_Tecnico.TP_Tecnico				LIKE @p_TP_Tecnico			OR @p_TP_Tecnico				IS NULL )
		AND	  (	TB_Tecnico.VL_Custo_Hora			= @p_VL_Custo_Hora			OR @p_VL_Custo_Hora				IS NULL )
		AND	  (	TB_Tecnico.FL_Ativo					= @p_FL_Ativo				OR @p_FL_Ativo					IS NULL )
		AND   ( TB_Tecnico.ID_USUARIO_COORDENADOR	= @p_ID_USUARIO_COORDENADOR OR @p_ID_USUARIO_COORDENADOR	IS NULL )
		AND   ( TB_Tecnico.ID_USUARIO_TECNICOREGIONAL	= @p_ID_USUARIO_SUPERVISOR OR @p_ID_USUARIO_SUPERVISOR	IS NULL )
		--AND   ( TB_Tecnico.ID_USUARIO				= @p_ID_USUARIO				OR @p_ID_USUARIO				IS NULL )
		AND   ( TB_Tecnico.ID_USUARIO IN (SELECT nidUsuario FROM fncRetornaUsuariosAcesso(@p_ID_USUARIO)) OR @p_ID_USUARIO IS NULL )
		AND   ( TB_Tecnico.CD_EMPRESA				= @p_CD_EMPRESA				OR @p_CD_EMPRESA				IS NULL )
		AND	  (	TB_Tecnico.FL_Ferias				= @p_FL_Ferias				OR @p_FL_Ferias					IS NULL )
		AND	  (	TB_Tecnico.NM_REDUZIDO				LIKE @p_NM_REDUZIDO			OR @p_NM_REDUZIDO				IS NULL )
		ORDER BY
				TB_Tecnico.NM_Tecnico			
		
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


