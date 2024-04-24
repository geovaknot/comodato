GO

/****** Object:  Table [dbo].[tbControlePlanoZero]    Script Date: 08/03/2024 09:11:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbControlePlanoZero](
	[idPlanoZero] [numeric](16, 0) IDENTITY(1,1) NOT NULL,
	[dtHoraCriacao] [datetime] NOT NULL,
	[idUsuarioCriacao] [bigint] NOT NULL,
	[dtHoraCancelamento] [datetime] NULL,
	[idUsuarioCancelamento] [bigint] NULL,
	[statusPlanoZero] [char](1) NOT NULL,
	[mensagem] [varchar](200) NULL,
 CONSTRAINT [PK_idPlanoZero] PRIMARY KEY CLUSTERED 
(
	[idPlanoZero] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[tbControlePlanoZero]  WITH CHECK ADD  CONSTRAINT [FK_tbControlePlanoZero_UsuarioCancelamento] FOREIGN KEY([idUsuarioCancelamento])
REFERENCES [dbo].[tbUsuario] ([nidUsuario])
GO

ALTER TABLE [dbo].[tbControlePlanoZero] CHECK CONSTRAINT [FK_tbControlePlanoZero_UsuarioCancelamento]
GO

ALTER TABLE [dbo].[tbControlePlanoZero]  WITH CHECK ADD  CONSTRAINT [FK_tbControlePlanoZero_UsuarioCriacao] FOREIGN KEY([idUsuarioCriacao])
REFERENCES [dbo].[tbUsuario] ([nidUsuario])
GO

ALTER TABLE [dbo].[tbControlePlanoZero] CHECK CONSTRAINT [FK_tbControlePlanoZero_UsuarioCriacao]
GO



