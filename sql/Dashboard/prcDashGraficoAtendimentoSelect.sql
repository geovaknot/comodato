GO
/****** Object:  StoredProcedure [dbo].[prcDashGraficoAtendimentoSelect]    Script Date: 13/05/2022 14:12:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcDashGraficoAtendimentoSelect]
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

			@vigenciaInicial	DATETIME,
			@vigenciaFinal		DATETIME 

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	CREATE TABLE #Resultado (
		TITULO		VARCHAR(25)	NULL,
		TOTAL		INT			NULL
	)
     
	BEGIN TRY
		
		SELECT @vigenciaInicial = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaINICIAL'
		SELECT @vigenciaFinal = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaFINAL'

		IF (@vigenciaInicial IS NULL)
			SELECT @vigenciaInicial = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-01-01')

		IF (@vigenciaFinal IS NULL)
			SELECT @vigenciaFinal = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-12-31') 

		SELECT * INTO #TB_CLIENTE
		FROM dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO)

		INSERT INTO #Resultado
		(
		    TITULO,
		    TOTAL
		)
		SELECT 
			'INSTALAÇÃO' AS TITULO,
			COUNT(*) AS TOTAL
		FROM dbo.tbOsPadrao (NOLOCK)
		--INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO
		--	ON dbo.tbOS.CD_TECNICO = TB_TECNICO.CD_TECNICO
		--INNER JOIN dbo.tbOsPadrao (NOLOCK)
		--	ON dbo.tbOS.ID_OS = dbo.tbOsPadrao.ID_OS

		INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE 
			ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO

		WHERE dbo.tbOsPadrao.CD_TIPO_OS = 3
			AND TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal

		INSERT INTO #Resultado
		(
		    TITULO,
		    TOTAL
		)
		SELECT 
			'OUTROS' AS TITULO,
			COUNT(*) AS TOTAL
		FROM dbo.tbOsPadrao (NOLOCK)
		--INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO
		--	ON dbo.tbOS.CD_TECNICO = TB_TECNICO.CD_TECNICO
		--INNER JOIN dbo.tbOsPadrao (NOLOCK)
		--	ON dbo.tbOS.ID_OS = dbo.tbOsPadrao.ID_OS

		INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE 
			ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO

		WHERE dbo.tbOsPadrao.CD_TIPO_OS = 4
			AND TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal

		INSERT INTO #Resultado
		(
		    TITULO,
		    TOTAL
		)
		SELECT 
			'PREVENTIVA' AS TITULO,
			COUNT(*) AS TOTAL
		FROM dbo.tbOsPadrao (NOLOCK)
		--INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO
		--	ON dbo.tbOS.CD_TECNICO = TB_TECNICO.CD_TECNICO
		--INNER JOIN dbo.tbOsPadrao (NOLOCK)
		--	ON dbo.tbOS.ID_OS = dbo.tbOsPadrao.ID_OS

		INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE 
			ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO

		WHERE dbo.tbOsPadrao.CD_TIPO_OS = 1
			AND TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal

		INSERT INTO #Resultado
		(
		    TITULO,
		    TOTAL
		)
		SELECT 
			'CORRETIVA' AS TITULO,
			COUNT(*) AS TOTAL
		FROM dbo.tbOsPadrao (NOLOCK)
		--INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO
		--	ON dbo.tbOS.CD_TECNICO = TB_TECNICO.CD_TECNICO
		--INNER JOIN dbo.tbOsPadrao (NOLOCK)
		--	ON dbo.tbOS.ID_OS = dbo.tbOsPadrao.ID_OS

		INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE 
			ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO

		WHERE dbo.tbOsPadrao.CD_TIPO_OS = 2	
			AND TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
		
		SELECT * FROM #Resultado
		ORDER BY
			TITULO	

		DROP TABLE #Resultado
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
