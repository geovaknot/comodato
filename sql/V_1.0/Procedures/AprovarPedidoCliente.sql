GO
/****** Object:  StoredProcedure [dbo].[AprovarPedidoCliente]    Script Date: 23/08/2021 16:23:09 ******/
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
CREATE PROCEDURE [dbo].[AprovarPedidoCliente]
	@p_ID_ITEM_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_QTD_RECEBIDA							NUMERIC(9,0)	= NULL
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
            set ST_STATUS_ITEM = '3',
                QTD_RECEBIDA = @p_QTD_RECEBIDA
                FROM TB_PEDIDO_PECA WITH (NOLOCK)
            where ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO
		--Correção 8/4/19 erro na aprov avulsa e status aprov manual:
		
		--COMMIT TRANSACTION
	END

END