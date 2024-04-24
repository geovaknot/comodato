alter table TB_PEDIDO add nidUsuarioAprovador bigint null

update TB_PEDIDO
set nidUsuarioAprovador = nidUsuario