using CoreTypes;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class SynchronizationManager : PosManager<SynchronizationManager>
    {
        public void Synchronize(HashSet<SynchTypes> synchTypes, bool withPictures, Action<string, string, double> progressAction, PosUserTypes userType)
        {

            if (synchTypes.Count == 0) return;

            if (synchTypes.Contains(SynchTypes.General))
            {

                SynchronizeGeneral(progressAction, userType);
            }

            if (synchTypes.Contains(SynchTypes.Products) && userType != PosUserTypes.Distributor)
            {
                SynchronizeAllProducts(progressAction, withPictures, userType);
            }
            else if (synchTypes.Contains(SynchTypes.Reserves) && userType != PosUserTypes.Distributor)
            {
                SynchronizeCars(progressAction);//cars
            }
            else if (synchTypes.Contains(SynchTypes.ReservesShort) && userType != PosUserTypes.Distributor)
            {
                SynchronizeProductsShort(progressAction);
            }
            else if (synchTypes.Contains(SynchTypes.SalesPrices) && userType != PosUserTypes.Distributor)
            {
                SynchronizeSalesPrices(progressAction);
            }

            if (synchTypes.Contains(SynchTypes.Customers))
            {
                SynchronizeCustomers(progressAction, userType);
            }
        }

        public bool HasUnsendObjects()
        {
            return DaoController.Current.IsNewCustomers();
        }

        public void SynchronizeGeneral(Action<string, string, double> progressAction, PosUserTypes userType)
        {
            string res = "";
            DataTable dt = null;




            progressAction(null, "Synchronizing Payment Methods", 22);
            NavDbController.Current.getPaymentMethods(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncPaymentMethods(dt);
            //progressAction(null, "Synchronizing Salesperson_Purchasers", 33);
            //NavDbController.Current.getSalespersons(ref res, ref dt); gio  დროებით გავთიშე
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncSalespersons(dt);
            //progressAction(null, "Synchronizing Item Categories", 44);
            //NavDbController.Current.getItemCategory(ref res, ref dt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItemCategory(dt);
            //progressAction(null, "Synchronizing Unit of Measures", 55);
            //NavDbController.Current.getUnitOfMeasures(ref res, ref dt); gio  es produqtebSi aucileblad
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncUnitOfMeasure(dt);
            //progressAction(null, "Synchronizing Product Groups", 66); gio produqtebSi
            //NavDbController.Current.getProductGroup(ref res, ref dt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncProductGroups(dt);
            progressAction(null, "Synchronizing Locations", 77);
            NavDbController.Current.getLocations(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncLocation(dt);
            //progressAction(null, "Synchronizing Manufacturers", 88);gio  es produqtebSi aucileblad
            //NavDbController.Current.getManufacturer(ref res, ref dt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncManufacturer(dt);
            //progressAction(null, "Synchronizing Vehicle Models", 95);gio axali
            //NavDbController.Current.getVehicleModel(ref res, ref dt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncVehicleModel(dt);
            progressAction(null, "Additional Parameters", 97);
            NavDbController.Current.getAdditionalParameters(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncAdditionalParameters(dt);
            progressAction(null, "Additional Parameters", 98);
            NavDbController.Current.getSuggestions(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemSuggestions(dt);
            progressAction(null, "User Setup", 99);
            NavDbController.Current.getUserSetup(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncUserSetup(dt);


            progressAction("მიმდინარეობს ზოგადი ცხრილების სინქრონიზაცია..", "Synchronizing Bank Accounts", 11);
            NavDbController.Current.getBankAccounts(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncBankAccounts(dt);

            progressAction(null, "Synchronizing Countries/Cities", 22);
            NavDbController.Current.getCountries(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncCountries(dt);

            NavDbController.Current.getPostCodesCities(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncPostCodesCities(dt);


            NavDbController.Current.getCompanyInformation(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncCompanyInformation(dt);


            //progressAction(null, "Payment Schedule", 99);gio  clientebshi
            //NavDbController.Current.getCustomerPaymentSchedule(ref res,  ref dt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncPaymentSchedule(dt);

            //progressAction(null, "vehicle groups", 99); gio produqtebshi
            //NavDbController.Current.getVehicleGroup(ref res, ref dt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncVehicleGroup(dt);
            //NavDbController.Current.getItemVehicleGroup(ref res, ref dt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItemVehicleGroup(dt);

            progressAction(null, "Synchronizing Projected Items", 99);
            DataTable itemProjectedItemReceiptsDT = null;
            NavDbController.Current.getProjectedItemReceiptsA(ref res, ref itemProjectedItemReceiptsDT);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DataTable itemProjectedItemTransitsDT = null;
            NavDbController.Current.getProjectedItemTransitsA(ref res, ref itemProjectedItemTransitsDT);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncProjectedItemReceipts(itemProjectedItemReceiptsDT, itemProjectedItemTransitsDT);

            progressAction(null, "messages", 99);
            SendServiceManager.Current.SendNewMessages();
            foreach (var i in DaoController.Current.GetStatusChangeMessages())
            {
                NavDbController.Current.updateMessageReadStatus(i, ref res);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            }

            var userId = DaoController.Current.GetUserBySalesPersonCode(PosSetting.Settings_SalesPersonCode);
            NavDbController.Current.geNavMessage(userId, ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncMessages(dt);

            progressAction(null, "Synchronizing Dimension Value", 99);

            NavDbController.Current.getDimensionValue(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncDimensionValue(dt);

            //switch (userType)
            //{
            //    case PosUserTypes.Distributor:
            //    case PosUserTypes.PreSaler:


            //        break;
            //    case PosUserTypes.Manager:
            //    case PosUserTypes.Shop:
            //        if (PosSetting.Settings_Show_Shelf == 1)
            //        {
            //            progressAction(null, "Synchronizing Stockkeeping Unit", 99);

            //            NavDbController.Current.getStockKeepingUnit(ref res, ref dt);
            //            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //            DaoController.Current.SyncStockKeepingUnit(dt);
            //        }

            //        break;
            //}


        }



        public void SynchronizeMessages()
        {
            string res = "";
            DataTable dt = null;
            SendServiceManager.Current.SendNewMessages();
            foreach (var i in DaoController.Current.GetStatusChangeMessages())
            {
                NavDbController.Current.updateMessageReadStatus(i, ref res);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            }

            var userId = DaoController.Current.GetUserBySalesPersonCode(PosSetting.Settings_SalesPersonCode);
            NavDbController.Current.geNavMessage(userId, ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncMessages(dt);
        }


        public void TestPictureSync()
        {
            DaoController.Current.ClearAllPictures();
            string res = "";
            DataTable itemPicturesDt = null;
            NavDbController.Current.getItemPicturesA(ref res, ref itemPicturesDt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            foreach (DataRow r in itemPicturesDt.Rows)
            {
                var itemNo = r["Item No_"].ToString();
                var picture_no = (int)r["Picture No_"];
                DataTable dtcontent = null;
                NavDbController.Current.getItemPictureContent(itemNo, picture_no, ref res, ref dtcontent);
                DaoController.Current.SyncItemPictures(dtcontent);
            }
        }

        //gio მანქანების სინქრონიზაცია

        public void SynchronizeCars(Action<string, string, double> progressAction)
        {
            string res = "";
            DataTable dt = null;

            progressAction(null, "Synchronizing Vehicle Models", 95);
            NavDbController.Current.getVehicleModel(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncVehicleModel(dt);

            progressAction("მიმდინარეობს ზოგადი ცხრილების სინქრონიზაცია..", "Synchronizing Bank Accounts", 11);
            NavDbController.Current.getBankAccounts(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncBankAccounts(dt);

            progressAction(null, "Synchronizing Countries/Cities", 22);
            NavDbController.Current.getCountries(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncCountries(dt);

            NavDbController.Current.getPostCodesCities(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncPostCodesCities(dt);

        }

        public void TestSyncManufacturer()
        {
            string res = "";
            DataTable dt = null;

            NavDbController.Current.GetSavedItemsForLaterSales(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncSavedItemsForLaterSales(dt);
        }

        public void TestSyncs()
        {
            string res = "";
            DataTable dt = null;

            NavDbController.Current.getItemCategory(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemCategory(dt);
        }


        public void TestSyncItemLedgerEntryFull()
        {
            string res = "";
            DataTable dt = null;

            NavDbController.Current.getItemLedgerEntriesFull(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemLedgerEntriesFull(dt);
        }

        public void TestSyncCompanyInformation()
        {
            string res = "";
            DataTable dt = null;

            NavDbController.Current.getCompanyInformation(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncCompanyInformation(dt);
        }


        public void SynchronizeAllProducts(Action<string, string, double> progressAction, bool withPictures, PosUserTypes userType)
        {
            string res = "";
            DataTable dt = null;
            DataTable itemsDt = null;


            progressAction("მიმდინარეობს მარაგების სინქრონიზაცია..", "Getting  Item Ledger Entries", 0);

            DaoController.Current.ClearItemLedgerEntries();
            DaoController.Current.ClearAllItems(withPictures);
            //gio
            progressAction(null, "Synchronizing Unit of Measures", 55);
            NavDbController.Current.getUnitOfMeasures(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncUnitOfMeasure(dt);

            progressAction(null, "Synchronizing Product Groups", 66);
            NavDbController.Current.getProductGroup(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncProductGroups(dt);

            progressAction(null, "Synchronizing Manufacturers", 88);
            NavDbController.Current.getManufacturer(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncManufacturer(dt);

            progressAction(null, "vehicle groups", 99);
            NavDbController.Current.getVehicleGroup(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncVehicleGroup(dt);

            NavDbController.Current.getItemVehicleGroup(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemVehicleGroup(dt);

            progressAction(null, "Payment Schedule", 99);
            NavDbController.Current.getCustomerPaymentSchedule(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncPaymentSchedule(dt);

            NavDbController.Current.GetSavedItemsForLaterSales(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncSavedItemsForLaterSales(dt);

            NavDbController.Current.getItemLedgerEntriesFull(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemLedgerEntriesFull(dt);


            //SynchronizeSalesPrices(progressAction);// gio ფასების სინქრონიზაცია

            SynchronizeProducts(progressAction, withPictures);




            progressAction(null, "Synchronizing Items", 10);


            switch (userType)
            {
                case PosUserTypes.Distributor:
                case PosUserTypes.PreSaler:
                    DaoController.Current.DeleteItems();
                    NavDbController.Current.getItemsAllForPriseler(ref res, ref itemsDt);
                    break;
                case PosUserTypes.Manager:
                    DaoController.Current.DeleteItems();
                    NavDbController.Current.getItemsAll(new int[] { 0, 1 }, ref res, ref itemsDt);
                    break;
                case PosUserTypes.Shop:
                    DaoController.Current.DeleteItems();
                    NavDbController.Current.getItemsAllForShop(ref res, ref itemsDt);
                    break;
            }



            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItems(itemsDt);

            //progressAction(null, "Synchronizing Items Cross References", 15); gio...
            //DataTable itemCrossReferencesDt = null;
            //NavDbController.Current.getItemCrossReferencesA(ref res, ref itemCrossReferencesDt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItemCrossReferences(itemCrossReferencesDt);

            //progressAction(null, "Synchronizing Customers", 20); gio...
            //DataTable itemCustomersDt = null;
            //NavDbController.Current.getItemCustomersA(ref res, ref itemCustomersDt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItemCustomers(itemCustomersDt);

            if (withPictures)
            {
                progressAction(null, "Synchronizing Items Pictures", 25);
                DataTable itemPicturesDt = null;
                NavDbController.Current.getItemPicturesA(ref res, ref itemPicturesDt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                var imgdict = DaoController.Current.GetItemPicturesIds();
                foreach (DataRow r in itemPicturesDt.Rows)
                {
                    var itemNo = r["Item No_"].ToString();
                    var picture_no = (int)r["Picture No_"];
                    DataTable dtcontent = null;
                    if (!imgdict.ContainsKey(Tuple.Create(itemNo, picture_no)))
                    {
                        NavDbController.Current.getItemPictureContent(itemNo, picture_no, ref res, ref dtcontent);
                        DaoController.Current.SyncItemPictures(dtcontent);
                    }
                }
            }



            //progressAction(null, "Synchronizing Items Unit Of Measures", 30); gio...
            //DataTable itemUnitOfMeasuresDt = null;
            //NavDbController.Current.getItemUnitOfMeasuresA(ref res, ref itemUnitOfMeasuresDt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItemUnitOfMeasures(itemUnitOfMeasuresDt);

            progressAction(null, "Synchronizing Items Sales Prices", 35);
            DataTable itemSalesPricesDt = null;
            NavDbController.Current.getItemSalesPricesA(ref res, ref itemSalesPricesDt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemSalesPrices(itemSalesPricesDt);


            NavDbController.Current.getItemCategory(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemCategory(dt);




            //progressAction(null, "Synchronizing Items VehicleModles", 40); gio es manqanis modelebshi
            //DataTable itemVehicleModlesDt = null;
            //NavDbController.Current.getItemVehicleModlesA(ref res,  ref itemVehicleModlesDt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItemVehicleModels(itemVehicleModlesDt);

            //progressAction(null, "Synchronizing Item Items", 45); gio...
            //DataTable itemItemsDt = null;
            //NavDbController.Current.getItemItemsA(ref res,  ref itemItemsDt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItemItems(itemItemsDt);



            //progressAction(null, "Synchronizing Item Comments", 60); gio..
            //DataTable itemCommentsDT = null;
            //NavDbController.Current.getItemCommentsA(ref res,  ref itemCommentsDT);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItemComments(itemCommentsDT);

            progressAction(null, "Synchronizing ItemLedgeEntries", 65);
            DataTable dtl = null, dts = null, dtr = null;
            Dictionary<string, decimal> ldict = new Dictionary<string, decimal>();
            Dictionary<string, decimal> sdict = new Dictionary<string, decimal>();
            Dictionary<string, decimal> rdict = new Dictionary<string, decimal>();
            NavDbController.Current.getItemLedgerEntryQuantityA(ref res, ref dtl);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            NavDbController.Current.getItemSalesLineA(ref res, ref dts);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            NavDbController.Current.getItemReturnLineA(ref res, ref dtr);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            foreach (DataRow r in dtl.Rows)
            {
                string itemNo = r["Item No_"].ToString();
                decimal val = (decimal)r["quantity"];
                ldict[itemNo] = val;
            }
            foreach (DataRow r in dts.Rows)
            {
                string itemNo = r["No_"].ToString();
                decimal val = (decimal)r["quantity"];
                sdict[itemNo] = val;
            }
            foreach (DataRow r in dtr.Rows)
            {
                string itemNo = r["No_"].ToString();
                decimal val = (decimal)r["quantity"];
                rdict[itemNo] = val;
            }


            var cnt = itemsDt.Rows.Count;
            int i = 0;
            foreach (DataRow r in itemsDt.Rows)
            {
                i++;
                progressAction(null, "Synchronizing ItemLedgeEntries", 70 + i * 30.0 / cnt);
                var item_no = r["No_"].ToString();
                decimal quantity = 0, val = 0;

                if (ldict.TryGetValue(item_no, out val))
                    quantity += val;
                if (sdict.TryGetValue(item_no, out val))
                    quantity -= val;
                if (rdict.TryGetValue(item_no, out val))
                    quantity += val;

                if (quantity > 0)
                    DaoController.Current.AddNewItemLedgeEntry(item_no, quantity, PosSetting.Settings_Location, r["Base Unit of Measure"].ToString());

            }
        }


        public void SynchronizeProductsShort(Action<string, string, double> progressAction)
        {
            string res = "";

            progressAction("მიმდინარეობს მარაგების სინქრონიზაცია..", "Getting  Item Ledger Entries", 0);

            DaoController.Current.ClearItemLedgerEntries();
            //DaoController.Current.ClearAllItemsShort();


            progressAction(null, "Synchronizing Items", 10);
            DataTable itemsDt = null;
            //NavDbController.Current.getItemsAll(ref res, ref itemsDt); //    NavDbController.Current.getItemsA(ref res, ref itemsDt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItems(itemsDt); //   DaoController.Current.SyncOrUpdateItems(itemsDt);


            progressAction(null, "Synchronizing ItemLedgeEntries", 65);
            DataTable dtl = null, dts = null, dtr = null;
            //Dictionary<string, decimal> ldict = new Dictionary<string, decimal>();
            Dictionary<string, decimal> sdict = new Dictionary<string, decimal>();
            Dictionary<string, decimal> rdict = new Dictionary<string, decimal>();
            NavDbController.Current.getItemLedgerEntryQuantityA(ref res, ref dtl);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            NavDbController.Current.getItemSalesLineA(ref res, ref dts);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            NavDbController.Current.getItemReturnLineA(ref res, ref dtr);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            foreach (DataRow r in dts.Rows)
            {
                string itemNo = r["No_"].ToString();
                decimal val = (decimal)r["quantity"];
                sdict[itemNo] = val;
            }
            foreach (DataRow r in dtr.Rows)
            {
                string itemNo = r["No_"].ToString();
                decimal val = (decimal)r["quantity"];
                rdict[itemNo] = val;
            }
            var itemsList = new List<Tuple<string, decimal, string, string>>();
            foreach (DataRow r in dtl.Rows)
            {
                var item_no = r["Item No_"].ToString();
                var uc = r["uc"].ToString();
                decimal quantity = (decimal)r["quantity"], val = 0;

                if (sdict.TryGetValue(item_no, out val))
                    quantity -= val;
                if (rdict.TryGetValue(item_no, out val))
                    quantity += val;

                if (quantity > 0)
                    itemsList.Add(Tuple.Create(item_no, quantity, PosSetting.Settings_Location, uc));
                // DaoController.Current.AddNewItemLedgeEntry(item_no, quantity, PosSetting.Settings_Location, uc);
            }

            DaoController.Current.AddNewItemLedgeEntries(itemsList);

            var itemNos = DaoController.Current.GetItemsNotInItems().Select(s => $"'{s}'").ToList();
            var itemsstring = string.Join(",", itemNos);
            if (!string.IsNullOrEmpty(itemsstring))
            {
                NavDbController.Current.getItemsByCodes(itemsstring, ref res, ref itemsDt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                DaoController.Current.SyncItems(itemsDt);
            }

            // load servie items
            //DaoController.Current.ClearServiceItems();
            //NavDbController.Current.getItemsAll(new int[] { 1 }, ref res, ref itemsDt);
            //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            //DaoController.Current.SyncItems(itemsDt);
            DataTable dt = null;
            NavDbController.Current.getItemLedgerEntriesFull(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemLedgerEntriesFull(dt);

            DataTable dtc = null;
            NavDbController.Current.getItemCategory(ref res, ref dtc);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemCategory(dtc);
        }




        public void SynchronizeProducts(Action<string, string, double> progressAction, bool withPicture)
        {
            string res = "";
            DataTable dt = null;
            DataTable dt1 = null;

            DataTable dtdam = null;
            progressAction("მიმდინარეობს მარაგების სინქრონიზაცია..", "Getting  Item Ledger Entries", 0);
            NavDbController.Current.getItemLedgerEntry(ref res, ref dtdam);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.ClearItemLedgerEntries();
            var cnt = dtdam.Rows.Count;
            int i = 0;
            foreach (DataRow r in dtdam.Rows)
            {
                i++;
                progressAction(null, "Synchronizing Items", 10 + i * 90.0 / cnt);
                var item_no = r["Item No_"].ToString();
                var quantity = (decimal)r["Quantity"] - NavDbController.Current.getItemSalesLine(item_no, ref res) + NavDbController.Current.getItemReturnLine(item_no, ref res);
                DaoController.Current.ClearItems(item_no, withPicture);

                //gio es itemi gadavides produqtebshi
                NavDbController.Current.getItems(ref res, item_no, ref dt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                DaoController.Current.SyncItems(dt);
                if ((dt.Rows.Count > 0) && (quantity > 0))
                    DaoController.Current.AddNewItemLedgeEntry(item_no, quantity, PosSetting.Settings_Location, dt.Rows[0]["Base Unit of Measure"].ToString());
                //
                NavDbController.Current.getItemCrossReferences(ref res, item_no, ref dt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                DaoController.Current.SyncItemCrossReferences(dt);

                NavDbController.Current.getItemCustomers(ref res, item_no, ref dt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                DaoController.Current.SyncItemCustomers(dt);

                //if (withPicture) gio es suratebshi
                //{
                //    NavDbController.Current.getItemPictures(ref res, item_no, ref dt);
                //    if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                //    DaoController.Current.SyncItemPictures(dt);
                //}

                NavDbController.Current.getItemUnitOfMeasures(ref res, item_no, ref dt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                DaoController.Current.SyncItemUnitOfMeasures(dt);



                //NavDbController.Current.getItemVehicleModles(ref res, item_no, ref dt); gio axali
                //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                //DaoController.Current.SyncItemVehicleModels(dt);

                NavDbController.Current.getItemItems(ref res, item_no, ref dt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                DaoController.Current.SyncItemItems(dt);

                ////Synch projected receipts
                //NavDbController.Current.getProjectedItemReceipts(ref res, item_no, ref dt);
                //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                //NavDbController.Current.getProjectedItemTransits(ref res, item_no, ref dt1);
                //if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                //DaoController.Current.SyncProjectedItemReceipts(dt, dt1);
                // synch comments
                NavDbController.Current.getItemComments(ref res, item_no, ref dt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                DaoController.Current.SyncItemComments(dt);
            }


        }

        public void SynchronizeCustomers(Action<string, string, double> progressAction, PosUserTypes userType)
        {
            progressAction("მიმდინარეობს კლიენტების სინქრონიზაცია..", "Clearing  Customeres", 0);
            DaoController.Current.ClearCustomers();
            if (userType != PosUserTypes.Distributor)
            {
                progressAction(null, "Synchronizing Customers with one presaler", 20);
                SynchronizeCustomersOnepresalers(progressAction, userType);

                //progressAction(null, "Synchronizing Customers with many presalers", 40);
                //SynchronizeCustomersManypresalers(progressAction);

                progressAction(null, "Synchronizing ReleasedSalesHeader", 60);
                SynchronizeReleasedOrders();
            }
            else
            {
                progressAction(null, "Synchronizing Customers", 20);
                SynchronizeCustomersForDistributors(progressAction);
                SynchronizeCustomersOnepresalers(progressAction, userType);
            }
            progressAction(null, "Synchronizing PresalersBudges", 80);
            SynchronizePresalerSchedule(progressAction);
        }

        public void SynchronizeSalesPrices(Action<string, string, double> progressAction)
        {
            progressAction("მიმდინარეობს ფასების სინქრონიზაცია..", "SalesPrices", 0);
            DaoController.Current.ClearSalesPrices();
            progressAction(null, "Synchronizing Items Sales Prices", 35);
            DataTable itemSalesPricesDt = null;
            string res = "";
            NavDbController.Current.getItemSalesPricesA(ref res, ref itemSalesPricesDt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncItemSalesPrices(itemSalesPricesDt);
        }

        public void SynchronizeCustomersOnepresalers(Action<string, string, double> progressAction, PosUserTypes userType)
        {
            string res = "";
            DataTable dt = null;
            DataTable dtdam = null;
            switch (userType)
            {
                case PosUserTypes.Distributor:
                    NavDbController.Current.getCustomersForDistibutor(ref res, ref dtdam);
                    break;
                case PosUserTypes.PreSaler:
                    NavDbController.Current.getCustomersForPresaler(ref res, ref dtdam);
                    break;
                case PosUserTypes.Manager:
                    NavDbController.Current.getCustomers(ref res, ref dtdam);
                    break;
                case PosUserTypes.Shop:
                    NavDbController.Current.getCustomersAll(ref res, ref dtdam);
                    break;
            }
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);

            POSMng.POSMng service = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            var cnt = dtdam.Rows.Count;
            int i = 0;
            foreach (DataRow r in dtdam.Rows)
            {
                i++;
                progressAction(null, "Synchronizing Customers with one presalers", 20 + i * 20.0 / cnt);
                var customer_no = r["No_"].ToString();
                decimal balanceAmount = 0;
                decimal salesBudgetAmount = 0;
                decimal salesActualAmount = 0;
                decimal recommendedSalesAmount = 0;
                int recommendedVisitsMonth = 0;
                service.GetCustomerFinDetails(customer_no, ref balanceAmount, ref salesBudgetAmount, ref salesActualAmount, ref recommendedSalesAmount, ref recommendedVisitsMonth, DateTime.Now.Date);
                DaoController.Current.SyncCustomers(r, balanceAmount, salesBudgetAmount, salesActualAmount, recommendedSalesAmount, recommendedVisitsMonth, null);
                NavDbController.Current.getCustomerShippingAddresses(ref res, customer_no, ref dt);
                if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                DaoController.Current.SyncShipToAddress(dt);
            }
        }

        public void SynchronizeCustomersManypresalers(Action<string, string, double> progressAction)
        {
            string res = "";
            DataTable dt = null;
            DataTable dtdam = null;
            NavDbController.Current.getSalesPersonShippingAddresses(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncShipToAddress(dt);
            POSMng.POSMng service = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            var cnt = dt.Rows.Count;
            int i = 0;
            foreach (DataRow r in dt.Rows)
            {
                i++;
                progressAction(null, "Synchronizing Customers with many presalers", 40 + i * 10.0 / cnt);
                var customer_no = r["Customer No_"].ToString();
                decimal balanceAmount = 0;
                decimal salesBudgetAmount = 0;
                decimal salesActualAmount = 0;
                decimal recommendedSalesAmount = 0;
                int recommendedVisitsMonth = 0;
                NavDbController.Current.getCustomer(ref res, customer_no, ref dtdam);
                service.GetCustomerFinDetails(customer_no, ref balanceAmount, ref salesBudgetAmount, ref salesActualAmount, ref recommendedSalesAmount, ref recommendedVisitsMonth, DateTime.Now.Date);
                var r1 = dtdam.Rows[0];
                var cst = DaoController.Current.GetCustomer(r1["No_"].ToString());
                if (cst == null)
                    DaoController.Current.SyncCustomers(r1, balanceAmount, salesBudgetAmount, salesActualAmount, recommendedSalesAmount, recommendedVisitsMonth, r["Visit Week Days"].ToString());
            }
            //Synchronize payment schedules
            NavDbController.Current.getSalesPersonShippingAddressesGrouped(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
        }

        public void SynchronizeCustomersForDistributors(Action<string, string, double> progressAction)
        {
            string res = "";
            DataTable dt = null;
            DataTable dtdam = null;
            NavDbController.Current.getShippingAgetShippingAddresses(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncShipToAddress(dt);
            POSMng.POSMng service = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            var cnt = dt.Rows.Count;
            int i = 0;
            foreach (DataRow r in dt.Rows)
            {
                i++;
                progressAction(null, "Synchronizing Customers with many presalers", 40 + i * 10.0 / cnt);
                var customer_no = r["Customer No_"].ToString();
                decimal balanceAmount = 0;
                decimal salesBudgetAmount = 0;
                decimal salesActualAmount = 0;
                decimal recommendedSalesAmount = 0;
                int recommendedVisitsMonth = 0;
                NavDbController.Current.getCustomer(ref res, customer_no, ref dtdam);
                service.GetCustomerFinDetails(customer_no, ref balanceAmount, ref salesBudgetAmount, ref salesActualAmount, ref recommendedSalesAmount, ref recommendedVisitsMonth, DateTime.Now.Date);
                var r1 = dtdam.Rows[0];
                var cst = DaoController.Current.GetCustomer(r1["No_"].ToString());
                if (cst == null)
                    DaoController.Current.SyncCustomers(r1, balanceAmount, salesBudgetAmount, salesActualAmount, recommendedSalesAmount, recommendedVisitsMonth, r["Shipping Week Days"].ToString());
            }
        }

        public void SynchronizeReleasedOrders()
        {
            DaoController.Current.ClearReleasedOrders();
            string res = "";
            DataTable dt = null;
            NavDbController.Current.getReleasedOrders(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            DaoController.Current.SyncReleasedOrders(dt);
        }

        public void SynchronizePresalerSchedule(Action<string, string, double> progressAction)
        {
            string res = "";
            DataTable dt = null;
            NavDbController.Current.getPresalersBudgetDemansions(ref res, ref dt);
            if (!string.IsNullOrEmpty(res)) throw new PosException(res);
            POSMng.POSMng service = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            var cnt = dt.Rows.Count;
            var i = 0;
            foreach (DataRow r in dt.Rows)
            {
                i++;
                progressAction(null, "Synchronizing PresalersBudges", 80);
                var region_no = r["RegionCode"].ToString();
                decimal badgetamount = 0;
                decimal actualAmount = 0;
                string ragion_name = "";
                service.GetSalesPersFinDetails(PosSetting.Settings_SalesPersonCode, region_no, ref badgetamount, ref actualAmount, ref ragion_name, DateTime.Now.Date);

                DaoController.Current.SyncPresalersBudges(r, badgetamount, actualAmount);
            }
        }
    }

    public enum SynchTypes
    {
        General,
        Reserves,
        Products,
        Customers,
        ReservesShort,
        SalesPrices
    }
}
