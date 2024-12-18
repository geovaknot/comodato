GO
/****** Object:  StoredProcedure [dbo].[prcDashBoxAtendimentoSelect]    Script Date: 13/05/2022 15:21:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[prcDashBoxAtendimentoSelect]
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

			@vigenciaInicial				DATETIME,
			@vigenciaFinal					DATETIME,
			@TOTAL_Visitas_Realizadas		INT,
			@TOTAL_OS_Realizadas			INT,
			@TOTAL_Tecnicos					INT,
			@TOTAL_Clientes_Perdidos		INT,
			@VALOR_Peca_Enviado_Item		DECIMAL(15,2),
			@VALOR_MetaPecaRecupEnviado     DECIMAL(15,2),
			@VALOR_Peca_Enviado_3M1			DECIMAL(15,2),
			@VALOR_Peca_Enviado_Recuperado	DECIMAL(15,2),
			@VALOR_Peca_Recuperada_Mes		DECIMAL(15,2),
			@ValorEnvioMensalPecas			DECIMAL(15,2)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY

		SELECT @vigenciaInicial = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaINICIAL'
		SELECT @vigenciaFinal = CONVERT(DATETIME, cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'vigenciaFINAL'
		SELECT @ValorEnvioMensalPecas = CONVERT(DECIMAL(15,2), cvlParametro) FROM dbo.tbParametro WHERE ccdParametro = 'ValorEnvioMensalPecas'

		IF (@vigenciaInicial IS NULL)
			SELECT @vigenciaInicial = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-01-01')

		IF (@vigenciaFinal IS NULL)
			SELECT @vigenciaFinal = CONVERT(DATETIME, CONVERT(VARCHAR, YEAR(DATEADD(HH,-3,GETUTCDATE()))) + '-12-31') 

		SELECT * INTO #TB_CLIENTE
		FROM dbo.fncDashConsultaTBCLIENTE(@p_CLIENTE, @p_CD_GRUPO, @p_nidUsuario, @p_CD_VENDEDOR, @p_CD_MODELO, @p_CD_LINHA_PRODUTO, @p_TECNICO)

		SET @TOTAL_Visitas_Realizadas = (
			SELECT COUNT(*) AS TOTAL 
			FROM dbo.tbVisitaPadrao (NOLOCK)

			--INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE (NOLOCK)
			--	ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE

			WHERE dbo.tbVisitaPadrao.st_Status_visita in (4) 
				AND dbo.tbVisitaPadrao.DT_DATA_VISITA BETWEEN @vigenciaInicial AND @vigenciaFinal
		)
	

		SET @TOTAL_OS_Realizadas = (
			SELECT COUNT(*) AS TOTAL 
			FROM dbo.tbOsPadrao (NOLOCK)
			--INNER JOIN tbOsPadrao (NOLOCK)
			--	ON dbo.tbOS.ID_VISITA = dbo.tbOsPadrao.ID_VISITA

			INNER JOIN #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE (NOLOCK)
				ON dbo.tbOsPadrao.CD_CLIENTE = TB_CLIENTE.CD_CLIENTE

			WHERE dbo.tbOSPadrao.ST_STATUS_OS IN (3)
				AND dbo.tbOsPadrao.DT_DATA_OS BETWEEN @vigenciaInicial AND @vigenciaFinal
		)
		
		SET @TOTAL_Tecnicos = (
			SELECT COUNT(*) AS TOTAL 
			FROM dbo.fncDashConsultaTBTECNICO(@p_TECNICO) AS TB_TECNICO --dbo.TB_TECNICO (NOLOCK)
			WHERE TB_TECNICO.FL_ATIVO = 'S' 
		)

		SET @TOTAL_Clientes_Perdidos = (
			SELECT COUNT(*) AS TOTAL 
			FROM #TB_CLIENTE AS TB_CLIENTE --dbo.TB_CLIENTE (NOLOCK)
			WHERE TB_CLIENTE.DT_DESATIVACAO is not null
		)

		SET @VALOR_Peca_Enviado_Item = (SELECT cvlParametro FROM dbo.tbParametro WHERE ccdParametro = 'ValorEnvioMensalPecas');
		SET @VALOR_Peca_Enviado_Item = @VALOR_Peca_Enviado_Item * 12;

		SET @VALOR_MetaPecaRecupEnviado = (SELECT cvlParametro FROM dbo.tbParametro WHERE ccdParametro = 'ValorMensalEnvioPecasREC');
		SET @VALOR_MetaPecaRecupEnviado = @VALOR_MetaPecaRecupEnviado * 12;

		----------Comentado em 07/08/2019
		--SET @VALOR_Peca_Enviado_Item = (
		--	@ValorEnvioMensalPecas / (SELECT datepart(dayofyear, DATEADD(HH,-3,GETUTCDATE()))))
		--------------------	
			--SELECT 
			--	SUM(TOTAL)
			--FROM 
			--	(
			--	SELECT 
			--		--ISNULL(SUM(dbo.TB_PEDIDO_PECA.QTD_APROVADA),0) AS TOTAL 
			--		dbo.TB_PEDIDO_PECA.QTD_APROVADA,
			--		dbo.TB_PEDIDO_PECA.VL_PECA,
			--		(dbo.TB_PEDIDO_PECA.QTD_APROVADA * dbo.TB_PEDIDO_PECA.VL_PECA) AS TOTAL
			--	FROM dbo.TB_PEDIDO (NOLOCK)
			--		INNER JOIN dbo.TB_PEDIDO_PECA (NOLOCK)
			--		ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
			--		INNER JOIN dbo.TB_CLIENTE (NOLOCK)
			--		ON dbo.TB_PEDIDO.CD_CLIENTE = dbo.TB_CLIENTE.CD_CLIENTE
			--		INNER JOIN dbo.TB_PECA (NOLOCK)
			--		ON dbo.TB_PEDIDO_PECA.CD_PECA = dbo.TB_PECA.CD_PECA
			--	WHERE dbo.TB_PEDIDO.ID_STATUS_PEDIDO IN (3,4) --(3-Aprovado 3M, 4-Recebido pelo Técnico)
			--		AND dbo.TB_PEDIDO.DT_CRIACAO BETWEEN @vigenciaInicial AND @vigenciaFinal
			--		AND dbo.TB_CLIENTE.DT_DESATIVACAO IS NULL
			--		AND (dbo.TB_CLIENTE.CD_CLIENTE = @p_CD_CLIENTE	OR @p_CD_CLIENTE IS NULL) 
			--	) Calculo
		

		SET @VALOR_Peca_Enviado_3M1 = (
			-------Comentado dia 07/08/2019
			--SELECT ISNULL(SUM(dbo.tbEstoqueMovi.VL_VALOR_PECA),0) AS TOTAL 
			--FROM dbo.tbEstoqueMovi (NOLOCK)
			--INNER JOIN tbEstoque (NOLOCK)
			--ON dbo.tbEstoqueMovi.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
			--WHERE dbo.tbEstoqueMovi.CD_TP_MOVIMENTACAO = 6 --Remessa 3M p/ Est. Int.
			--AND tbEstoque.TP_ESTOQUE_TEC_3M = '3M1'

			SELECT ISNULL(SUM(pe.VL_PECA * pp.QTD_APROVADA_3M1),0) --+SUM(pe.VL_PECA * pp.QTD_APROVADA_3M2)
			FROM tb_pedido p
			INNER JOIN  TB_PEDIDO_PECA pp 
				ON p.ID_PEDIDO = pp.ID_PEDIDO
			INNER JOIN TB_PECA pe 
				ON pe.CD_PECA = pp.CD_PECA
			WHERE p.DT_ENVIO BETWEEN @vigenciaInicial AND @vigenciaFinal
				AND pp.ST_STATUS_ITEM IN (3,4)
		)

		SET @VALOR_Peca_Enviado_Recuperado = (
			-------Comentado dia 07/08/2019
			--SELECT ISNULL(SUM(dbo.tbEstoqueMovi.VL_VALOR_PECA),0) AS TOTAL 
			--FROM dbo.tbEstoqueMovi (NOLOCK)
			--INNER JOIN tbEstoque (NOLOCK)
			--ON dbo.tbEstoqueMovi.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
			--WHERE dbo.tbEstoqueMovi.CD_TP_MOVIMENTACAO = 6 --Remessa 3M p/ Est. Int.
			--AND tbEstoque.TP_ESTOQUE_TEC_3M = '3M2'

			SELECT ISNULL(SUM(pe.VL_PECA * pp.QTD_APROVADA_3M2),0) --+SUM(pe.VL_PECA * pp.QTD_APROVADA_3M2)
			FROM tb_pedido p
			INNER JOIN  TB_PEDIDO_PECA pp 
				ON p.ID_PEDIDO = pp.ID_PEDIDO
			INNER JOIN TB_PECA pe 
				ON pe.CD_PECA = pp.CD_PECA
			WHERE p.DT_ENVIO BETWEEN @vigenciaInicial AND @vigenciaFinal
				AND pp.ST_STATUS_ITEM IN (3,4)
		)

		SET @VALOR_Peca_Recuperada_Mes = (
			-------Comentado dia 07/08/2019
			--SELECT ISNULL(SUM(dbo.tbEstoqueMovi.VL_VALOR_PECA),0) AS TOTAL 
			--FROM dbo.tbEstoqueMovi (NOLOCK)
			--INNER JOIN tbEstoque (NOLOCK)
			--ON dbo.tbEstoqueMovi.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
			--WHERE dbo.tbEstoqueMovi.CD_TP_MOVIMENTACAO = 6 --Remessa 3M p/ Est. Int.
			--AND tbEstoque.TP_ESTOQUE_TEC_3M = '3M2'
			--AND MONTH(dbo.tbEstoqueMovi.DT_MOVIMENTACAO) = MONTH(DATEADD(HH,-3,GETUTCDATE()))


			SELECT ISNULL(SUM(pe.VL_PECA * em.QT_PECA),0) --em.CD_PECA,em.QT_PECA,em.VL_VALOR_PECA,pe.VL_PECA, em.DT_MOVIMENTACAO , 
			FROM tbEstoqueMovi em
			INNER JOIN tbEstoque e 
				ON e.ID_ESTOQUE = em.ID_ESTOQUE
			INNER JOIN TB_PECA pe 
				ON pe.CD_PECA = em.CD_PECA
			WHERE e.CD_ESTOQUE = '3M2-RECUP'
				AND em.DT_MOVIMENTACAO BETWEEN @vigenciaInicial AND @vigenciaFinal
				AND em.CD_TP_MOVIMENTACAO IN (3,4)
		)

		SELECT	@TOTAL_Visitas_Realizadas AS TOTAL_Visitas_Realizadas, 
				@TOTAL_OS_Realizadas AS TOTAL_OS_Realizadas,
				@TOTAL_Tecnicos AS TOTAL_Tecnicos,
				@TOTAL_Clientes_Perdidos AS TOTAL_Clientes_Perdidos,
				@VALOR_Peca_Enviado_Item AS VALOR_Peca_Enviado_Item,
				@VALOR_MetaPecaRecupEnviado AS VALOR_MetaPecaRecupEnviado,
				@VALOR_Peca_Enviado_3M1 AS VALOR_Peca_Enviado_3M1,
				@VALOR_Peca_Enviado_Recuperado AS VALOR_Peca_Enviado_Recuperado,
				@VALOR_Peca_Recuperada_Mes AS VALOR_Peca_Recuperada_Mes
		
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


