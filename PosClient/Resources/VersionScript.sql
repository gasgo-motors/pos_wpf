USE [POSWR1]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

alter table [dbo].[Settings] add [Settings_Printers] [nvarchar](50) NULL
go


alter table [dbo].[Item] add  [Sorting Number] [nvarchar](30)  NULL;
go


ALTER VIEW [dbo].[ItemsView]
AS
SELECT i.*, 
 ( select sum(l.Quantity) from dbo.ItemLedgerEntry l where l.ItemNo = i.No_) as quantitiy,
(SELECT p.ParameterValueText FROM dbo.AdditionalParameters p WHERE p.No_ = i.No_  FOR XML PATH('')) OeNumbers  ,
(SELECT p.VehicleGroupNo FROM dbo.ItemVehicleGroup p WHERE p.ItemNo = i.No_  FOR XML PATH('')) VehicleGroups  
FROM            dbo.Item i

GO



ALTER procedure [dbo].[ItemsViewProcedure]( @customerCode varchar(20), @customerGroup varchar(20), @locationCode varchar(50)  )
AS
begin
 set nocount on;
SELECT i.*, 
isnull( dbo.getCustomerItemUnitPrice(i.no_, @customerCode, i.BaseUnitOfMeasure, @customerGroup), i.UnitPrice ) as unitnewprice,
 ( select sum(l.Quantity) from dbo.ItemLedgerEntry l where l.ItemNo = i.No_) as quantitiy,
(SELECT p.ParameterValueText FROM dbo.AdditionalParameters p WHERE p.No_ = i.No_  FOR XML PATH('')) OeNumbers  ,
(SELECT p.VehicleGroupNo FROM dbo.ItemVehicleGroup p WHERE p.ItemNo = i.No_  FOR XML PATH('')) VehicleGroups  ,
(SELECT top 1 s.[Shelf No_ AS] from dbo.StockkeepingUnit s WHERE s.[Item No_] = i.No_ AND [Location Code] = @locationCode) as Shelf
FROM  dbo.Item i
order by i.[Sorting Number], i.No_
end;


GO

