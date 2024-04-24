ALTER TABLE TB_DadosPagamento ALTER COLUMN NRSolicitacaoSESM Numeric(6)

ALTER TABLE TB_DadosPagamento ALTER COLUMN NRLinhaNF Numeric(3)

CREATE INDEX index_NRSolicitacaoSESM
ON TB_DadosPagamento (NRSolicitacaoSESM);

ALTER TABLE TB_DadosFaturamento ADD SituacaoBpcs CHAR(10);

ALTER TABLE TB_DadosPagamento DROP CONSTRAINT FK_ID_ATIVO_CLIENTE;
ALTER TABLE TB_DadosPagamento DROP COLUMN ID_ATIVO_CLIENTE;



ALTER TABLE TB_DadosPagamento ADD ID_DADOS_FATURAMENTO numeric(6, 0);



ALTER TABLE [dbo].[TB_DadosPagamento] WITH CHECK ADD CONSTRAINT [FK_ID_Dados_Faturamento] FOREIGN KEY([ID_DADOS_FATURAMENTO])
REFERENCES [dbo].[TB_DadosFaturamento] ([ID])
ON DELETE CASCADE
GO



ALTER TABLE [dbo].[TB_DadosPagamento] CHECK CONSTRAINT [FK_ID_Dados_Faturamento]
GO