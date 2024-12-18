GO
/****** Object:  StoredProcedure [dbo].[prcAtualizarRecebimentoPedidoDePecaPendente]    Script Date: 08/03/2022 10:49:06 ******/
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
CREATE PROCEDURE [dbo].[prcAtualizarRecebimentoPedidoDePecaPendente]
	@p_ID_ITEM_PEDIDO								NUMERIC(9,0)	= NULL
AS
BEGIN

	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--SET FMTONLY OFF;
	--SET XACT_ABORT ON;

	BEGIN

		--BEGIN TRANSACTION
		Update TB_PEDIDO_PECA
			set atualizado = 'P',
				QTD_ULTIMO_RECEBIMENTO = 0
		where ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO
		AND @p_ID_ITEM_PEDIDO is not null
            
		--Correção 8/4/19 erro na aprov avulsa e status aprov manual:
		
		--COMMIT TRANSACTION
	END

END