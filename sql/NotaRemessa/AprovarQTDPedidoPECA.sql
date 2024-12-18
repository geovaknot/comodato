GO
/****** Object:  StoredProcedure [dbo].[AprovarQTDPedidoPECA]    Script Date: 18/07/2022 17:24:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Andre Farinelli
-- Create date: 06/11/2018
-- Description:	Gerar lote de aprovação de peças
-- de uma solicitação
-- =============================================
ALTER PROCEDURE [dbo].[AprovarQTDPedidoPECA]
	@p_ID_ITEM_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_QTD_APROVADA							NUMERIC(9,0)	= NULL,
	@p_QTD_3M1								NUMERIC(9,0)	= NULL,
	@p_QTD_3M2								NUMERIC(9,0)	= NULL
AS
BEGIN

	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--SET FMTONLY OFF;
	--SET XACT_ABORT ON;

	BEGIN

		--BEGIN TRANSACTION
		update TB_PEDIDO_PECA
            set ST_STATUS_ITEM = '8',
                QTD_APROVADA = @p_QTD_APROVADA,
				EnviadoBPCS = 'N'
            from TB_PEDIDO_PECA WITH (NOLOCK)
            where ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO
		--Correção 8/4/19 erro na aprov avulsa e status aprov manual:

		IF(@p_QTD_3M1 IS NOT NULL AND @p_QTD_3M1 > 0)
		BEGIN
			update TB_PEDIDO_PECA
            set QTD_APROVADA_3M1 = @p_QTD_3M1
            from TB_PEDIDO_PECA WITH (NOLOCK)
            where ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO
		END
		IF(@p_QTD_3M2 IS NOT NULL AND @p_QTD_3M2 > 0)
		BEGIN
			update TB_PEDIDO_PECA
            set QTD_APROVADA_3M2 = @p_QTD_3M2
            from TB_PEDIDO_PECA WITH (NOLOCK)
            where ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO
		END
		--COMMIT TRANSACTION
	END

END