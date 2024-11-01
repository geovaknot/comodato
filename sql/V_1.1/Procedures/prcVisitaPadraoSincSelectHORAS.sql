GO
/****** Object:  StoredProcedure [dbo].[prcVisitaPadraoSincSelectHORAS]    Script Date: 22/02/2022 15:00:59 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcVisitaPadraoSincSelectHORAS]
	@p_CD_TECNICO				varchar(6)			= NULL
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
		
		SELECT top 30 dbo.tbVisitaPadrao.*,
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
		WHERE ( dbo.TB_TECNICO.CD_TECNICO				= @p_CD_TECNICO		OR @p_CD_TECNICO	IS NULL )
		--OR   ( dbo.TB_TECNICO.ID_USUARIO_COORDENADOR	= @p_ID_USUARIO		OR @p_ID_USUARIO	IS NULL )
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