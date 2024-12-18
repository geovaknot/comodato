GO

/****** Object:  Table [dbo].[tbPotencialPecas]    Script Date: 08/03/2024 09:17:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tbPotencialPecas](
	[codPeca] [varchar](15) NOT NULL,
	[potencialPeca] [numeric](15, 3) NOT NULL,
 CONSTRAINT [PK_cdPeca] PRIMARY KEY CLUSTERED 
(
	[codPeca] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


