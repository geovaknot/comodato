GO
/****** Object:  StoredProcedure [dbo].[prcClienteSelect]    Script Date: 25/04/2023 16:25:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_Cliente
-- =============================================
ALTER PROCEDURE [dbo].[prcClienteSelect]
	@p_nidUsuario				NUMERIC(18,0)	= NULL,
	@p_CD_CLIENTE				NUMERIC(6,0)	= NULL,
	@p_CD_GRUPO					VARCHAR(10)		= NULL,
	@p_CD_RAC					VARCHAR(05)		= NULL,
	@p_CD_VENDEDOR				NUMERIC(6,0)	= NULL,
	@p_NR_CNPJ					VARCHAR(20)		= NULL,
	@p_NM_CLIENTE				VARCHAR(50)		= NULL,
	@p_EN_ENDERECO				VARCHAR(50)		= NULL,
	@p_EN_BAIRRO				VARCHAR(50)		= NULL,
	@p_EN_CIDADE				VARCHAR(50)		= NULL,
	@p_EN_ESTADO				VARCHAR(03)		= NULL,
	@p_EN_CEP					VARCHAR(10)		= NULL,
	@p_CD_REGIAO				VARCHAR(06)		= NULL,
	@p_CD_FILIAL				VARCHAR(05)		= NULL,
	@p_CD_ABC					VARCHAR(02)		= NULL,
	@p_CL_CLIENTE				VARCHAR(05)		= NULL,
	@p_TX_TELEFONE				VARCHAR(25)		= NULL,
	@p_TX_FAX					VARCHAR(25)		= NULL,
	@p_DT_DESATIVACAO			DATETIME		= NULL,
	@p_CD_EXECUTIVO				NUMERIC(3,0)	= NULL,
	@p_BPCS						BINARY(1)		= NULL,
	@p_QT_PERIODO				INT				= NULL,
	@p_FL_PESQ_SATISF			nchar(2)		= NULL,
	@p_ID_SEGMENTO				BIGINT			= NULL,
	@p_nidUsuarioCliente		NUMERIC(18,0)	= NULL,  -- Para não ser confundido com o nidUsuario (1o parametro), este refere-se ao novo campo nidUsuario vinculado ao cliente
	@p_FL_KAT_FIXO				BIT				= NULL,
	@p_DS_CLASSIFICACAO_KAT		VARCHAR(02)		= NULL,
	@p_FL_AtivaPlanoZero		VARCHAR(01)		= NULL,
	@p_QTD_PeriodoPlanoZero		numeric(02)		= NULL
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
		
		DECLARE @p_CLIENTE INT = NULL
		SET @p_CLIENTE = (SELECT TOP 1 CD_CLIENTE FROM tbUsuarioCliente WHERE nidUsuario = @p_nidUsuario)
		IF (@p_CLIENTE > 0)
		BEGIN 
			SELECT
				TB_CLIENTE.*,
				TB_VENDEDOR.NM_VENDEDOR,
				TB_VENDEDOR.NM_APE_VENDEDOR,
				TB_VENDEDOR.TX_EMAIL as EMAIL_VENDEDOR,
				TB_EXECUTIVO.NM_EXECUTIVO,
				TB_GRUPO.DS_GRUPO,
				dbo.V_REGIAO.DS_REGIAO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO_CLIENTE.CD_ORDEM
				,dbo.tbSegmento.id_segmento AS ID_SEGMENTO
				,dbo.tbSegmento.ds_segmento AS DS_SEGMENTO

				,tbUsuarioTecnicoRegional.cnmNome As cnmNomeTecRegional
				,tbUsuarioTecnicoRegional.cdsLogin AS cdsLoginTecRegional
				,tbUsuarioTecnicoRegional.cdsEmail AS cdsEmailTecRegional

			FROM	TB_CLIENTE
			LEFT JOIN TB_VENDEDOR 
			ON TB_CLIENTE.CD_VENDEDOR = TB_VENDEDOR.CD_VENDEDOR
			LEFT JOIN TB_EXECUTIVO 
			ON TB_CLIENTE.CD_EXECUTIVO = TB_EXECUTIVO.CD_EXECUTIVO
			LEFT JOIN TB_GRUPO 
			ON TB_CLIENTE.CD_GRUPO = TB_GRUPO.CD_GRUPO
			LEFT JOIN dbo.V_REGIAO 
			ON TB_CLIENTE.CD_REGIAO = V_REGIAO.CD_REGIAO
			LEFT JOIN dbo.TB_TECNICO_CLIENTE 
			ON TB_CLIENTE.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
			LEFT JOIN dbo.TB_TECNICO 
			ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO
			LEFT JOIN dbo.tbSegmento 
			ON dbo.TB_CLIENTE.ID_SEGMENTO = dbo.tbSegmento.id_segmento

			LEFT JOIN dbo.tbUsuario AS tbUsuarioTecnicoRegional
			ON tbUsuarioTecnicoRegional.nidUsuario = dbo.TB_CLIENTE.ID_USUARIO_TECNICOREGIONAL

			WHERE ( TB_CLIENTE.CD_CLIENTE = @p_CLIENTE OR @p_CLIENTE IS NULL )
			AND ( TB_TECNICO_CLIENTE.CD_ORDEM < 2 OR TB_TECNICO_CLIENTE.CD_ORDEM IS NULL )
			ORDER BY
			TB_CLIENTE.NM_CLIENTE,
			TB_CLIENTE.CD_CLIENTE
		END 
		ELSE IF dbo.fncRestringirConsultaUsuario(ISNULL(@p_nidUsuario, 0)) = 1
		BEGIN 
			SELECT	TB_CLIENTE.*,
				TB_VENDEDOR.NM_VENDEDOR,
				TB_VENDEDOR.NM_APE_VENDEDOR,
				TB_EXECUTIVO.NM_EXECUTIVO,
				TB_GRUPO.DS_GRUPO,
				dbo.V_REGIAO.DS_REGIAO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO_CLIENTE.CD_ORDEM
				,dbo.tbSegmento.id_segmento AS ID_SEGMENTO
				,dbo.tbSegmento.ds_segmento AS DS_SEGMENTO

				,tbUsuarioTecnicoRegional.cnmNome As cnmNomeTecRegional
				,tbUsuarioTecnicoRegional.cdsLogin AS cdsLoginTecRegional
				,tbUsuarioTecnicoRegional.cdsEmail AS cdsEmailTecRegional

			FROM	TB_CLIENTE
			LEFT JOIN TB_VENDEDOR 
			ON TB_CLIENTE.CD_VENDEDOR = TB_VENDEDOR.CD_VENDEDOR
			LEFT JOIN TB_EXECUTIVO 
			ON TB_CLIENTE.CD_EXECUTIVO = TB_EXECUTIVO.CD_EXECUTIVO
			LEFT JOIN TB_GRUPO 
			ON TB_CLIENTE.CD_GRUPO = TB_GRUPO.CD_GRUPO
			LEFT JOIN dbo.V_REGIAO 
			ON TB_CLIENTE.CD_REGIAO = V_REGIAO.CD_REGIAO
			LEFT JOIN dbo.TB_TECNICO_CLIENTE 
			ON TB_CLIENTE.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
			LEFT JOIN dbo.TB_TECNICO 
			ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO
			LEFT JOIN dbo.tbSegmento 
			ON dbo.TB_CLIENTE.ID_SEGMENTO = dbo.tbSegmento.id_segmento

			LEFT JOIN dbo.tbUsuario AS tbUsuarioTecnicoRegional
			ON tbUsuarioTecnicoRegional.nidUsuario = dbo.TB_CLIENTE.ID_USUARIO_TECNICOREGIONAL

			WHERE ( TB_TECNICO_CLIENTE.CD_ORDEM		< 2							OR TB_TECNICO_CLIENTE.CD_ORDEM IS NULL )
			AND ( TB_CLIENTE.CD_CLIENTE				= @p_CD_CLIENTE				OR @p_CD_CLIENTE				IS NULL )
			AND ( TB_CLIENTE.CD_GRUPO				LIKE @p_CD_GRUPO			OR @p_CD_GRUPO					IS NULL )
			AND ( TB_CLIENTE.CD_RAC					LIKE @p_CD_RAC				OR @p_CD_RAC					IS NULL )
			AND ( TB_CLIENTE.CD_VENDEDOR			= @p_CD_VENDEDOR			OR @p_CD_VENDEDOR				IS NULL )
			AND ( TB_CLIENTE.NR_CNPJ				LIKE @p_NR_CNPJ				OR @p_NR_CNPJ					IS NULL )
			AND ( TB_CLIENTE.NM_CLIENTE				LIKE @p_NM_CLIENTE			OR @p_NM_CLIENTE				IS NULL )
			AND ( TB_CLIENTE.EN_ENDERECO			LIKE @p_EN_ENDERECO			OR @p_EN_ENDERECO				IS NULL )
			AND ( TB_CLIENTE.EN_BAIRRO				LIKE @p_EN_BAIRRO			OR @p_EN_BAIRRO					IS NULL )
			AND ( TB_CLIENTE.EN_CIDADE				LIKE @p_EN_CIDADE			OR @p_EN_CIDADE					IS NULL )
			AND ( TB_CLIENTE.EN_ESTADO				= @p_EN_ESTADO				OR @p_EN_ESTADO					IS NULL )
			AND ( TB_CLIENTE.EN_CEP					LIKE @p_EN_CEP				OR @p_EN_CEP					IS NULL )
			AND ( TB_CLIENTE.CD_REGIAO				LIKE @p_CD_REGIAO			OR @p_CD_REGIAO					IS NULL )
			AND ( TB_CLIENTE.CD_FILIAL				LIKE @p_CD_FILIAL			OR @p_CD_FILIAL					IS NULL )
			AND ( TB_CLIENTE.CD_ABC					= @p_CD_ABC					OR @p_CD_ABC					IS NULL )
			AND ( TB_CLIENTE.CL_CLIENTE				LIKE @p_CL_CLIENTE			OR @p_CL_CLIENTE				IS NULL )
			AND ( TB_CLIENTE.TX_TELEFONE			LIKE @p_TX_TELEFONE			OR @p_TX_TELEFONE				IS NULL )
			AND ( TB_CLIENTE.TX_FAX					LIKE @p_TX_FAX				OR @p_TX_FAX					IS NULL )
			AND ( TB_CLIENTE.DT_DESATIVACAO			= @p_DT_DESATIVACAO			OR @p_DT_DESATIVACAO			IS NULL )
			AND ( TB_CLIENTE.CD_EXECUTIVO			= @p_CD_EXECUTIVO			OR @p_CD_EXECUTIVO				IS NULL )
			AND ( TB_CLIENTE.BPCS					= @p_BPCS					OR @p_BPCS						IS NULL )
			AND ( TB_CLIENTE.QT_PERIODO				= @p_QT_PERIODO				OR @p_QT_PERIODO				IS NULL )
			AND ( TB_CLIENTE.FL_PESQ_SATISF			= @p_FL_PESQ_SATISF			OR @p_FL_PESQ_SATISF			IS NULL )
			AND ( TB_CLIENTE.ID_SEGMENTO			= @p_ID_SEGMENTO			OR @p_ID_SEGMENTO				IS NULL )
			AND ( TB_TECNICO_CLIENTE.CD_CLIENTE IN (
				SELECT CD_CLIENTE FROM TB_TECNICO_CLIENTE WHERE CD_TECNICO IN (
					SELECT TB_TECNICO.CD_TECNICO FROM TB_TECNICO WHERE TB_TECNICO.ID_USUARIO IN (
						SELECT nidUsuario FROM fncRetornaUsuariosAcesso(@p_nidUsuario)) GROUP BY CD_TECNICO
							) GROUP BY CD_CLIENTE) OR @p_nidUsuario IS NULL)

			--AND ( TB_CLIENTE.nidUsuario				= @p_nidUsuarioCliente		OR @p_nidUsuarioCliente			IS NULL )
			AND (@p_nidUsuarioCliente IS NULL OR EXISTS(SELECT TOP 1 nidUsuario 
														FROM tbUsuarioCliente (NOLOCK) 
														WHERE tbUsuarioCliente.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE 
														AND tbUsuarioCliente.nidUsuario = @p_nidUsuarioCliente))
			AND ( TB_CLIENTE.FL_KAT_FIXO			= @p_FL_KAT_FIXO			OR @p_FL_KAT_FIXO				IS NULL )
			AND ( TB_CLIENTE.DS_CLASSIFICACAO_KAT	= @p_DS_CLASSIFICACAO_KAT	OR @p_DS_CLASSIFICACAO_KAT		IS NULL )
			-- complementa a busca com os clientes somente vinculados ao técnico do usuário informado
			--AND ( TB_CLIENTE.CD_CLIENTE IN ( SELECT DISTINCT 
			--									dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
			--								FROM dbo.TB_TECNICO 
			--								INNER JOIN dbo.TB_TECNICO_CLIENTE 
			--								ON dbo.TB_TECNICO.CD_TECNICO = dbo.TB_TECNICO_CLIENTE.CD_TECNICO
			--								WHERE ID_USUARIO = @p_nidUsuario
			--								)
			--)
			ORDER BY
				TB_CLIENTE.NM_CLIENTE,
				TB_CLIENTE.CD_CLIENTE

		END 
		ELSE 
		BEGIN
			-- Se não fizer parte, faz a consulta normal  
			SELECT	TB_CLIENTE.*,
				TB_VENDEDOR.NM_VENDEDOR,
				TB_VENDEDOR.NM_APE_VENDEDOR,
				TB_VENDEDOR.TX_EMAIL as EMAIL_VENDEDOR,
				TB_EXECUTIVO.NM_EXECUTIVO,
				TB_GRUPO.DS_GRUPO,
				dbo.V_REGIAO.DS_REGIAO,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_TECNICO.CD_TECNICO,
				dbo.TB_TECNICO_CLIENTE.CD_ORDEM
				,dbo.tbSegmento.id_segmento AS ID_SEGMENTO
				,dbo.tbSegmento.ds_segmento AS DS_SEGMENTO

				,tbUsuarioTecnicoRegional.cnmNome As cnmNomeTecRegional
				,tbUsuarioTecnicoRegional.cdsLogin AS cdsLoginTecRegional
				,tbUsuarioTecnicoRegional.cdsEmail AS cdsEmailTecRegional

			FROM	TB_CLIENTE
			LEFT JOIN TB_VENDEDOR
			ON TB_CLIENTE.CD_VENDEDOR = TB_VENDEDOR.CD_VENDEDOR
			LEFT JOIN TB_EXECUTIVO
			ON TB_CLIENTE.CD_EXECUTIVO = TB_EXECUTIVO.CD_EXECUTIVO
			LEFT JOIN TB_GRUPO
			ON TB_CLIENTE.CD_GRUPO = TB_GRUPO.CD_GRUPO
			LEFT JOIN dbo.V_REGIAO
			ON TB_CLIENTE.CD_REGIAO = V_REGIAO.CD_REGIAO
			LEFT JOIN dbo.TB_TECNICO_CLIENTE
			ON TB_CLIENTE.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
			LEFT JOIN dbo.TB_TECNICO
			ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO
			LEFT JOIN dbo.tbSegmento 
			ON dbo.TB_CLIENTE.ID_SEGMENTO = dbo.tbSegmento.id_segmento

			LEFT JOIN dbo.tbUsuario AS tbUsuarioTecnicoRegional
			ON tbUsuarioTecnicoRegional.nidUsuario = dbo.TB_CLIENTE.ID_USUARIO_TECNICOREGIONAL

			WHERE ( TB_TECNICO_CLIENTE.CD_ORDEM		< 2							OR TB_TECNICO_CLIENTE.CD_ORDEM		IS NULL )
			AND ( TB_CLIENTE.CD_CLIENTE				= @p_CD_CLIENTE				OR @p_CD_CLIENTE					IS NULL )
			AND ( TB_CLIENTE.CD_GRUPO				LIKE @p_CD_GRUPO			OR @p_CD_GRUPO						IS NULL )
			AND ( TB_CLIENTE.CD_RAC					LIKE @p_CD_RAC				OR @p_CD_RAC						IS NULL )
			AND ( TB_CLIENTE.CD_VENDEDOR			= @p_CD_VENDEDOR			OR @p_CD_VENDEDOR					IS NULL )
			AND ( TB_CLIENTE.NR_CNPJ				LIKE @p_NR_CNPJ				OR @p_NR_CNPJ						IS NULL )
			AND ( TB_CLIENTE.NM_CLIENTE				LIKE @p_NM_CLIENTE			OR @p_NM_CLIENTE					IS NULL )
			AND ( TB_CLIENTE.EN_ENDERECO			LIKE @p_EN_ENDERECO			OR @p_EN_ENDERECO					IS NULL )
			AND ( TB_CLIENTE.EN_BAIRRO				LIKE @p_EN_BAIRRO			OR @p_EN_BAIRRO						IS NULL )
			AND ( TB_CLIENTE.EN_CIDADE				LIKE @p_EN_CIDADE			OR @p_EN_CIDADE						IS NULL )
			AND ( TB_CLIENTE.EN_ESTADO				= @p_EN_ESTADO				OR @p_EN_ESTADO						IS NULL )
			AND ( TB_CLIENTE.EN_CEP					LIKE @p_EN_CEP				OR @p_EN_CEP						IS NULL )
			AND ( TB_CLIENTE.CD_REGIAO				LIKE @p_CD_REGIAO			OR @p_CD_REGIAO						IS NULL )
			AND ( TB_CLIENTE.CD_FILIAL				LIKE @p_CD_FILIAL			OR @p_CD_FILIAL						IS NULL )
			AND ( TB_CLIENTE.CD_ABC					= @p_CD_ABC					OR @p_CD_ABC						IS NULL )
			AND ( TB_CLIENTE.CL_CLIENTE				LIKE @p_CL_CLIENTE			OR @p_CL_CLIENTE					IS NULL )
			AND ( TB_CLIENTE.TX_TELEFONE			LIKE @p_TX_TELEFONE			OR @p_TX_TELEFONE					IS NULL )
			AND ( TB_CLIENTE.TX_FAX					LIKE @p_TX_FAX				OR @p_TX_FAX						IS NULL )
			AND ( TB_CLIENTE.DT_DESATIVACAO			= @p_DT_DESATIVACAO			OR @p_DT_DESATIVACAO				IS NULL )
			AND ( TB_CLIENTE.CD_EXECUTIVO			= @p_CD_EXECUTIVO			OR @p_CD_EXECUTIVO					IS NULL )
			AND ( TB_CLIENTE.BPCS					= @p_BPCS					OR @p_BPCS							IS NULL )
			AND ( TB_CLIENTE.QT_PERIODO				= @p_QT_PERIODO				OR @p_QT_PERIODO					IS NULL )
			AND ( TB_CLIENTE.FL_PESQ_SATISF			= @p_FL_PESQ_SATISF			OR @p_FL_PESQ_SATISF				IS NULL )
			--AND ( TB_TECNICO_CLIENTE.CD_CLIENTE IN (
			--	SELECT CD_CLIENTE FROM TB_TECNICO_CLIENTE WHERE CD_TECNICO IN (
			--		SELECT TB_TECNICO.CD_TECNICO FROM TB_TECNICO WHERE TB_TECNICO.ID_USUARIO IN (
			--			SELECT nidUsuario FROM fncRetornaUsuariosAcesso(@p_nidUsuario)) GROUP BY CD_TECNICO
			--				) GROUP BY CD_CLIENTE) OR @p_nidUsuario IS NULL)
			--AND ( TB_CLIENTE.nidUsuario				= @p_nidUsuarioCliente		OR @p_nidUsuarioCliente				IS NULL )
			AND (@p_nidUsuarioCliente IS NULL OR EXISTS(SELECT TOP 1 nidUsuario 
														FROM tbUsuarioCliente (NOLOCK) 
														WHERE tbUsuarioCliente.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE 
														AND tbUsuarioCliente.nidUsuario = @p_nidUsuarioCliente))
			AND ( TB_CLIENTE.FL_KAT_FIXO			= @p_FL_KAT_FIXO			OR @p_FL_KAT_FIXO					IS NULL )
			AND ( TB_CLIENTE.DS_CLASSIFICACAO_KAT	= @p_DS_CLASSIFICACAO_KAT	OR @p_DS_CLASSIFICACAO_KAT			IS NULL )
			ORDER BY
				TB_CLIENTE.NM_CLIENTE,
				TB_CLIENTE.CD_CLIENTE
		END  
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





