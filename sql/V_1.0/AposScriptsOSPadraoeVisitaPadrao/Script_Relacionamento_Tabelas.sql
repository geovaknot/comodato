-------------------------- LIMPAR TABELAS ANTES DE RODAR OS SCRIPTS ABAIXO --------------------------

--delete from tbPecaOS
select * from tbPecaOS

--delete from tbPECAOSBAK
select * from tbPECAOSBAK

--delete from tbPendenciaOS
select * from tbPendenciaOS

--delete from tbRRComent
select * from tbRRComent

--delete from tbRRRelatorioReclamacao
select * from tbRRRelatorioReclamacao

--delete from tbOSPadrao
select * from tbOSPadrao

-------------------------- 1 TABELA: tbRRComent --------------------------

GO

ALTER TABLE [dbo].[tbRRComent] DROP CONSTRAINT [FK__tbRRComen__ID_RE__401E899F]
GO

ALTER TABLE [dbo].[tbRRComent]  WITH CHECK ADD  CONSTRAINT [FK__tbRRComen__ID_RE__401E899F] FOREIGN KEY([ID_RELATORIO_RECLAMACAO])
REFERENCES [dbo].[tbRRRelatorioReclamacao] ([ID_RELATORIO_RECLAMACAO])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tbRRComent] CHECK CONSTRAINT [FK__tbRRComen__ID_RE__401E899F]
GO


-------------------------- 2 TABELA: tbPecaOS --------------------------

GO

ALTER TABLE [dbo].[tbPecaOS] DROP CONSTRAINT [FK_tbPecaOS_tbOS]
GO

ALTER TABLE [dbo].[tbPecaOS]  WITH NOCHECK ADD  CONSTRAINT [FK_tbPecaOS_tbOSPadrao] FOREIGN KEY([ID_OS])
REFERENCES [dbo].[tbOSPadrao] ([ID_OS])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tbPecaOS] CHECK CONSTRAINT [FK_tbPecaOS_tbOSPadrao]
GO


-------------------------- 3 TABELA: tbPendenciaOS --------------------------

GO

ALTER TABLE [dbo].[tbPendenciaOS] DROP CONSTRAINT [FK_tbPendenciaOS_tbOS]
GO

ALTER TABLE [dbo].[tbPendenciaOS]  WITH NOCHECK ADD  CONSTRAINT [FK_tbPendenciaOS_tbOSPadrao] FOREIGN KEY([ID_OS])
REFERENCES [dbo].[tbOSPadrao] ([ID_OS])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tbPendenciaOS] CHECK CONSTRAINT [FK_tbPendenciaOS_tbOSPadrao]
GO


-------------------------- 4 TABELA: tbRRRelatorioReclamacao --------------------------

GO

ALTER TABLE [dbo].[tbRRRelatorioReclamacao] DROP CONSTRAINT [FK_tbRRRelatorioReclamacao_tbOS]
GO

ALTER TABLE [dbo].[tbRRRelatorioReclamacao]  WITH CHECK ADD  CONSTRAINT [FK_tbRRRelatorioReclamacao_tbOSPadrao] FOREIGN KEY([ID_OS])
REFERENCES [dbo].[tbOSPadrao] ([ID_OS])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tbRRRelatorioReclamacao] CHECK CONSTRAINT [FK_tbRRRelatorioReclamacao_tbOSPadrao]
GO