GO

/****** Object:  Table [dbo].[tbLogPlanoZero]    Script Date: 08/03/2024 09:23:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbLogPlanoZero](
	[idPlanoZero] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[tipoPlanoZero] [char](1) NOT NULL,
	[codTecnicoCliente] [varchar](6) NOT NULL,
	[codPeca] [varchar](15) NOT NULL,
	[dtHoraCriacao] [datetime] NOT NULL,
	[idUsuarioCriacao] [bigint] NOT NULL,
	[potenciaLPecas] [numeric](15, 3) NULL,
	[qtdUltimoAno] [numeric](15, 3) NULL,
	[qtdPZACalculada] [numeric](15, 3) NULL,
	[qtdPedidoPZ] [numeric](15, 3) NULL,
	[potencialTotal] [numeric](15, 3) NULL,
	[qtdClientes] [numeric](6, 0) NULL,
	[fatorPonderacao] [numeric](3, 0) NULL,
	[nPedigoGerado] [bigint] NULL,
	[idPedido] [numeric](9, 0) NULL,
	[qtdEstoque] [numeric](15, 3) NULL,
 CONSTRAINT [PK_idPlanoZero_tipoPlanoZero_codTecnicoCliente_codPeca] PRIMARY KEY CLUSTERED 
(
	[idPlanoZero] ASC,
	[tipoPlanoZero] ASC,
	[codTecnicoCliente] ASC,
	[codPeca] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbLogPlanoZero]  WITH CHECK ADD  CONSTRAINT [FK_tbLogPlanoZero_idPedido] FOREIGN KEY([idPedido])
REFERENCES [dbo].[TB_PEDIDO] ([ID_PEDIDO])
GO

ALTER TABLE [dbo].[tbLogPlanoZero] CHECK CONSTRAINT [FK_tbLogPlanoZero_idPedido]
GO

ALTER TABLE [dbo].[tbLogPlanoZero]  WITH CHECK ADD  CONSTRAINT [FK_tbLogPlanoZero_UsuarioCriacao] FOREIGN KEY([idUsuarioCriacao])
REFERENCES [dbo].[tbUsuario] ([nidUsuario])
GO

ALTER TABLE [dbo].[tbLogPlanoZero] CHECK CONSTRAINT [FK_tbLogPlanoZero_UsuarioCriacao]
GO


