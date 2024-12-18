GO
/****** Object:  StoredProcedure [dbo].[prcDashBoxAtendimentoAndamentoSelect]    Script Date: 13/05/2022 14:29:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[prcDashBoxAtendimentoAndamentoSelect]
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

	BEGIN TRY
		
		SELECT @vigenciaInicial = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaINICIAL'
		SELECT @vigenciaFinal = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaFINAL'

		IF (@vigenciaInicial IS NULL)
			SELECT @vigenciaInicial = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-01-01')

		IF (@vigenciaFinal IS NULL)
			SELECT @vigenciaFinal = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-12-31') 

		SELECT 
			COUNT(ST_TP_STATUS_VISITA_OS) AS TOTAL 
		FROM 
			(SELECT DISTINCT 
				TB_TECNICO.CD_TECNICO,
				(SELECT 
					TOP 1 dbo.tbOsPadrao.ST_STATUS_OS
				FROM dbo.tbOsPadrao (NOLOCK)
				WHERE dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO --dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
				AND TB_CLIENTE.DT_DESATIVACAO IS NULL
				AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
				ORDER BY dbo.tbOsPadrao.DT_DATA_OS DESC
				) AS ST_TP_STATUS_VISITA_OS
			FROM	dbo.TB_TECNICO_CLIENTE

			INNER JOIN dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO) AS TB_CLIENTE --dbo.TB_CLIENTE 
				ON dbo.TB_TECNICO_CLIENTE.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
			INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO
				ON dbo.TB_TECNICO_CLIENTE.CD_TECNICO = TB_TECNICO.CD_TECNICO

			WHERE  dbo.TB_TECNICO_CLIENTE.CD_ORDEM = 1
				AND TB_TECNICO.FL_ATIVO = 'S'			-- Tecnico ATIVO
			) Visitas
		WHERE ST_TP_STATUS_VISITA_OS IN (2) -- Aberta, Portaria, Integração e Treinamento

		--SELECT 
		--	COUNT(DISTINCT dbo.tbOsPadrao.ID_VISITA) AS TOTAL
		--FROM dbo.tbOsPadrao (NOLOCK)
		--INNER JOIN dbo.fncDashConsultaTBCLIENTE2(@p_CLIENTE,  @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR) AS TB_CLIENTE --dbo.TB_CLIENTE (NOLOCK)
		--	ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE
		--INNER JOIN dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO
		--	ON dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO
		--WHERE dbo.tbOsPadrao.ST_TP_STATUS_VISITA_OS	IN (1, 2, 8, 9, 10) --Nova, Aberta, Portaria, Integração e Treinamento
		--	AND TB_CLIENTE.DT_DESATIVACAO IS NULL
		--	--AND (dbo.TB_CLIENTE.CD_CLIENTE		= @p_CD_CLIENTE		OR @p_CD_CLIENTE	IS NULL)
		--	AND (TB_CLIENTE.CD_VENDEDOR		= @p_CD_VENDEDOR	OR @p_CD_VENDEDOR	IS NULL)
		
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



