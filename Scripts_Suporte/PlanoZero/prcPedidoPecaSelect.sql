USE [COMODATODEV]
GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaSelect]    Script Date: 05/02/2024 15:11:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Caio Carneiro
-- Create date: 19/03/2018
-- Description:	Seleção de dados na tabela 
--              TB_PEDIDO_PECA
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoPecaSelect]
	@p_ID_ITEM_PEDIDO					NUMERIC(9,0)	= NULL,
	@p_ID_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_CD_PECA							VARCHAR(15)		= NULL,
	@p_QTD_SOLICITADA					NUMERIC(15,3)	= NULL,
	@p_QTD_APROVADA						NUMERIC(15,3)	= NULL,
	@p_QTD_RECEBIDA						NUMERIC(15,3)	= NULL,
	@p_TX_APROVADO						VARCHAR(1)		= NULL,
	@p_NR_DOC_ORI						NUMERIC(18,0)	= NULL,
	@p_ST_STATUS_ITEM					CHAR(1)			= NULL,
	@p_DS_OBSERVACAO					VARCHAR(MAX)	= NULL,
	@p_DS_DIR_FOTO						VARCHAR(255)	= NULL,
	@p_ID_ESTOQUE_DEBITO				BIGINT			= NULL,
	@p_ID_ESTOQUE_DEBITO_3M2			BIGINT			= NULL,
	@p_VL_PECA							BIGINT			= NULL,
	@p_ID_LOTE_APROVACAO				BIGINT			= NULL,
	@p_nidUsuarioAtualizacao			NUMERIC(18,0)	= NULL

AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@TP_TIPO_PEDIDO		CHAR(1),
			@CD_TECNICO			VARCHAR(06)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		
		SELECT	@TP_TIPO_PEDIDO = TP_TIPO_PEDIDO,
				@CD_TECNICO		= CD_TECNICO 
		FROM dbo.TB_PEDIDO 
		INNER JOIN dbo.TB_PEDIDO_PECA
		ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
		WHERE dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO

		IF(@TP_TIPO_PEDIDO = 'T')
		BEGIN
			--CREATE TABLE #tbModelo
			--(
			--	CD_MODELO	VARCHAR(15) NULL,
			--	QUANTIDADE	INT			NULL
			--)
      
			--CREATE TABLE #tbPlanoZero
			--(
			--	CD_PECA				VARCHAR(15)		NULL,
			--	QT_ESTOQUE_MINIMO	NUMERIC(15,3)	NULL,
			--	QUANTIDADE			INT				NULL,
			--	QT_SUGERIDA_PZ		NUMERIC(15,3)	NULL,
			--	CD_CRITICIDADE_ABC	VARCHAR(1)		NULL
			--)

			--INSERT INTO #tbModelo 
			--( 
			--	CD_MODELO, 
			--	QUANTIDADE 
			--)
			--SELECT 
			--	dbo.TB_ATIVO_FIXO.CD_MODELO,
			--	COUNT(dbo.TB_ATIVO_FIXO.CD_MODELO) AS QUANTIDADE
			--	FROM dbo.TB_TECNICO_CLIENTE
			--	INNER JOIN dbo.TB_ATIVO_CLIENTE 
			--		ON dbo.TB_TECNICO_CLIENTE.CD_CLIENTE = dbo.TB_ATIVO_CLIENTE.CD_CLIENTE
			--	INNER JOIN dbo.TB_ATIVO_FIXO 
			--		ON dbo.TB_ATIVO_CLIENTE.CD_ATIVO_FIXO = dbo.TB_ATIVO_FIXO.CD_ATIVO_FIXO
			--	WHERE 
			--		dbo.TB_TECNICO_CLIENTE.CD_TECNICO = @CD_TECNICO
			--	AND dbo.TB_TECNICO_CLIENTE.CD_ORDEM = 1 
			--	AND dbo.TB_ATIVO_FIXO.FL_STATUS = 1
			--GROUP BY
			--	dbo.TB_ATIVO_FIXO.CD_MODELO
			--ORDER BY 
			--	dbo.TB_ATIVO_FIXO.CD_MODELO

			--INSERT INTO #tbPlanoZero 
			--( 
			--	CD_PECA,
			--    QT_ESTOQUE_MINIMO,
			--	QUANTIDADE,
			--    QT_SUGERIDA_PZ,
			--    CD_CRITICIDADE_ABC 
			--)		
			--SELECT	dbo.tbPlanoZero.CD_PECA,
			--		SUM(dbo.tbPlanoZero.QT_ESTOQUE_MINIMO) AS QT_ESTOQUE_MINIMO,
			--		SUM(#tbModelo.QUANTIDADE) AS QUANTIDADE,
			--		CASE 
			--		WHEN SUM(dbo.tbPlanoZero.QT_ESTOQUE_MINIMO) < 0 THEN
			--			SUM((1 * #tbModelo.QUANTIDADE))      
			--		ELSE      
			--			SUM((dbo.tbPlanoZero.QT_ESTOQUE_MINIMO * #tbModelo.QUANTIDADE))
			--		END  AS QT_SUGERIDA_PZ,
			--		MAX(tbPlanoZero.CD_CRITICIDADE_ABC) AS CD_CRITICIDADE_ABC
			--FROM dbo.tbPlanoZero
			--INNER JOIN #tbModelo
			--ON dbo.tbPlanoZero.CD_MODELO COLLATE Latin1_General_CI_AS = #tbModelo.CD_MODELO
			--GROUP BY 
			--	dbo.tbPlanoZero.CD_PECA
			--ORDER BY dbo.tbPlanoZero.CD_PECA

			CREATE TABLE #tbPlanoZero (
				CD_PECA VARCHAR(12),
				DS_PECA VARCHAR(250),
				TX_UNIDADE VARCHAR(5),
				CD_CRITICIDADE_ABC CHAR(1),
				QT_PECA_NO_MOD INT,
				QT_SUGERIDA_PZ INT				
			)
			INSERT #tbPlanoZero 
			EXEC [dbo].[prcPlanoZeroPedidoTecnico] @CD_TECNICO;

			SELECT	dbo.TB_PEDIDO_PECA.*,
					dbo.TB_PECA.DS_PECA,
					dbo.TB_PECA.TX_UNIDADE,
					dbo.tbEstoquePeca.QT_PECA_ATUAL,

					--#tbPlanoZero.QT_ESTOQUE_MINIMO,
					#tbPlanoZero.QT_SUGERIDA_PZ,
					#tbPlanoZero.CD_CRITICIDADE_ABC,
					ROW_NUMBER() OVER(ORDER BY dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO ASC) AS NR_LINHA,
					pcCli.QT_PECA_ATUAL as QTD_ESTOQUE_CLIENTE
			FROM	dbo.TB_PEDIDO_PECA
			LEFT JOIN dbo.TB_PECA
			ON dbo.TB_PEDIDO_PECA.CD_PECA = dbo.TB_PECA.CD_PECA
			LEFT OUTER JOIN dbo.tbEstoquePeca
			ON dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO = dbo.tbEstoquePeca.ID_ESTOQUE
			AND dbo.TB_PEDIDO_PECA.CD_PECA = dbo.tbEstoquePeca.CD_PECA
			LEFT JOIN dbo.TB_PEDIDO
			ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
			LEFT JOIN dbo.tbEstoque
			ON dbo.tbEstoque.CD_CLIENTE = dbo.TB_PEDIDO.CD_CLIENTE
			left join tbEstoquePeca pcCli
			ON pcCli.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE 
			AND pcCli.CD_PECA = dbo.TB_PEDIDO_PECA.CD_PECA
			LEFT JOIN #tbPlanoZero 
			ON #tbPlanoZero.CD_PECA COLLATE Latin1_General_CI_AS = dbo.TB_PEDIDO_PECA.CD_PECA
			WHERE (	dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO		= @p_ID_ITEM_PEDIDO		OR @p_ID_ITEM_PEDIDO	IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.ID_PEDIDO			= @p_ID_PEDIDO			OR @p_ID_PEDIDO			IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.CD_PECA				= @p_CD_PECA			OR @p_CD_PECA			IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.QTD_SOLICITADA		= @p_QTD_SOLICITADA		OR @p_QTD_SOLICITADA	IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.QTD_APROVADA			= @p_QTD_APROVADA		OR @p_QTD_APROVADA		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.QTD_RECEBIDA			= @p_QTD_RECEBIDA		OR @p_QTD_RECEBIDA		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.TX_APROVADO			= @p_TX_APROVADO		OR @p_TX_APROVADO		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.NR_DOC_ORI			= @p_NR_DOC_ORI			OR @p_NR_DOC_ORI		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.ST_STATUS_ITEM		= @p_ST_STATUS_ITEM		OR @p_ST_STATUS_ITEM	IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.DS_OBSERVACAO		LIKE @p_DS_OBSERVACAO	OR @p_DS_OBSERVACAO		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.DS_DIR_FOTO			LIKE @p_DS_DIR_FOTO		OR @p_DS_DIR_FOTO		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO	= @p_ID_ESTOQUE_DEBITO	OR @p_ID_ESTOQUE_DEBITO	IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2	= @p_ID_ESTOQUE_DEBITO_3M2 OR @p_ID_ESTOQUE_DEBITO_3M2 IS NULL)
			AND	  (	dbo.TB_PEDIDO_PECA.VL_PECA				= @p_VL_PECA			OR @p_VL_PECA			IS NULL)
			AND	  (	dbo.TB_PEDIDO_PECA.ID_LOTE_APROVACAO	= @p_ID_LOTE_APROVACAO	OR @p_ID_LOTE_APROVACAO IS NULL)
			ORDER BY
					dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO			 

			If(OBJECT_ID('tempdb..#tbModelo') Is Not Null)
			BEGIN
				DROP TABLE #tbModelo
			END

			If(OBJECT_ID('tempdb..#tbPlanoZero') Is Not Null)
			BEGIN
				DROP TABLE #tbPlanoZero
			END

		END 
		ELSE
		BEGIN       
			SELECT	dbo.TB_PEDIDO_PECA.*,
					dbo.TB_PECA.DS_PECA,
					dbo.TB_PECA.TX_UNIDADE,
					dbo.tbEstoquePeca.QT_PECA_ATUAL,
					NULL AS QT_SUGERIDA_PZ,
					ROW_NUMBER() OVER(ORDER BY dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO ASC) AS NR_LINHA,
					pcCli.QT_PECA_ATUAL as QTD_ESTOQUE_CLIENTE
			FROM	dbo.TB_PEDIDO_PECA
			LEFT JOIN dbo.TB_PECA
			ON dbo.TB_PEDIDO_PECA.CD_PECA = dbo.TB_PECA.CD_PECA
			LEFT JOIN dbo.TB_PEDIDO
			ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
			LEFT JOIN dbo.tbEstoque
			ON dbo.tbEstoque.CD_CLIENTE = dbo.TB_PEDIDO.CD_CLIENTE
			left join tbEstoquePeca pcCli
			ON pcCli.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE 
			AND pcCli.CD_PECA = dbo.TB_PEDIDO_PECA.CD_PECA
			LEFT OUTER JOIN dbo.tbEstoquePeca
			ON dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO = dbo.tbEstoquePeca.ID_ESTOQUE
			AND dbo.TB_PEDIDO_PECA.CD_PECA = dbo.tbEstoquePeca.CD_PECA
			WHERE (	dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO		= @p_ID_ITEM_PEDIDO		OR @p_ID_ITEM_PEDIDO	IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.ID_PEDIDO			= @p_ID_PEDIDO			OR @p_ID_PEDIDO			IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.CD_PECA				= @p_CD_PECA			OR @p_CD_PECA			IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.QTD_SOLICITADA		= @p_QTD_SOLICITADA		OR @p_QTD_SOLICITADA	IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.QTD_APROVADA			= @p_QTD_APROVADA		OR @p_QTD_APROVADA		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.QTD_RECEBIDA			= @p_QTD_RECEBIDA		OR @p_QTD_RECEBIDA		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.TX_APROVADO			= @p_TX_APROVADO		OR @p_TX_APROVADO		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.NR_DOC_ORI			= @p_NR_DOC_ORI			OR @p_NR_DOC_ORI		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.ST_STATUS_ITEM		= @p_ST_STATUS_ITEM		OR @p_ST_STATUS_ITEM	IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.DS_OBSERVACAO		LIKE @p_DS_OBSERVACAO	OR @p_DS_OBSERVACAO		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.DS_DIR_FOTO			LIKE @p_DS_DIR_FOTO		OR @p_DS_DIR_FOTO		IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO	= @p_ID_ESTOQUE_DEBITO	OR @p_ID_ESTOQUE_DEBITO	IS NULL )
			AND	  (	dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2	= @p_ID_ESTOQUE_DEBITO_3M2 OR @p_ID_ESTOQUE_DEBITO_3M2 IS NULL)
			AND	  (	dbo.TB_PEDIDO_PECA.VL_PECA				= @p_VL_PECA			OR @p_VL_PECA			IS NULL)
			AND	  (	dbo.TB_PEDIDO_PECA.ID_LOTE_APROVACAO	= @p_ID_LOTE_APROVACAO	OR @p_ID_LOTE_APROVACAO IS NULL)
			ORDER BY
					dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO			 
		END 

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



