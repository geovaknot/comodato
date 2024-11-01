GO
/****** Object:  StoredProcedure [dbo].[prcDashBoxPecaTrocadaSelect]    Script Date: 16/05/2022 08:50:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcDashBoxPecaTrocadaSelect]
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

			@vigenciaInicial				DATETIME,
			@vigenciaFinal					DATETIME

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
		
		--SELECT 
		--	ISNULL(SUM(dbo.tbPecaOS.QT_PECA), 0) AS TOTAL
		--FROM dbo.tbPecaOS (NOLOCK)
		--INNER JOIN dbo.tbOS (NOLOCK)
		--	ON dbo.tbPecaOS.ID_OS = dbo.tbOS.ID_OS
		--INNER JOIN dbo.tbVisita (NOLOCK)
		--	ON dbo.tbOS.ID_VISITA = dbo.tbVisita.ID_VISITA
		--INNER JOIN dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO) AS TB_CLIENTE
		--	ON dbo.tbVisita.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		--WHERE 
		--	dbo.tbOS.DT_DATA_ABERTURA		BETWEEN @vigenciaInicial AND @vigenciaFinal
		--	AND TB_CLIENTE.DT_DESATIVACAO	IS NULL
		--	AND (TB_CLIENTE.CD_VENDEDOR		= @p_CD_VENDEDOR OR @p_CD_VENDEDOR IS NULL)
		
		SELECT ISNULL(SUM(dbo.tbPecaOS.QT_PECA), 0) AS TOTAL 
		FROM dbo.tbOsPadrao (NOLOCK)
		--INNER JOIN dbo.tbOS (NOLOCK)
		--	ON tbOS.ID_VISITA = tbVisita.ID_VISITA
		INNER JOIN dbo.tbPecaOS (NOLOCK)
			ON tbPecaOS.ID_OS = tbOsPadrao.ID_OS

		INNER JOIN dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO) AS TB_CLIENTE --dbo.TB_CLIENTE 
			ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
			ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO
		
		WHERE TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
			AND ( dbo.tbOSPadrao.ST_STATUS_OS		= 3 
					OR dbo.tbOSPadrao.ST_STATUS_OS	IS NULL )		

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


