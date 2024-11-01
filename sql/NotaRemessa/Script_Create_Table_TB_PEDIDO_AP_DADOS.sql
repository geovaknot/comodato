GO

/****** Object:  Table [dbo].[TB_PEDIDO_AP_DADOS]    Script Date: 24/06/2022 14:34:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TB_PEDIDO_AP_DADOS](
	[ID] [numeric](9, 0) IDENTITY(1,1) NOT NULL,
	[ID_ITEM_PEDIDO] [numeric](9, 0) NOT NULL,
	[ID_PEDIDO] [numeric](9, 0) NULL,
	[CD_PECA] [varchar](15) NOT NULL,
	[QTD_SOLICITADA] [numeric](15, 3) NULL,
	[QTD_APROVADA] [numeric](15, 3) NULL,
	[DS_OBSERVACAO] [nvarchar](max) NULL,
	[ID_ESTOQUE_DEBITO] [bigint] NULL,
	[ID_ESTOQUE_DEBITO_3M2] [bigint] NULL,
	[QTD_APROVADA_3M1] [numeric](15, 3) NULL,
	[QTD_APROVADA_3M2] [numeric](15, 3) NULL,
	[VOLUME] [numeric](14, 2) NULL,
	[DS_APROVADOR] [varchar](150) NULL,
	[RAMAL] [bigint] NULL,
	[DS_TELEFONE] [varchar](11) NULL,
	[RESP_CLIENTE] [varchar](150) NULL
 CONSTRAINT [PK_TB_PEDIDO_AP_DADOS] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


ALTER TABLE [dbo].[TB_PEDIDO_AP_DADOS]  WITH NOCHECK ADD  CONSTRAINT [FK_TB_PEDIDO_AP_DADOS_TB_PEDIDO] FOREIGN KEY([ID_PEDIDO])
REFERENCES [dbo].[TB_PEDIDO] ([ID_PEDIDO])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[TB_PEDIDO_AP_DADOS] CHECK CONSTRAINT [FK_TB_PEDIDO_AP_DADOS_TB_PEDIDO]
GO


