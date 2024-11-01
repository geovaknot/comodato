GO
/****** Object:  StoredProcedure [dbo].[prcDashBoxPesquisaSatisfacaoSelect]    Script Date: 13/05/2022 14:48:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcDashBoxPesquisaSatisfacaoSelect]
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
			@vigenciaFinal		DATETIME,
			@Nota				DECIMAL(18,2),
			@Divisor			INT,
			@Media				DECIMAL(18,2)

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
		
		SELECT 
			@Nota = ISNULL(SUM(dbo.tbSatisfResposta.NM_NOTA_PESQ), 0), 
			@Divisor = ISNULL(COUNT(*), 0)
		FROM dbo.tbOsPadrao (NOLOCK)
		INNER JOIN dbo.tbSatisfResposta (NOLOCK)
		ON tbSatisfResposta.ID_OS = tbOsPadrao.ID_OS

		INNER JOIN dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO) AS TB_CLIENTE --dbo.TB_CLIENTE 
			ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO

		WHERE TB_TECNICO.FL_ATIVO = 'S'		-- Tecnico ATIVO
			--AND dbo.tbOsPadrao.DT_DATA_VISITA BETWEEN @vigenciaInicial AND @vigenciaFinal
			AND dbo.tbSatisfResposta.DT_DATA_RESPOSTA BETWEEN @vigenciaInicial AND @vigenciaFinal

		IF @Divisor = 0
			SET @Divisor = 1

		SET @Media = @Nota / @Divisor;

		SELECT @Media AS TOTAL

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


