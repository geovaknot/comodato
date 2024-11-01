GO
/****** Object:  StoredProcedure [dbo].[prcPedidoSelectSolicitacaoPecaBPCS]    Script Date: 14/07/2022 10:02:13 ******/
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
CREATE PROCEDURE [dbo].[prcPedidoSelectSolicitacaoPecaBPCS]
	@p_ID_PEDIDO				NUMERIC(9,0) = null,
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
				dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2,
				
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
				dbo.TB_PEDIDO_PECA.DS_DIR_FOTO,
				dbo.TB_PEDIDO_PECA.Estoque_Tec_Aprov,
				dbo.TB_PEDIDO_PECA.Estoque_Cli_Aprov,
				dbo.tbEstoque.CD_ESTOQUE
			INTO #tbPed		
			FROM 
				dbo.TB_PEDIDO
				INNER JOIN dbo.TB_PEDIDO_PECA
				ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
				INNER JOIN dbo.TB_PECA
				ON dbo.TB_PEDIDO_PECA.CD_PECA = dbo.TB_PECA.CD_PECA
				LEFT JOIN #tbPZ 
				ON #tbPZ.CD_PECA COLLATE Latin1_General_CI_AS = dbo.TB_PEDIDO_PECA.CD_PECA
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoque.ID_ESTOQUE = dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO 
					OR dbo.tbEstoque.ID_ESTOQUE = dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2
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
				dbo.tb_pedido_peca.ID_ESTOQUE_DEBITO_3M2,
				QTD_APROVADA,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M1,
				dbo.TB_PEDIDO_PECA.QTD_APROVADA_3M2,
				
				QTD_RECEBIDA,				
				ST_STATUS_ITEM,
				dbo.TB_PEDIDO_PECA.VL_PECA,
				#tbPZ.QT_SUGERIDA_PZ,
			    CD_CRITICIDADE_ABC,
				ID_LOTE_APROVACAO,
				dbo.TB_PEDIDO_PECA.DS_DIR_FOTO,
				dbo.TB_PEDIDO_PECA.Estoque_Tec_Aprov,
				dbo.TB_PEDIDO_PECA.Estoque_Cli_Aprov,
				dbo.tbEstoque.CD_ESTOQUE
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
				dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2,
				
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
				dbo.TB_PEDIDO_PECA.DS_DIR_FOTO,
				dbo.TB_PEDIDO_PECA.Estoque_Tec_Aprov,
				dbo.TB_PEDIDO_PECA.Estoque_Cli_Aprov,
				dbo.tbEstoque.CD_ESTOQUE
			FROM 
				dbo.TB_PEDIDO
				INNER JOIN dbo.TB_PEDIDO_PECA
				ON dbo.TB_PEDIDO.ID_PEDIDO = dbo.TB_PEDIDO_PECA.ID_PEDIDO
				LEFT JOIN dbo.TB_PECA --SL00036630
				ON dbo.TB_PEDIDO_PECA.CD_PECA = dbo.TB_PECA.CD_PECA
				INNER JOIN dbo.tbEstoque 
					ON dbo.tbEstoque.ID_ESTOQUE = dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO 
					OR dbo.tbEstoque.ID_ESTOQUE = dbo.TB_PEDIDO_PECA.ID_ESTOQUE_DEBITO_3M2
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



