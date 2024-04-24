GO

/****** Object:  Table [dbo].[TB_PONDERACAO_pz]    Script Date: 25/04/2023 08:43:24 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_PONDERACAO_pz](
	[ID] [numeric](8, 0) IDENTITY NOT NULL,
	[MIN_CLIENTES] [numeric](6, 0) NOT NULL,
	[MAX_CLIENTES] [numeric](6, 0) NOT NULL,
	[FATOR] [numeric](3, 0) NOT NULL,
	[DataInclusao] [datetime] NOT NULL,
	[nidUsuario] [numeric](8,0) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TB_PONDERACAO_pz] ADD  CONSTRAINT [DF_TB_PONDERACAO_pz_DataInclusao]  DEFAULT (getdate()) FOR [DataInclusao]
GO


