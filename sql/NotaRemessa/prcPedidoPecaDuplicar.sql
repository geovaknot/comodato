GO
/****** Object:  StoredProcedure [dbo].[prcPedidoPecaDuplicar]    Script Date: 31/08/2022 17:23:48 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Alex Natalino
-- Create date: 03/07/2018
-- Description:	Update das QTD_APROVADA na tabela
--               TB_PEDIDO_PECA que ficaram vazias
-- =============================================
ALTER PROCEDURE [dbo].[prcPedidoPecaDuplicar]
	@p_ID_ITEM_PEDIDO						NUMERIC(9,0)	= NULL
AS
BEGIN

	-- Declaração de Variáveis
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN 

		update TB_PEDIDO_PECA
			set Duplicado = 'S'
		where ID_ITEM_PEDIDO = @p_ID_ITEM_PEDIDO
	
	END 

END


