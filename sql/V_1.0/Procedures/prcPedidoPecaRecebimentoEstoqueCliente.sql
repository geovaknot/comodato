GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaRecebimentoEstoqueCliente]    Script Date: 01/09/2021 15:49:51 ******/
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
CREATE PROCEDURE [dbo].[prcPedidoPecaRecebimentoEstoqueCliente]
	@p_ID_ESTOQUE_PECA							NUMERIC(9,0)	= NULL,
	@p_QT_PECA_ATUAL							DECIMAL(9,0)	= NULL
AS
BEGIN

	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--SET FMTONLY OFF;
	--SET XACT_ABORT ON;

	BEGIN

		--BEGIN TRANSACTION
		update tbEstoquePeca
            set QT_PECA_ATUAL = @p_QT_PECA_ATUAL
            from tbEstoquePeca WITH (NOLOCK)
            where ID_ESTOQUE_PECA = @p_ID_ESTOQUE_PECA
		--Correção 8/4/19 erro na aprov avulsa e status aprov manual:
		
		--COMMIT TRANSACTION
	END

END