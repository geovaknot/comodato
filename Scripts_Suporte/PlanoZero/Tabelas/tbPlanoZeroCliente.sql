GO

/****** Object:  Table [dbo].[tbPlanoZeroCliente]    Script Date: 08/03/2024 09:20:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbPlanoZeroCliente](
	[codCliente] [varchar](6) NOT NULL,
	[codPeca] [varchar](15) NOT NULL,
	[potencialPecas] [numeric](15, 3) NULL,
	[qtdPZACalculada] [numeric](15, 3) NULL,
	[qtdEstoque] [numeric](15, 3) NULL,
	[qtdPedidoPZ] [numeric](15, 3) NULL,
	[qtdUltimoAno] [numeric](15, 3) NULL,
 CONSTRAINT [PK_codCliente_codPeca] PRIMARY KEY CLUSTERED 
(
	[codCliente] ASC,
	[codPeca] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


