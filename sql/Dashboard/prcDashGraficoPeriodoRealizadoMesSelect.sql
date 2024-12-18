GO
/****** Object:  StoredProcedure [dbo].[prcDashGraficoPeriodoRealizadoMesSelect]    Script Date: 13/05/2022 14:55:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Alex Natalino
-- Create date: 12/03/2018
-- Description:	Seleção de dados para Dashboard
--              
-- =============================================
ALTER PROCEDURE [dbo].[prcDashGraficoPeriodoRealizadoMesSelect]
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

			--@CD_TECNICO			VARCHAR(06),
			@ID_VISITA			BIGINT,
			@DT_DATA_VISITA		DATETIME,
			@TEMPO				NUMERIC(18,5)	= 0,
			@QT_PERIODOS		BIGINT			= 0,
			@vigenciaInicial	DATETIME,
			@vigenciaFinal		DATETIME 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		SELECT @vigenciaInicial = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaINICIAL'
		SELECT @vigenciaFinal = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaFINAL'

		IF (@vigenciaInicial IS NULL)
			SELECT @vigenciaInicial = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-01-01')

		IF (@vigenciaFinal IS NULL)
			SELECT @vigenciaFinal = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-12-31') 
		
		CREATE TABLE #AnoAtual 
		(
			CD_MES				NUMERIC(06),
			DS_MES				VARCHAR(10),
			QT_PERIODOS			BIGINT
		)

		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '01', 'JAN', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '02', 'FEV', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '03', 'MAR', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '04', 'ABR', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '05', 'MAI', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '06', 'JUN', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '07', 'JUL', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '08', 'AGO', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '09', 'SET', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '10', 'OUT', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '11', 'NOV', 0)
		INSERT INTO #AnoAtual (CD_MES, DS_MES, QT_PERIODOS) VALUES (CONVERT(VARCHAR,YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '12', 'DEZ', 0)

		SELECT * INTO #TB_CLIENTE
		FROM dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO)

		--CREATE TABLE #tbAgendaTecnico
		--(
		--	CD_CLIENTE				NUMERIC(6,0)	NULL,
		--	NM_CLIENTE				VARCHAR(50)		NULL,
		--	EN_CIDADE				VARCHAR(50)		NULL,
		--	EN_ESTADO				VARCHAR(03)		NULL,
		--	CD_REGIAO				VARCHAR(06)		NULL,
		--	DS_REGIAO				VARCHAR(50)		NULL,
		--	DT_DESATIVACAO			DATETIME		NULL,
		--	CD_TECNICO_PRINCIPAL	VARCHAR(06)		NULL,
		--	NM_TECNICO_PRINCIPAL	VARCHAR(50)		NULL,
		--	QT_PERIODO				INT				NULL,
		--	NR_ORDENACAO			INT				NULL,
		--	ID_VISITA				BIGINT			NULL,
		--	ST_TP_STATUS_VISITA_OS	INT				NULL,
		--	DT_DATA_VISITA			DATETIME		NULL,
		--	DS_TP_STATUS_VISITA_OS	VARCHAR(20)		NULL,
		--	ID_AGEND				BIGINT			NULL,
		--	CD_MES					VARCHAR(06)		NULL
		--)

		--DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		--SELECT DISTINCT 
		--	TB_TECNICO.CD_TECNICO
		--FROM	dbo.TB_TECNICO_CLIENTE

		--INNER JOIN dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO) AS TB_CLIENTE --dbo.TB_CLIENTE 
		--	ON dbo.TB_TECNICO_CLIENTE.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		--INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
		--	ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO

		--WHERE  dbo.TB_TECNICO_CLIENTE.CD_ORDEM = 1
		--		AND TB_CLIENTE.DT_DESATIVACAO IS NULL
		--		AND TB_TECNICO.FL_ATIVO = 'S'
		--		AND (TB_CLIENTE.CD_VENDEDOR				= @p_CD_VENDEDOR		OR @p_CD_VENDEDOR	IS NULL)
		--ORDER BY
		--	TB_TECNICO.CD_TECNICO			
	
		--OPEN C1
		--FETCH NEXT FROM C1
		--	INTO @CD_TECNICO
	
		--WHILE @@FETCH_STATUS = 0
		--BEGIN
			
		--	INSERT INTO #tbAgendaTecnico 
		--	(
		--		CD_CLIENTE,
		--		NM_CLIENTE,
		--		EN_CIDADE,
		--		EN_ESTADO,
		--		CD_REGIAO,
		--		DS_REGIAO,
		--		DT_DESATIVACAO,
		--		CD_TECNICO_PRINCIPAL,
		--		NM_TECNICO_PRINCIPAL,
		--		QT_PERIODO,
		--		NR_ORDENACAO,
		--		ID_VISITA,
		--		ST_TP_STATUS_VISITA_OS,
		--		DT_DATA_VISITA,
		--		DS_TP_STATUS_VISITA_OS,
		--		ID_AGEND
		--	)
		--	EXEC dbo.prcAgendaSelectAtendimento 
		--		@p_nidUsuario = NULL,
		--		@p_CD_CLIENTE = NULL,
		--		@p_CD_TECNICO = @CD_TECNICO,
		--		@p_nvlQtdeTecnicos = NULL,
		--		@p_ST_TP_STATUS_VISITA_OS = NULL,
		--		@p_CD_REGIAO = NULL
	
		--	FETCH NEXT FROM C1
		--		INTO @CD_TECNICO;
		--END;
		--CLOSE C1;
		--DEALLOCATE C1;     

		--UPDATE #tbAgendaTecnico SET 
		--	CD_MES = CONVERT(CHAR(06), #tbAgendaTecnico.DT_DATA_VISITA, 112)

		--SELECT
		--	#AnoAtual.CD_MES,
		--	#AnoAtual.DS_MES,
		--	--CONVERT(CHAR(06), #tbAgendaTecnico.DT_DATA_VISITA, 112) AS CD_MES 
		--	ISNULL(SUM(#tbAgendaTecnico.QT_PERIODO),0) AS TOTAL
		--FROM #AnoAtual
		--LEFT JOIN #tbAgendaTecnico 
		--ON #AnoAtual.CD_MES = #tbAgendaTecnico.CD_MES
		--AND #tbAgendaTecnico.DT_DATA_VISITA BETWEEN @vigenciaInicial AND @vigenciaFinal
		--GROUP BY 
		--	#AnoAtual.CD_MES,
		--	#AnoAtual.DS_MES
		--ORDER BY
		--	#AnoAtual.CD_MES
        
		--If(OBJECT_ID('tempdb..#tbAgendaTecnico') Is Not Null)
		--BEGIN
		--	DROP TABLE #tbAgendaTecnico
		--END

		-- Busca todas as visitas dos clientes no ano período
		DECLARE C1 CURSOR LOCAL FORWARD_ONLY FAST_FORWARD FOR
		SELECT 
			dbo.tbOsPadrao.ID_OS AS ID_VISITA,
			dbo.tbOsPadrao.DT_DATA_OS AS DT_DATA_VISITA 
		FROM dbo.tbOsPadrao (NOLOCK) 

		INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE 
			ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO

		--INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE (NOLOCK)
		--	ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		--INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO
		--	ON tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO
		WHERE TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
		
		OPEN C1
		FETCH NEXT FROM C1
			INTO @ID_VISITA, @DT_DATA_VISITA

		WHILE @@FETCH_STATUS = 0
		BEGIN
			-- Calcula o tempo de cada visita
			SET @TEMPO = dbo.fncCalcularTempoGastoVisita(@ID_VISITA)

			-- Converte em período
			SET @QT_PERIODOS = @TEMPO / 3

			UPDATE #AnoAtual SET QT_PERIODOS = QT_PERIODOS + @QT_PERIODOS
			WHERE RIGHT(CD_MES,2) = MONTH(@DT_DATA_VISITA)

			FETCH NEXT FROM C1
				INTO @ID_VISITA, @DT_DATA_VISITA;
		END;
		CLOSE C1;
		DEALLOCATE C1;	
		
		-- Busca a SOMA QT_PERIODOS dos clientes
		SELECT @QT_PERIODOS = ISNULL(SUM(QT_PERIODO),0)
		FROM #TB_CLIENTE

		SELECT *, @QT_PERIODOS AS TOTAL_KAT FROM #AnoAtual

		DROP TABLE #AnoAtual
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


