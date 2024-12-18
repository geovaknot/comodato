GO
/****** Object:  StoredProcedure [dbo].[prcDashListaTecnicoSelect]    Script Date: 13/05/2022 11:35:20 ******/
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
ALTER PROCEDURE [dbo].[prcDashListaTecnicoSelect]
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
		
		SELECT DISTINCT 
			TB_TECNICO.CD_TECNICO,
			TB_TECNICO.NM_TECNICO,
			TB_TECNICO.NM_REDUZIDO,
			TB_TECNICO.EN_ESTADO,
			(SELECT 
				COUNT(*) 
			FROM dbo.tbOsPadrao (NOLOCK)
			WHERE dbo.tbOsPadrao.CD_TECNICO = TB_TECNICO.CD_TECNICO
			AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
			) AS VISITAS,
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

		ORDER BY
			ST_TP_STATUS_VISITA_OS DESC,
			TB_TECNICO.NM_TECNICO			
		
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


