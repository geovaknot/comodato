GO
/****** Object:  StoredProcedure [dbo].[prcPedidoSelectSolicitacaoPeca]    Script Date: 12/01/2022 09:53:35 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Alex Natalino
-- Create date: 02/03/2011
-- Description:	Seleção de dados para Solicitação
--              de Peças (Editar)
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoSelectSolicitacaoPeca]
	@p_ID_PEDIDO				NUMERIC(9,0),
	@p_CD_TECNICO				VARCHAR(6),
	@p_CD_PECA					VARCHAR(15) = NULL
AS
BEGIN

	-- Declaração de Variáveis
	DECLARE @cdsErrorMessage	NVARCHAR(4000),
			@nidErrorSeverity	INT,
			@nidErrorState		INT,
			@TP_TIPO_PEDIDO		CHAR(1)

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET FMTONLY OFF;
    
	BEGIN TRY
		
		SELECT @TP_TIPO_PEDIDO = TP_TIPO_PEDIDO 
		FROM dbo.TB_PEDIDO 
		WHERE dbo.TB_PEDIDO.ID_PEDIDO = @p_ID_PEDIDO

		IF(@TP_TIPO_PEDIDO = 'T')
		--BEGIN
		--	CREATE TABLE #tbModelo
		--	(
		--		CD_MODELO	VARCHAR(15) NULL,
		--		QUANTIDADE	INT			NULL
		--	)

		--	CREATE TABLE #tbPlanoZero
		--	(
		--		CD_PECA				VARCHAR(15)		NULL,
		--		QT_ESTOQUE_MINIMO	NUMERIC(15,3)	NULL,
		--		QUANTIDADE			INT				NULL,
		--		QT_SUGERIDA_PZ		NUMERIC(15,3)	NULL,
		--		CD_CRITICIDADE_ABC	VARCHAR(1)		NULL
		--	)

		--	INSERT INTO #tbModelo 
		--	( 
		--		CD_MODELO, 
		--		QUANTIDADE 
		--	)
		--	SELECT 
		--		dbo.TB_ATIVO_FIXO.CD_MODELO,
		--		COUNT(dbo.TB_ATIVO_FIXO.CD_MODELO) AS QUANTIDADE
		--		FROM dbo.TB_TECNICO_CLIENTE
		--		INNER JOIN dbo.TB_ATIVO_CLIENTE 
		--			ON dbo.TB_TECNICO_CLIENTE.CD_CLIENTE = dbo.TB_ATIVO_CLIENTE.CD_CLIENTE
		--		INNER JOIN dbo.TB_ATIVO_FIXO 
		--			ON dbo.TB_ATIVO_CLIENTE.CD_ATIVO_FIXO = dbo.TB_ATIVO_FIXO.CD_ATIVO_FIXO
		--		WHERE 
		--			dbo.TB_TECNICO_CLIENTE.CD_TECNICO = @p_CD_TECNICO
		--		AND dbo.TB_TECNICO_CLIENTE.CD_ORDEM = 1 
		--		AND dbo.TB_ATIVO_FIXO.FL_STATUS = 1
		--	GROUP BY
		--		dbo.TB_ATIVO_FIXO.CD_MODELO
		--	ORDER BY 
		--		dbo.TB_ATIVO_FIXO.CD_MODELO

		--	INSERT INTO #tbPlanoZero 
		--	( 
		--		CD_PECA,
		--	    QT_ESTOQUE_MINIMO,
		--		QUANTIDADE,
		--	    QT_SUGERIDA_PZ,
		--	    CD_CRITICIDADE_ABC 
		--	)		
		--	SELECT	dbo.tbPlanoZero.CD_PECA,
		--			SUM(dbo.tbPlanoZero.QT_ESTOQUE_MINIMO) AS QT_ESTOQUE_MINIMO,
		--			SUM(#tbModelo.QUANTIDADE) AS QUANTIDADE,
		--			CASE 
		--			WHEN SUM(dbo.tbPlanoZero.QT_ESTOQUE_MINIMO) < 0 THEN
		--				SUM((1 * #tbModelo.QUANTIDADE))      
		--			ELSE      
		--				SUM((dbo.tbPlanoZero.QT_ESTOQUE_MINIMO * #tbModelo.QUANTIDADE))
		--			END  AS QT_SUGERIDA_PZ,
		--			MAX(tbPlanoZero.CD_CRITICIDADE_ABC) AS CD_CRITICIDADE_ABC
		--	FROM dbo.tbPlanoZero
		--	INNER JOIN #tbModelo
		--	ON dbo.tbPlanoZero.CD_MODELO COLLATE Latin1_General_CI_AS = #tbModelo.CD_MODELO
		--	GROUP BY 
		--		dbo.tbPlanoZero.CD_PECA
		--	ORDER BY dbo.tbPlanoZero.CD_PECA

		--	SELECT  
		--		dbo.TB_PEDIDO.ID_PEDIDO,
		--		dbo.TB_PEDIDO.ID_STATUS_PEDIDO,
		--		dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO,
		--		dbo.TB_PECA.CD_PECA,
		--		dbo.TB_PECA.DS_PECA,
		--		dbo.TB_PECA.TX_UNIDADE,
		--		dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO,

		--		(SELECT 
		--			MAX(dbo.tbEstoqueMovi.DT_MOVIMENTACAO) 
		--		FROM dbo.tbEstoqueMovi 
		--		INNER JOIN dbo.tbEstoquePeca 
		--			ON dbo.tbEstoqueMovi.CD_PECA		= dbo.tbEstoquePeca.CD_PECA 
		--			AND dbo.tbEstoqueMovi.ID_ESTOQUE	= dbo.tbEstoquePeca.ID_ESTOQUE 
		--		INNER JOIN dbo.tbEstoque 
		--			ON dbo.tbEstoquePeca.ID_ESTOQUE		= dbo.tbEstoque.ID_ESTOQUE
		--		WHERE	dbo.tbEstoqueMovi.CD_TP_MOVIMENTACAO	= 4 -- Ajuste de Saida de Estoque
		--		AND		dbo.tbEstoque.FL_ATIVO					= 'S'
		--		AND		dbo.tbEstoquePeca.CD_PECA				= dbo.TB_PECA.CD_PECA 
		--		AND		dbo.tbEstoque.CD_TECNICO				= @p_CD_TECNICO
		--		) AS DT_MOVIMENTACAO,

		--		(SELECT TOP 1 
		--			dbo.tbEstoquePeca.QT_PECA_ATUAL 
		--		FROM dbo.tbEstoquePeca 
		--		INNER JOIN dbo.tbEstoque 
		--			ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
		--		WHERE tbEstoque.FL_ATIVO		= 'S'
		--		AND dbo.tbEstoquePeca.CD_PECA	= dbo.TB_PECA.CD_PECA 
		--		AND dbo.tbEstoque.CD_TECNICO	= @p_CD_TECNICO
		--		) AS QT_PECA_ATUAL,													--------------------- minha consulta usando o CD_TECNICO e não o ID_USU_RESPONSAVEL

		--		--(SELECT	TOP 1 
		--		--	dbo.tbEstoquePeca.QT_PECA_ATUAL
		--		--FROM dbo.TB_TECNICO (nolock)
		--		--	INNER JOIN dbo.tbEstoque
		--		--	ON dbo.TB_TECNICO.ID_USUARIO = dbo.tbEstoque.ID_USU_RESPONSAVEL
		--		--	INNER JOIN dbo.tbEstoquePeca (nolock)
		--		--	ON dbo.tbEstoque.ID_ESTOQUE = dbo.tbEstoquePeca.ID_ESTOQUE
		--		--	INNER JOIN dbo.TB_PECA (nolock) AS P
		--		--	ON dbo.tbEstoquePeca.CD_PECA = dbo.TB_PECA.CD_PECA
		--		--WHERE	dbo.TB_TECNICO.CD_TECNICO		= @p_CD_TECNICO -- através do técnico vincula os ID_USUARIOS no primeiro JOIN  
		--		--AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'TEC'			-- forçar a consulta do estoque somente do TÉCNICO
		--		--AND	  ( P.CD_PECA				= dbo.TB_PECA.CD_PECA)
		--		--) AS QT_PECA_ATUAL_RESPONSAVEL,										--------------------- consulta usando o ID_USU_RESPONSAVEL

		--		CASE ISNULL(dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO, 0)
		--		WHEN 0 THEN 
		--			(SELECT TOP 1 
		--				dbo.tbEstoquePeca.QT_PECA_ATUAL 
		--			FROM dbo.tbEstoquePeca 
		--			INNER JOIN dbo.tbEstoque 
		--				ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
		--			WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
		--			AND		dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
		--			AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = '3M1')
		--		ELSE 
		--			(SELECT TOP 1 
		--				dbo.tbEstoquePeca.QT_PECA_ATUAL 
		--			FROM dbo.tbEstoquePeca 
		--			INNER JOIN dbo.tbEstoque
		--				ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
		--			WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
		--			AND 	dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
		--			AND		dbo.tbEstoquePeca.ID_ESTOQUE	= dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO)
		--		END AS QT_PECA_ATUAL_3M,

		--		(SELECT TOP 1 
		--			dbo.tbEstoquePeca.QT_PECA_ATUAL 
		--		FROM dbo.tbEstoquePeca 
		--		INNER JOIN dbo.tbEstoque 
		--			ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
		--		WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
		--		AND		dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
		--		AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = '3M2'
		--		) AS QT_PECA_ATUAL_3M2,

		--		dbo.TB_PEDIDO_PECA.QTD_SOLICITADA,
		--		dbo.TB_PEDIDO_PECA.QTD_RECEBIDA,
		--		dbo.TB_PEDIDO_PECA.QTD_APROVADA,
		--		dbo.TB_PEDIDO_PECA.ST_STATUS_ITEM,

		--		--INÍCIO TESTE ANDRÉ
		--		dbo.TB_PECA.VL_PECA,
		--		CASE WHEN (dbo.TB_PEDIDO_PECA.QTD_APROVADA IS NOT NULL) 
		--		THEN dbo.TB_PEDIDO_PECA.QTD_APROVADA * dbo.TB_PECA.VL_PECA 
		--		ELSE dbo.TB_PEDIDO_PECA.QTD_SOLICITADA * dbo.TB_PECA.VL_PECA END AS VALOR_TOTAL_PECA,
		--		--FIM TESTE ANDRÉ

		--		#tbPlanoZero.QT_ESTOQUE_MINIMO,
		--	    #tbPlanoZero.QT_SUGERIDA_PZ,
		--	    #tbPlanoZero.CD_CRITICIDADE_ABC 
		--	INTO #tbTemp1
		--	FROM 
		--		dbo.TB_PEDIDO
		--		INNER JOIN dbo.TB_PEDIDO_PECA
		--		ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
		--		INNER JOIN dbo.TB_PECA
		--		ON dbo.TB_PEDIDO_PECA.CD_PECA = dbo.TB_PECA.CD_PECA
		--		LEFT JOIN #tbPlanoZero 
		--		ON #tbPlanoZero.CD_PECA COLLATE Latin1_General_CI_AS = dbo.TB_PEDIDO_PECA.CD_PECA
		--	WHERE	dbo.TB_PEDIDO.ID_PEDIDO		= @p_ID_PEDIDO
		--		AND (dbo.TB_PECA.CD_PECA		= @p_CD_PECA	OR @p_CD_PECA	IS NULL)
		--	ORDER BY 
		--		dbo.TB_PECA.DS_PECA

		--	-------------------------------------------------------------INICIO TESTE ANDRE

		--	CREATE TABLE #tbTemp2 (
		--		CD_PECA VARCHAR(12),
		--		DS_PECA VARCHAR(250),
		--		TX_UNIDADE VARCHAR(5),
		--		CD_CRITICIDADE_ABC CHAR(1),
		--		QTD_MIN_SOLICITADA INT
		--	)
		--	INSERT #tbTemp2 
		--	EXEC [dbo].[prcPlanoZeroPedidoTecnico] @p_CD_TECNICO;

		--	SELECT 
		--		ID_PEDIDO,
		--		ID_STATUS_PEDIDO,
		--		ID_ITEM_PEDIDO,
		--		#tbTemp1.CD_PECA,
		--		#tbTemp1.DS_PECA,
		--		#tbTemp1.TX_UNIDADE,
		--		ID_ESTOQUE_DEBITO,
		--		DT_MOVIMENTACAO,
		--		QT_PECA_ATUAL,
		--		QT_PECA_ATUAL_3M,
		--		QT_PECA_ATUAL_3M2,
		--		#tbTemp2.QTD_MIN_SOLICITADA AS QTD_SOLICITADA,
		--		QTD_RECEBIDA,
		--		QTD_APROVADA,
		--		ST_STATUS_ITEM,
		--		VL_PECA,
		--		VALOR_TOTAL_PECA,
		--		#tbTemp1.QT_ESTOQUE_MINIMO,
		--	    tbPlanoZero.QT_PECA_MODELO AS QT_SUGERIDA_PZ,
		--	    tbPlanoZero.CD_CRITICIDADE_ABC
		--	FROM
		--		#tbTemp1 
		--		LEFT JOIN #tbTemp2 ON #tbTemp1.CD_PECA COLLATE Latin1_General_CI_AS = #tbTemp2.CD_PECA COLLATE Latin1_General_CI_AS
		--		LEFT JOIN tbPlanoZero ON #tbTemp1.CD_PECA COLLATE Latin1_General_CI_AS = tbPlanoZero.CD_PECA COLLATE Latin1_General_CI_AS
		--	GROUP BY 
		--		ID_PEDIDO,
		--		ID_STATUS_PEDIDO,
		--		ID_ITEM_PEDIDO,
		--		#tbTemp1.CD_PECA,
		--		#tbTemp1.DS_PECA,
		--		#tbTemp1.TX_UNIDADE,
		--		ID_ESTOQUE_DEBITO,
		--		DT_MOVIMENTACAO,
		--		QT_PECA_ATUAL,
		--		QT_PECA_ATUAL_3M,
		--		QT_PECA_ATUAL_3M2,
		--		#tbTemp2.QTD_MIN_SOLICITADA,
		--		QTD_RECEBIDA,
		--		QTD_APROVADA,
		--		ST_STATUS_ITEM,
		--		VL_PECA,
		--		VALOR_TOTAL_PECA,
		--		#tbTemp1.QT_ESTOQUE_MINIMO,
		--	    tbPlanoZero.QT_PECA_MODELO,
		--	    tbPlanoZero.CD_CRITICIDADE_ABC

		--	-------------------------------------------------------------FINAL TESTE ANDRE


		--	If(OBJECT_ID('tempdb..#tbModelo') Is Not Null)
		--	BEGIN
		--		DROP TABLE #tbModelo
		--	END

		--	If(OBJECT_ID('tempdb..#tbPlanoZero') Is Not Null)
		--	BEGIN
		--		DROP TABLE #tbPlanoZero
		--	END

		--	-------------------------------------------------------------INICIO TESTE ANDRE

		--	If(OBJECT_ID('tempdb..#tbTemp1') Is Not Null)
		--	BEGIN
		--		DROP TABLE #tbTemp1
		--	END

		--	If(OBJECT_ID('tempdb..#tbTemp2') Is Not Null)
		--	BEGIN
		--		DROP TABLE #tbTemp2
		--	END

		--	-------------------------------------------------------------FINAL TESTE ANDRE
		BEGIN
			CREATE TABLE #tbPZ (
				CD_PECA VARCHAR(12),
				DS_PECA VARCHAR(250),
				TX_UNIDADE VARCHAR(5),
				CD_CRITICIDADE_ABC CHAR(1),
				QT_PECA_NO_MOD INT,
				QT_SUGERIDA_PZ INT				
			)
			INSERT #tbPZ 
			EXEC [dbo].[prcPlanoZeroPedidoTecnico] @p_CD_TECNICO;

			
			SELECT  
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.ID_STATUS_PEDIDO,
				dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO,
				dbo.TB_PEDIDO_PECA.DS_OBSERVACAO,
				dbo.TB_PECA.CD_PECA,
				dbo.TB_PECA.DS_PECA,
				dbo.TB_PECA.TX_UNIDADE,
				dbo.TB_PEDIDO_PECA.DESCRICAO_PECA,
				dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO,
				ISNULL(dbo.TB_PEDIDO_PECA.ID_LOTE_APROVACAO,0) AS ID_LOTE_APROVACAO,
				(SELECT 
					MAX(dbo.tbEstoqueMovi.DT_MOVIMENTACAO) 
				FROM dbo.tbEstoqueMovi 
				INNER JOIN dbo.tbEstoquePeca 
					ON dbo.tbEstoqueMovi.CD_PECA		= dbo.tbEstoquePeca.CD_PECA 
					AND dbo.tbEstoqueMovi.ID_ESTOQUE	= dbo.tbEstoquePeca.ID_ESTOQUE 
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoquePeca.ID_ESTOQUE		= dbo.tbEstoque.ID_ESTOQUE
				WHERE	dbo.tbEstoqueMovi.CD_TP_MOVIMENTACAO	= 4 -- Ajuste de Saida de Estoque
				AND		dbo.tbEstoque.FL_ATIVO					= 'S'
				AND		dbo.tbEstoquePeca.CD_PECA				= dbo.TB_PECA.CD_PECA 
				AND		dbo.tbEstoque.CD_TECNICO				= @p_CD_TECNICO
				) AS DT_MOVIMENTACAO,
				(SELECT TOP 1 
					dbo.tbEstoquePeca.QT_PECA_ATUAL 
				FROM dbo.tbEstoquePeca 
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
				WHERE tbEstoque.FL_ATIVO		= 'S'
				AND dbo.tbEstoquePeca.CD_PECA	= dbo.TB_PECA.CD_PECA 
				AND dbo.tbEstoque.CD_TECNICO	= @p_CD_TECNICO
				) AS QT_PECA_ATUAL,
				CASE ISNULL(dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO, 0)
				WHEN 0 THEN 
					(SELECT TOP 1 
						dbo.tbEstoquePeca.QT_PECA_ATUAL 
					FROM dbo.tbEstoquePeca 
					INNER JOIN dbo.tbEstoque 
						ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
					WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
					AND		dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
					AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = '3M1')
				ELSE 
					(SELECT TOP 1 
						dbo.tbEstoquePeca.QT_PECA_ATUAL 
					FROM dbo.tbEstoquePeca 
					INNER JOIN dbo.tbEstoque
						ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
					WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
					AND 	dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
					AND		dbo.tbEstoquePeca.ID_ESTOQUE	= dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO)
				END AS QT_PECA_ATUAL_3M,
				(SELECT TOP 1 
					dbo.tbEstoquePeca.QT_PECA_ATUAL 
				FROM dbo.tbEstoquePeca 
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
				WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
				AND		dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
				AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = '3M2'
				) AS QT_PECA_ATUAL_3M2,
				dbo.TB_PEDIDO_PECA.QTD_SOLICITADA,
				dbo.TB_PEDIDO_PECA.QTD_RECEBIDA,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M1,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M2,
				dbo.TB_PEDIDO_PECA.ST_STATUS_ITEM,
				-- Valor da Peça agora vem da tb_Pedido_Peca para preservar o valor real no momento do pedido
				--dbo.TB_PECA.VL_PECA,
				ISNULL(dbo.TB_PEDIDO_PECA.VL_PECA, 0) AS VL_PECA,
				CASE WHEN (dbo.TB_PEDIDO_PECA.QTD_APROVADA IS NOT NULL) 
					THEN 
						--dbo.TB_PEDIDO_PECA.QTD_APROVADA * dbo.TB_PECA.VL_PECA 
						dbo.TB_PEDIDO_PECA.QTD_APROVADA * ISNULL(dbo.TB_PEDIDO_PECA.VL_PECA, 0)
					ELSE 
						--dbo.TB_PEDIDO_PECA.QTD_SOLICITADA * dbo.TB_PECA.VL_PECA END AS VALOR_TOTAL_PECA,
						dbo.TB_PEDIDO_PECA.QTD_SOLICITADA * ISNULL(dbo.TB_PEDIDO_PECA.VL_PECA, 0) 
					END AS VALOR_TOTAL_PECA,
				#tbPZ.QT_PECA_NO_MOD AS QT_ESTOQUE_MINIMO,
			    #tbPZ.QT_SUGERIDA_PZ,
			    #tbPZ.CD_CRITICIDADE_ABC,
				ROW_NUMBER() OVER(ORDER BY dbo.TB_PECA.DS_PECA, ID_LOTE_APROVACAO) AS NR_LINHA,
				dbo.TB_PEDIDO_PECA.DS_DIR_FOTO
			INTO #tbPed		
			FROM 
				dbo.TB_PEDIDO
				INNER JOIN dbo.TB_PEDIDO_PECA
				ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
				INNER JOIN dbo.TB_PECA
				ON dbo.TB_PEDIDO_PECA.CD_PECA = dbo.TB_PECA.CD_PECA
				LEFT JOIN #tbPZ 
				ON #tbPZ.CD_PECA COLLATE Latin1_General_CI_AS = dbo.TB_PEDIDO_PECA.CD_PECA
			WHERE	dbo.TB_PEDIDO.ID_PEDIDO		= @p_ID_PEDIDO
				AND (dbo.TB_PECA.CD_PECA		= @p_CD_PECA	OR @p_CD_PECA	IS NULL)
			GROUP BY
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.ID_STATUS_PEDIDO,
				dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO,
				dbo.TB_PEDIDO_PECA.DS_OBSERVACAO,
				dbo.TB_PECA.CD_PECA,
				dbo.TB_PECA.DS_PECA,
				dbo.TB_PECA.TX_UNIDADE,
				dbo.TB_PEDIDO_PECA.DESCRICAO_PECA,
				dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO,
				#tbPZ.QT_PECA_NO_MOD,
				dbo.TB_PEDIDO_PECA.QTD_SOLICITADA,
				QTD_APROVADA,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M1,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M2,

				QTD_RECEBIDA,				
				ST_STATUS_ITEM,
				dbo.TB_PEDIDO_PECA.VL_PECA,
				#tbPZ.QT_SUGERIDA_PZ,
			    CD_CRITICIDADE_ABC,
				ID_LOTE_APROVACAO,
				dbo.TB_PEDIDO_PECA.DS_DIR_FOTO
			ORDER BY 
				dbo.TB_PECA.DS_PECA,
				ID_LOTE_APROVACAO

			SELECT * FROM #tbPed


			If(OBJECT_ID('tempdb..#tbPZ') Is Not Null)
			BEGIN
				DROP TABLE #tbPZ
			END 

			If(OBJECT_ID('tempdb..#tbPed') Is Not Null)
			BEGIN
				DROP TABLE #tbPed
			END 


		END      
		ELSE
		BEGIN
			SELECT  
				dbo.TB_PEDIDO.ID_PEDIDO,
				dbo.TB_PEDIDO.ID_STATUS_PEDIDO,
				dbo.TB_PEDIDO_PECA.ID_ITEM_PEDIDO,
				dbo.TB_PEDIDO_PECA.DS_OBSERVACAO,
				dbo.TB_PECA.CD_PECA,
				dbo.TB_PECA.DS_PECA,
				dbo.TB_PEDIDO_PECA.DESCRICAO_PECA,
				dbo.TB_PECA.TX_UNIDADE,
				dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO,

				(SELECT 
					MAX(dbo.tbEstoqueMovi.DT_MOVIMENTACAO) 
				FROM dbo.tbEstoqueMovi 
				INNER JOIN dbo.tbEstoquePeca 
					ON dbo.tbEstoqueMovi.CD_PECA		= dbo.tbEstoquePeca.CD_PECA 
					AND dbo.tbEstoqueMovi.ID_ESTOQUE	= dbo.tbEstoquePeca.ID_ESTOQUE 
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoquePeca.ID_ESTOQUE		= dbo.tbEstoque.ID_ESTOQUE
				WHERE	dbo.tbEstoqueMovi.CD_TP_MOVIMENTACAO	= 4 -- Ajuste de Saida de Estoque
				AND		dbo.tbEstoque.FL_ATIVO					= 'S'
				AND		dbo.tbEstoquePeca.CD_PECA				= dbo.TB_PECA.CD_PECA 
				AND		dbo.tbEstoque.CD_TECNICO				= @p_CD_TECNICO
				) AS DT_MOVIMENTACAO,

				(SELECT TOP 1 
					dbo.tbEstoquePeca.QT_PECA_ATUAL 
				FROM dbo.tbEstoquePeca 
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
				WHERE tbEstoque.FL_ATIVO		= 'S'
				AND dbo.tbEstoquePeca.CD_PECA	= dbo.TB_PECA.CD_PECA 
				AND dbo.tbEstoque.CD_TECNICO	= @p_CD_TECNICO
				) AS QT_PECA_ATUAL,													--------------------- minha consulta usando o CD_TECNICO e não o ID_USU_RESPONSAVEL

				(SELECT TOP 1 
					IsNull(dbo.tbEstoquePeca.QT_PECA_ATUAL,0) 
				FROM dbo.tbEstoquePeca 
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
				WHERE tbEstoque.FL_ATIVO		= 'S'
				AND dbo.tbEstoquePeca.CD_PECA	= dbo.TB_PECA.CD_PECA 
				AND dbo.tbEstoque.CD_CLIENTE	= dbo.TB_PEDIDO.CD_CLIENTE
				AND dbo.tbEstoque.TP_ESTOQUE_TEC_3M	 = 'CLI'
				) AS QT_PECA_ATUAL_CLIENTE,													


				--(SELECT	TOP 1 
				--	dbo.tbEstoquePeca.QT_PECA_ATUAL
				--FROM dbo.TB_TECNICO (nolock)
				--	INNER JOIN dbo.tbEstoque
				--	ON dbo.TB_TECNICO.ID_USUARIO = dbo.tbEstoque.ID_USU_RESPONSAVEL
				--	INNER JOIN dbo.tbEstoquePeca (nolock)
				--	ON dbo.tbEstoque.ID_ESTOQUE = dbo.tbEstoquePeca.ID_ESTOQUE
				--	INNER JOIN dbo.TB_PECA (nolock) AS P
				--	ON dbo.tbEstoquePeca.CD_PECA = dbo.TB_PECA.CD_PECA
				--WHERE	dbo.TB_TECNICO.CD_TECNICO		= @p_CD_TECNICO -- através do técnico vincula os ID_USUARIOS no primeiro JOIN  
				--AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = 'TEC'			-- forçar a consulta do estoque somente do TÉCNICO
				--AND	  ( P.CD_PECA				= dbo.TB_PECA.CD_PECA)
				--) AS QT_PECA_ATUAL_RESPONSAVEL,										--------------------- consulta usando o ID_USU_RESPONSAVEL

				CASE ISNULL(dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO, 0)
				WHEN 0 THEN 
					(SELECT TOP 1 
						dbo.tbEstoquePeca.QT_PECA_ATUAL 
					FROM dbo.tbEstoquePeca 
					INNER JOIN dbo.tbEstoque 
						ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
					WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
					AND		dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
					AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = '3M1')
				ELSE 
					(SELECT TOP 1 
						dbo.tbEstoquePeca.QT_PECA_ATUAL 
					FROM dbo.tbEstoquePeca 
					INNER JOIN dbo.tbEstoque
						ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
					WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
					AND 	dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
					AND		dbo.tbEstoquePeca.ID_ESTOQUE	= dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO)
				END AS QT_PECA_ATUAL_3M,

				(SELECT TOP 1 
					dbo.tbEstoquePeca.QT_PECA_ATUAL 
				FROM dbo.tbEstoquePeca 
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoquePeca.ID_ESTOQUE = dbo.tbEstoque.ID_ESTOQUE
				WHERE	dbo.tbEstoque.FL_ATIVO			= 'S' 
				AND		dbo.tbEstoquePeca.CD_PECA		= dbo.TB_PECA.CD_PECA 
				AND		dbo.tbEstoque.TP_ESTOQUE_TEC_3M = '3M2'
				) AS QT_PECA_ATUAL_3M2,

				dbo.TB_PEDIDO_PECA.QTD_SOLICITADA,
				dbo.TB_PEDIDO_PECA.QTD_RECEBIDA,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M1,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M2,
				dbo.TB_PEDIDO_PECA.ST_STATUS_ITEM,

				--INÍCIO TESTE ANDRÉ
				--dbo.TB_PECA.VL_PECA,
				ISNULL(dbo.TB_PEDIDO_PECA.VL_PECA, 0) AS VL_PECA,
				CASE WHEN (dbo.TB_PEDIDO_PECA.QTD_APROVADA IS NOT NULL) 
					THEN 
						--dbo.TB_PEDIDO_PECA.QTD_APROVADA * dbo.TB_PECA.VL_PECA 
						dbo.TB_PEDIDO_PECA.QTD_APROVADA * ISNULL(dbo.TB_PEDIDO_PECA.VL_PECA, 0) 
						
					ELSE 
						--dbo.TB_PEDIDO_PECA.QTD_SOLICITADA * dbo.TB_PECA.VL_PECA 
						dbo.TB_PEDIDO_PECA.QTD_SOLICITADA * ISNULL(dbo.TB_PEDIDO_PECA.VL_PECA, 0) 
					END AS VALOR_TOTAL_PECA,
				--FIM TESTE ANDRÉ

				NULL AS QT_ESTOQUE_MINIMO,
			    NULL AS QT_SUGERIDA_PZ,
			    NULL AS CD_CRITICIDADE_ABC,
				dbo.TB_PEDIDO_PECA.VL_PECA,
				ISNULL(dbo.TB_PEDIDO_PECA.ID_LOTE_APROVACAO,0) AS ID_LOTE_APROVACAO,
				ROW_NUMBER() OVER(ORDER BY dbo.TB_PECA.DS_PECA) AS NR_LINHA,
				dbo.TB_PEDIDO_PECA.DS_DIR_FOTO
			FROM 
				dbo.TB_PEDIDO
				INNER JOIN dbo.TB_PEDIDO_PECA
				ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
				LEFT JOIN dbo.TB_PECA --SL00036630
				ON dbo.TB_PEDIDO_PECA.CD_PECA = dbo.TB_PECA.CD_PECA
			WHERE	dbo.TB_PEDIDO.ID_PEDIDO		= @p_ID_PEDIDO
				AND (dbo.TB_PECA.CD_PECA		= @p_CD_PECA	OR @p_CD_PECA	IS NULL)
			ORDER BY 
				dbo.TB_PECA.DS_PECA
		
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

--drop table #tbPZ



