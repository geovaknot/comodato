GO
/****** Object:  StoredProcedure [dbo].[prcVisitaPadraoSelect]    Script Date: 12/11/2021 14:02:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcVisitaPadraoSelect]
	@p_ID_VISITA				BIGINT			= NULL, 
	@p_DT_DATA_VISITA			DATETIME		= NULL,
	@p_DT_INICIO				DATETIME		= NULL,
	@p_DT_FIM					DATETIME		= NULL,	
	@p_ST_STATUS_VISITA			INT				= NULL,
	@p_CD_CLIENTE				INT				= NULL,
	@p_CD_TECNICO				VARCHAR(06)		= NULL,
	@p_DS_OBSERVACAO			VARCHAR(MAX)	= NULL,
	@p_CD_MOTIVO_VISITA			INT				= NULL,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0)	= NULL,
	@p_CD_REGIAO				VARCHAR(06)		= NULL
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
		
		SELECT	dbo.tbVisitaPadrao.*,
				dbo.TB_CLIENTE.NM_CLIENTE,
				dbo.TB_CLIENTE.EN_CIDADE,
				dbo.TB_CLIENTE.EN_ESTADO,
				dbo.TB_CLIENTE.EN_ENDERECO,
				dbo.TB_CLIENTE.EN_BAIRRO,
				dbo.TB_CLIENTE.EN_CEP,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_TECNICO.CD_EMPRESA,
				dbo.tbTpStatusVisitaPadrao.DS_STATUS_VISITA,
				dbo.tbTpMotivoVisitaPadrao.DS_MOTIVO_VISITA,
				dbo.TB_TECNICO_CLIENTE.CD_ORDEM,
				dbo.TB_CLIENTE.CD_REGIAO,
				dbo.V_REGIAO.DS_REGIAO,
				dbo.TB_EMPRESA.NM_EMPRESA
		FROM	dbo.tbVisitaPadrao
		INNER JOIN dbo.TB_CLIENTE
			ON dbo.tbVisitaPadrao.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.TB_TECNICO
			ON dbo.tbVisitaPadrao.CD_TECNICO = dbo.TB_TECNICO.CD_TECNICO
		INNER JOIN dbo.tbTpStatusVisitaPadrao
			ON dbo.tbVisitaPadrao.ST_STATUS_VISITA = dbo.tbTpStatusVisitaPadrao.ST_STATUS_VISITA
		INNER JOIN dbo.tbTpMotivoVisitaPadrao
			ON dbo.tbVisitaPadrao.CD_MOTIVO_VISITA = dbo.tbTpMotivoVisitaPadrao.CD_MOTIVO_VISITA
		LEFT JOIN dbo.TB_TECNICO_CLIENTE
			ON dbo.tbVisitaPadrao.CD_TECNICO = dbo.TB_TECNICO_CLIENTE.CD_TECNICO
			AND dbo.tbVisitaPadrao.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
		LEFT JOIN dbo.v_regiao
			on dbo.tb_cliente.cd_regiao = dbo.v_regiao.cd_regiao
		LEFT JOIN dbo.TB_EMPRESA
			on dbo.TB_TECNICO.CD_EMPRESA = dbo.TB_EMPRESA.CD_EMPRESA
		WHERE (	dbo.tbVisitaPadrao.ID_VISITA			= @p_ID_VISITA			OR @p_ID_VISITA			IS NULL )
		AND	  (	dbo.tbVisitaPadrao.DT_DATA_VISITA		>= @p_DT_DATA_VISITA	OR @p_DT_DATA_VISITA	IS NULL )
		AND   ( dbo.tbVisitaPadrao.ST_STATUS_VISITA		= @p_ST_STATUS_VISITA	OR @p_ST_STATUS_VISITA	IS NULL )
		AND   ( dbo.tbVisitaPadrao.CD_CLIENTE			= @p_CD_CLIENTE			OR @p_CD_CLIENTE		IS NULL )
		AND   ( dbo.tbVisitaPadrao.CD_TECNICO			= @p_CD_TECNICO			OR @p_CD_TECNICO		IS NULL )
		AND   ( dbo.tbVisitaPadrao.DS_OBSERVACAO		LIKE @p_DS_OBSERVACAO	OR @p_DS_OBSERVACAO		IS NULL )
		AND   ( dbo.tbVisitaPadrao.CD_MOTIVO_VISITA		= @p_CD_MOTIVO_VISITA	OR @p_CD_MOTIVO_VISITA	IS NULL )
		AND   ( dbo.TB_CLIENTE.CD_REGIAO				= @p_CD_REGIAO			OR @p_CD_REGIAO			IS NULL )
		AND	  (	dbo.tbVisitaPadrao.DT_DATA_VISITA		>= @p_DT_INICIO			OR @p_DT_INICIO			IS NULL )
		AND	  (	convert(date, dbo.tbVisitaPadrao.DT_DATA_VISITA, 23) <= convert(date, @p_DT_FIM, 23) OR @p_DT_FIM IS NULL )
		ORDER BY
				dbo.tbVisitaPadrao.DT_DATA_VISITA DESC 

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

