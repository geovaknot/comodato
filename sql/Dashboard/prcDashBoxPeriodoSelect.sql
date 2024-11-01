GO
/****** Object:  StoredProcedure [dbo].[prcDashBoxPeriodoSelect]    Script Date: 13/05/2022 14:36:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[prcDashBoxPeriodoSelect]
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
			@QT_PERIODO_vigencia			INT,
			@PERCENTUAL_Dias_percorridos	NUMERIC(5,2),
			@ID_VISITA						BIGINT,
			@TEMPO							NUMERIC(18,5)	= 0,
			@TOTAL_VISITA					NUMERIC(18,5)	= 0,
			@TOTAL_VISITA_REALIZADO			NUMERIC(18,5)	= 0,
			@TOTAL_VISITA_PLANEJADO			NUMERIC(18,5)	= 0,
			@PERCENTUAL_periodos_realizado	NUMERIC(18,2),

			@PERCENTUAL_GRAFICO_REALIZADO	NUMERIC(18,2),
			@PERCENTUAL_GRAFICO_GAP			NUMERIC(18,2),
			@PERCENTUAL_GRAFICO_RESTANTE	NUMERIC(18,2),
			@TIPO_GAP						CHAR(1)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		SELECT * 
		INTO #TB_CLIENTE
		FROM dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO) AS TB_CLIENTE

		-- Total de Períodos
		SELECT 
			@TOTAL = SUM(QT_PERIODO)
		FROM #TB_CLIENTE;

		-- Percentuais do Gráfico
		SELECT @vigenciaInicial = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaINICIAL'
		SELECT @vigenciaFinal = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaFINAL'

		IF (@vigenciaInicial IS NULL)
			SELECT @vigenciaInicial = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-01-01')

		IF (@vigenciaFinal IS NULL)
			SELECT @vigenciaFinal = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-12-31')

		SET @QT_PERIODO_vigencia = @TOTAL;

/*
		SELECT 
			@QT_PERIODO_vigencia = SUM(QT_PERIODO) 
		FROM dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO) AS TB_CLIENTE --dbo.TB_CLIENTE 
		WHERE TB_CLIENTE.DT_DESATIVACAO		IS NULL
		--AND (dbo.TB_CLIENTE.NM_CLIENTE	LIKE @p_NM_CLIENTE		OR @p_NM_CLIENTE	IS NULL)      
		--AND (dbo.TB_CLIENTE.CD_CLIENTE	= @p_CD_CLIENTE			OR @p_CD_CLIENTE	IS NULL)
		AND (TB_CLIENTE.CD_VENDEDOR			= @p_CD_VENDEDOR		OR @p_CD_VENDEDOR	IS NULL)
*/		

		SET @PERCENTUAL_Dias_percorridos = (CONVERT(INT, DATEDIFF(DAY, @vigenciaInicial, DATEADD(HH,-3,GETUTCDATE())) + 1) * 100) / CONVERT(INT, DATEDIFF(DAY, @vigenciaInicial, @vigenciaFinal) + 1)

		-- Busca todas as visitas dos clientes no ano período
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT 
			dbo.tbOsPadrao.ID_OS 
		FROM dbo.tbOsPadrao (NOLOCK) 

		--INNER JOIN dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR) AS TB_CLIENTE --dbo.TB_CLIENTE 
		--	ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN #TB_CLIENTE AS TB_CLIENTE
			ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO
		WHERE TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
		
		OPEN C1
		FETCH NEXT FROM C1
			INTO @ID_VISITA

		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Calcula o tempo de cada visita
			SET @TEMPO = dbo.fncCalcularTempoGastoVisita(@ID_VISITA)
			-- Soma o tempo de cada visita
			SET @TOTAL_VISITA = @TOTAL_VISITA + @TEMPO

			FETCH NEXT FROM C1
				INTO @ID_VISITA;
		END;
		CLOSE C1;
		DEALLOCATE C1;		

		-- Converte o tempo gasto em períodos
		SET @TOTAL_VISITA = @TOTAL_VISITA / 3
		SET @TOTAL_VISITA_REALIZADO = @TOTAL_VISITA
		IF @QT_PERIODO_vigencia <> 0
			SET @PERCENTUAL_periodos_realizado = (@TOTAL_VISITA * 100) / @QT_PERIODO_vigencia
		ELSE 
			SET @PERCENTUAL_periodos_realizado = 0

		IF @PERCENTUAL_periodos_realizado > 100
			SET @PERCENTUAL_periodos_realizado = 100

		--SELECT @QT_PERIODO_vigencia AS QT_Periodos_vigencia, @TOTAL_VISITA AS QT_Periodos_consumidos, @PERCENTUAL_periodos_realizado AS PERCENTUAL_periodos_realizado

		IF @PERCENTUAL_periodos_realizado < @PERCENTUAL_Dias_percorridos
		BEGIN 
			SET @TIPO_GAP = '-'
			SET @PERCENTUAL_GRAFICO_REALIZADO = @PERCENTUAL_periodos_realizado
			SET @PERCENTUAL_GRAFICO_GAP = @PERCENTUAL_Dias_percorridos - @PERCENTUAL_periodos_realizado
			SET @PERCENTUAL_GRAFICO_RESTANTE = 100 - (@PERCENTUAL_GRAFICO_REALIZADO + @PERCENTUAL_GRAFICO_GAP)
		END 
		ELSE IF @PERCENTUAL_periodos_realizado > @PERCENTUAL_Dias_percorridos
		BEGIN 
			SET @TIPO_GAP = '+'
			SET @PERCENTUAL_GRAFICO_REALIZADO = @PERCENTUAL_periodos_realizado - (@PERCENTUAL_periodos_realizado - @PERCENTUAL_Dias_percorridos)
			SET @PERCENTUAL_GRAFICO_GAP = @PERCENTUAL_periodos_realizado - @PERCENTUAL_Dias_percorridos
			SET @PERCENTUAL_GRAFICO_RESTANTE = 100 - (@PERCENTUAL_GRAFICO_REALIZADO + @PERCENTUAL_GRAFICO_GAP)
		END 
		ELSE 
		BEGIN
			SET @TIPO_GAP = '+'      
			SET @PERCENTUAL_GRAFICO_REALIZADO = @PERCENTUAL_periodos_realizado
			SET @PERCENTUAL_GRAFICO_GAP = 0
			SET @PERCENTUAL_GRAFICO_RESTANTE = 100 - (@PERCENTUAL_GRAFICO_REALIZADO + @PERCENTUAL_GRAFICO_GAP)
		END 

		SET @TOTAL_VISITA_PLANEJADO = ((@PERCENTUAL_GRAFICO_REALIZADO + @PERCENTUAL_GRAFICO_GAP) * @TOTAL) / 100

		SELECT	@TOTAL AS TOTAL, 
				@TOTAL_VISITA_REALIZADO AS TOTAL_VISITA_REALIZADO, 
				@TOTAL_VISITA_PLANEJADO AS TOTAL_VISITA_PLANEJADO, 
				@PERCENTUAL_GRAFICO_REALIZADO AS PERCENTUAL_GRAFICO_REALIZADO, 
				@PERCENTUAL_GRAFICO_GAP AS PERCENTUAL_GRAFICO_GAP, 
				@PERCENTUAL_GRAFICO_RESTANTE AS PERCENTUAL_GRAFICO_RESTANTE, 
				@TIPO_GAP AS TIPO_GAP

		--SELECT @TOTAL AS TOTAL, 19 AS PERCENTUAL_GRAFICO_REALIZADO, 60 AS PERCENTUAL_GRAFICO_GAP, 21 AS PERCENTUAL_GRAFICO_RESTANTE, '-' AS TIPO_GAP

		DROP TABLE #TB_CLIENTE
					
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



