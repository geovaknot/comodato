GO

/****** Object:  Table [dbo].[TB_DadosFaturamento]    Script Date: 11/11/2021 09:48:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TB_DadosFaturamento](
	[ID] [numeric](6, 0) NOT NULL,
	[CD_Cliente] [varchar](8) NULL,
	[NRAtivo] [varchar](15) NULL,
	[CD_Material] [varchar](15) NULL,
	[DepartamentoVenda] [varchar](2) NULL,
	[AluguelApos3anos] [numeric](15,2) NULL,
	[DT_UltimoFaturamento] [Datetime] NULL,
	[nidUsuarioSolicitacao] [numeric](8,0) NULL,
	[DT_Solicitacao] [Datetime] NULL,
	[HR_solicitacao] [varchar](5) NULL,
	CONSTRAINT PK_TB_DadosFaturamento PRIMARY KEY (ID)
) ON [PRIMARY]

GO




