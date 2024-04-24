alter table tb_pedido_peca add Estoque_Cli_Aprov numeric(15,3) 
alter table tb_pedido_peca add Estoque_Tec_Aprov numeric(15,3)
alter table tb_pedido add DT_Aprovacao DateTime

update TB_PEDIDO
set DT_Aprovacao = DT_CRIACAO
where ID_STATUS_PEDIDO not in (1,2,7)