GO
/****** Object:  StoredProcedure [dbo].[prcOSPadraoSelectHoras]    Script Date: 23/02/2022 08:26:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcOSPadraoSelectHoras]
	@p_ID_OS				BIGINT		= NULL, 
	@p_DT_DATA_OS			DATETIME	= NULL,	
	@p_DT_INICIO			DATETIME	= NULL,
	@p_DT_FIM				DATETIME	= NULL,
	@p_ST_STATUS_OS			INT		= NULL,
	@p_CD_TIPO_OS			INT		= NULL,
	@p_CD_CLIENTE			INT		= NULL,
	@p_CD_TECNICO			VARCHAR(06)	= Null,
	@p_nidUsuarioAtualizacao	NUMERIC(18,0) = NULL,
	@p_CD_REGIAO				VARCHAR(06) = NULL


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

		
		SELECT top 30	dbo.tbOSPadrao.*,
				dbo.TB_CLIENTE.NM_CLIENTE,
				dbo.TB_CLIENTE.EN_CIDADE,
				dbo.TB_CLIENTE.EN_ESTADO,
				dbo.TB_CLIENTE.EN_ENDERECO,
				dbo.TB_CLIENTE.EN_BAIRRO,
				dbo.TB_CLIENTE.EN_CEP,
				dbo.TB_TECNICO.NM_TECNICO,
				dbo.TB_TECNICO.NM_REDUZIDO,
				dbo.tbTpStatusOSPadrao.DS_STATUS_OS,
				dbo.tbTpOSPadrao.DS_TIPO_OS,
				dbo.TB_TECNICO_CLIENTE.CD_ORDEM,
				dbo.TB_CLIENTE.CD_REGIAO,
				dbo.V_REGIAO.DS_REGIAO,
				dbo.TB_EMPRESA.CD_EMPRESA,
				dbo.TB_EMPRESA.NM_EMPRESA,
				dbo.TB_ATIVO_FIXO.TX_ANO_MÁQUINA,
				dbo.TB_MODELO.DS_MODELO,
				dbo.TB_MODELO.CD_GRUPO_MODELO as GRUPO_MODELO_ATIVO,
				coalesce(
					(select max(qt_periodo) from 
					dbo.fncRetornaTecnicoClienteAgenda2Tec(dbo.TB_TECNICO.CD_TECNICO) 
					where cd_cliente = dbo.TB_CLIENTE.CD_CLIENTE)
				, 0) QT_PERIODO
		FROM	dbo.tbOSPadrao
		INNER JOIN dbo.TB_CLIENTE
			ON dbo.tbOSPadrao.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
		INNER JOIN dbo.TB_TECNICO
			ON dbo.tbOSPadrao.CD_TECNICO = dbo.TB_TECNICO.CD_TECNICO
		INNER JOIN dbo.tbTpStatusOSPadrao
			ON dbo.tbOSPadrao.ST_STATUS_OS = dbo.tbTpStatusOSPadrao.ST_STATUS_OS
		INNER JOIN dbo.tbTpOSPadrao
			ON dbo.tbOSPadrao.CD_TIPO_OS = dbo.tbTpOSPadrao.CD_TIPO_OS
		LEFT JOIN dbo.TB_TECNICO_CLIENTE
			ON dbo.tbOSPadrao.CD_TECNICO = dbo.TB_TECNICO_CLIENTE.CD_TECNICO
			AND dbo.tbOSPadrao.CD_CLIENTE = dbo.TB_TECNICO_CLIENTE.CD_CLIENTE
		LEFT JOIN dbo.v_regiao
			on dbo.tb_cliente.cd_regiao = dbo.v_regiao.cd_regiao
		LEFT JOIN dbo.TB_EMPRESA
			on dbo.TB_TECNICO.CD_EMPRESA = dbo.TB_EMPRESA.CD_EMPRESA
		LEFT JOIN dbo.TB_ATIVO_FIXO
			on dbo.tbOSPadrao.CD_ATIVO_FIXO = dbo.TB_ATIVO_FIXO.CD_ATIVO_FIXO
		LEFT JOIN dbo.TB_MODELO
			on dbo.TB_ATIVO_FIXO.CD_MODELO = dbo.TB_MODELO.CD_MODELO
		WHERE 
		( dbo.tbOSPadrao.CD_TECNICO			= @p_CD_TECNICO	)
		
		ORDER BY
			dbo.tbOSPadrao.DT_DATA_OS DESC 
		
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

--select * from TB_ATIVO_FIXO
--select * from TB_MODELO