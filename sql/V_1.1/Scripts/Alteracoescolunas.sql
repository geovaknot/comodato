select * from tb_dadosfaturamento

select * from TB_DadosPagamento

delete from TB_DadosFaturamento where id = 10

DBCC CHECKIDENT('TB_DadosFaturamento', RESEED, 0)

ALTER TABLE TB_DadosPagamento Add SituacaoBcps char(10)

exec sp_rename 'TB_DadosPagamento.[SituacaoBcps]', 'SituacaoBpcs', 'column'


select * from TB_ATIVO_CLIENTE where id_ativo_cliente = 20832