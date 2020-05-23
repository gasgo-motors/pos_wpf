using CoreTypes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class NavDbController : Singleton<NavDbController>
    {
        private SqlConnection _sc = null;
        private SqlCommand _SqlCommand = null;

        private Setting setting;

        public void SetSetting(Setting _setting)
        {
            setting = _setting;
            SqlConnectionStringBuilder b = new SqlConnectionStringBuilder();
            b.DataSource = setting.Settings_NavSQLServer;
            b.InitialCatalog = setting.Settings_NavSQLDatabase;
            b.UserID = setting.Settings_NavSQLUser;
            b.Password = setting.Settings_NavSQLPass;
            _sc = new SqlConnection(b.ToString());
        }
        private bool fillDataTable(string commandText, ref string errorMessage, ref DataTable result)
        {

            _SqlCommand = new SqlCommand(commandText, _sc)
            {
                CommandTimeout = 300
            };
            _SqlCommand.CommandType = CommandType.Text;
            DataTable dt = null;
            try
            {
                _sc.Open();
                dt = new DataTable();
                dt.Load(_SqlCommand.ExecuteReader());
                result = dt;
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                _sc.Close();
                return false;
            }
            finally { _sc.Close(); }
            return true;
        }

        private bool ExecuteDBScript(string commandText, CommandType commandType, ref string errorMessage)
        {
            _SqlCommand = new SqlCommand(commandText, _sc);
            _SqlCommand.CommandType = commandType;

            try
            {
                _sc.Open();
                _SqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                _sc.Close();
                return false;
            }
            finally { _sc.Close(); }
            return true;
        }
        // gio

        public bool getStockkeepingUnit(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Location Code]
                ,[Item No_]
                ,[Variant Code]
                ,[Shelf No_]
                ,[Shelf No_ AS]
  FROM [dbo].[{0}$Stockkeeping Unit]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }


        public bool getBankAccounts(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
      ,[Account Type]
      ,[Type]
      ,[Name]
      ,[Responsibilty Center]
  FROM [dbo].[{0}$Bank Account]
  WHERE [Responsibilty Center] = '{1}' AND [Default Cash Account] = 1";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_ResponsibilityCenter);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getCountries(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Code]
        ,[Name]
  FROM [dbo].[{0}$Country_Region]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getPostCodesCities(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Code]
        ,[City]
  FROM [dbo].[{0}$Post Code]
  WHERE [Country_Region Code] = 'GE'";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getPaymentMethods(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Code]
      ,[Description]
  FROM [dbo].[{0}$Payment Method]
  WHERE [POS Payment Method] = 1";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getSalespersons(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Code]
      ,[Name],[Motivation Picture]
  FROM [dbo].[{0}$Salesperson_Purchaser]
  WHERE [Code] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemCategory(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT [Code]
      ,[Description]
  FROM [dbo].[{0}$Item Category]", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getProductGroup(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT [Item Category Code]
      ,[Code]
      ,[Description]
  FROM [dbo].[{0}$Product Group]", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getUnitOfMeasures(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT [Code]
      ,[Description]
  FROM [dbo].[{0}$Unit of Measure]", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getLocations(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT 
       [Code]
      ,[Name]
      ,[Address]
      ,[City]
      ,[Contact]
      ,[Responsibility Center]
  FROM [dbo].[{0}$Location]", setting.Settings_NavCompanyName);
            s = string.Format(s, setting.Settings_ResponsibilityCenter);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getManufacturer(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT [Code]
                    ,[Name]
                    FROM [dbo].[{0}$Manufacturer]", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }
        //TT
        public bool getVehicleModel(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT [Manufacturer Code]
,[Model No_]
,[Cab Type]
,[Engine]
,[Manufacturing Start Date]
,[Output_PH]
,[Manufacturer Name]
,[Model Description]
,[Manufacturing End Date]
,[Drive Type]
,[Series]
,[Output_KW]
,[Tech_ Engine Capacity CC]
,[Cylinder]
,[Valves per Cylinder]
,[Engine Type]
,[Transmission]
,[Fuel Mixture Formation]
,[Brake System]
,[Brake Type]
,[ASR]
,[Catalytic Converter Type]
,[Air Condition]
,[Drive]
,[Voltage]
,[Wheel base]
,[TecDoc ID]
FROM [dbo].[{0}$Vehicle Model]", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }
        //End TT (TecDoc ID)

        public bool getAdditionalParameters(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT
      [Table ID]
      ,[No_]
      ,[Parameter No_]
      ,[Parameter Value Text]
      ,[Parameter Value Dec_]
      ,[Parameter Description]
      ,[Related Parameter No_]
      ,[Related Parameter Value Text]
      ,[Related Parameter Value Dec_]
      ,[Related Parameter Description]
      ,[Sequence No_]
  FROM [dbo].[{0}$Additional Parameter]", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getSuggestions(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT [No_]
        FROM [dbo].[{0}$Suggestion]
        where [Type] = 0", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getUserSetup(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT [User ID]
      ,[Salespers__Purch_ Code]
  FROM [dbo].[{0}$User Setup]", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool geNavMessage(string userId, ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT 
      [Entry No_]
      ,[Sender ID]
      ,[Receiver ID]
      ,[Text]
      ,[Send DateTime]
      ,[Read DateTime]
      ,[IsRead]
      ,[Sender Salespers_ Code]
      ,[Receiver Salespers_ Code]
  FROM [dbo].[{0}$POS Message Entry] 
  where ([Sender ID] = '{1}' or [Receiver ID] = '{1}' ) and [Send DateTime] > DATEADD(day,-31,  GETDATE() )", setting.Settings_NavCompanyName, userId);
            return fillDataTable(s, ref errorMessage, ref result);
        }




        public bool getItemLedgerEntry(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Item No_],SUM([Quantity]) AS Quantity 
FROM [dbo].[{0}$Item Ledger Entry] 
WHERE [Location Code] = '{1}' 
GROUP BY [Item No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_Location);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public decimal getItemLedgerEntryQuantity(string ItemNo, ref string errorMessage)
        {
            DataTable dt = null;

            string s = @"SELECT SUM([Quantity]) AS quantity 
FROM [dbo].[{0}$Item Ledger Entry] 
WHERE [Location Code] = '{1}' AND [Item No_] = '{2}'";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_Location, ItemNo);
            fillDataTable(s, ref errorMessage, ref dt);
            if (dt.Rows.Count > 0 && dt.Rows[0]["quantity"] != DBNull.Value)
                return (decimal)dt.Rows[0]["quantity"];
            return 0;
        }

        public bool getItemLedgerEntryQuantityA(ref string errorMessage, ref DataTable result)
        {

            string s = @"SELECT [Item No_],  SUM([Quantity]) AS quantity , min([Unit of Measure Code]) as uc
FROM [dbo].[{0}$Item Ledger Entry] 
WHERE [Location Code] = '{1}' GROUP BY [Item No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_Location);
            return fillDataTable(s, ref errorMessage, ref result);

        }

        public decimal getItemSalesLine(string ItemNo, ref string errorMessage)
        {
            DataTable dt = null;

            string s = @"SELECT SUM([Qty_ to Ship (Base)]) as quantity 
  FROM [dbo].[{0}$Sales Line]
  WHERE [Type] = 2 AND [No_] = '{1}' AND ([Document Type] = 1 OR [Document Type] = 2) AND [Location Code] = '{2}'";
            s = string.Format(s, setting.Settings_NavCompanyName, ItemNo, setting.Settings_Location);
            fillDataTable(s, ref errorMessage, ref dt);
            if (dt.Rows.Count > 0 && dt.Rows[0]["quantity"] != DBNull.Value)
                return (decimal)dt.Rows[0]["quantity"];
            return 0;
        }

        public bool getItemSalesLineA(ref string errorMessage, ref DataTable result)
        {

            string s = @"SELECT [No_], SUM([Outstanding Qty_ (Base)]) as quantity 
  FROM [dbo].[{0}$Sales Line]
  WHERE [Type] = 2 AND ([Document Type] = 1 OR [Document Type] = 2) AND [Location Code] = '{1}'
  GROUP BY [No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_Location);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public decimal getItemReturnLine(string ItemNo, ref string errorMessage)
        {
            DataTable dt = null;

            string s = @"SELECT SUM([Outstanding Qty_ (Base)]) as quantity 
  FROM [dbo].[{0}$Sales Line]
  WHERE [Type] = 2 AND [No_] = '{1}' AND ([Document Type] = 3 OR [Document Type] = 5) AND [Location Code] = '{2}'";
            s = string.Format(s, setting.Settings_NavCompanyName, ItemNo, setting.Settings_Location);
            fillDataTable(s, ref errorMessage, ref dt);
            if (dt.Rows.Count > 0 && dt.Rows[0]["quantity"] != DBNull.Value)
                return (decimal)dt.Rows[0]["quantity"];
            return 0;
        }

        public bool getItemReturnLineA(ref string errorMessage, ref DataTable result)
        {

            string s = @"SELECT [No_], SUM([Return Qty_ to Receive (Base)]) as quantity 
  FROM [dbo].[{0}$Sales Line]
  WHERE [Type] = 2  AND ([Document Type] = 3 OR [Document Type] = 5) AND [Location Code] = '{1}'
  GROUP BY [No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_Location);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItems(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [No_]
,[No_ 2]
,[Description]
,[Description 2]
,[Large Description]
,[Unit Price]
,[Base Unit of Measure]
,[Allow Invoice Disc_]
,[Vendor No_]
,[Vendor Item No_]
,[Gross Weight]
,[Net Weight]
,[Unit Volume]
,[Price Includes VAT]
,[Item Category Code]
,[Product Group Code]
,[Manufacturer Code]
,[ABC Category Grouping Code]
,[ABC Category Code]
,[Comment AS]
,[Parameters Grouping Code AS]
,[Brand Number AS]
,[Promoted Item]
,[Type]
,[Sorting Number]
FROM [dbo].[{0}$Item]
WHERE[No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemsA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
,[No_ 2]
,[Description]
,[Description 2]
,[Large Description]
,[Unit Price]
,[Base Unit of Measure]
,[Allow Invoice Disc_]
,[Vendor No_]
,[Vendor Item No_]
,[Gross Weight]
,[Net Weight]
,[Unit Volume]
,[Price Includes VAT]
,[Item Category Code]
,[Product Group Code]
,[Manufacturer Code]
,[ABC Category Grouping Code]
,[ABC Category Code]
,[Comment AS]
,[Parameters Grouping Code AS]
,[Brand Number AS]
,[Promoted Item]
,[Type]
,[Sorting Number]
FROM [dbo].[{0}$Item]
where [Manufacturing Policy] = 0";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemsAll(int[] types, ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
,[No_ 2]
,[Description]
,[Description 2]
,[Large Description]
,[Unit Price]
,[Base Unit of Measure]
,[Allow Invoice Disc_]
,[Vendor No_]
,[Vendor Item No_]
,[Gross Weight]
,[Net Weight]
,[Unit Volume]
,[Price Includes VAT]
,[Item Category Code]
,[Product Group Code]
,[Manufacturer Code]
,[ABC Category Grouping Code]
,[ABC Category Code]
,[Comment AS]
,[Parameters Grouping Code AS]
,[Brand Number AS]
,[Promoted Item]
,[Type]
,[Sorting Number]
FROM [dbo].[{0}$Item] where [Type] in ({1}) ";
            s = string.Format(s, setting.Settings_NavCompanyName, string.Join(",",types )  );
            return fillDataTable(s, ref errorMessage, ref result);
        }

        //gio

        public bool getItemsAllForPriseler(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
,[No_ 2]
,[Description]
,[Description 2]
,[Large Description]
,[Unit Price]
,[Base Unit of Measure]
,[Allow Invoice Disc_]
,[Vendor No_]
,[Vendor Item No_]
,[Gross Weight]
,[Net Weight]
,[Unit Volume]
,[Price Includes VAT]
,[Item Category Code]
,[Product Group Code]
,[Manufacturer Code]
,[ABC Category Grouping Code]
,[ABC Category Code]
,[Comment AS]
,[Parameters Grouping Code AS]
,[Brand Number AS]
,[Promoted Item]
,[Type]
,[Sorting Number]
FROM [dbo].[{0}$Item] WHERE [Gen_ Prod_ Posting Group] = 'AUTO PARTS'";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemsAllForShop(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
,[No_ 2]
,[Description]
,[Description 2]
,[Large Description]
,[Unit Price]
,[Base Unit of Measure]
,[Allow Invoice Disc_]
,[Vendor No_]
,[Vendor Item No_]
,[Gross Weight]
,[Net Weight]
,[Unit Volume]
,[Price Includes VAT]
,[Item Category Code]
,[Product Group Code]
,[Manufacturer Code]
,[ABC Category Grouping Code]
,[ABC Category Code]
,[Comment AS]
,[Parameters Grouping Code AS]
,[Brand Number AS]
,[Promoted Item]
,[Type]
,[Sorting Number]
FROM [dbo].[{0}$Item] WHERE [Gen_ Prod_ Posting Group] = 'AUTO PARTS' or [Gen_ Prod_ Posting Group] = 'AUTOSERV'";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }
        //

        public bool getItemsByCodes(string items, ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
,[No_ 2]
,[Description]
,[Description 2]
,[Large Description]
,[Unit Price]
,[Base Unit of Measure]
,[Allow Invoice Disc_]
,[Vendor No_]
,[Vendor Item No_]
,[Gross Weight]
,[Net Weight]
,[Unit Volume]
,[Price Includes VAT]
,[Item Category Code]
,[Product Group Code]
,[Manufacturer Code]
,[ABC Category Grouping Code]
,[ABC Category Code]
,[Comment AS]
,[Parameters Grouping Code AS]
,[Brand Number AS]
,[Promoted Item]
,[Type]
,[Sorting Number]
FROM [dbo].[{0}$Item]
where [No_] in ( {1}   )
";
            s = string.Format(s, setting.Settings_NavCompanyName, items);
            return fillDataTable(s, ref errorMessage, ref result);
        }


        public bool getAllItems(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
FROM [dbo].[{0}$Item]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemCrossReferences(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [Item No_],[Variant Code],[Unit of Measure],[Cross-Reference Type],[Cross-Reference Type No_],[Cross-Reference No_],
(SELECT [Qty_ per Unit of Measure] FROM [dbo].[{0}$Item Unit of Measure]
WHERE [Item No_] = [dbo].[{0}$Item Cross Reference].[Item No_] AND Code =
[dbo].[{0}$Item Cross Reference].[Unit of Measure]) AS Qty_PerUnitOfMeasure
FROM [dbo].[{0}$Item Cross Reference]
WHERE [Item No_] = '{1}' AND [Cross-Reference Type] = 3 and [Unit of Measure] != ''";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemCrossReferencesA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Item No_],[Variant Code],[Unit of Measure],[Cross-Reference Type],[Cross-Reference Type No_],[Cross-Reference No_],
(SELECT [Qty_ per Unit of Measure] FROM [dbo].[{0}$Item Unit of Measure]
WHERE [Item No_] = [dbo].[{0}$Item Cross Reference].[Item No_] AND Code =
[dbo].[{0}$Item Cross Reference].[Unit of Measure]) AS Qty_PerUnitOfMeasure
FROM [dbo].[{0}$Item Cross Reference]
WHERE [Cross-Reference Type] = 3 and [Unit of Measure] != ''";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }


        public bool getItemCustomers(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Customer No_]
,[Variant Code]
,[Like]
FROM [dbo].[{0}$Item Customer]
WHERE [Item No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemCustomersA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Customer No_]
,[Variant Code]
,[Like]
FROM [dbo].[{0}$Item Customer]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemPictures(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Picture No_]
,[Description]
,[Content]
FROM [dbo].[{0}$Item Picture]
WHERE [Item No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemPicturesA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Picture No_]
,[Description]
FROM [dbo].[{0}$Item Picture]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemPictureContent(string item_no, int picture_no, ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT   [Item No_]
,[Picture No_]
,[Description]
,[Content]
FROM [dbo].[{0}$Item Picture]
where [Item No_] = '{1}' and [Picture No_] = {2}";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no, picture_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemUnitOfMeasures(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Code]
,[Qty_ per Unit of Measure]
,[Length]
,[Width]
,[Height]
,[Cubage]
,[Weight]
FROM [dbo].[{0}$Item Unit of Measure]
WHERE [Item No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemUnitOfMeasuresA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Code]
,[Qty_ per Unit of Measure]
,[Length]
,[Width]
,[Height]
,[Cubage]
,[Weight]
FROM [dbo].[{0}$Item Unit of Measure]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        /// gio

        public bool getItemNo_(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Item No_]

                        FROM [dbo].[{0}$Sales Price]
                        group by [Item No_]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }//ფასების ცხრილიდან ყველა item ნომერის წამოღება

        public bool getItemSalesPricesOne(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT top 1 *
                        FROM [dbo].[{0}$Sales Price]
                        where [Item No_] = '{1}'
                        order by [Starting Date] desc";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }//ფასების ცხრილიდან ერთი ჩანაწერის წამოღება where item No_

        public bool getItemSalesPricesOnePrew(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT top 1 *
                        FROM [dbo].[{0}$Sales Price]
                        where [Item No_] = '{1}'
                        order by [Starting Date] desc";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }//ფასების ცხრილიდან ერთი ჩანაწერის წამოღება where item No_


        public bool getItemSalesPrices(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Sales Type]
,[Sales Code]
,[Starting Date]
,[Currency Code]
,[Variant Code]
,[Unit of Measure Code]
,[Minimum Quantity]
,[Unit Price]
,[Price Includes VAT]
,[Allow Invoice Disc_]
,[VAT Bus_ Posting Gr_ (Price)]
,[Ending Date]
,[Allow Line Disc_]
FROM [dbo].[{0}$Sales Price]
WHERE [Item No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemSalesPricesA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Sales Type]
,[Sales Code]
,[Starting Date]
,[Currency Code]
,[Variant Code]
,[Unit of Measure Code]
,[Minimum Quantity]
,[Unit Price]
,[Price Includes VAT]
,[Allow Invoice Disc_]
,[VAT Bus_ Posting Gr_ (Price)]
,[Ending Date]
,[Allow Line Disc_]
FROM
(select *, ROW_NUMBER()
OVER(
PARTITION BY [Sales Code]
,[Item No_]
ORDER BY [Starting Date]  DESC
) AS seqnum
FROM [dbo].[{0}$Sales Price]) t
WHERE seqnum <=2
";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }
        //TT
        public bool getItemVehicleModles(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Manufacturer Code]
,[Model No_]
,[Cab Type]
,[Engine]
,[Manufacturing Start Date]
,[Output_PH]
,[Item Description]
,[Manufacturer Name]
,[Model Description]
,[Manufacturing End Date]
,[TecDoc ID]
FROM [dbo].[{0}$Item Vehicle Model]
WHERE[Item No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemVehicleModlesA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Item No_]
,[Manufacturer Code]
,[Model No_]
,[Cab Type]
,[Engine]
,[Manufacturing Start Date]
,[Output_PH]
,[Item Description]
,[Manufacturer Name]
,[Model Description]
,[Manufacturing End Date]
,[TecDoc ID]
FROM [dbo].[{0}$Item Vehicle Model]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }
        //End TT(TecDoc ID)
        public bool getItemItems(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [Base Item No_]
,[Related Item No_]
,[Quantity]
,[Base Unit of Measure]
,[Related Unit of Measure]
FROM [dbo].[{0}$Item Item]
WHERE[Base Item No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemItemsA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Base Item No_]
,[Related Item No_]
,[Quantity]
,[Base Unit of Measure]
,[Related Unit of Measure]
FROM [dbo].[{0}$Item Item]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemComments(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT [timestamp]
      ,[Table Name]
      ,[No_]
      ,[Line No_]
      ,[Date]
      ,[Code]
      ,[Comment]
  FROM [dbo].[{0}$Comment Line]
  where  [Table Name] = 3 and No_ =  '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemCommentsA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [timestamp]
      ,[Table Name]
      ,[No_]
      ,[Line No_]
      ,[Date]
      ,[Code]
      ,[Comment]
  FROM [dbo].[{0}$Comment Line]
  where  [Table Name] = 3";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getProjectedItemReceipts(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT  SUM([Quantity (Base)]) AS ItemQuantity
		,[No_], [Expected Receipt Date] 
  FROM [dbo].[{0}$Purchase Line]
  WHERE Type = 2 AND [Location Code] in ( '{2}', '{3}' ) AND [Document Type] = 1 AND [No_] = '{1}'
  GROUP BY [No_], [Expected Receipt Date]
  Order by [No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no, setting.Settings_Location, setting.Settings_TransitLocation);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getProjectedItemReceiptsA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT  SUM([Quantity (Base)]) AS ItemQuantity
		,[No_], [Expected Receipt Date] 
  FROM [dbo].[{0}$Purchase Line]
  WHERE Type = 2 AND [Location Code] in ( '{1}', '{2}' ) AND [Document Type] = 1 
  GROUP BY [No_], [Expected Receipt Date]
  Order by [No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_Location, setting.Settings_TransitLocation);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getProjectedItemTransits(ref string errorMessage, string item_no, ref DataTable result)
        {
            string s = @"SELECT  SUM([Remaining Quantity]) AS ItemQuantity
		,[Item No_],[Posting Date]
  FROM [dbo].[{0}$Item Ledger Entry]
  WHERE [Entry Type] = 0 AND [Location Code] = '{2}' AND [Open] = 1 AND [Item No_] = '{1}'
  GROUP BY [Item No_], [Posting Date]";
            s = string.Format(s, setting.Settings_NavCompanyName, item_no, setting.Settings_TransitLocation);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getProjectedItemTransitsA(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT  SUM([Remaining Quantity]) AS ItemQuantity
		,[Item No_],[Posting Date]
  FROM [dbo].[{0}$Item Ledger Entry]
  WHERE [Entry Type] = 0 AND [Location Code] = '{1}' AND [Open] = 1
  GROUP BY [Item No_], [Posting Date]";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_TransitLocation);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getCustomers(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
,[Name]
,[Name 2]
,[Address]
,[Address 2]
,[City]
,[Contact]
,[Phone No_]
,[Credit Limit (LCY)]
,[Customer Price Group]
,[Payment Terms Code]
,[Salesperson Code]
,[Shipping Agent Code]
,[Customer Disc_ Group]
,[Bill-to Customer No_]
,[Payment Method Code]
,[Prices Including VAT]
,[VAT Registration No_]
FROM [dbo].[{0}$Customer]
WHERE [Salesperson Code] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        ///  gio
        public bool getCustomersNoSalesPerson(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT 
                    [Customer No_]
                    FROM [dbo].[{0}$Salesperson Ship-to Address]
                WHERE [Salesperson Code] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }//Ship-to address ცხრილიდან კლიენტების ნომრების ამოღება

        private string getCustomersNoSalesPersonToString()
        {
            string res = "";
            string Result = "";
            DataTable dtdam = null;

            getCustomersNoSalesPerson(ref res, ref dtdam);

            int i = 0;

            foreach (DataRow r in dtdam.Rows)
            {
                Result += (i == 0 ? $"'{r["Customer No_"]}'" : $",'{r["Customer No_"]}'");
                i++;
            }

            return Result;
        }//Ship-to address ცხრილიდან კლიენტების ნომრების სტრინგში გაერთიანება

        public bool getCustomersForPresaler(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT c.[No_]
,c.[Name]
,c.[Address]
,c.[City]
,c.[Contact]
,c.[Phone No_]
,c.[Credit Limit (LCY)]
,c.[Customer Price Group]
,c.[Payment Terms Code]
,c.[Salesperson Code]
,c.[Shipping Agent Code]
,c.[Customer Disc_ Group]
,c.[Bill-to Customer No_]
,c.[Payment Method Code]
,c.[Prices Including VAT]
,c.[VAT Registration No_]
,

(select top 1 [Visit Week Days] FROM [Gasgo].[dbo].[Gasgo$Salesperson Ship-to Address] s where s.[Customer No_] =  [No_] AND s.[Salesperson Code] = '{2}') [Visit Week Days]
FROM [dbo].[{0}$Customer] c

WHERE [No_] in({1})";



            s = string.Format(s, setting.Settings_NavCompanyName, getCustomersNoSalesPersonToString(), setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }


        public bool getCustomersNoDistibutor(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT 
                    [Customer No_]
                    FROM [dbo].[{0}$Salesperson Ship-to Address]
                WHERE [Shipping Agent Code] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }//Ship-to address ცხრილიდან კლიენტების ნომრების ამოღება

        private string getCustomersNoDistibutorToString()
        {
            string res = "";
            string Result = "";
            DataTable dtdam = null;

            getCustomersNoDistibutor(ref res, ref dtdam);

            int i = 0;

            foreach (DataRow r in dtdam.Rows)
            {
                Result += (i == 0 ? $"'{r["Customer No_"]}'" : $",'{r["Customer No_"]}'");
                i++;
            }

            return Result;
        }//Ship-to address ცხრილიდან კლიენტების ნომრების სტრინგში გაერთიანება

        public bool getCustomersForDistibutor(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT c.[No_]
,c.[Name]
,c.[Address]
,c.[City]
,c.[Contact]
,c.[Phone No_]
,c.[Credit Limit (LCY)]
,c.[Customer Price Group]
,c.[Payment Terms Code]
,c.[Salesperson Code]
,c.[Shipping Agent Code]
,c.[Customer Disc_ Group]
,c.[Bill-to Customer No_]
,c.[Payment Method Code]
,c.[Prices Including VAT]
,c.[VAT Registration No_]
,

(select top 1 [Shipping Week Days] FROM [Gasgo].[dbo].[Gasgo$Salesperson Ship-to Address] s where [Customer No_] =  [No_] AND s.[Shipping Agent Code] = '{2}' ) [Visit Week Days]
FROM [dbo].[{0}$Customer] c
WHERE [No_] in({1})";
            s = string.Format(s, setting.Settings_NavCompanyName, getCustomersNoDistibutorToString(), setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }



        public bool getCustomersAll(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT c.[No_]
,c.[Name]
,c.[Address]
,c.[City]
,c.[Contact]
,c.[Phone No_]
,c.[Credit Limit (LCY)]
,c.[Customer Price Group]
,c.[Payment Terms Code]
,c.[Salesperson Code]
,c.[Shipping Agent Code]
,c.[Customer Disc_ Group]
,c.[Bill-to Customer No_]
,c.[Payment Method Code]
,c.[Prices Including VAT]
,c.[VAT Registration No_]
,
(select top 1 [Visit Week Days] FROM [Gasgo].[dbo].[Gasgo$Salesperson Ship-to Address] s where [Customer No_] =  c.[No_] AND [Salesperson Code] = '{1}' ) [Visit Week Days]
FROM [dbo].[{0}$Customer] c";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getCustomer(ref string errorMessage, string customer_no, ref DataTable result)
        {
            string s = @"SELECT [No_]
,[Name]
,[Name 2]
,[Address]
,[Address 2]
,[City]
,[Contact]
,[Phone No_]
,[Credit Limit (LCY)]
,[Customer Price Group]
,[Payment Terms Code]
,[Salesperson Code]
,[Shipping Agent Code]
,[Customer Disc_ Group]
,[Bill-to Customer No_]
,[Payment Method Code]
,[Prices Including VAT]
,[VAT Registration No_]
FROM [dbo].[{0}$Customer]
WHERE [No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, customer_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getCustomerShippingAddresses(ref string errorMessage, string customer_no, ref DataTable result)
        {
            string s = @"SELECT [Customer No_]
,[Code]
,[Name]
,[Name 2]
,[Address]
,[Address 2]
,[City]
,[Contact]
,[Phone No_]
,[Shipping Agent Code]
FROM [dbo].[{0}$Ship-to Address]
WHERE [Customer No_] = '{1}' ";
            s = string.Format(s, setting.Settings_NavCompanyName, customer_no);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getSalesPersonShippingAddresses(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Customer No_]
,[Code]
,[Name]
,[Name 2]
,[Address]
,[Address 2]
,[City]
,[Contact]
,[Phone No_]
,[Shipping Agent Code]
,[Visit Week Days]
,[Shipping Week Days]
FROM [dbo].[{0}$Ship-to Address]
WHERE [Salesperson Code] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getShippingAgetShippingAddresses(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Customer No_]
,[Code]
,[Name]
,[Name 2]
,[Address]
,[Address 2]
,[City]
,[Contact]
,[Phone No_]
,[Shipping Agent Code]
,[Visit Week Days]
,[Shipping Week Days]
FROM [dbo].[{0}$Ship-to Address]
WHERE [Shipping Agent Code] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getSalesPersonShippingAddressesGrouped(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Customer No_]
FROM [dbo].[{0}$Ship-to Address]
WHERE [Salesperson Code] = '{1}'
GROUP BY [Customer No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getCustomerPaymentSchedule(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Entry No_]
,[Entry Type]
,[Second Party No_]
,[Document No_]
,[Date]
,[Amount]
,[Remaining Amount]
FROM [dbo].[{0}$Payment Schedule]
WHERE [Entry Type] = 0 
AND [Second Party Type] = 0 
AND [Remaining Amount] > 0";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getReleasedOrders(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [No_]
      ,[Document Type]
      ,[Due Date]
      ,[Posting Date]
      ,[Shipment Date]
      ,[Sell-to Customer No_]
      ,[Sell-to Customer Name]
      ,[Sell-to Address]
      ,[Sell-to City]
      ,[Sell-to Post Code]
      ,[Sell-to Country_Region Code]
      ,[Ship-to Address]
      ,[Ship-to Code]
      ,[Ship-to City]
      ,[Ship-to Country_Region Code]
      ,[Status]
      ,[Payment Terms Code]
      ,[Payment Method Code]
      ,[Shipment Method Code]
      ,[Currency Code]
      ,[Currency Factor]
      ,[Customer Price Group]
      ,[Prices Including VAT]
      ,[Invoice Discount Calculation]
      ,[Invoice Discount Value]
      ,[Salesperson Code]
      ,[VAT Registration No_]
      ,[External Document No_]
      ,[Shipping Agent Code]
      ,[Responsibility Center]
      ,[Posting Description]
  FROM [dbo].[{0}$Sales Header]
  WHERE [Salesperson Code] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getReleasedOrderAmount(string orderNo, ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Document No_]
    ,SUM([Amount]) AS Amount
  FROM [dbo].[{0}$Sales Line]
  WHERE [Document No_] = '{1}'
  GROUP BY [Document No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, orderNo);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getReleasedOrderAmountIncVat(string orderNo, ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Document No_]
    ,SUM([Amount Including VAT]) AS AmountIncVAT
  FROM [dbo].[{0}$Sales Line]
  WHERE [Document No_] = '{1}'
  GROUP BY [Document No_]";
            s = string.Format(s, setting.Settings_NavCompanyName, orderNo);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getReleasedOrderLines(string orderNo, ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Document No_]
      ,[Document Type]
      ,[Line No_]
      ,[Type]
      ,[No_]
      ,[Description]
      ,[Large Description]
      ,[Quantity]
      ,[Unit Price]
      ,[Sell-to Customer No_]
      ,[Location Code]
      ,[Inv_ Discount Amount]
      ,[Line Discount _]
      ,[Line Discount Amount]
      ,[Amount]
      ,[Amount Including VAT]
      ,[Gross Weight]
      ,[Net Weight]
      ,[Unit Volume]
      ,[Unit of Measure Code]
      ,[Quantity (Base)]
      ,[Responsibility Center]
      ,[Item Category Code]
      ,[Product Group Code]
  FROM [dbo].[{0}$Sales Line]
  WHERE [Document No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, orderNo);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getReleasedOrderPayments(string orderNo, ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Journal Template Name]
      ,[Journal Batch Name]
      ,[Line No_]
      ,[Account Type]
      ,[Account No_]
      ,[Posting Date]
      ,[Document No_]
      ,[Description]
      ,[Bal_ Account Type]
      ,[Bal_ Account No_]
      ,[Amount]
      ,[Salespers__Purch_ Code]
      ,[Payment Method Code]
      ,[Responsibility Center]
  FROM [dbo].[{0}$Gen_ Journal Line]
  WHERE [Document No_] = '{1}' AND [Journal Template Name] = '{2}' AND [Journal Batch Name] = '{3}'";
            s = string.Format(s, setting.Settings_NavCompanyName, orderNo, setting.Settings_JnlTemplateName, setting.Settings_JnlBatchName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getReleasedOrderPaymentSchedule(string orderNo, ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Entry No_]
      ,[Entry Type]
      ,[Document No_]
      ,[Date]
      ,[Amount]
      ,[Remaining Amount]
  FROM [dbo].[{0}$Payment Schedule]
  WHERE [Document No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, orderNo);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getPresalersBudgetDemansions(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT [Global Dimension 2 Code] AS RegionCode
,(SELECT [Name] FROM [dbo].[Gasgo$Dimension Value]
WHERE [Dimension Code] = (SELECT [Shortcut Dimension 2 Code]
FROM [dbo].[{0}$General Ledger Setup])
AND [Code] = [Global Dimension 2 Code]) AS RegionName
,SUM([Sales Amount]) AS BudgetAmount
FROM [dbo].[{0}$Item Budget Entry]
WHERE [Budget Dimension 1 Code] = (SELECT [Dimension Value Code]
FROM [dbo].[{0}$Default Dimension]
WHERE [Table ID] = 13
AND [Dimension Code] = (SELECT [Shortcut Dimension 3 Code]
FROM [dbo].[{0}$General Ledger Setup])
AND [No_] = '{1}')
AND [Date] = '{2}'
GROUP BY [Global Dimension 2 Code]";
            s = string.Format(s, setting.Settings_NavCompanyName, setting.Settings_SalesPersonCode, string.Format("{0}-{1}-01", DateTime.Today.Year, DateTime.Today.Month));
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool updateMessageReadStatus(string entryNo, ref string errorMessage)
        {
            string s = @"UPDATE [dbo].[{0}$POS Message Entry]
   SET  [Read DateTime] = GETDATE(), [IsRead] = 1
 WHERE [Entry No_] = '{1}'";
            s = string.Format(s, setting.Settings_NavCompanyName, entryNo);
            return ExecuteDBScript(s, CommandType.Text, ref errorMessage);
        }

        public bool getVehicleGroup(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT
      [No_]
      ,[Description]
        FROM [dbo].[{0}$Vehicle Group]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getItemVehicleGroup(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT 
      [Item No_]
      ,[Vehicle Group No_]
      ,[Item Description]
      ,[Vehicle Group Description]
  FROM [dbo].[{0}$Item Vehicle Group]";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }


        public bool getVendors(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT [No_]
      ,[Name]
      ,[Country_Region Code]
      ,[VAT Registration No_]
  FROM [dbo].[{0}$Vendor]
  where No_ like 'PV%' or No_ like 'RESP%'", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getVehicles(ref string errorMessage, ref DataTable result)
        {
            string s = String.Format(@"SELECT
      [Vehicle No_]
      ,[Model]
      ,[Owner No_]
      ,[Owner Name]
      ,[Ownership]
      ,[Registration No_]
      ,[Trail Registration No_]
      ,[Tranzit Location Code]
      ,[Type]
      ,[Driver Code]
      ,[Driver Name]
      ,[Driver ID]
      ,[WIN Code]
  FROM [dbo].[{0}$Vehicle]", setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }

        public bool getDimensionValue(ref string errorMessage, ref DataTable result)
        {
            string s = @"SELECT 
        *
  FROM [dbo].[{0}$Dimension Value] where [Dimension Value Type] = 0 and ISNUMERIC(Code) > 0 AND [Dimension Code] = 'AREA'";
            s = string.Format(s, setting.Settings_NavCompanyName);
            return fillDataTable(s, ref errorMessage, ref result);
        }


    }
}
