update TB_PEDIDO_PECA
	set ST_STATUS_ITEM = 6
where (ID_ITEM_PEDIDO in (select pp.ID_ITEM_PEDIDO from 
	tb_pedido p
		inner join TB_PEDIDO_PECA pp on
		pp.ID_PEDIDO = p.ID_PEDIDO
		and pp.ST_STATUS_ITEM = 1
	where p.ID_STATUS_PEDIDO = 2))



