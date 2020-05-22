USE [POSWR1]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


DROP TABLE [dbo].[StockkeepingUnit];
go



CREATE TABLE [dbo].[StockkeepingUnit](
	[Location Code] [nvarchar](50) NOT NULL,
	[Item No_] [nvarchar](20) NOT NULL,
	[Variant Code] [nvarchar](50) NOT NULL,
	[Shelf No_] [nvarchar](10) NULL,
	[Shelf No_ AS] [nvarchar](100) NULL,
 CONSTRAINT [Stockkeeping Unit] PRIMARY KEY CLUSTERED 
(
	[Location Code] ASC,
	[Item No_] ASC,
	[Variant Code] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO