GO
/****** Object:  StoredProcedure [dbo].[prcAtualizaStatusItemdePedido]    Script Date: 15/09/2022 13:45:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		André Farinelli
-- Create date: 08/11/2018
-- Description:	Update dos ST_STATUS_ITEM na tabela
--               TB_PEDIDO_PECA por lote
-- =============================================
ALTER PROCEDURE [dbo].[prcAtualizaStatusItemdePedido]
	@p_ID_ITEM_PEDIDO						NUMERIC(9,0)	= NULL,
	@p_ST_STATUS_ITEM						INT				= NULL
AS
BEGIN

	-- Declaração de Variáveis
	

            /*STATUS PEDIDO:										|	STATUS ITEM:
            1	Novo/Rascunho			-							|	1-Novo/Rascunho
            2	Solicitado				Solicitar à 3M				|	2-Pendente
            3	Aprovado				Aprovar Itens				|	3-Aprovado
            4	Recebido				Confirmar Recebimento		|	4-Cancelado
            5	Pendente				Registrar Pendências		|	5-Recebido
            6	Recebido com Pendência	Registrar Peças Pendentes   |   6-Solicitado
            7	Cancelado				Cancelar Itens              |   7-Recebido com Pendência
            */

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN 

		update TB_PEDIDO_PECA
			set ST_STATUS_ITEM = @p_ST_STATUS_ITEM
		where
			ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO
			AND @p_ID_ITEM_PEDIDO is not null 
	
	END 

	

END

