GO
/****** Object:  StoredProcedure [dbo].[prcDashGraficoAtendimentoTecnicoRegionalSelect]    Script Date: 13/05/2022 14:51:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcDashGraficoAtendimentoTecnicoRegionalSelect]
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
	DECLARE @cdsErrorMessage				NVARCHAR(4000),
			@nidErrorSeverity				INT,
			@nidErrorState					INT,
			@TOTAL							INT,

			@vigenciaInicial				DATETIME,
			@vigenciaFinal					DATETIME,
			@CD_TECNICO						VARCHAR(06),
			@QT_PERIODO_vigencia			INT,
			@PERCENTUAL_Dias_percorridos	NUMERIC(18,2),
			@ID_VISITA						BIGINT,
			@TEMPO							NUMERIC(18,5)	= 0,
			@TOTAL_VISITA					NUMERIC(18,5)	= 0,
			@TOTAL_VISITA_REALIZADO			NUMERIC(18,5)	= 0,
			@TOTAL_VISITA_PLANEJADO			NUMERIC(18,5)	= 0,
			@PERCENTUAL_periodos_realizado	NUMERIC(18,2),

			@PERCENTUAL_GRAFICO_REALIZADO	NUMERIC(18,2),
			@PERCENTUAL_GRAFICO_GAP			NUMERIC(18,2)
			--@PERCENTUAL_GRAFICO_RESTANTE	NUMERIC(5,2)
			--@TIPO_GAP						CHAR(1)


	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

--	BEGIN TRY

		SELECT * 
		INTO #TB_CLIENTE
		FROM dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO) AS TB_CLIENTE -- (NOLOCK)

		-- Total de Períodos
		SELECT 
			@TOTAL = ISNULL(SUM(QT_PERIODO),0)
		FROM #TB_CLIENTE;

		SELECT @vigenciaInicial = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaINICIAL'
		SELECT @vigenciaFinal = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaFINAL'

		IF (@vigenciaInicial IS NULL)
			SELECT @vigenciaInicial = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-01-01')

		IF (@vigenciaFinal IS NULL)
			SELECT @vigenciaFinal = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-12-31') 

		SET @QT_PERIODO_vigencia = @TOTAL;
		SET @PERCENTUAL_Dias_percorridos = (CONVERT(INT, DATEDIFF(DAY, @vigenciaInicial, DATEADD(HH,-3,GETUTCDATE())) + 1) * 100) / CONVERT(INT, DATEDIFF(DAY, @vigenciaInicial, @vigenciaFinal) + 1)

		SELECT 
			dbo.tbOSPadrao.CD_TECNICO, 
			COUNT(*) TOTAL,
			0 AS TOTAL_VISITA_REALIZADO,
			0 AS TOTAL_VISITA_PLANEJADO
		INTO #Tecnicos
		FROM dbo.tbOSPadrao (NOLOCK)
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOSPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO
		WHERE 
			DT_DATA_OS			BETWEEN @vigenciaInicial AND @vigenciaFinal 
			AND TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
		GROUP BY 
			dbo.tbOSPadrao.CD_TECNICO 
		ORDER BY 
			TOTAL DESC 

		-- Faz a consulta por CD_TECNICO
		DECLARE C0 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT CD_TECNICO
		FROM #Tecnicos

		OPEN C0
		FETCH NEXT FROM C0
			INTO @CD_TECNICO

		WHILE @@FETCH_STATUS = 0
		BEGIN 

			-- Busca todas as visitas de cada Técnico no período
			DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
			SELECT 
				dbo.tbOSPadrao.ID_OS 
			FROM dbo.tbOSPadrao (NOLOCK) 

			INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE 
				ON dbo.tbOSPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
			INNER JOIN dbo.fncDashConsultaTBTECNICO(@CD_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
				ON dbo.tbOSPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO

			--INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE (NOLOCK)
			--	ON dbo.tbVisita.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
			--INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO
			--	ON tbVisita.CD_TECNICO = TB_TECNICO.CD_TECNICO
			WHERE 
			--YEAR(dbo.tbVisita.DT_DATA_VISITA)	= YEAR(DATEADD(HH,-3,GETUTCDATE()))
				TB_CLIENTE.DT_DESATIVACAO IS NULL	-- Cliente ATIVO
				AND TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
				AND dbo.tbOSPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
				AND (TB_CLIENTE.CD_VENDEDOR		= @p_CD_VENDEDOR OR @p_CD_VENDEDOR IS NULL)
				--AND TB_TECNICO.CD_TECNICO = @CD_TECNICO
			OPEN C1
			FETCH NEXT FROM C1
				INTO @ID_VISITA

			WHILE @@FETCH_STATUS = 0
			BEGIN
				-- Calcula o tempo de cada visita
				SET @TEMPO = dbo.fncCalcularTempoGastoVisita(@ID_VISITA)
				-- Soma o tempo de cada visita
				SET @TOTAL_VISITA = ISNULL(@TOTAL_VISITA,0) + ISNULL(@TEMPO,0)

				FETCH NEXT FROM C1
					INTO @ID_VISITA;
			END;
			CLOSE C1;
			DEALLOCATE C1;		

			-- Converte o tempo gasto em períodos
			SET @TOTAL_VISITA = ISNULL(@TOTAL_VISITA,0) / 3
			
			SET @TOTAL_VISITA_REALIZADO = @TOTAL_VISITA
			IF ISNULL(@QT_PERIODO_vigencia,0) <> 0
				SET @PERCENTUAL_periodos_realizado = (@TOTAL_VISITA * 100) / @QT_PERIODO_vigencia
			ELSE 
				SET @PERCENTUAL_periodos_realizado = 0

			IF @PERCENTUAL_periodos_realizado > 100
				SET @PERCENTUAL_periodos_realizado = 100

			--SELECT @QT_PERIODO_vigencia AS QT_Periodos_vigencia, @TOTAL_VISITA AS QT_Periodos_consumidos, @PERCENTUAL_periodos_realizado AS PERCENTUAL_periodos_realizado

			IF @PERCENTUAL_periodos_realizado < @PERCENTUAL_Dias_percorridos
			BEGIN 
				--SET @TIPO_GAP = '-'
				SET @PERCENTUAL_GRAFICO_REALIZADO = @PERCENTUAL_periodos_realizado
				SET @PERCENTUAL_GRAFICO_GAP = @PERCENTUAL_Dias_percorridos - @PERCENTUAL_periodos_realizado
				--SET @PERCENTUAL_GRAFICO_RESTANTE = 100 - (@PERCENTUAL_GRAFICO_REALIZADO + @PERCENTUAL_GRAFICO_GAP)
			END 
			ELSE IF @PERCENTUAL_periodos_realizado > @PERCENTUAL_Dias_percorridos
			BEGIN 
				--SET @TIPO_GAP = '+'
				SET @PERCENTUAL_GRAFICO_REALIZADO = @PERCENTUAL_periodos_realizado - (@PERCENTUAL_periodos_realizado - @PERCENTUAL_Dias_percorridos)
				SET @PERCENTUAL_GRAFICO_GAP = @PERCENTUAL_periodos_realizado - @PERCENTUAL_Dias_percorridos
				--SET @PERCENTUAL_GRAFICO_RESTANTE = 100 - (@PERCENTUAL_GRAFICO_REALIZADO + @PERCENTUAL_GRAFICO_GAP)
			END 
			ELSE 
			BEGIN
				--SET @TIPO_GAP = '+'      
				SET @PERCENTUAL_GRAFICO_REALIZADO = @PERCENTUAL_periodos_realizado
				SET @PERCENTUAL_GRAFICO_GAP = 0
				--SET @PERCENTUAL_GRAFICO_RESTANTE = 100 - (@PERCENTUAL_GRAFICO_REALIZADO + @PERCENTUAL_GRAFICO_GAP)
			END 

			SET @TOTAL_VISITA_PLANEJADO = ((@PERCENTUAL_GRAFICO_REALIZADO + @PERCENTUAL_GRAFICO_GAP) * @TOTAL) / 100

			UPDATE #Tecnicos SET 
				TOTAL_VISITA_REALIZADO = ISNULL(@TOTAL_VISITA_REALIZADO,0),
				TOTAL_VISITA_PLANEJADO = ISNULL(@TOTAL_VISITA_PLANEJADO,0)
			WHERE CD_TECNICO = @CD_TECNICO

			FETCH NEXT FROM C0
				INTO @CD_TECNICO;
		END;

		CLOSE C0;
		DEALLOCATE C0;

		--SELECT 
		--	#Tecnicos.*,
		--	dbo.TB_TECNICO.NM_REDUZIDO
		--FROM #Tecnicos
		--INNER JOIN dbo.TB_TECNICO (NOLOCK)
		--	ON TB_TECNICO.CD_TECNICO = #Tecnicos.CD_TECNICO
		--ORDER BY 
		--	dbo.TB_TECNICO.NM_REDUZIDO

		SET @p_TECNICO = '%' + LTRIM(RTRIM(@p_TECNICO)) + '%'

		SELECT 
			dbo.tbUsuario.nidUsuario AS CD_TECNICO,
			SUBSTRING(dbo.tbUsuario.cnmNome, 0, 10) AS NM_REDUZIDO,
			ISNULL(tbTecnicos.TOTAL,0) AS TOTAL,
			ISNULL(tbTecnicos.TOTAL_VISITA_REALIZADO,0) AS TOTAL_VISITA_REALIZADO,
			ISNULL(tbTecnicos.TOTAL_VISITA_PLANEJADO,0) AS TOTAL_VISITA_PLANEJADO
		FROM dbo.tbUsuario (NOLOCK)
		INNER JOIN dbo.tbUsuarioPerfil (NOLOCK)
			ON tbUsuarioPerfil.nidUsuario = tbUsuario.nidUsuario
		LEFT JOIN (
		SELECT 
			dbo.TB_TECNICO.ID_USUARIO_TECNICOREGIONAL,
			SUM(TOTAL) AS TOTAL,
			SUM(TOTAL_VISITA_REALIZADO) AS TOTAL_VISITA_REALIZADO,
			SUM(TOTAL_VISITA_PLANEJADO) AS TOTAL_VISITA_PLANEJADO
		FROM #Tecnicos
		INNER JOIN dbo.TB_TECNICO (NOLOCK)
			ON TB_TECNICO.CD_TECNICO = #Tecnicos.CD_TECNICO
		GROUP BY
			dbo.TB_TECNICO.ID_USUARIO_TECNICOREGIONAL
		) tbTecnicos
			ON tbTecnicos.ID_USUARIO_TECNICOREGIONAL = dbo.tbUsuario.nidUsuario
		WHERE nidPerfil = (SELECT nidPerfil FROM dbo.tbPerfil (NOLOCK) WHERE cdsPerfil = 'Técnico 3M')
			AND dbo.tbUsuario.bidAtivo = 1
			AND (dbo.tbUsuario.cnmNome LIKE @p_TECNICO OR @p_TECNICO IS NULL)
		ORDER BY 
			SUBSTRING(dbo.tbUsuario.cnmNome, 0, 10)


		DROP TABLE #TB_CLIENTE
		DROP TABLE #Tecnicos

	--END TRY

	--BEGIN CATCH

	--	SELECT	@cdsErrorMessage	= ERROR_MESSAGE(),
	--			@nidErrorSeverity	= ERROR_SEVERITY(),
	--			@nidErrorState		= ERROR_STATE();

	--	-- Use RAISERROR inside the CATCH block to return error
	--	-- information about the original error that caused
	--	-- execution to jump to the CATCH block.
	--	RAISERROR (@cdsErrorMessage, -- Message text.
	--			   @nidErrorSeverity, -- Severity.
	--			   @nidErrorState -- State.
	--			   )

	--END CATCH

END


