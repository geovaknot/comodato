ALTER TABLE dbo.TB_DadosFaturamento ADD EnviadoBpcs bit NULL;

update dbo.TB_DadosFaturamento
set EnviadoBpcs = 0

ALTER TABLE dbo.TB_DadosFaturamento ADD Ativo bit NULL;

update dbo.TB_DadosFaturamento
set Ativo = 1