GO
/****** Object:  StoredProcedure [dbo].[prcDashListaClienteSelect]    Script Date: 13/05/2022 10:57:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcDashListaClienteSelect]
	@p_CD_GRUPO				VARCHAR(10)		= NULL,
	@p_CLIENTE				VARCHAR(100)	= NULL,
	@p_CD_MODELO			VARCHAR(15)		= NULL,
	@p_TECNICO				VARCHAR(100)	= NULL,
	@p_nidUsuario			NUMERIC(18,0)	= NULL,
	@p_CD_VENDEDOR			NUMERIC(6,0)	= NULL,
	@p_CD_LINHA_PRODUTO		INT				= NULL

AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,

			@CD_CLIENTE			NUMERIC(6,0),
			@CD_MES				NUMERIC(04),
			--@NREQP				INT,
			@VL_VENDAS			NUMERIC(18,5),
			@DEPCOM				NUMERIC(18,5),
			@VL_MANUTENCAO		NUMERIC(18,5),
			@DEPRECIACAO		NUMERIC(18,5),
			@CUSTO				NUMERIC(18,5),
			@GM_ANO				NUMERIC(18,2),
			@ANO				INT,
			@vigenciaInicial	DATETIME,
			@vigenciaFinal		DATETIME 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		CREATE TABLE #tbCliente
		(
			CD_CLIENTE				NUMERIC(6, 0)	NOT NULL,
			CD_GRUPO				VARCHAR(10)		NULL,
			CD_RAC					VARCHAR(5)		NULL,
			CD_VENDEDOR				NUMERIC(6, 0)	NULL,
			NR_CNPJ					VARCHAR(20)		NULL,
			NM_CLIENTE				VARCHAR(50)		NOT NULL,
			EN_ENDERECO				VARCHAR(50)		NULL,
			EN_BAIRRO				VARCHAR(50)		NULL,
			EN_CIDADE				VARCHAR(50)		NULL,
			EN_ESTADO				VARCHAR(3)		NULL,
			EN_CEP					VARCHAR(10)		NULL,
			CD_REGIAO				VARCHAR(6)		NULL,
			CD_FILIAL				VARCHAR(5)		NULL,
			CD_ABC					VARCHAR(2)		NULL,
			CL_CLIENTE				VARCHAR(5)		NULL,
			TX_TELEFONE				VARCHAR(25)		NULL,
			TX_FAX					VARCHAR(25)		NULL,
			DT_DESATIVACAO			DATETIME		NULL,
			CD_EXECUTIVO			NUMERIC(3, 0)	NULL,
			BPCS					BINARY(1)		NULL,
			QT_PERIODO				INT				NULL,
			TX_EMAIL				VARCHAR(100)	NULL,
			NIDUSUARIO				NUMERIC(18, 0)	NULL,
			FL_PESQ_SATISF			NCHAR(1)		NULL,
			NM_QTD_PERIODO_PREVISTO NUMERIC(18, 0)	NULL,
			ID_SEGMENTO				BIGINT			NULL,
			FL_KAT_FIXO				BIT				NULL,
			DS_CLASSIFICACAO_KAT	VARCHAR(2)		NULL
		)

		INSERT INTO #tbCliente
			SELECT TB_CLIENTE.* 
			FROM dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO) AS TB_CLIENTE 
	--		INNER JOIN tbSegmento s 
	--			ON s.id_segmento = TB_CLIENTE.ID_SEGMENTO
    --		WHERE s.ds_segmentomin <> 'DISTR'

		CREATE TABLE #tbTecnico
		(
			CD_TECNICO VARCHAR(6) NOT NULL,
			NM_TECNICO VARCHAR(50) NOT NULL,
			NM_REDUZIDO VARCHAR(20) NOT NULL,
			EN_ENDERECO VARCHAR(100) NULL,
			EN_BAIRRO VARCHAR(30) NULL,
			EN_CIDADE VARCHAR(30) NULL,
			EN_ESTADO VARCHAR(2) NULL,
			EN_CEP VARCHAR(9) NULL,
			TX_TELEFONE VARCHAR(20) NULL,
			TX_FAX VARCHAR(20) NULL,
			TX_EMAIL VARCHAR(255) NULL,
			TP_TECNICO VARCHAR(1) NOT NULL,
			VL_CUSTO_HORA NUMERIC(14, 2) NULL,
			FL_ATIVO VARCHAR(1) NULL,
			ID_USUARIO_COORDENADOR NUMERIC(18, 0) NULL,
			ID_USUARIO NUMERIC(18, 0) NULL,
			CD_EMPRESA NUMERIC(18, 0) NULL,
			FL_FERIAS VARCHAR(1) NULL,
		)
        
		INSERT INTO #tbTecnico
		SELECT * FROM dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO
		WHERE TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO

		SET @ANO = YEAR(DATEADD(YY,-1,DATEADD(HH,-3,GETUTCDATE())))

		SELECT @vigenciaInicial = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaINICIAL'
		SELECT @vigenciaFinal = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaFINAL'

		IF (@vigenciaInicial IS NULL)
			SELECT @vigenciaInicial = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-01-01')

		IF (@vigenciaFinal IS NULL)
			SELECT @vigenciaFinal = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-12-31')

		CREATE TABLE #tbDashCliente 
		(
			CD_CLIENTE				NUMERIC(6,0)	NOT NULL,
			NM_CLIENTE				VARCHAR(50)		NULL,
			EN_CIDADE				VARCHAR(50)		NULL,
			EN_ESTADO				VARCHAR(03)		NULL,
			CD_REGIAO				VARCHAR(02)		NULL,
			DS_REGIAO				VARCHAR(30)		NULL,
			PERIODOS				INT				NULL,
			ATIVOS					INT				NULL,
			VISITAS					INT				NULL,
			ST_TP_STATUS_VISITA_OS	INT				NULL,
			CD_TECNICO_PRINCIPAL	VARCHAR(06)		NULL,
			NM_TECNICO_PRINCIPAL	VARCHAR(50)		NULL,
			PESQUISA				NUMERIC(18,2)	NULL,
			SOLICIT					INT				NULL,
			KAT						VARCHAR(02)		NULL,
			GM_ANO_ANTERIOR			INT				NULL,
			GM_ANO_ATUAL			INT				NULL,
			VENDAS_ANO_ANTERIOR		NUMERIC(18,2)	NULL,
			VENDAS_ANO_ATUAL		NUMERIC(18,2)	NULL
		)

		INSERT INTO #tbDashCliente 
		(
			CD_CLIENTE,
			NM_CLIENTE,
			EN_CIDADE,
			EN_ESTADO,
			CD_REGIAO,
			DS_REGIAO,
			PERIODOS,
			KAT,
			VISITAS,
			ST_TP_STATUS_VISITA_OS,
			CD_TECNICO_PRINCIPAL,
			NM_TECNICO_PRINCIPAL,
			PESQUISA,
			SOLICIT				
		)
		SELECT 
			TB_CLIENTE.CD_CLIENTE,
			TB_CLIENTE.NM_CLIENTE,
			TB_CLIENTE.EN_CIDADE,
			TB_CLIENTE.EN_ESTADO,
			dbo.V_REGIAO.CD_REGIAO,
			dbo.V_REGIAO.DS_REGIAO,
			TB_CLIENTE.QT_PERIODO,
			TB_CLIENTE.DS_CLASSIFICACAO_KAT,
			(SELECT 
				COUNT(DISTINCT dbo.tbospadrao.ID_OS) 
			FROM dbo.tbOsPadrao (NOLOCK)
			WHERE dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
				AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
			) AS VISITAS,
			--(SELECT 
			--	TOP 1 dbo.tbOsPadrao.ST_TP_STATUS_VISITA_OS
			--	--COUNT(DISTINCT dbo.tbOsPadrao.ID_OS)
			--FROM dbo.tbOsPadrao (NOLOCK)
			--WHERE dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
			--AND dbo.tbOsPadrao.ST_TP_STATUS_VISITA_OS	IN (1, 2, 8, 9, 10) --Nova, Aberta, Portaria, Integração e Treinamento
			--AND TB_CLIENTE.DT_DESATIVACAO IS NULL
			--) AS ST_TP_STATUS_VISITA_OS,
			(SELECT 
				TOP 1 dbo.tbOsPadrao.ST_STATUS_OS
			FROM dbo.tbOsPadrao (NOLOCK)
			WHERE dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE --dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO --dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
			AND TB_CLIENTE.DT_DESATIVACAO IS NULL
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
			ORDER BY dbo.tbOsPadrao.DT_DATA_OS DESC
			) AS ST_TP_STATUS_VISITA_OS,

			(SELECT TOP 1 T.CD_TECNICO 
				FROM #tbTecnico AS T --dbo.TB_TECNICO (NOLOCK) AS T
				INNER JOIN dbo.TB_TECNICO_CLIENTE (NOLOCK) AS TC
					ON T.CD_TECNICO = TC.CD_TECNICO COLLATE SQL_Latin1_General_CP1_CI_AI
				WHERE TC.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
				AND TC.CD_ORDEM = 1 
			) AS CD_TECNICO_PRINCIPAL,
			(SELECT TOP 1 T.NM_REDUZIDO --T.NM_TECNICO 
				FROM #tbTecnico AS T --dbo.TB_TECNICO (NOLOCK) AS T
				INNER JOIN dbo.TB_TECNICO_CLIENTE (NOLOCK) AS TC
					ON T.CD_TECNICO = TC.CD_TECNICO COLLATE SQL_Latin1_General_CP1_CI_AI
				WHERE TC.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
				AND TC.CD_ORDEM = 1 
			) AS NM_TECNICO_PRINCIPAL,
			(SELECT
				CASE   
					  WHEN COUNT(DISTINCT dbo.tbSatisfResposta.ID_OS) > 0 
						THEN ISNULL(SUM(dbo.tbSatisfResposta.NM_NOTA_PESQ),0) / ISNULL(COUNT(dbo.tbSatisfResposta.ID_OS), 1)   
					  WHEN COUNT(DISTINCT dbo.tbSatisfResposta.ID_OS) = 0 
						THEN NULL   
				   END  
				FROM
					dbo.tbSatisfResposta (NOLOCK)
					INNER JOIN dbo.tbOsPadrao (NOLOCK)
					ON dbo.tbSatisfResposta.ID_OS = dbo.tbOsPadrao.ID_OS
				WHERE NOT ID_PESQUISA_SATISF IS NULL 
				AND dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
			) AS PESQUISA,
			(SELECT 
				COUNT(*) AS TOTAL
				FROM dbo.tbWfPedidoEquip (NOLOCK)
				WHERE dbo.tbWfPedidoEquip.DT_PEDIDO		BETWEEN @vigenciaInicial	AND @vigenciaFinal
				AND (dbo.tbWfPedidoEquip.CD_MODELO		= @p_CD_MODELO				OR @p_CD_MODELO IS NULL)
				AND dbo.tbWfPedidoEquip.TP_PEDIDO = 'E'
				AND DBO.tbWfPedidoEquip.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
			) AS SOLICT
		FROM #tbCliente AS TB_CLIENTE --dbo.TB_CLIENTE (NOLOCK)
		INNER JOIN dbo.V_REGIAO (NOLOCK)
			ON TB_CLIENTE.CD_REGIAO = dbo.V_REGIAO.CD_REGIAO COLLATE SQL_Latin1_General_CP1_CI_AI
		--WHERE TB_CLIENTE.DT_DESATIVACAO IS NULL
			--AND (dbo.TB_CLIENTE.NM_CLIENTE				LIKE @p_NM_CLIENTE		OR @p_NM_CLIENTE	IS NULL)      
			--AND (dbo.TB_CLIENTE.CD_CLIENTE				= @p_CD_CLIENTE			OR @p_CD_CLIENTE	IS NULL)
			--AND (TB_CLIENTE.CD_VENDEDOR				= @p_CD_VENDEDOR		OR @p_CD_VENDEDOR	IS NULL)
		ORDER BY
			ST_TP_STATUS_VISITA_OS DESC,      
			TB_CLIENTE.NM_CLIENTE,
			TB_CLIENTE.CD_CLIENTE,
			dbo.V_REGIAO.DS_REGIAO

		CREATE TABLE #tbDashClienteValores
		(
			CD_CLIENTE				NUMERIC(6,0)	NOT NULL,
			CD_MES					NUMERIC(04)		NOT NULL,
			--NREQP					INT				NULL,
			VL_VENDAS				NUMERIC(18,5)	NULL,
			DEPCOM					NUMERIC(18,5)	NULL,
			VL_MANUTENCAO			NUMERIC(18,5)	NULL,
			DEPRECIACAO				NUMERIC(18,5)	NULL,
			CUSTO					NUMERIC(18,5)	NULL
		)

		INSERT INTO #tbDashClienteValores
		(
			CD_CLIENTE,
			CD_MES,
			--NREQP,
			VL_VENDAS,
			DEPCOM,
			VL_MANUTENCAO,
			DEPRECIACAO,
			CUSTO
		)
		SELECT 
			dbo.vwDashVendasLinha.CD_CLIENTE,
			LEFT(dbo.vwDashVendasLinha.CD_MES, 4) AS CD_MES,
			--dbo.fncCountEQPCliente(dbo.vwDashVendasLinha.CD_CLIENTE) AS NREQP,
			SUM(dbo.vwDashVendasLinha.TOT_VENDAS) AS VL_VENDAS,
			SUM(dbo.vwDashVendasLinha.DEPCOM) AS DEPCOM,
			COALESCE(SUM(dbo.vwDashManutencao.TOT_PECAS),0) + COALESCE(SUM(dbo.vwDashManutencao.TOT_MAO_OBRA),0) AS VL_MANUTENCAO,
			ABS(SUM(dbo.vwDashDepreciacao.TOT_DEPRECIACAO)) AS DEPRECIACAO,
			(SUM(dbo.vwDashVendasLinha.TOT_VENDAS) * SUM(dbo.vwDashCustoLinha.CUSTO)) AS CUSTO
		FROM 
			dbo.vwDashVendasLinha (NOLOCK)
			LEFT OUTER JOIN dbo.vwDashManutencao (NOLOCK)
			ON dbo.vwDashVendasLinha.CD_CLIENTE = dbo.vwDashManutencao.CD_CLIENTE
			AND dbo.vwDashVendasLinha.CD_LINHA_PRODUTO = dbo.vwDashManutencao.CD_LINHA_PRODUTO
			AND dbo.vwDashVendasLinha.CD_MES = dbo.vwDashManutencao.CD_MES

			LEFT OUTER JOIN dbo.vwDashDepreciacao (NOLOCK)
			ON dbo.vwDashManutencao.CD_CLIENTE = dbo.vwDashDepreciacao.CD_CLIENTE
			AND dbo.vwDashManutencao.CD_LINHA_PRODUTO = dbo.vwDashDepreciacao.CD_LINHA_PRODUTO
			AND LEFT(dbo.vwDashManutencao.CD_MES,4) = dbo.vwDashDepreciacao.CD_MES

			LEFT OUTER JOIN dbo.vwDashCustoLinha (NOLOCK)
			ON dbo.vwDashDepreciacao.CD_CLIENTE = dbo.vwDashCustoLinha.CD_CLIENTE
			AND dbo.vwDashDepreciacao.CD_LINHA_PRODUTO = dbo.vwDashCustoLinha.CD_LINHA_PRODUTO
			AND dbo.vwDashDepreciacao.CD_MES = dbo.vwDashCustoLinha.CD_MES

			INNER JOIN #tbCliente AS TB_CLIENTE --dbo.TB_CLIENTE (NOLOCK)
			ON dbo.vwDashVendasLinha.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE

		WHERE --TB_CLIENTE.DT_DESATIVACAO IS NULL
			/*AND*/ LEFT(dbo.vwDashVendasLinha.CD_MES, 4)	IN (YEAR(DATEADD(YY,-1,DATEADD(HH,-3,GETUTCDATE()))), YEAR(DATEADD(HH,-3,GETUTCDATE())))
			--AND (dbo.TB_CLIENTE.NM_CLIENTE				LIKE @p_NM_CLIENTE		OR @p_NM_CLIENTE	IS NULL)      
			--AND (dbo.TB_CLIENTE.CD_CLIENTE				= @p_CD_CLIENTE			OR @p_CD_CLIENTE	IS NULL)
			--AND (TB_CLIENTE.CD_VENDEDOR				= @p_CD_VENDEDOR		OR @p_CD_VENDEDOR	IS NULL)

		GROUP BY
			dbo.vwDashVendasLinha.CD_CLIENTE,
			LEFT(dbo.vwDashVendasLinha.CD_MES, 4)

		ORDER BY 
			dbo.vwDashVendasLinha.CD_CLIENTE,
			CD_MES

		-- A partir da temporária #tbDashCliente busca todos os respectivos clientes em #tbDashClienteValores
		-- para finalizar os cálculos e atualizar os valores de GM e VENDAS (anos anteriores e atuais)
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT	CD_CLIENTE
		FROM #tbDashCliente 
		
		OPEN C1
		FETCH NEXT FROM C1
			INTO @CD_CLIENTE

		WHILE @@FETCH_STATUS = 0
		BEGIN
        
			WHILE @ANO <= YEAR(DATEADD(HH,-3,GETUTCDATE()))
			BEGIN
				-- Busca valores do ano       
				SELECT	
					@CD_MES			= CD_MES,
					--@NREQP			= ISNULL(NREQP,0),
					@VL_VENDAS		= ISNULL(VL_VENDAS,0),
					@DEPCOM			= ISNULL(DEPCOM,0),
					@VL_MANUTENCAO	= ISNULL(VL_MANUTENCAO,0),
					@DEPRECIACAO	= ISNULL(DEPRECIACAO,0),
					@CUSTO			= ISNULL(CUSTO,0)
				FROM #tbDashClienteValores
				WHERE CD_CLIENTE = @CD_CLIENTE
				AND CD_MES = @ANO

				IF @VL_VENDAS <> 0 
				BEGIN
					SET @GM_ANO = 1 - ((@VL_MANUTENCAO + @DEPRECIACAO + @CUSTO) / @VL_VENDAS) - @DEPCOM
				END
  
				IF @ANO = YEAR(DATEADD(YY,-1,DATEADD(HH,-3,GETUTCDATE())))
				BEGIN
					-- Ano anterior
					UPDATE #tbDashCliente SET 
						ATIVOS				= dbo.fncCountEQPCliente(@CD_CLIENTE),
						GM_ANO_ANTERIOR		= @GM_ANO,
						VENDAS_ANO_ANTERIOR	= @VL_VENDAS
					WHERE CD_CLIENTE		= @CD_CLIENTE
				END
				ELSE IF @ANO = YEAR(DATEADD(HH,-3,GETUTCDATE()))
				BEGIN
					-- Ano atual              
					UPDATE #tbDashCliente SET 
						ATIVOS				= dbo.fncCountEQPCliente(@CD_CLIENTE),
						GM_ANO_ATUAL		= @GM_ANO,
						VENDAS_ANO_ATUAL	= @VL_VENDAS
					WHERE CD_CLIENTE		= @CD_CLIENTE
				END
				              
				-- Incrementa 1 no ano
				SET @ANO = @ANO + 1

				-- Limpa variáveis
				SET @CD_MES			= NULL
				--SET @NREQP			= NULL
				SET @VL_VENDAS		= NULL
				SET @DEPCOM			= NULL
				SET @VL_MANUTENCAO	= NULL
				SET @DEPRECIACAO	= NULL
				SET @CUSTO			= NULL
				SET @GM_ANO			= NULL
			END          
			-- Retorna para o ano anterior
			SET @ANO = YEAR(DATEADD(YY,-1,DATEADD(HH,-3,GETUTCDATE())))

			FETCH NEXT FROM C1
				INTO @CD_CLIENTE;
		END;
		CLOSE C1;
		DEALLOCATE C1;		
		
		-- Se não tiver informado parâmetro TECNICO, traz todos os clientes inclusive os quem não possuem Técnico
		--IF (@p_TECNICO IS NULL)
		--BEGIN 
			SELECT * FROM #tbDashCliente
			ORDER BY 
				ST_TP_STATUS_VISITA_OS DESC,      
				NM_CLIENTE,
				CD_CLIENTE,
				DS_REGIAO
		--END
		--ELSE
		--BEGIN
		---- Se tiver informado parâmetro TECNICO, traz somente os clientes COM tecnicos
		--	SELECT * FROM #tbDashCliente
		--	WHERE NOT NM_TECNICO_PRINCIPAL IS NULL 
		--	ORDER BY 
		--		ST_TP_STATUS_VISITA_OS DESC,      
		--		NM_CLIENTE,
		--		CD_CLIENTE,
		--		DS_REGIAO
		--END

		DROP TABLE #tbDashClienteValores
		DROP TABLE #tbDashCliente
		DROP TABLE #tbCliente
		DROP TABLE #tbTecnico

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



--select * from tbOSPadrao
--select * from tbSatisfResposta
