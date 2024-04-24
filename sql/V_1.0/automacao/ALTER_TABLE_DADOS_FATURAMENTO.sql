GO

/****** Object:  Table [dbo].[TB_DadosFaturamento]    Script Date: 18/11/2021 15:52:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE [dbo].[TB_DadosFaturamento]
ADD ID_ATIVO_CLIENTE [numeric](9, 0) NOT NULL

GO

ALTER TABLE [dbo].[TB_DadosFaturamento]
ADD CONSTRAINT [FK_ATIVO_CLIENTE] Foreign Key (ID_ATIVO_CLIENTE)
REFERENCES [dbo].[TB_ATIVO_CLIENTE](ID_ATIVO_CLIENTE)

GO


