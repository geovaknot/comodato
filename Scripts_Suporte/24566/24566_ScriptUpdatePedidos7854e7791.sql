select * from tb_pedido where cd_pedido in (7854, 7791) 

select * from TB_PEDIDO_PECA where id_pedido in (19126)

select * from TB_PEDIDO_PECA where id_pedido in (19126) AND ST_STATUS_ITEM not in ('4')

update TB_PEDIDO_PECA
	set ST_STATUS_ITEM = '5' 
where id_pedido in (19126) AND ST_STATUS_ITEM not in ('4')

select * from TB_PEDIDO_PECA where id_pedido in (19189) AND ST_STATUS_ITEM not in ('4')

update TB_PEDIDO_PECA
	set ST_STATUS_ITEM = '5' 
where id_pedido in (19189) AND ST_STATUS_ITEM not in ('4')


update TB_PEDIDO
set ID_STATUS_PEDIDO = 4
where ID_PEDIDO in (19189, 19126)