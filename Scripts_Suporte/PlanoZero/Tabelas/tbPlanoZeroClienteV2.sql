GO

/****** Object:  Table [dbo].[tbPlanoZeroClienteV2]    Script Date: 08/03/2024 09:21:36 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbPlanoZeroClienteV2](
	[codCliente] [varchar](6) NOT NULL,
	[codPeca] [varchar](15) NOT NULL,
	[potencialPecas] [numeric](15, 3) NULL,
	[qtdPZACalculada] [numeric](15, 3) NULL,
	[qtdEstoque] [numeric](15, 3) NULL,
	[qtdPedidoPZ] [numeric](15, 3) NULL,
	[qtdUltimoAno] [numeric](15, 3) NULL
) ON [PRIMARY]
GO


