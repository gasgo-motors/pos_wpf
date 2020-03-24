USE [POSWR1]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER TABLE dbo.Customer add	[Customer Posting Group] [nvarchar](10)  NULL
go

ALTER TABLE dbo.Settings add	[Settings_RsUsername] [nvarchar](50) NULL
ALTER TABLE dbo.Settings add	[Settings_RsPassword] [nvarchar](50) NULL
ALTER TABLE dbo.Settings add	[Settings_RsServiceUsername] [nvarchar](50) NULL
ALTER TABLE dbo.Settings add	[Settings_RsServicePassword] [nvarchar](50) NULL
go



ALTER TABLE dbo.SalesLine add	[Service Provider] [nvarchar](10)  NULL
ALTER TABLE dbo.SalesLine add	[Service Provider Name] [nvarchar](50)  NULL
ALTER TABLE dbo.SalesLine add	[Customer Vehicle] [nvarchar](10)  NULL
go
ALTER TABLE dbo.DeletedSalesLine add	[Service Provider] [nvarchar](10)  NULL
ALTER TABLE dbo.DeletedSalesLine add	[Service Provider Name] [nvarchar](50)  NULL
ALTER TABLE dbo.DeletedSalesLine add	[Customer Vehicle] [nvarchar](10)  NULL
go
ALTER TABLE dbo.PostedSalesLine add	[Service Provider] [nvarchar](10)  NULL
ALTER TABLE dbo.PostedSalesLine add	[Service Provider Name] [nvarchar](50)  NULL
ALTER TABLE dbo.PostedSalesLine add	[Customer Vehicle] [nvarchar](10)  NULL
go
ALTER TABLE dbo.ReleasedSalesLine add	[Service Provider] [nvarchar](10)  NULL
ALTER TABLE dbo.ReleasedSalesLine add	[Service Provider Name] [nvarchar](50)  NULL
ALTER TABLE dbo.ReleasedSalesLine add	[Customer Vehicle] [nvarchar](10)  NULL
go




CREATE TABLE [dbo].[Vendor](
	[No_] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Country_Region Code] [nvarchar](10) NOT NULL,
	[VAT Registration No_] [nvarchar](20) NOT NULL,
 CONSTRAINT [pk_Vendor] PRIMARY KEY CLUSTERED 
(
	[No_] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Vehicle](
	[Vehicle No_] [nvarchar](10) NOT NULL,
	[Model] [nvarchar](30) NOT NULL,
	[Owner No_] [nvarchar](20) NOT NULL,
	[Owner Name] [nvarchar](50) NOT NULL,
	[Ownership] [int] NOT NULL,
	[Registration No_] [nvarchar](10) NOT NULL,
	[Trail Registration No_] [nvarchar](10) NOT NULL,
	[Tranzit Location Code] [nvarchar](10) NOT NULL,
	[Type] [int] NOT NULL,
	[Driver Code] [nvarchar](30) NOT NULL,
	[Driver Name] [nvarchar](50) NOT NULL,
	[Driver ID] [nvarchar](30) NOT NULL,
	[WIN Code] [nvarchar](20) NOT NULL,
 CONSTRAINT [pk_Vehicle] PRIMARY KEY CLUSTERED 
(
	[Vehicle No_] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO