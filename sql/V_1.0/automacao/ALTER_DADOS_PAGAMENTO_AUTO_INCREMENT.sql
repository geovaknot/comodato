USE [COMODATODEV]
GO

/****** Object:  Table [dbo].[TB_DadosPagamento]    Script Date: 22/11/2021 15:35:43 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
ALTER TABLE  [dbo].[TB_DadosPagamento] DROP COLUMN ID
ALTER TABLE [dbo].[TB_DadosPagamento] ADD ID NUMERIC(6,0) IDENTITY(1,1)

GO



