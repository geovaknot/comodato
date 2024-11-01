GO
/****** Object:  StoredProcedure [dbo].[prcRelatorioPedidosSumarizado]    Script Date: 03/06/2022 09:04:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Andre Farinelli/Paulo Rabelo
-- Alter date: 30/04/2020
-- Description:	Seleção de dados para relatório
--              de Pedidos Sumarizado
-- =============================================
ALTER PROCEDURE [dbo].[prcRelatorioPedidosSumarizado]
	@p_DT_INICIAL	DATETIME,
	@p_DT_FINAL		DATETIME
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
		
			SELECT 
				TB_PEDIDO.DT_CRIACAO,
				TB_PEDIDO.CD_PEDIDO,
				TB_PECA.CD_PECA,
				TB_PECA.DS_PECA,
				TB_PECA.TX_UNIDADE,
				TB_PEDIDO.CD_TECNICO,
				TB_TECNICO.NM_TECNICO,
				TB_PEDIDO.CD_CLIENTE,
				TB_PEDIDO.ID_STATUS_PEDIDO,
				tbStatusPedido.DS_STATUS_PEDIDO,
				TB_PEDIDO.TP_TIPO_PEDIDO,
				TB_PEDIDO_PECA.ID_ITEM_PEDIDO,
				TB_PEDIDO_PECA.QTD_SOLICITADA,
				TB_PEDIDO_PECA.QTD_RECEBIDA,
				TB_PEDIDO_PECA.QTD_APROVADA,
				TB_PEDIDO_PECA.QTD_APROVADA_3M1,
				TB_PEDIDO_PECA.QTD_APROVADA_3M2,
				TB_PEDIDO_PECA.ST_STATUS_ITEM,
				TB_PEDIDO_PECA.TX_APROVADO,
				IsNull(TB_PECA.VL_PECA,0) as VL_PECA,
				TB_PEDIDO.DT_Aprovacao
			FROM 
				TB_PEDIDO
				INNER JOIN TB_PEDIDO_PECA
				ON TB_PEDIDO.ID_PEDIDO = TB_PEDIDO_PECA.ID_PEDIDO
				INNER JOIN tbStatusPedido
				ON TB_PEDIDO.ID_STATUS_PEDIDO = tbStatusPedido.ID_STATUS_PEDIDO
				INNER JOIN TB_TECNICO
				ON TB_PEDIDO.CD_TECNICO = TB_TECNICO.CD_TECNICO				
				INNER JOIN TB_PECA
				ON TB_PEDIDO_PECA.CD_PECA = TB_PECA.CD_PECA
				INNER JOIN tbLoteAprovacao
				ON TB_PEDIDO_PECA.ID_LOTE_APROVACAO = tbLoteAprovacao.ID_LOTE_APROVACAO
			WHERE	
			--TB_PEDIDO.DT_CRIACAO BETWEEN @p_DT_INICIAL AND @p_DT_FINAL
			TB_PEDIDO.DT_APROVACAO BETWEEN @p_DT_INICIAL AND @p_DT_FINAL
			AND (TB_PEDIDO_PECA.ST_STATUS_ITEM = 3 or TB_PEDIDO_PECA.ST_STATUS_ITEM = 5 or TB_PEDIDO_PECA.ST_STATUS_ITEM = 7) --APROVADO
			ORDER BY 
				TB_PECA.DS_PECA, TB_PEDIDO.DT_CRIACAO
	   
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



