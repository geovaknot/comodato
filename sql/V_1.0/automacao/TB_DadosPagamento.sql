GO

/****** Object:  Table [dbo].[TB_DadosPagamento]    Script Date: 11/11/2021 09:48:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [TB_DadosPagamento](
	[ID] [numeric](6, 0) NOT NULL,
	[CD_Cliente] [varchar](8) NULL,
	[NRAtivo] [varchar](15) NULL,
	[NRSolicitacaoSESM] [varchar](10) NULL,
	[DT_Solicitacao] [Datetime] NULL,
	[NR_NotaFiscal] [numeric](10, 0) NULL,
	[SerieNF] [varchar](3) NULL,
	[NRLinhaNF] [varchar](3) NULL,
	CONSTRAINT PK_TB_DadosPagamento PRIMARY KEY (ID)
) ON [PRIMARY]

GO




