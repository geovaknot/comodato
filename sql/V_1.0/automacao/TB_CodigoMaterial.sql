GO

/****** Object:  Table [dbo].[TB_CodigoMaterial]    Script Date: 30/11/2021 17:28:55 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_CodigoMaterial](
	[ID] [numeric](6, 0) IDENTITY(1,1) PRIMARY KEY NOT NULL,
	[CD_Material] [varchar](15) NULL,
	[CD_Descricao] [varchar](255) NULL)