using CoreTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Annotations;
using DataLayer.Extensions;
using DataLayer.Models;

namespace DataLayer
{
    //gio

    public class SalesContainer
    {
        public string ItemNo { get; set; }
        public DateTime StartingDate { get; set; }
    }

    public class DaoController : Singleton<DaoController>
    {
        public static readonly Dictionary<OrderBaseTypes, Type> SalesHeaderBases = new Dictionary<OrderBaseTypes, Type>
        {
            {OrderBaseTypes.Current, typeof (SalesHeader)},
            {OrderBaseTypes.Posted, typeof (PostedSalesHeader)},
            {OrderBaseTypes.Released, typeof (ReleasedSalesHeader)},
        };

        public static readonly Dictionary<OrderBaseTypes, Type> SalesLinesBases = new Dictionary<OrderBaseTypes, Type>
        {
            {OrderBaseTypes.Current, typeof (SalesLine)},
            {OrderBaseTypes.Posted, typeof (PostedSalesLine)},
            {OrderBaseTypes.Released, typeof (ReleasedSalesLine)},
        };

        public static readonly Dictionary<OrderBaseTypes, Type> GenJounralLineBases = new Dictionary<OrderBaseTypes, Type>
        {
            {OrderBaseTypes.Current, typeof (GenJournalLine)},
            {OrderBaseTypes.Posted, typeof (PostedGenJournalLine)},
            {OrderBaseTypes.Released, typeof (ReleasedGenJournalLine)},
        };

        public static readonly Dictionary<OrderBaseTypes, Type> PaymentSchedulesBases = new Dictionary<OrderBaseTypes, Type>
        {
            {OrderBaseTypes.Current, typeof (PaymentSchedule)},
            {OrderBaseTypes.Posted, typeof (PostedPaymentSchedule)},
            {OrderBaseTypes.Released, typeof (ReleasedPaymentSchedule)},
        };

        public POSUser getUserByUserNameAndPassword(string username, string password)
        {
            using (var e = new POSWR1Entities())
            {
                return e.POSUsers.FirstOrDefault(i => i.POSUser_UserID == username && i.POSUser_Password == password);
            }
        }


        public List<POSUser> getUsers()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                res = e.POSUsers.ToList();
            }
            return res;
        }

        public List<ItemVehicleModel> GetItemVehicleModels()
        {
            List<ItemVehicleModel> res;
            using (var e = new POSWR1Entities())
            {
                res = e.ItemVehicleModels.ToList();
                //res = new List<ItemVehicleModel>();
            }
            return res;
        }
        public List<byte[]> GetItemImage(string itemNo)
        {
            List<ItemVehicleModel> res;
            using (var e = new POSWR1Entities())
            {
                return e.ItemPictures.Where(i => i.ItemNo_ == itemNo).Select(i => i.Content).ToList();
            }
        }

        public List<RegionBudgetActual> GetRegionBudgetActual()
        {
            using (var e = new POSWR1Entities())
            {
                return e.RegionBudgetActuals.ToList();
            }
        }

        public List<Tuple<string, byte[]>> GetRelatedItemImages(string itemNo)
        {
            using (var e = new POSWR1Entities())
            {
                var res = new List<Tuple<string, byte[]>>();
                var relatedItems = e.ItemItems.Where(i => i.BaseItemNo_ == itemNo).Select(i => i.RelatedItemNo_).ToList();
                Random rnd = new Random();
                relatedItems = relatedItems.OrderBy(i => rnd.Next()).ToList();
                foreach (var r in relatedItems)
                {
                    var c = e.ItemPictures.FirstOrDefault(i => i.ItemNo_ == r);
                    if (c != null && c.Content != null)
                        res.Add(Tuple.Create(r, c.Content));
                    if (res.Count == 3) break;
                }
                if (res.Count < 3) res.Add(null);
                if (res.Count < 3) res.Add(null);
                if (res.Count < 3) res.Add(null);
                return res;
            }
        }

        public List<Tuple<string, byte[]>> GetSuggestedImages(string customerNo)
        {
            using (var e = new POSWR1Entities())
            {
                //var res = new List<Tuple<string, byte[]>>();
                var dt = DateTime.Today.AddDays(-30);

                var items1 =
                    e.ReleasedSalesLines.Where(i => e.ReleasedSalesHeaders.Any(h => h.Sell_toCustomerNo == customerNo && h.No_ == i.DocumentNo_ && h.PostingDate >= dt)).Select(i => i.No_)
                        .Distinct()
                        .Where(j => e.ItemLedgerEntries.Where(k => k.ItemNo == j).Sum(k => k.Quantity) > 0)
                        .ToList();
                var items = e.ItemSuggestions.Select(i => i.ItemNo).Distinct().ToList();
                items1.AddRange(items);
                items = items1.ToList();
                var itemPictures1 =
                    e.ItemPictures.Where(i => items.Contains(i.ItemNo_))
                        .ToList()
                        .Select(i => Tuple.Create(i.ItemNo_, i.Content))
                        .ToList();
                var itemPictures = new List<Tuple<string, byte[]>>();
                foreach (var item in items)
                {
                    var p = itemPictures1.FirstOrDefault(i => i.Item1 == item);
                    if (p != null)
                        itemPictures.Add(p);
                }


                //foreach (var r in items)
                //{
                //    var c = e.ItemPictures.FirstOrDefault(i => i.ItemNo_ == r);
                //    if (c != null && c.Content != null)
                //        res.Add(Tuple.Create(r, c.Content));
                //}
                return itemPictures;
            }
        }

        public bool? GetItemCustomerLike(string itemNo, string customerNo)
        {
            using (var e = new POSWR1Entities())
            {
                var ic = e.ItemCustomers.FirstOrDefault(i => i.ItemNo_ == itemNo && i.CustomerNo_ == customerNo);
                if (ic != null)
                    return ic.Like;
                return null;
            }
        }

        public List<VehicleModel> GetItemVehicleModelsList(string itemNo)
        {
            using (var e = new POSWR1Entities())
            {
                return e.VehicleModels.Where(
                    i => e.ItemVehicleModels.Any(j => j.ItemNo_ == itemNo && j.ManufacturerCode == i.ManufacturerCode &&
                                                      j.ModelNo_ == i.ModelNo_ && j.CabType == i.CabType &&
                                                      j.Engine == i.Engine
                                                      && j.ManufacturingStartDate == i.ManufacturingStartDate

                        )).ToList();
            }
        }

        public string GetItemBox(string itemNo)
        {
            using (var e = new POSWR1Entities())
            {
                var um = e.ItemUnitOfMeasures.FirstOrDefault(i => i.ItemNo_ == itemNo && i.Code == "BOX");
                if (um != null)
                    return Math.Round(um.Qty_PerUnitOfMeasure, 2).ToString();
                return null;
            }
        }

        public POSUser getUser(string username)
        {
            using (var e = new POSWR1Entities())
            {
                return e.POSUsers.FirstOrDefault(i => i.POSUser_UserID == username);
            }
        }

        public string GetUserBySalesPersonCode(string salesPersonCode)
        {
            using (var e = new POSWR1Entities())
            {
                var u = e.UserSetups.FirstOrDefault(i => i.Salespers_Purch_Code == salesPersonCode);
                if (u != null)
                    return u.UserID;
                return null;
            }
        }

        public List<POSMessageEntry> GetMessages(string userid, string anotheruserid)
        {
            using (var e = new POSWR1Entities())
            {
                var list =
                    e.POSMessageEntries.Where(
                        i =>
                            (i.SenderID == userid && i.ReceiverID == anotheruserid) ||
                            (i.SenderID == anotheruserid && i.ReceiverID == userid))
                        .OrderBy(i => i.SendDateTime)
                        .ToList();
                list.ForEach(i =>
                {
                    i.CurrentUserId = userid;
                });
                return list;
            }
        }

        public void UpdateReadStatus(string userId, string anotherUserId)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (var l in e.POSMessageEntries.Where(i => (i.SenderID == anotherUserId && i.ReceiverID == userId && !i.IsRead)))
                {
                    l.IsRead = true;
                    l.ReadDateTime = DateTime.Now;
                    l.ReadStatusChange = true;
                }
                e.SaveChanges();
            }
        }

        public void SendNewMessage(string senderId, string reciverId, string senderSCode, string recieverSCode, string text)
        {
            using (var e = new POSWR1Entities())
            {
                var msg = new POSMessageEntry
                {
                    CurrentUserId = senderId,
                    Text = text,
                    IsRead = false,
                    SenderID = senderId,
                    ReceiverID = reciverId,
                    ReceiverSalespersCode = recieverSCode,
                    SendDateTime = DateTime.Now,
                    SenderSalespersCode = senderSCode
                };
                e.POSMessageEntries.Add(msg);
                e.SaveChanges();
            }
        }

        public List<string> GetUserSetupIds(string userId = null)
        {
            using (var e = new POSWR1Entities())
            {
                return e.UserSetups.Where(i => userId == null || (userId != i.UserID)).Select(i => i.UserID).ToList();
            }
        }

        public List<Tuple<string, string, int>> GetUserMessagesCounts(string userId)
        {
            using (var e = new POSWR1Entities())
            {
                var l = e.UserSetups.Where(i => userId != i.UserID).Select(i => new
                {
                    userId = i.UserID,
                    salesPersonCode = i.Salespers_Purch_Code,
                    newMessagesCount =
                        e.POSMessageEntries.Count(c => c.SenderID == i.UserID && c.ReceiverID == userId && !c.IsRead)
                }).ToList();
                l = l.OrderByDescending(i => i.newMessagesCount).ThenBy(i => i.userId).ToList();
                var res = new List<Tuple<string, string, int>>();
                foreach (var i in l)
                {
                    res.Add(Tuple.Create(i.userId, i.salesPersonCode, i.newMessagesCount));
                }
                return res;
            }
        }

        public void DeleteMessage(long messageId)
        {
            using (var e = new POSWR1Entities())
            {
                var m = e.POSMessageEntries.FirstOrDefault(i => i.Id == messageId);
                if (m != null)
                {
                    e.POSMessageEntries.Remove(m);
                    e.SaveChanges();
                }
            }
        }

        public void SaveUser(string oldusername, string username, string password, string firstname, string lastname, bool isBlocked, int userType, string salesPersonCode)
        {
            using (var e = new POSWR1Entities())
            {
                if (!string.IsNullOrEmpty(oldusername))
                {
                    var prsnold = e.POSUsers.First(i => i.POSUser_UserID == oldusername);
                    e.POSUsers.Remove(prsnold);
                }
                var prsn = new POSUser();
                prsn.POSUser_UserID = username;
                prsn.POSUser_Password = password;
                prsn.POSUser_FirstName = firstname;
                prsn.POSUser_LastName = lastname;
                prsn.POSUser_IsBlocked = isBlocked;
                prsn.POSUser_type = userType;
                prsn.POSUser_SalesPerson_Code = salesPersonCode;
                prsn.ModifiedDate = DateTime.Now;
                e.POSUsers.Add(prsn);
                e.SaveChanges();
            }
        }

        public string SaveCustomer(Customer customer)
        {
            using (var e = new POSWR1Entities())
            {
                if (e.Customers.Any(i => i.VATRegistrationNo_ == customer.VATRegistrationNo_))
                    throw new Exception("საიდენთიფიკაციო ნომერი უკვერ არსებობს");
                var post = GetPostCodeCities().Find(c => c.City == customer.City);
                var r = 0;
                var rnd = new Random();
                while (true)
                {
                    r = rnd.Next(10000);
                    string id = "NC_" + r.ToString("D4");
                    if (!e.Customers.Any(i => i.No_ == id)) break;
                }
                customer.PostCode = post?.PostCode;
                customer.No_ = "NC_" + r.ToString("D4");
                e.Customers.Add(customer);
                e.Ship_to_Address.Add(new Ship_to_Address
                {
                    CustomerNo_ = customer.No_,
                    Code = "001",
                    Name = customer.Name,
                    Name_2 = "",
                    FullName = null,
                    Address = customer.Address,
                    Address_2 = "",
                    City = customer.City,
                    Contact = "",
                    PhoneNo_ = "",
                    ShippingAgentCode = ""
                });
                e.SaveChanges();
                return customer.No_;
            }
        }

        public void UpdateCustomer(Customer customer)
        {
            using (var e = new POSWR1Entities())
            {
                if (e.Customers.Any(i => i.VATRegistrationNo_ == customer.VATRegistrationNo_ && i.No_ != customer.No_))
                    throw new Exception("საიდენთიფიკაციო ნომერი უკვერ არსებობს");

                e.Customers.Remove(e.Customers.First(j => j.No_ == customer.No_));

                e.Customers.Add(customer);

                e.Ship_to_Address.RemoveRange(e.Ship_to_Address.Where(j => j.CustomerNo_ == customer.No_));

                e.Ship_to_Address.Add(new Ship_to_Address
                {
                    CustomerNo_ = customer.No_,
                    Code = "001",
                    Name = customer.Name,
                    Name_2 = "",
                    FullName = null,
                    Address = customer.Address,
                    Address_2 = "",
                    City = customer.City,
                    Contact = "",
                    PhoneNo_ = "",
                    ShippingAgentCode = ""
                });
                e.SaveChanges();
            }
        }

        public Setting GetSettings()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                return e.Settings.FirstOrDefault();
            }
        }

        public byte[] GetMotivationPicture()
        {
            using (var e = new POSWR1Entities())
            {
                return e.Salesperson_Purchaser.Select(i => i.MotivationPicture).FirstOrDefault();
            }
        }

        public void UpdateSettings(Setting setting)
        {
            using (var e = new POSWR1Entities())
            {
                //e.Settings.Attach(setting);
                if (!e.Settings.Any())
                    e.Settings.Add(setting);
                else
                {
                    e.Entry(setting).State = EntityState.Modified;
                }
                e.SaveChanges();
            }
        }

        public void SetLikeSent(string customerNo, string itemNo)
        {
            using (var e = new POSWR1Entities())
            {
                var like = e.ItemCustomers.FirstOrDefault(i => i.CustomerNo_ == customerNo && i.ItemNo_ == itemNo);
                if (like != null)
                {
                    like.IsNew = false;
                    e.SaveChanges();
                }
            }
        }

        public void UpdateCustomerStatus(string customerNo, string newCode)
        {
            using (var e = new POSWR1Entities())
            {
                var customer = e.Customers.First(i => i.No_ == customerNo);
                var ncst = new Customer
                {
                    Address = customer.Address,
                    ModifiedDate = customer.ModifiedDate,
                    No_ = newCode,
                    Balance = customer.Balance,
                    City = customer.City,
                    Contact = customer.Contact,
                    Country_RegionCode = customer.Country_RegionCode,
                    CreditLimit_LCY_ = customer.CreditLimit_LCY_,
                    CustomerPriceGroup = customer.CustomerPriceGroup,
                    E_Mail = customer.E_Mail,
                    FullName = customer.FullName,
                    InvoiceDisc_Group = customer.InvoiceDisc_Group,
                    IsNewCustomer = false,
                    ModifiedUserID = customer.ModifiedUserID,
                    Name = customer.Name,
                    NeedsVATInvoice = customer.NeedsVATInvoice,
                    PaymentMethodCode = customer.PaymentMethodCode,
                    PaymentTermsCode = customer.PaymentTermsCode,
                    PhoneNo_ = customer.PhoneNo_,
                    PostCode = customer.PostCode,
                    PriceIncludingVAT = customer.PriceIncludingVAT,
                    RecommendedSalesAmount = customer.RecommendedSalesAmount,
                    RecommendedVisitsMonth = customer.RecommendedVisitsMonth,
                    SalesActualAmount = customer.SalesActualAmount,
                    SalesBudgetAmount = customer.SalesBudgetAmount,
                    ShipToAddress = customer.ShipToAddress,
                    Type = customer.Type,
                    VATRegistrationNo_ = customer.VATRegistrationNo_,
                    VendorNo_ = customer.VendorNo_,
                    VisitWeekDays = customer.VisitWeekDays,
                    AreaCode = customer.AreaCode,
                    Mobile_ = customer.Mobile_
                };
                e.Customers.Remove(customer);
                e.Customers.Add(ncst);
                e.SaveChanges();

                foreach (var h in e.SalesHeaders.Where(i => i.Sell_toCustomerNo == customerNo).ToList())
                {
                    h.Sell_toCustomerNo = newCode;
                }

                foreach (var s in e.SalesLines.Where(i => i.Sell_toCustomerNo == customerNo).ToList())
                {
                    s.Sell_toCustomerNo = newCode;
                }

                foreach (var j in e.GenJournalLines.Where(i => i.Bal_AccountNo_ == customerNo).ToList())
                {
                    j.Bal_AccountNo_ = newCode;
                }

                foreach (var j in e.PostedPaymentSchedules.Where(i => i.CustomerNo == customerNo).ToList())
                {
                    j.CustomerNo = newCode;
                }

                var oldShipAddress = e.Ship_to_Address.FirstOrDefault(i => i.CustomerNo_ == customerNo);
                if (oldShipAddress != null)
                {
                    var newShipAddress = new Ship_to_Address
                    {
                        CustomerNo_ = newCode,
                        Address = oldShipAddress.Address,
                        Address_2 = oldShipAddress.Address_2,
                        City = oldShipAddress.City,
                        Code = oldShipAddress.Code,
                        Contact = oldShipAddress.Contact,
                        FullName = oldShipAddress.FullName,
                        Name = oldShipAddress.Name,
                        Name_2 = oldShipAddress.Name_2,
                        PhoneNo_ = oldShipAddress.PhoneNo_,
                        ShippingAgentCode = oldShipAddress.ShippingAgentCode
                    };
                    e.Ship_to_Address.Remove(oldShipAddress);
                    e.Ship_to_Address.Add(newShipAddress);
                }
                e.SaveChanges();
            }
        }



        #region synchronization

        // gio

        public void SyncStockkeepingUnit(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                //e.Database.ExecuteSqlCommand("delete from [dbo].[keepingUnit]");
                ////foreach (DataRow r in dt.Rows)
                ////{
                //keepingUnit s = new keepingUnit();
                //s.Location_Code = "Location_Code";
                //s.Item_No_ = "Item_No"; 
                //s.Variant_Code = "Variant Code";
                //s.Shelf_No_ = "Shelf No_";
                //s.Unit_Cost = 0;
                //s.Standard_Cost = 0;
                //s.Last_Direct_Cost = 0;
                //s.Vendor_No_ = "Vendor No_";
                //s.Vendor_Item_No_ = "Vendor Item No_";
                //s.Lead_Time_Calculation = "Lead Time Calculation";
                //s.Reorder_Point = 0;
                //s.Maximum_Inventory = 0;
                //s.Reorder_Quantity = 0;
                //s.Last_Date_Modified = DateTime.Now;
                //s.Assembly_Policy = 1;
                //s.Transfer_Level_Code = 1;
                //s.Lot_Size = 0;
                //s.Discrete_Order_Quantity = 0;
                //s.Maximum_Order_Quantity = 0;
                //s.Minimum_Order_Quantity = 0;
                //s.Safety_Stock_Quantity = 0;
                //s.Order_Multiple = 0;
                //s.Safety_Lead_Time = "Safety Lead Time";
                //s.Components_at_Location = "Components at Location";
                //s.Flushing_Method = 1;
                //s.Replenishment_System = 1;
                //s.Time_Bucket = "Time Bucket";
                //s.Reordering_Policy = 1;
                //s.Include_Inventory = 1;
                //s.Manufacturing_Policy = 1;
                //s.Rescheduling_Period = "Rescheduling Period";
                //s.Lot_Accumulation_Period = "Lot Accumulation Period";
                //s.Dampener_Period = "Dampener Period";
                //s.Dampener_Quantity = 0;
                //s.Overflow_Level = 0;
                //s.Transfer_from_Code = "Transfer-from Code";
                //s.Special_Equipment_Code = "Special Equipment Code";
                //s.Put_away_Template_Code = "Put-away Template Code";
                //s.Put_away_Unit_of_Measure_Code = "Put-away Unit of Measure Code";
                //s.Phys_Invt_Counting_Period_Code = "Phys Invt Counting Period Code";
                //s.Last_Counting_Period_Update = DateTime.Now;
                //s.Use_Cross_Docking = 0;
                //s.Next_Counting_Start_Date = DateTime.Now;
                //s.Next_Counting_End_Date = DateTime.Now;
                //s.Shelf_No__AS = "Shelf No_ AS";

                //e.keepingUnits.Add(s);
                ////}
                //e.SaveChanges();
            }
        }

        private int? GetIntnullFromDataColumn(object column)
        {
            if (column == null || column == DBNull.Value || string.IsNullOrEmpty(column.ToString()))
                return null;
            return (int)column;
        }

        private byte[] GetBytesnullFromDataColumn(object column)
        {
            if (column == null || column == DBNull.Value)
                return null;
            return (byte[])column;
        }

        private int GetIntFromDataColumn(object column)
        {
            if (column == null || column == DBNull.Value || string.IsNullOrEmpty(column.ToString()))
                return 0;
            return (int)column;
        }

        private decimal? GetDecimalnullFromDataColumn(object column)
        {
            if (column == null || column == DBNull.Value || string.IsNullOrEmpty(column.ToString()))
                return null;
            return (decimal?)column;
        }

        private DateTime GetDateTimeFromDataColumn(object column)
        {
            if (column == null || column == DBNull.Value || string.IsNullOrEmpty(column.ToString()))
                return DateTime.Now;
            return (DateTime)column;
        }

        private DateTime? GetDateTimenullFromDataColumn(object column)
        {
            if (column == null || column == DBNull.Value || string.IsNullOrEmpty(column.ToString()))
                return null;
            return (DateTime?)column;
        }

        public void SyncBankAccounts(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from [dbo].[BankAccount]");
                foreach (DataRow r in dt.Rows)
                {
                    BankAccount u = new BankAccount();
                    u.No_ = r["No_"].ToString();
                    u.AccountType = (int)r["Account Type"];
                    u.Type = (int)r["Type"];
                    u.Name = r["Name"].ToString();
                    u.ResponsibiltyCenter = r["Responsibilty Center"].ToString();
                    e.BankAccounts.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncCountries(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from [dbo].[Country]");
                foreach (DataRow r in dt.Rows)
                {
                    Country u = new Country();
                    u.Code = r["Code"].ToString();
                    u.Name = r["Name"].ToString();
                    e.Countries.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncPostCodesCities(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from [dbo].[PostCodeCity]");
                foreach (DataRow r in dt.Rows)
                {
                    PostCodeCity u = new PostCodeCity();
                    u.PostCode = r["Code"].ToString();
                    u.City = r["City"].ToString();
                    e.PostCodeCities.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncPaymentMethods(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from [dbo].[PaymentMethod]");
                foreach (DataRow r in dt.Rows)
                {
                    PaymentMethod u = new PaymentMethod();
                    u.Code = r["Code"].ToString();
                    u.Description = r["Description"].ToString();
                    e.PaymentMethods.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncSalespersons(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from [dbo].[Salesperson/Purchaser]");
                foreach (DataRow r in dt.Rows)
                {
                    Salesperson_Purchaser u = new Salesperson_Purchaser();
                    u.Code = r["Code"].ToString();
                    u.Name = r["Name"].ToString();
                    if (r["Motivation Picture"] != DBNull.Value)
                        u.MotivationPicture = GetBytesnullFromDataColumn(r["Motivation Picture"]);
                    else
                        u.MotivationPicture = null;
                    e.Salesperson_Purchaser.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemCategory(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.ItemCategory");
                foreach (DataRow r in dt.Rows)
                {
                    ItemCategory u = new ItemCategory();
                    u.Code = r["Code"].ToString();
                    u.Description = r["Description"].ToString();
                    e.ItemCategories.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncUnitOfMeasure(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.UnitOfMeasure");
                foreach (DataRow r in dt.Rows)
                {
                    UnitOfMeasure u = new UnitOfMeasure();
                    u.Code = r["Code"].ToString();
                    u.Description = r["Description"].ToString();

                    e.UnitOfMeasures.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncProductGroups(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.ProductGroup");
                foreach (DataRow r in dt.Rows)
                {
                    ProductGroup u = new ProductGroup();
                    u.ItemCategoryCode = r["Item Category Code"].ToString();
                    u.Code = r["Code"].ToString();
                    u.Description = r["Description"].ToString();
                    e.ProductGroups.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncLocation(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.Location");
                foreach (DataRow r in dt.Rows)
                {
                    Location u = new Location();
                    u.Code = r["Code"].ToString();
                    u.Name = r["Name"].ToString();
                    u.Address = r["Address"].ToString();
                    u.City = r["City"].ToString();
                    u.Contact = r["Contact"].ToString();
                    u.ResponsibilityCenter = r["Responsibility Center"].ToString();
                    e.Locations.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncManufacturer(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.Manufacturer");
                foreach (DataRow r in dt.Rows)
                {
                    Manufacturer u = new Manufacturer();
                    u.Code = r["Code"].ToString();
                    u.Name = r["Name"].ToString();
                    e.Manufacturers.Add(u);
                }
                e.SaveChanges();
            }
        }

        public void SyncVehicleModel(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.VehicleModel");
                foreach (DataRow r in dt.Rows)
                {
                    VehicleModel u = new VehicleModel();
                    u.ManufacturerCode = r["Manufacturer Code"].ToString();
                    u.ModelNo_ = r["Model No_"].ToString();
                    u.CabType = r["Cab Type"].ToString();
                    u.Engine = r["Engine"].ToString();
                    u.ManufacturerName = r["Manufacturer Name"].ToString();
                    u.ModelDescription = r["Model Description"].ToString();

                    u.ManufacturingStartDate = GetDateTimeFromDataColumn(r["Manufacturing Start Date"]);
                    u.ManufacturingEndDate = GetDateTimenullFromDataColumn(r["Manufacturing End Date"]);
                    u.DriveType = GetIntnullFromDataColumn(r["Drive Type"]);
                    u.Series = r["Series"].ToString();

                    u.Output_KW_ = r["Output_KW"].ToString();
                    u.Output_PH_ = r["Output_PH"].ToString();
                    u.Tech_EngineCapacity_CC_ = r["Tech_ Engine Capacity CC"].ToString();
                    u.Cylinder = GetIntnullFromDataColumn(r["Cylinder"]);
                    u.ValvesperCylinder = GetIntnullFromDataColumn(r["Valves per Cylinder"]);

                    u.EngineType = GetIntnullFromDataColumn(r["Engine Type"]);
                    u.Transmission = GetIntnullFromDataColumn(r["Transmission"]);
                    u.FuelMixtureFormation = GetIntnullFromDataColumn(r["Fuel Mixture Formation"]);
                    u.BrakeSystem = GetIntnullFromDataColumn(r["Brake System"]);
                    u.BrakeType = GetIntnullFromDataColumn(r["Brake Type"]);
                    u.ASR = GetIntnullFromDataColumn(r["ASR"]);
                    u.CatalyticConverterType = GetIntnullFromDataColumn(r["Catalytic Converter Type"]);
                    u.AirCondition = GetIntnullFromDataColumn(r["Air Condition"]);
                    u.Drive = GetIntnullFromDataColumn(r["Drive"]);
                    u.Voltage = GetDecimalnullFromDataColumn(r["Voltage"]);
                    u.Wheelbase = r["Wheel base"].ToString();
                    //TT
                    u.TecDoc_ID = r["TecDoc ID"].ToString();
                    //END TT(TecDoc ID)
                    e.VehicleModels.Add(u);
                }
                e.SaveChanges();

            }
        }

        public void SyncItems(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                var itemsList = new List<Item>();
                foreach (DataRow r in dt.Rows)
                {
                    Item b = new Item();
                    b.No_ = r["No_"].ToString();
                    b.No_2 = r["No_ 2"].ToString();
                    b.Description = r["Description"].ToString();
                    b.Description2 = r["Description 2"].ToString();
                    b.UnitPrice = (decimal)r["Unit Price"];
                    b.BaseUnitOfMeasure = r["Base Unit of Measure"].ToString();
                    b.LargeDescription = r["Large Description"].ToString();
                    b.AllowInvoiceDisc = (bool)(r["Allow Invoice Disc_"].ToString() == "1");
                    b.VendorNo = r["Vendor No_"].ToString();
                    b.VendorItemNo = r["Vendor Item No_"].ToString();
                    b.GrossWeight = (decimal)r["Gross Weight"];
                    b.NetWeight = (decimal)r["Net Weight"];
                    b.UnitVolume = (decimal)r["Unit Volume"];
                    b.PriceIncludesVAT = r["Price Includes VAT"].ToString() == "1";
                    b.ItemCategoryCode = r["Item Category Code"].ToString();
                    b.ProductGroupCode = r["Product Group Code"].ToString();

                    b.ManufacturerCode = r["Manufacturer Code"].ToString();
                    b.ABCCategoryGroupingCode = r["ABC Category Grouping Code"].ToString();
                    b.ABCCategoryCode = r["ABC Category Code"].ToString();
                    b.CommentAS = r["Comment AS"].ToString();
                    b.ParametersGroupingCodeAS = r["Parameters Grouping Code AS"].ToString();
                    b.BrandNumberAS = r["Brand Number AS"].ToString();
                    b.PromotedItem = (bool)(r["Promoted Item"].ToString() == "1");
                    itemsList.Add(b);
                    //e.Items.Add(b);
                }
                e.Items.AddRange(itemsList);

                e.BulkSaveChanges(b => b.BatchSize = 2000);
            }
        }

        public void SyncOrUpdateItems(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    var isNew = false;
                    var no_ = r["No_"].ToString();
                    Item b = e.Items.FirstOrDefault(i => i.No_ == no_);
                    if (b == null)
                    {
                        b = new Item
                        {
                            No_ = no_
                        };
                        isNew = true;
                    }
                    // b.No_ = r["No_"].ToString();
                    b.No_2 = r["No_ 2"].ToString();
                    b.Description = r["Description"].ToString();
                    b.Description2 = r["Description 2"].ToString();
                    b.UnitPrice = (decimal)r["Unit Price"];
                    b.BaseUnitOfMeasure = r["Base Unit of Measure"].ToString();
                    b.LargeDescription = r["Large Description"].ToString();
                    b.AllowInvoiceDisc = (bool)(r["Allow Invoice Disc_"].ToString() == "1");
                    b.VendorNo = r["Vendor No_"].ToString();
                    b.VendorItemNo = r["Vendor Item No_"].ToString();
                    b.GrossWeight = (decimal)r["Gross Weight"];
                    b.NetWeight = (decimal)r["Net Weight"];
                    b.UnitVolume = (decimal)r["Unit Volume"];
                    b.PriceIncludesVAT = r["Price Includes VAT"].ToString() == "1";
                    b.ItemCategoryCode = r["Item Category Code"].ToString();
                    b.ProductGroupCode = r["Product Group Code"].ToString();

                    b.ManufacturerCode = r["Manufacturer Code"].ToString();
                    b.ABCCategoryGroupingCode = r["ABC Category Grouping Code"].ToString();
                    b.ABCCategoryCode = r["ABC Category Code"].ToString();
                    b.CommentAS = r["Comment AS"].ToString();
                    b.ParametersGroupingCodeAS = r["Parameters Grouping Code AS"].ToString();
                    b.BrandNumberAS = r["Brand Number AS"].ToString();
                    b.PromotedItem = (bool)(r["Promoted Item"].ToString() == "1");
                    if (isNew)
                        e.Items.Add(b);
                }
                e.SaveChanges();
            }
        }


        public void SyncAdditionalParameters(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.AdditionalParameters");
                foreach (DataRow r in dt.Rows)
                {
                    AdditionalParameter a = new AdditionalParameter();
                    a.TableID = (int)r["Table ID"];
                    a.No_ = r["No_"].ToString();
                    a.ParameterNo_ = r["Parameter No_"].ToString();
                    a.ParameterValueText = r["Parameter Value Text"].ToString();
                    a.ParameterValueDec_ = (decimal)r["Parameter Value Dec_"];
                    a.ParameterDescription = r["Parameter Description"].ToString();
                    a.RelatedParameterNo_ = r["Related Parameter No_"].ToString();
                    a.RelatedParameterValueText = r["Related Parameter Value Text"].ToString();
                    a.RelatedParameterValueDec_ = (decimal)r["Related Parameter Value Dec_"];
                    a.RelatedParameterDescription = r["Related Parameter Description"].ToString();
                    a.SequenceNo_ = (int)r["Sequence No_"];
                    e.AdditionalParameters.Add(a);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemSuggestions(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.ItemSuggestions");
                foreach (DataRow r in dt.Rows)
                {
                    ItemSuggestion a = new ItemSuggestion();
                    a.ItemNo = r["No_"].ToString();
                    e.ItemSuggestions.Add(a);
                }
                e.SaveChanges();
            }
        }

        public void SyncUserSetup(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.UserSetup");
                foreach (DataRow r in dt.Rows)
                {
                    UserSetup a = new UserSetup();
                    a.UserID = r["User ID"].ToString();
                    a.Salespers_Purch_Code = r["Salespers__Purch_ Code"].ToString();
                    e.UserSetups.Add(a);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemCrossReferences(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                var items = new List<ItemCrossReference>();
                foreach (DataRow r in dt.Rows)
                {
                    ItemCrossReference b = new ItemCrossReference();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.VariantCode = r["Variant Code"].ToString();
                    b.UnitOfMeasure = r["Unit of Measure"].ToString();
                    b.Cross_ReferenceNo_ = r["Cross-Reference No_"].ToString();
                    b.Qty_PerUnitOfMeasure = (decimal)r["Qty_PerUnitOfMeasure"];
                    b.Cross_ReferenceType = (int)r["Cross-Reference Type"];
                    b.Cross_ReferenceTypeNo_ = r["Cross-Reference Type No_"].ToString();
                    items.Add(b);
                    // e.ItemCrossReferences.Add(b);
                }
                e.ItemCrossReferences.AddRange(items);
                e.BulkSaveChanges(b => b.BatchSize = 1000);
            }
        }

        public void SyncItemCrossReferences(List<DataRow> drs)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in drs)
                {
                    ItemCrossReference b = new ItemCrossReference();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.VariantCode = r["Variant Code"].ToString();
                    b.UnitOfMeasure = r["Unit of Measure"].ToString();
                    b.Cross_ReferenceNo_ = r["Cross-Reference No_"].ToString();
                    b.Qty_PerUnitOfMeasure = (decimal)r["Qty_PerUnitOfMeasure"];
                    b.Cross_ReferenceType = (int)r["Cross-Reference Type"];
                    b.Cross_ReferenceTypeNo_ = r["Cross-Reference Type No_"].ToString();
                    e.ItemCrossReferences.Add(b);
                }
                e.SaveChanges();
            }
        }



        public void SyncItemCustomers(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                var items = new List<ItemCustomer>();
                foreach (DataRow r in dt.Rows)
                {
                    ItemCustomer b = new ItemCustomer();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.CustomerNo_ = r["Customer No_"].ToString();
                    b.Like = r["Like"].ToString() == "1";
                    b.VariantCode = r["Variant Code"].ToString();
                    b.IsNew = false;
                    items.Add(b);
                    //e.ItemCustomers.Add(b);
                }
                e.ItemCustomers.AddRange(items);
                e.BulkSaveChanges(b => b.BatchSize = 2000);
                // e.SaveChanges();
            }
        }

        public void SyncItemCustomers(List<DataRow> drs)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in drs)
                {
                    ItemCustomer b = new ItemCustomer();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.CustomerNo_ = r["Customer No_"].ToString();
                    b.Like = r["Like"].ToString() == "1";
                    b.VariantCode = r["Variant Code"].ToString();
                    b.IsNew = false;
                    e.ItemCustomers.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemPictures(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    ItemPicture b = new ItemPicture();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.PictureNo_ = (int)r["Picture No_"];
                    b.Description = r["Description"].ToString();
                    b.Content = GetBytesnullFromDataColumn(r["Content"]);
                    e.ItemPictures.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemUnitOfMeasures(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                var items = new List<ItemUnitOfMeasure>();
                foreach (DataRow r in dt.Rows)
                {
                    ItemUnitOfMeasure b = new ItemUnitOfMeasure();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.Code = r["Code"].ToString();

                    b.Qty_PerUnitOfMeasure = (decimal)r["Qty_ per Unit of Measure"];
                    b.Length = GetDecimalnullFromDataColumn(r["Length"]);
                    b.Width = GetDecimalnullFromDataColumn(r["Width"]);
                    b.Height = GetDecimalnullFromDataColumn(r["Height"]);
                    b.Cubage = GetDecimalnullFromDataColumn(r["Cubage"]);
                    b.Weight = GetDecimalnullFromDataColumn(r["Weight"]);
                    items.Add(b);
                    // e.ItemUnitOfMeasures.Add(b);
                }
                e.ItemUnitOfMeasures.AddRange(items);
                e.BulkSaveChanges(b => b.BatchSize = 2000);
                //e.SaveChanges();
            }
        }

        public void SyncItemUnitOfMeasures(List<DataRow> drs)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in drs)
                {
                    ItemUnitOfMeasure b = new ItemUnitOfMeasure();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.Code = r["Code"].ToString();

                    b.Qty_PerUnitOfMeasure = (decimal)r["Qty_ per Unit of Measure"];
                    b.Length = GetDecimalnullFromDataColumn(r["Length"]);
                    b.Width = GetDecimalnullFromDataColumn(r["Width"]);
                    b.Height = GetDecimalnullFromDataColumn(r["Height"]);
                    b.Cubage = GetDecimalnullFromDataColumn(r["Cubage"]);
                    b.Weight = GetDecimalnullFromDataColumn(r["Weight"]);
                    e.ItemUnitOfMeasures.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemSalesPrices(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                var items = new List<SalesPrice>();
                var sales = new List<SalesContainer>();



                foreach (DataRow r in dt.Rows)
                {

                    SalesPrice b = new SalesPrice();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.SalesType = (int)r["Sales Type"];
                    b.SalesCode = r["Sales Code"].ToString();
                    b.StartingDate = (DateTime)r["Starting Date"];
                    b.CurrencyCode = r["Currency Code"].ToString();
                    b.VariantCode = r["Variant Code"].ToString();
                    b.UnitOfMeasureCode = r["Unit of Measure Code"].ToString();
                    b.MinimumQuantity = (decimal)r["Minimum Quantity"];
                    b.UnitPrice = (decimal)r["Unit Price"];
                    b.PriceIncludingVAT = r["Price Includes VAT"].ToString() == "1";
                    b.AllowInvoiceDisc = r["Allow Invoice Disc_"].ToString() == "1";
                    b.VATBus_PostingGr__Price_ = r["VAT Bus_ Posting Gr_ (Price)"].ToString();
                    b.EndingDate = (DateTime)r["Ending Date"];
                    b.AllowLineDisc_ = r["Allow Line Disc_"].ToString() == "1";
                    items.Add(b);
                    // e.SalesPrices.Add(b);
                }


                e.SalesPrices.AddRange(items);
                e.BulkSaveChanges(b => b.BatchSize = 2000);
                //e.SaveChanges();
            }
        }

        public void SyncItemSalesPrices(List<DataRow> drs)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in drs)
                {
                    SalesPrice b = new SalesPrice();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.SalesType = (int)r["Sales Type"];
                    b.SalesCode = r["Sales Code"].ToString();
                    b.StartingDate = (DateTime)r["Starting Date"];
                    b.CurrencyCode = r["Currency Code"].ToString();
                    b.VariantCode = r["Variant Code"].ToString();
                    b.UnitOfMeasureCode = r["Unit of Measure Code"].ToString();
                    b.MinimumQuantity = (decimal)r["Minimum Quantity"];
                    b.UnitPrice = (decimal)r["Unit Price"];
                    b.PriceIncludingVAT = r["Price Includes VAT"].ToString() == "1";
                    b.AllowInvoiceDisc = r["Allow Invoice Disc_"].ToString() == "1";
                    b.VATBus_PostingGr__Price_ = r["VAT Bus_ Posting Gr_ (Price)"].ToString();
                    b.EndingDate = (DateTime)r["Ending Date"];
                    b.AllowLineDisc_ = r["Allow Line Disc_"].ToString() == "1";

                    e.SalesPrices.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemVehicleModels(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                var items = new List<ItemVehicleModel>();
                foreach (DataRow r in dt.Rows)
                {
                    ItemVehicleModel b = new ItemVehicleModel();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.ManufacturerCode = r["Manufacturer Code"].ToString();

                    b.ModelNo_ = r["Model No_"].ToString();
                    b.CabType = r["Cab Type"].ToString();
                    b.Engine = r["Engine"].ToString();
                    b.ManufacturingStartDate = (DateTime)r["Manufacturing Start Date"];
                    b.OutputPH = r["Output_PH"].ToString();

                    b.ItemDescription = r["Item Description"].ToString();
                    b.ManufacturerName = r["Manufacturer Name"].ToString();
                    b.ModelDescription = r["Model Description"].ToString();
                    b.ManufacturingEndDate = (DateTime)r["Manufacturing End Date"];
                    //TT
                    b.TecDoc_ID = r["TecDoc ID"].ToString();
                    //End TT
                    items.Add(b);
                    // e.ItemVehicleModels.Add(b);
                }
                var index = 0;
                var bitems = new List<ItemVehicleModel>();
                foreach (var item in items)
                {
                    bitems.Add(item);
                    index++;
                    if (index >= 20000)
                    {
                        e.ItemVehicleModels.AddRange(bitems);
                        e.BulkSaveChanges(b => b.BatchSize = 5000);
                        index = 0;
                        bitems.Clear();
                    }
                }
                if (bitems.Count > 0)
                {
                    e.ItemVehicleModels.AddRange(bitems);
                    e.BulkSaveChanges(b => b.BatchSize = 5000);
                }
            }
        }

        public void SyncItemVehicleModels(List<DataRow> drs)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in drs)
                {
                    ItemVehicleModel b = new ItemVehicleModel();
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.ManufacturerCode = r["Manufacturer Code"].ToString();

                    b.ModelNo_ = r["Model No_"].ToString();
                    b.CabType = r["Cab Type"].ToString();
                    b.Engine = r["Engine"].ToString();
                    b.ManufacturingStartDate = (DateTime)r["Manufacturing Start Date"];
                    b.OutputPH = r["Output_PH"].ToString();

                    b.ItemDescription = r["Item Description"].ToString();
                    b.ManufacturerName = r["Manufacturer Name"].ToString();
                    b.ModelDescription = r["Model Description"].ToString();
                    b.ManufacturingEndDate = (DateTime)r["Manufacturing End Date"];
                    e.ItemVehicleModels.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemItems(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                var items = new List<ItemItem>();
                foreach (DataRow r in dt.Rows)
                {
                    ItemItem b = new ItemItem();
                    b.BaseItemNo_ = r["Base Item No_"].ToString();
                    b.RelatedItemNo_ = r["Related Item No_"].ToString();
                    b.Quantity = (decimal)r["Quantity"];
                    b.BaseUnitOfMeasure = r["Base Unit of Measure"].ToString();
                    b.RelatedUnitOfMeasure = r["Related Unit of Measure"].ToString();
                    items.Add(b);
                    //e.ItemItems.Add(b);
                }
                e.ItemItems.AddRange(items);
                e.BulkSaveChanges(b => b.BatchSize = 10000);
                // e.SaveChanges();
            }
        }

        public void SyncItemItems(List<DataRow> drs)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in drs)
                {
                    ItemItem b = new ItemItem();
                    b.BaseItemNo_ = r["Base Item No_"].ToString();
                    b.RelatedItemNo_ = r["Related Item No_"].ToString();
                    b.Quantity = (decimal)r["Quantity"];
                    b.BaseUnitOfMeasure = r["Base Unit of Measure"].ToString();
                    b.RelatedUnitOfMeasure = r["Related Unit of Measure"].ToString();

                    e.ItemItems.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemComments(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    Comment_Line b = new Comment_Line();
                    b.Table_Name = 27;
                    b.No_ = r["No_"].ToString();
                    b.Line_No_ = int.Parse(r["Line No_"].ToString());
                    b.Date = (DateTime)r["Date"];
                    b.Code = r["Code"].ToString();
                    b.Comment = r["Comment"].ToString();
                    b.IsNew = false;
                    b.IsActive = true;
                    e.Comment_Line.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemComments(List<DataRow> drs)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in drs)
                {
                    Comment_Line b = new Comment_Line();
                    b.Table_Name = 27;
                    b.No_ = r["No_"].ToString();
                    b.Line_No_ = int.Parse(r["Line No_"].ToString());
                    b.Date = (DateTime)r["Date"];
                    b.Code = r["Code"].ToString();
                    b.Comment = r["Comment"].ToString();
                    b.IsNew = false;
                    b.IsActive = true;
                    e.Comment_Line.Add(b);
                }
                e.SaveChanges();
            }
        }


        public void SyncProjectedItemReceipts(DataTable dt, DataTable dt1)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.ProjectedItemReceipt");
                foreach (DataRow r in dt.Rows)
                {
                    ProjectedItemReceipt b = new ProjectedItemReceipt();
                    b.ReceiptDate = (DateTime)r["Expected Receipt Date"];
                    b.ItemNo_ = r["No_"].ToString();
                    b.Quantity = (decimal)r["ItemQuantity"];
                    e.ProjectedItemReceipts.Add(b);
                }
                e.SaveChanges();
                foreach (DataRow r in dt1.Rows)
                {
                    ProjectedItemReceipt b = new ProjectedItemReceipt();
                    b.ReceiptDate = (DateTime)r["Posting Date"];
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.Quantity = (decimal)r["ItemQuantity"];

                    var ukvedam =
                        e.ProjectedItemReceipts.FirstOrDefault(
                            i => i.ItemNo_ == b.ItemNo_ && i.ReceiptDate == b.ReceiptDate);
                    if (ukvedam != null)
                        ukvedam.Quantity = ukvedam.Quantity + b.Quantity;
                    else
                        e.ProjectedItemReceipts.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncProjectedItemReceipts(List<DataRow> drs1, List<DataRow> drs2)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in drs1)
                {
                    ProjectedItemReceipt b = new ProjectedItemReceipt();
                    b.ReceiptDate = (DateTime)r["Expected Receipt Date"];
                    b.ItemNo_ = r["No_"].ToString();
                    b.Quantity = (decimal)r["ItemQuantity"];
                    e.ProjectedItemReceipts.Add(b);
                }
                foreach (DataRow r in drs2)
                {
                    ProjectedItemReceipt b = new ProjectedItemReceipt();
                    b.ReceiptDate = (DateTime)r["Posting Date"];
                    b.ItemNo_ = r["Item No_"].ToString();
                    b.Quantity = (decimal)r["ItemQuantity"];

                    var ukvedam =
                        e.ProjectedItemReceipts.FirstOrDefault(
                            i => i.ItemNo_ == b.ItemNo_ && i.ReceiptDate == b.ReceiptDate);
                    if (ukvedam != null)
                        ukvedam.Quantity = ukvedam.Quantity + b.Quantity;
                    else
                        e.ProjectedItemReceipts.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncCustomers(DataRow r, decimal balanceAmount, decimal salesBudgetAmount, decimal salesActualAmount, decimal recommendedSalesAmount, int recommendedVisitsMonth, string visitWeekDays)
        {
            using (var e = new POSWR1Entities())
            {
                Customer b = new Customer();
                b.No_ = r["No_"].ToString();
                b.Name = r["Name"].ToString();
                b.Address = r["Address"].ToString();

                b.City = r["City"].ToString();
                b.Contact = r["Contact"].ToString();
                b.PhoneNo_ = r["Phone No_"].ToString();

                b.CreditLimit_LCY_ = GetDecimalnullFromDataColumn(r["Credit Limit (LCY)"]);
                b.CustomerPriceGroup = r["Customer Price Group"].ToString();
                b.PaymentTermsCode = r["Payment Terms Code"].ToString();

                //b.co = r["Salesperson Code"].ToString();
                //b.ship = r["Shipping Agent Code"].ToString();
                b.InvoiceDisc_Group = r["Customer Disc_ Group"].ToString();
                //b.custom = r["Bill-to Customer No_"].ToString();
                b.PaymentMethodCode = r["Payment Method Code"].ToString();
                b.PriceIncludingVAT = r["Prices Including VAT"].ToString() == "1";

                b.VATRegistrationNo_ = r["VAT Registration No_"].ToString();
                b.VisitWeekDays = string.IsNullOrEmpty(visitWeekDays) ? r["Visit Week Days"].ToString() : visitWeekDays;
                b.Balance = balanceAmount;
                b.SalesBudgetAmount = salesBudgetAmount;
                b.SalesActualAmount = salesActualAmount;
                b.RecommendedSalesAmount = recommendedSalesAmount;
                b.RecommendedVisitsMonth = recommendedVisitsMonth;
                e.Customers.Add(b);
                e.SaveChanges();
            }
        }

        public void SyncShipToAddress(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    var cn = r["Customer No_"].ToString();
                    var cd = r["Code"].ToString();
                    var c = e.Ship_to_Address.FirstOrDefault(i => i.CustomerNo_ == cn && i.Code == cd);
                    if (c != null) continue;

                    Ship_to_Address b = new Ship_to_Address();
                    b.CustomerNo_ = r["Customer No_"].ToString();
                    b.Code = r["Code"].ToString();

                    b.Name = r["Name"].ToString();
                    b.Name_2 = r["Name 2"].ToString();
                    b.Address = r["Address"].ToString();
                    b.Address_2 = r["Address 2"].ToString();
                    b.City = r["City"].ToString();
                    b.Contact = r["Contact"].ToString();
                    b.PhoneNo_ = r["Phone No_"].ToString();
                    b.ShippingAgentCode = r["Shipping Agent Code"].ToString();
                    e.Ship_to_Address.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncPaymentSchedule(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.PaymentSchedule");
                foreach (DataRow r in dt.Rows)
                {
                    PaymentSchedule b = new PaymentSchedule();
                    b.EntryNo = (int)r["Entry No_"];
                    b.EntryType = (int)r["Entry Type"];

                    b.CustomerNo = r["Second Party No_"].ToString();
                    b.DocumentNo = r["Document No_"].ToString();
                    b.Date = (DateTime)r["Date"];
                    b.Amount = (decimal)r["Amount"];
                    b.RemeiningAmount = (decimal)r["Remaining Amount"];
                    e.PaymentSchedules.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncVehicleGroup(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    e.Database.ExecuteSqlCommand("delete from dbo.VehicleGroup");
                    VehicleGroup b = new VehicleGroup();
                    b.No = r["No_"].ToString();
                    b.Description = r["Description"].ToString();
                    e.VehicleGroups.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncItemVehicleGroup(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.ItemVehicleGroup");
                foreach (DataRow r in dt.Rows)
                {
                    ItemVehicleGroup b = new ItemVehicleGroup();
                    b.ItemNo = r["Item No_"].ToString();
                    b.VehicleGroupNo = r["Vehicle Group No_"].ToString();
                    b.ItemDescription = r["Item Description"].ToString();
                    b.VehicleGroupDescription = r["Vehicle Group Description"].ToString();
                    e.ItemVehicleGroups.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncReleasedOrders(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    ReleasedSalesHeader b = new ReleasedSalesHeader();
                    b.No_ = r["No_"].ToString();
                    b.DocumentType = (int)r["Document Type"];
                    b.DueDate = (DateTime)r["Due Date"];
                    b.PostingDate = (DateTime)r["Posting Date"];
                    b.ShipmentDate = (DateTime)r["Shipment Date"];

                    b.Sell_toCustomerNo = r["Sell-to Customer No_"].ToString();
                    b.Sell_toCustomerName = r["Sell-to Customer Name"].ToString();
                    b.Sell_toAddress = r["Sell-to Address"].ToString();
                    b.Sell_toCity = r["Sell-to City"].ToString();
                    b.Sell_toPostCode = r["Sell-to Post Code"].ToString();
                    b.Sell_toCountry = r["Sell-to Country_Region Code"].ToString();
                    b.Ship_toAddress = r["Ship-to Address"].ToString();
                    b.Ship_toAddressCode = r["Ship-to Code"].ToString();
                    b.Ship_toCity = r["Ship-to City"].ToString();
                    b.Ship_toCountry = r["Ship-to Country_Region Code"].ToString();
                    b.Status = (int)r["Status"];
                    b.PaymentTermsCode = r["Payment Terms Code"].ToString();
                    b.PaymentMethodCode = r["Payment Method Code"].ToString();
                    b.ShipmentMethodCode = r["Shipment Method Code"].ToString();
                    b.CurrencyCode = r["Currency Code"].ToString();
                    b.CurrencyFactor = (decimal)r["Currency Factor"];
                    b.CustomerPriceGroup = r["Customer Price Group"].ToString();
                    b.PricesIncludingVat = (bool)(r["Prices Including VAT"].ToString() == "1");
                    b.InvoiceDiscountCalc = (int)r["Invoice Discount Calculation"];
                    b.InvoiceDiscountAmount = (decimal)r["Invoice Discount Value"];
                    b.SalespersonCode = r["Salesperson Code"].ToString();
                    b.VATRegistrationNo_ = r["VAT Registration No_"].ToString();
                    b.SalespersonCode = r["External Document No_"].ToString();
                    b.SalespersonCode = r["Shipping Agent Code"].ToString();
                    b.SalespersonCode = r["Responsibility Center"].ToString();
                    b.SalespersonCode = r["Posting Description"].ToString();
                    //Get released Order Amounts
                    string res = "";
                    DataTable dta = null;
                    DataTable dtaiv = null;
                    DataTable dtLines = null;
                    decimal amount = 0;
                    decimal amountIncVat = 0;
                    NavDbController.Current.getReleasedOrderAmount(b.No_, ref res, ref dta);
                    if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                    if ((dta.Rows.Count > 0) & (dta != null))
                        amount = (decimal)dta.Rows[0]["Amount"];
                    res = "";
                    NavDbController.Current.getReleasedOrderAmountIncVat(b.No_, ref res, ref dtaiv);
                    if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                    if ((dtaiv.Rows.Count > 0) & (dtaiv != null))
                        amountIncVat = (decimal)dtaiv.Rows[0]["AmountIncVAT"];
                    b.Amount = amount;
                    b.AmountIncludingVat = amountIncVat;
                    //Get and sync sales lines
                    res = "";
                    NavDbController.Current.getReleasedOrderLines(b.No_, ref res, ref dtLines);
                    if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                    DaoController.Current.SyncReleasedOrderLines(dtLines);
                    //Get Journal Lines
                    NavDbController.Current.getReleasedOrderPayments(b.No_, ref res, ref dtLines);
                    if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                    DaoController.Current.SyncReleasedOrderPayments(dtLines);
                    //Get Payment Schedule
                    NavDbController.Current.getReleasedOrderPaymentSchedule(b.No_, ref res, ref dtLines);
                    if (!string.IsNullOrEmpty(res)) throw new PosException(res);
                    DaoController.Current.SyncReleasedOrderPaymentSchedule(dtLines);
                    //Add order to entity
                    e.ReleasedSalesHeaders.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncReleasedOrderLines(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    ReleasedSalesLine b = new ReleasedSalesLine();
                    b.DocumentNo_ = r["Document No_"].ToString();
                    b.DocumentType = (int)r["Document Type"];
                    b.LineNo_ = (int)r["Line No_"];
                    b.Type = (int)r["Type"];

                    b.No_ = r["No_"].ToString();
                    b.Description = r["Description"].ToString();
                    b.LargeDescription = r["Large Description"].ToString();
                    b.Quantity = (decimal)r["Quantity"];
                    b.UnitPrice = (decimal)r["Unit Price"];
                    b.Sell_toCustomerNo = r["Sell-to Customer No_"].ToString();
                    b.LocationCode = r["Location Code"].ToString();
                    b.Inv_DiscountAmount = (decimal)r["Inv_ Discount Amount"];
                    b.LineDiscountPercent = (decimal)r["Line Discount _"];
                    b.LineDiscountAmount = (decimal)r["Line Discount Amount"];
                    b.Amount = (decimal)r["Amount"];
                    b.AmountIncludingVAT = (decimal)r["Amount Including VAT"];
                    b.GrossWeight = (decimal)r["Gross Weight"];
                    b.NetWeight = (decimal)r["Net Weight"];
                    b.UnitVolume = (decimal)r["Unit Volume"];
                    b.UnitOfMeasureCode = r["Unit of Measure Code"].ToString();
                    b.QuantityBase = (decimal)r["Quantity (Base)"];

                    b.ResponsibilityCenter = r["Responsibility Center"].ToString();
                    b.ItemCategoryCode = r["Item Category Code"].ToString();
                    b.ProdSubGroupCode = r["Product Group Code"].ToString();
                    e.ReleasedSalesLines.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncReleasedOrderPayments(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    ReleasedGenJournalLine b = new ReleasedGenJournalLine();
                    b.TemplateName = r["Journal Template Name"].ToString();
                    b.BatchName = r["Journal Batch Name"].ToString();
                    b.LineNo_ = (int)r["Line No_"];
                    b.PostingDate = (DateTime)r["Posting Date"];
                    b.AccountType = (int)r["Account Type"];
                    b.AccountNo_ = r["Account No_"].ToString();
                    b.DocumentNo = r["Document No_"].ToString();
                    b.Description = r["Description"].ToString();
                    b.Bal_AccountType = (int)r["Bal_ Account Type"];
                    b.Bal_AccountNo_ = r["Bal_ Account No_"].ToString();
                    b.Amount = (decimal)r["Amount"];

                    b.ResponsibilityCenter = r["Responsibility Center"].ToString();
                    b.Salespers__Purch_Code = r["Salespers__Purch_ Code"].ToString();
                    b.PaymentMethodCode = r["Payment Method Code"].ToString();
                    e.ReleasedGenJournalLines.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncReleasedOrderPaymentSchedule(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (DataRow r in dt.Rows)
                {
                    ReleasedPaymentSchedule b = new ReleasedPaymentSchedule();
                    b.EntryNo = (int)r["Entry No_"];
                    b.EntryType = (int)r["Entry Type"];
                    b.DocumentNo = r["Document No_"].ToString();
                    b.Date = (DateTime)r["Date"];
                    b.Amount = (decimal)r["Amount"];
                    b.RemeiningAmount = (decimal)r["Remaining Amount"];

                    e.ReleasedPaymentSchedules.Add(b);
                }
                e.SaveChanges();
            }
        }

        public void SyncPresalersBudges(DataRow r, decimal BudgetAmount, decimal SalesActualAmount)
        {
            using (var e = new POSWR1Entities())
            {
                RegionBudgetActual b = new RegionBudgetActual();
                b.ActualAmount = SalesActualAmount;
                b.BudgetAmount = BudgetAmount;
                b.RegionName = r["RegionName"].ToString();
                b.RegionCode = r["RegionCode"].ToString();
                e.RegionBudgetActuals.Add(b);
                e.SaveChanges();
            }
        }


        public void AddNewItemLedgeEntry(string item_no, decimal quantity, string locationCode, string unitOfMeasure)
        {
            using (var e = new POSWR1Entities())
            {
                ItemLedgerEntry u = new ItemLedgerEntry
                {
                    EntryType = 0,
                    ItemNo = item_no,
                    Quantity = quantity,
                    PostingDate = DateTime.Now,
                    LocationCode = locationCode,
                    Qty_PerUnitOfMeasure = 1,
                    UnitOfMeasureCode = unitOfMeasure
                };
                e.ItemLedgerEntries.Add(u);
                e.SaveChanges();
            }
        }

        public void AddNewItemLedgeEntries(List<Tuple<string, decimal, string, string>> iList)
        {
            using (var e = new POSWR1Entities())
            {
                foreach (var i in iList)
                {
                    ItemLedgerEntry u = new ItemLedgerEntry
                    {
                        EntryType = 0,
                        ItemNo = i.Item1,
                        Quantity = i.Item2,
                        PostingDate = DateTime.Now,
                        LocationCode = i.Item3,
                        Qty_PerUnitOfMeasure = 1,
                        UnitOfMeasureCode = i.Item4
                    };
                    e.ItemLedgerEntries.Add(u);
                }
                e.SaveChanges();
            }
        }


        public List<string> GetItemsNotInItems()
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.ItemLedgerEntries.Where(i => e.Items.All(j => j.No_ != i.ItemNo)).Select(i => i.ItemNo).ToList();
            }
        }


        public void ClearItems()
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.ItemLedgerEntry");
                e.Database.ExecuteSqlCommand("delete from dbo.Item");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemCrossReference");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemCustomer");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemPicture");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemUnitOfMeasure");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemVehicleModel");
                e.Database.ExecuteSqlCommand("delete from dbo.SalesPrice");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemItem");
                e.Database.ExecuteSqlCommand("delete from dbo.ProjectedItemReceipt");
                e.SaveChanges();
            }
        }

        public void ClearSalesPrices()
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.SalesPrice");
                e.SaveChanges();
            }
        }

        public void ClearItemLedgerEntries()
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ItemLedgerEntry");
                e.SaveChanges();
            }
        }


        public void ClearAllPictures()
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ItemPicture");
                e.SaveChanges();
            }
        }

        public void ClearReleasedOrders()
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.ReleasedSalesHeader");
                e.Database.ExecuteSqlCommand("delete from dbo.ReleasedSalesLine");
                e.Database.ExecuteSqlCommand("delete from dbo.ReleasedGenJournalLine");
                e.Database.ExecuteSqlCommand("delete from dbo.ReleasedPaymentSchedule");
                e.SaveChanges();
            }
        }

        public void ClearItems(string item_no, bool withPicture)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.Item where [No_] = '" + item_no + "' ");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemCrossReference where [ItemNo_] = '" + item_no + "' ");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemCustomer where [ItemNo_] = '" + item_no + "' ");
                if (withPicture)
                    e.Database.ExecuteSqlCommand("delete from dbo.ItemPicture where [ItemNo_] = '" + item_no + "' ");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemUnitOfMeasure where [ItemNo_] = '" + item_no + "' ");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemVehicleModel where [ItemNo_] = '" + item_no + "' ");
                e.Database.ExecuteSqlCommand("delete from dbo.SalesPrice where [ItemNo_] = '" + item_no + "' ");
                e.Database.ExecuteSqlCommand("delete from dbo.ItemItem where [BaseItemNo_] = '" + item_no + "' ");
                //e.Database.ExecuteSqlCommand("delete from dbo.ProjectedItemReceipt where [ItemNo_] = '" + item_no + "' ");
                e.Database.ExecuteSqlCommand("delete from dbo.Comment_Line where No_ ='" + item_no + "' ");
                e.SaveChanges();
            }
        }

        public void ClearItemsOnly(string item_no)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.Item where [No_] = '" + item_no + "' ");
                e.SaveChanges();
            }
        }

        public void DeleteItems() // gio
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.Item");
                e.SaveChanges();
            }
        }

        public void ClearAllItems(bool withPictures)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.Item");
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ItemCrossReference");
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ItemCustomer");
                if (withPictures)
                    e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ItemPicture");
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ItemUnitOfMeasure");
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ItemVehicleModel");
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.SalesPrice");
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ItemItem");
                //e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.ProjectedItemReceipt");
                e.Database.ExecuteSqlCommand("delete from dbo.Comment_Line");
                e.SaveChanges();
            }
        }

        public void ClearAllItemsShort()
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("TRUNCATE TABLE dbo.Item");
                e.SaveChanges();
            }
        }

        public void ClearCustomers()
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.Customer");
                e.Database.ExecuteSqlCommand("DELETE FROM [dbo].[Ship-to-Address]");
                e.Database.ExecuteSqlCommand("DELETE FROM [dbo].RegionBudgetActual");
                e.Database.ExecuteSqlCommand("delete from dbo.PaymentSchedule");
                e.SaveChanges();
            }
        }

        #endregion

        public List<Customer> GetCustomers()
        {
            using (var e = new POSWR1Entities())
            {
                var cdate = DateTime.Today;
                var list = e.Customers.Select(i => new
                {
                    No_ = i.No_,
                    Name = i.Name,
                    PaymentMethodCode = i.PaymentMethodCode,
                    Address = i.Address,
                    Balance = i.Balance,
                    City = i.City,
                    Contact = i.Contact,
                    Country_RegionCode = i.Country_RegionCode,
                    CreditLimit_LCY_ = i.CreditLimit_LCY_,
                    CustomerPriceGroup = i.CustomerPriceGroup,
                    E_Mail = i.E_Mail,
                    FullName = i.FullName,
                    InvoiceDisc_Group = i.InvoiceDisc_Group,
                    IsNewCustomer = i.IsNewCustomer,
                    ModifiedDate = i.ModifiedDate,
                    ModifiedUserID = i.ModifiedUserID,
                    NeedsVATInvoice = i.NeedsVATInvoice,
                    PaymentTermsCode = i.PaymentTermsCode,
                    PhoneNo_ = i.PhoneNo_,
                    PostCode = i.PostCode,
                    PriceIncludingVAT = i.PriceIncludingVAT,
                    RecommendedSalesAmount = i.RecommendedSalesAmount,
                    RecommendedVisitsMonth = i.RecommendedVisitsMonth,
                    SalesActualAmount = i.SalesActualAmount,
                    SalesBudgetAmount = i.SalesBudgetAmount,
                    ShipToAddress = i.ShipToAddress,
                    Type = i.Type,
                    VATRegistrationNo_ = i.VATRegistrationNo_,
                    VendorNo_ = i.VendorNo_,
                    VisitWeekDays = i.VisitWeekDays,
                    i.AreaCode,
                    i.Mobile_,
                    PaymentSchedule =
                       e.PaymentSchedules.Where(j => j.Date == cdate && j.CustomerNo == i.No_).Sum(j => j.Amount),
                    Addres1 = e.Ship_to_Address.Where(j => j.CustomerNo_ == i.No_).Min(j => j.Address),
                    City1 = e.Ship_to_Address.Where(j => j.CustomerNo_ == i.No_).Min(j => j.City)
                }).ToList();
                return list.Select(i => new Customer
                {
                    No_ = i.No_,
                    Name = i.Name,
                    PaymentMethodCode = i.PaymentMethodCode,
                    Address = i.Address,
                    Balance = i.Balance,
                    City = i.City,
                    Contact = i.Contact,
                    Country_RegionCode = i.Country_RegionCode,
                    CreditLimit_LCY_ = i.CreditLimit_LCY_,
                    CustomerPriceGroup = i.CustomerPriceGroup,
                    E_Mail = i.E_Mail,
                    FullName = i.FullName,
                    InvoiceDisc_Group = i.InvoiceDisc_Group,
                    IsNewCustomer = i.IsNewCustomer,
                    ModifiedDate = i.ModifiedDate,
                    ModifiedUserID = i.ModifiedUserID,
                    NeedsVATInvoice = i.NeedsVATInvoice,
                    PaymentTermsCode = i.PaymentTermsCode,
                    PhoneNo_ = i.PhoneNo_,
                    PostCode = i.PostCode,
                    PriceIncludingVAT = i.PriceIncludingVAT,
                    RecommendedSalesAmount = i.RecommendedSalesAmount,
                    RecommendedVisitsMonth = i.RecommendedVisitsMonth,
                    SalesActualAmount = i.SalesActualAmount,
                    SalesBudgetAmount = i.SalesBudgetAmount,
                    ShipToAddress = i.ShipToAddress,
                    Type = i.Type,
                    VATRegistrationNo_ = i.VATRegistrationNo_,
                    VendorNo_ = i.VendorNo_,
                    VisitWeekDays = i.VisitWeekDays,
                    PaymentSchedule = i.PaymentSchedule,
                    Address1 = !string.IsNullOrEmpty(i.Addres1) ? i.Addres1 : i.Address,
                    City1 = !string.IsNullOrEmpty(i.City1) ? i.City1 : i.City,
                    AreaCode = i.AreaCode,
                    Mobile_= i.Mobile_
                }).ToList();
            }
        }

        public Customer GetCustomer(string code)
        {
            using (var e = new POSWR1Entities())
            {
                return e.Customers.FirstOrDefault(i => i.No_ == code);
            }
        }


        public List<GenJournalLine> GetGeneralGenJournalLines()
        {
            using (var e = new POSWR1Entities())
            {
                return e.GenJournalLines.Where(i => i.IsGeneral == true).ToList();
            }
        }




        public List<Customer> GetNewCustomers()
        {
            using (var e = new POSWR1Entities())
            {
                return e.Customers.Where(i => i.IsNewCustomer == true).ToList();
            }
        }

        public List<ItemCustomer> GetNewLikes()
        {
            using (var e = new POSWR1Entities())
            {
                return e.ItemCustomers.Where(i => i.IsNew).ToList();
            }
        }

        public List<Comment_Line> GetNewComments()
        {
            using (var e = new POSWR1Entities())
            {
                return e.Comment_Line.Where(i => i.IsNew == true && i.IsActive == true).ToList();
            }
        }

        public void UpdateNewCommentStatuses()
        {
            using (var e = new POSWR1Entities())
            {
                e.Comment_Line.RemoveRange(e.Comment_Line.Where(i => i.IsNew && !i.IsActive));
                foreach (var c in e.Comment_Line.Where(i => i.IsNew))
                {
                    c.IsNew = false;
                }
                e.SaveChanges();
            }
        }


        public List<Dimension_Value> GetDimensionValue()
        {
            List<Dimension_Value> res;
            using (var e = new POSWR1Entities())
            {
                res = e.Dimension_Values.OrderBy(i => i.Code).ToList();
            }
            return res;
        }

        public List<ItemsViewProcedure_Result> GetItems(string customerCode, string customerGroup, string locationCode)
        {
            using (var e = new POSWR1Entities())
            {
                return e.ItemsViewProcedure(customerCode, customerGroup, locationCode).ToList();
            }
        }

        public List<Salesperson_Purchaser> GetSalesPersons()
        {
            using (var e = new POSWR1Entities())
            {
                return e.Salesperson_Purchaser.ToList();
            }
        }

        public List<PosUserType> GetUserTypes()
        {
            using (var e = new POSWR1Entities())
            {
                return e.PosUserTypes.ToList();
            }
        }

        public List<Ship_to_Address> GetShipToAddresses(string customerNo)
        {
            using (var e = new POSWR1Entities())
            {
                return e.Ship_to_Address.Where(i => i.CustomerNo_ == customerNo).ToList();
            }
        }

        public List<Vendor> GetVendors()
        {
            using (var e = new POSWR1Entities())
            {
                return e.Vendors.Where(i => i.No_.StartsWith("PV")).ToList();
            }
        }



        public List<Vehicle> GetCustomerVehicles(string customerNo)
        {
            using (var e = new POSWR1Entities())
            {
                return e.Vehicles.Where(i => i.Owner_No_ == customerNo).ToList();
            }
        }



        public string GenerateNewKey(string numberCount, string salesHeaderPrefix = null)
        {
            string res = "";
            using (var e = new POSWR1Entities())
            {
                if (salesHeaderPrefix == null)
                    salesHeaderPrefix = e.Settings.First().Settings_SalesHeaderPrefix;
                long raodenoba = 0, autonum = 0;
                char d = '_';
                if (salesHeaderPrefix.Contains("_") || salesHeaderPrefix.Contains("-"))
                {
                    d = salesHeaderPrefix.Contains("_") ? '_' : '-';
                    var prefixes = salesHeaderPrefix.Split(d);
                    salesHeaderPrefix = prefixes[0];
                    if (long.TryParse(prefixes[1], out autonum) && prefixes[1].Length > 3)
                    {
                        raodenoba = prefixes[1].Length;
                    }
                }
                AutoNumber a = e.AutoNumbers.FirstOrDefault(i => i.AutoNumber_Prefix == salesHeaderPrefix);
                if (a == null)
                {
                    a = new AutoNumber { AutoNumber_Prefix = salesHeaderPrefix, AutoNumber_LastNum = 0 };
                    e.AutoNumbers.Add(a);
                }
                a.AutoNumber_LastNum = Math.Max(autonum, a.AutoNumber_LastNum.Value + 1);
                if (raodenoba < 4)
                    long.TryParse(numberCount, out raodenoba);
                if (raodenoba < 4)
                    res = salesHeaderPrefix + d + a.AutoNumber_LastNum;
                else
                {
                    res = string.Format("{0}{2}{1:D" + raodenoba + "}", salesHeaderPrefix, a.AutoNumber_LastNum, d); // salesHeaderPrefix + "_" + a.AutoNumber_LastNum;
                }
                e.SaveChanges();
            }
            return res;
        }

        public void AddSalesLine(SalesLine line)
        {
            using (var e = new POSWR1Entities())
            {
                e.SalesLines.Add(line);
                e.SaveChanges();
            }
        }

        public void ClearItemLedgerByDocumentNo(string documentNo)
        {
            using (var e = new POSWR1Entities())
            {
                e.ItemLedgerEntries.RemoveRange(e.ItemLedgerEntries.Where(i => i.DocumentNo_ == documentNo));
                e.SaveChanges();
            }
        }

        public void AddItemLedgerEntry(ItemLedgerEntry entry)
        {
            using (var e = new POSWR1Entities())
            {
                e.ItemLedgerEntries.Add(entry);
                e.SaveChanges();
            }
        }

        public void CreateOrder(SalesHeader header, List<SalesLine> lines, List<PaymentSchedule> schedules,
            List<GenJournalLine> journals, DateTime date)
        {
            using (var e = new POSWR1Entities())
            {
                var dbHedaer =
                    e.SalesHeaders.FirstOrDefault(i => i.No_ == header.No_ && i.DocumentType == header.DocumentType);

                //gio Date    
                if (dbHedaer != null)
                {

                    string newDuraction = (DateTime.Now - date).Minutes + ":" + (DateTime.Now - date).Seconds;

                    string[] orderDuractionSplit = header.OrderDuraction.Split(':');
                    string[] newDuractionSplit = newDuraction.Split(':');

                    int minutes = Int32.Parse(orderDuractionSplit[0]) + Int32.Parse(newDuractionSplit[0]);
                    int seconts = Int32.Parse(orderDuractionSplit[1]) + Int32.Parse(newDuractionSplit[1]);

                    double res = seconts / 60;

                    if (res == 0)
                    {

                        int resMinutes = minutes + (int)Math.Floor(res);

                        header.OrderClosedDate = DateTime.Now;
                        header.OrderDuraction = resMinutes + ":" + seconts;
                    }
                    else
                    {
                        string decimal_places = "0:00";

                        var regex = new System.Text.RegularExpressions.Regex("(?<=[\\.])[0-9]+");
                        if (regex.IsMatch(res.ToString()))
                        {
                            decimal_places = regex.Match(res.ToString()).Value;
                        }

                        int resMinutes = minutes + (int)Math.Floor(res);

                        header.OrderClosedDate = DateTime.Now;
                        header.OrderDuraction = resMinutes + ":" + decimal_places;
                    }


                    e.SalesHeaders.Remove(dbHedaer);
                }
                else
                {
                    header.OrderStartDate = date;
                    header.OrderClosedDate = DateTime.Now;
                    header.OrderDuraction = (DateTime.Now - date).Minutes + ":" + (DateTime.Now - date).Seconds;
                }




                e.SalesHeaders.Add(header);

                e.SalesLines.RemoveRange(e.SalesLines.Where(i => i.DocumentNo_ == header.No_));
                ClearItemLedgerByDocumentNo(header.No_);

                for (int r = 0; r < lines.Count; r++)
                {
                    var sl = lines[r];
                    if (sl.OrderType != 0) continue;
                    var lq = e.ItemLedgerEntries.Where(j => j.ItemNo == sl.No_).Sum(k => k.Quantity);
                    if (lq == null) lq = 0;
                    if (sl.Quantity > lq)
                    {
                        var maxLine = lines.Max(i => i.LineNo_) + 1;
                        lines.Add(new SalesLine
                        {
                            DocumentNo_ = header.No_,
                            DocumentType = 1,
                            LineNo_ = maxLine,
                            Type = 2,
                            No_ = sl.No_,
                            Description = sl.Description,
                            Quantity = sl.Quantity - lq.Value,
                            UnitPrice = sl.UnitPrice,
                            Sell_toCustomerNo = header.Sell_toCustomerNo,
                            LocationCode = sl.LocationCode,
                            AmountIncludingVAT = 0,
                            UnitOfMeasureCode = sl.UnitOfMeasureCode,
                            OrderType = 1,
                            LineDiscountAmount = 0,
                            LineDiscountPercent = 0,
                            Service_Provider = sl.Service_Provider,
                            Customer_Vehicle = sl.Customer_Vehicle
                        });
                        sl.Quantity = lq.Value;
                        if (sl.Quantity > 0)
                        {
                            AddItemLedgerEntry(new ItemLedgerEntry()
                            {
                                EntryType = 0,
                                ItemNo = sl.No_,
                                Quantity = -sl.Quantity,
                                PostingDate = DateTime.Now,
                                LocationCode = sl.LocationCode,
                                Qty_PerUnitOfMeasure = 1,
                                UnitOfMeasureCode = sl.UnitOfMeasureCode,
                                DocumentType = sl.DocumentType,
                                DocumentNo_ = sl.DocumentNo_
                            });
                        }
                    }
                    else
                    {
                        AddItemLedgerEntry(new ItemLedgerEntry()
                        {
                            EntryType = 0,
                            ItemNo = sl.No_,
                            Quantity = -sl.Quantity,
                            PostingDate = DateTime.Now,
                            LocationCode = sl.LocationCode,
                            Qty_PerUnitOfMeasure = 1,
                            UnitOfMeasureCode = sl.UnitOfMeasureCode,
                            DocumentType = sl.DocumentType,
                            DocumentNo_ = sl.DocumentNo_
                        });
                    }
                }


                e.SalesLines.AddRange(lines.Where(i => i.Quantity > 0));



                e.PaymentSchedules.RemoveRange(e.PaymentSchedules.Where(i => i.DocumentNo == header.No_));
                e.PaymentSchedules.AddRange(schedules);



                e.GenJournalLines.RemoveRange(e.GenJournalLines.Where(i => i.DocumentNo == header.No_));
                int mt = 0;
                foreach (var g in journals)
                {
                    g.LineNo_ = ++mt;
                }
                e.GenJournalLines.AddRange(journals);

                e.SaveChanges();
            }
        }

        public void CreateGeneralGurnalLine(GenJournalLine journal)
        {
            using (var e = new POSWR1Entities())
            {

                int? mt = e.GenJournalLines.Where(i => i.BatchName == journal.BatchName).Max(i => (int?)i.LineNo_);
                if (mt == null) journal.LineNo_ = 1;
                else
                    journal.LineNo_ = mt.Value + 1;
                e.GenJournalLines.Add(journal);
                e.SaveChanges();
            }
        }

        public void CreateReleaseOrder(ReleasedSalesHeader header, List<ReleasedSalesLine> lines, List<ReleasedPaymentSchedule> schedules,
            List<ReleasedGenJournalLine> genjournal
            )
        {
            using (var e = new POSWR1Entities())
            {
                var dbHedaer =
                    e.ReleasedSalesHeaders.FirstOrDefault(i => i.No_ == header.No_ && i.DocumentType == header.DocumentType);
                if (dbHedaer != null)
                    e.ReleasedSalesHeaders.Remove(dbHedaer);
                e.ReleasedSalesHeaders.Add(header);
                e.ReleasedSalesLines.RemoveRange(e.ReleasedSalesLines.Where(i => i.DocumentNo_ == header.No_));
                e.ReleasedSalesLines.AddRange(lines);
                e.ReleasedPaymentSchedules.RemoveRange(e.ReleasedPaymentSchedules.Where(i => i.DocumentNo == header.No_));
                if (schedules != null)
                    e.ReleasedPaymentSchedules.AddRange(schedules);
                e.ReleasedGenJournalLines.RemoveRange(e.ReleasedGenJournalLines.Where(i => i.DocumentNo == header.No_));
                if (genjournal != null)
                    e.ReleasedGenJournalLines.AddRange(genjournal);

                e.SaveChanges();
            }
        }

        public List<OrderShortEntry> GetOrdersList(OrderBaseTypes orderType)
        {
            using (var e = new POSWR1Entities())
            {
                return (e.Set(SalesHeaderBases[orderType]) as IEnumerable<ISalesHeader>).Select(i => new OrderShortEntry
                {
                    No_ = i.No_,
                    OrderType = orderType,
                    Sell_toCustomerNo = i.Sell_toCustomerNo,
                    PostingDate = i.PostingDate,
                    Sell_toCustomerName = i.Sell_toCustomerName,
                    AmountIncludingVat = i.AmountIncludingVat,
                    DocumentType = i.DocumentType
                }).OrderByDescending(i => i.PostingDate).ToList();
            }
        }

        public List<VehicleModel> GetVehicleModelsList()
        {
            using (var e = new POSWR1Entities())
            {
                return e.VehicleModels.ToList();
            }
        }

        public List<OrderShortEntry> GetOrdersListByFilter(OrderBaseTypes orderType, string no, string code, string name, DateTime? from, DateTime? to)
        {
            if (string.IsNullOrEmpty(no)) no = null;
            else no = no.ToUpper();
            if (string.IsNullOrEmpty(code)) code = null;
            else code = code.ToUpper();
            if (string.IsNullOrEmpty(name)) name = null;
            if (to.HasValue)
                to = to.Value.AddDays(1).AddSeconds(-1);
            using (var e = new POSWR1Entities())
            {
                if (orderType != OrderBaseTypes.Released)
                {
                    return
                        (e.Set(SalesHeaderBases[orderType]) as IEnumerable<ISalesHeader>).Select(
                            i => new OrderShortEntry
                            {
                                No_ = i.No_,
                                OrderType = orderType,
                                Sell_toCustomerNo = i.Sell_toCustomerNo,
                                PostingDate = i.PostingDate,
                                Sell_toCustomerName = i.Sell_toCustomerName,
                                AmountIncludingVat = i.AmountIncludingVat
                            }).Where(i => (no == null || i.No_.Contains(no)) &&
                                          (code == null || i.Sell_toCustomerNo.Contains(code)) &&
                                          (name == null || i.Sell_toCustomerName.Contains(name)) &&
                                          (from == null || i.PostingDate >= from) &&
                                          (to == null || i.PostingDate <= to)
                            ).OrderByDescending(i => i.PostingDate).ToList();
                }
                else
                {
                    return
                        (e.Set(SalesHeaderBases[orderType]) as IEnumerable<ISalesHeader>).Where(i => i.DocumentType != 0).Select(
                            i => new OrderShortEntry
                            {
                                No_ = i.No_,
                                OrderType = orderType,
                                Sell_toCustomerNo = i.Sell_toCustomerNo,
                                PostingDate = i.PostingDate,
                                Sell_toCustomerName = i.Sell_toCustomerName,
                                AmountIncludingVat = i.AmountIncludingVat
                            }).Where(i => (no == null || i.No_.Contains(no)) &&
                                          (code == null || i.Sell_toCustomerNo.Contains(code)) &&
                                          (name == null || i.Sell_toCustomerName.Contains(name)) &&
                                          (from == null || i.PostingDate >= from) &&
                                          (to == null || i.PostingDate <= to)
                            ).OrderByDescending(i => i.PostingDate).ToList();
                }
            }
        }

        public bool HasClientOrders(string clientNo)
        {
            using (var e = new POSWR1Entities())
            {
                return e.SalesHeaders.Any(i => i.Sell_toCustomerNo == clientNo);
            }
        }

        public ISalesHeader GetCurrentOrder(string no_, OrderBaseTypes orderType, bool full = false)
        {
            using (var e = new POSWR1Entities())
            {
                var header = (e.Set(SalesHeaderBases[orderType]) as IEnumerable<ISalesHeader>).FirstOrDefault(i => i.No_ == no_);
                if (full)
                {
                    header.SalesLines =
                        (e.Set(SalesLinesBases[orderType]) as IEnumerable<ISalesLine>).Where(i => i.DocumentNo_ == no_);
                    header.JournalLines =
                        (e.Set(GenJounralLineBases[orderType]) as IEnumerable<IGenJournalLine>).Where(i => i.DocumentNo == no_);
                    header.PaymentSchedules =
                        (e.Set(PaymentSchedulesBases[orderType]) as IEnumerable<IPaymentSchedule>).Where(i => i.DocumentNo == no_);
                }
                return header;
            }
        }


        public List<Country> GetCountries()
        {
            List<Country> res;
            using (var e = new POSWR1Entities())
            {
                res = e.Countries.OrderBy(i => i.Name).ToList();
            }
            return res;
        }

        public List<PostCodeCity> GetPostCodeCities()
        {
            List<PostCodeCity> res;
            using (var e = new POSWR1Entities())
            {
                res = e.PostCodeCities.OrderBy(i => i.City).ToList();
            }
            return res;
        }


        public List<string> GetOeNumbers(string filter)
        {
            List<string> res;
            using (var e = new POSWR1Entities())
            {
                res =
                    e.AdditionalParameters.Where(i => i.ParameterValueText.StartsWith(filter)).Select(i => i.ParameterValueText).Distinct().ToList();
            }
            return res;
        }


        public void RemoveSalesHeader(ISalesHeader order)
        {
            using (var e = new POSWR1Entities())
            {
                switch (order.OrderBaseType)
                {
                    case OrderBaseTypes.Current:
                        e.PaymentSchedules.RemoveRange(e.PaymentSchedules.Where(i => i.DocumentNo == order.No_));
                        e.GenJournalLines.RemoveRange(e.GenJournalLines.Where(i => i.DocumentNo == order.No_));
                        e.SalesLines.RemoveRange(e.SalesLines.Where(i => i.DocumentNo_ == order.No_));
                        e.SalesHeaders.RemoveRange(e.SalesHeaders.Where(i => i.No_ == order.No_));
                        e.ItemLedgerEntries.RemoveRange(e.ItemLedgerEntries.Where(i => i.DocumentNo_ == order.No_));
                        break;
                    case OrderBaseTypes.Released:
                        e.ReleasedPaymentSchedules.RemoveRange(e.ReleasedPaymentSchedules.Where(i => i.DocumentNo == order.No_));
                        e.ReleasedSalesLines.RemoveRange(e.ReleasedSalesLines.Where(i => i.DocumentNo_ == order.No_));
                        e.ReleasedSalesHeaders.RemoveRange(e.ReleasedSalesHeaders.Where(i => i.No_ == order.No_));
                        break;
                }
                e.SaveChanges();
            }
        }

        public void RemoveCustomer(string customerNo)
        {
            using (var e = new POSWR1Entities())
            {
                e.Customers.RemoveRange(e.Customers.Where(i => i.No_ == customerNo && i.IsNewCustomer == true));
                e.Ship_to_Address.RemoveRange(e.Ship_to_Address.Where(i => i.CustomerNo_ == customerNo));
                e.SaveChanges();
            }
        }

        public List<Manufacturer> getManufactureLists()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {

                return e.Manufacturers.Where(i => e.VehicleModels.Any(j => j.ManufacturerCode == i.Code)).ToList();
            }
        }

        public List<VehicleGroup> getVehicleGroupList()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                return e.VehicleGroups.ToList();
            }
        }
        public List<string> getBrandLists()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                return e.Items.Select(i => i.ManufacturerCode).Distinct().ToList();
            }
        }


        public List<Tuple<string, string>> getModelNosLists()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                return
                    e.VehicleModels.Select(i => new { i.ModelNo_, i.ModelDescription })
                        .ToList()
                        .Select(i => Tuple.Create(i.ModelNo_, i.ModelDescription))
                        .ToList();
            }
        }

        public List<string> getCabTypesLists()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                return e.VehicleModels.Select(i => i.CabType).Distinct().ToList();
            }
        }

        public List<string> getEnginesLists()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                return e.VehicleModels.Select(i => i.Engine).Distinct().ToList();
            }
        }

        public List<DateTime> getManufactureYearsListsFrom()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                return e.VehicleModels.Select(i => i.ManufacturingStartDate).OrderBy(i => i).Distinct().ToList();
            }
        }

        public List<DateTime?> getManufactureYearsListsTo()
        {
            List<POSUser> res;
            using (var e = new POSWR1Entities())
            {
                return e.VehicleModels.Select(i => i.ManufacturingEndDate).OrderBy(i => i).Distinct().ToList();
            }
        }

        public List<ProjectedItemReceipt> GetProjectedItemReceiptByItem(string itemNo)
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.ProjectedItemReceipts.Where(i => i.ItemNo_ == itemNo)
                        .OrderByDescending(i => i.ReceiptDate)
                        .ToList();
            }
        }

        public List<ProjectedItemReceipt> GetProjectedItemReceipts()
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.ProjectedItemReceipts.ToList();
            }
        }

        public List<AdditionalParameter> AdditionalParameters()
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.AdditionalParameters.ToList();
            }
        }

        public bool IsNewCustomers()
        {
            using (var e = new POSWR1Entities())
            {
                return e.Customers.Any(i => i.IsNewCustomer == true);
            }
        }


        public List<SalesHeader> GetSalesHeaderList(List<string> orderNos)
        {
            using (var e = new POSWR1Entities())
            {
                if (orderNos == null || orderNos.Count == 0 || orderNos.Count == e.SalesHeaders.Count())
                    return
                        e.SalesHeaders.ToList();
                return e.SalesHeaders.Where(i => orderNos.Any(j => j == i.No_)).ToList();

            }
        }

        public List<SalesLine> GetSalesLinesList()
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.SalesLines.ToList();
            }
        }

        public List<GenJournalLine> GetJournalLines()
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.GenJournalLines.ToList();
            }
        }

        public List<GenJouranlView> GetJournalLinesView()
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.GenJournalLines.Select(i => new GenJouranlView
                    {
                        PaymentMethodCode = i.PaymentMethodCode,
                        Amount = i.Amount,
                        DocumentNo = i.DocumentNo,
                        LineNo = i.LineNo_,
                        AccountNo_ = i.AccountNo_,
                        PostingDate = i.PostingDate,
                        PostingDateReal = i.PostingDate,
                        Bal_AccountNo_ = i.Bal_AccountNo_,
                        Bal_AccountType = i.Bal_AccountType,
                        AccountType = i.AccountType,
                        Bal_AccountName = e.Customers.FirstOrDefault(j => j.No_ == i.Bal_AccountNo_).Name
                    }).ToList();
            }
        }

        public List<PaymentSchedule> GetPaymentSchedules()
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.PaymentSchedules.ToList();
            }
        }

        public List<PaymentSchedule> GetPaymentSchedules(string customerNo)
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.PaymentSchedules.Where(i => i.CustomerNo == customerNo).ToList();
            }
        }


        public List<ReleasedSalesLine> GetReleasedSalesLines(string customerNo)
        {
            using (var e = new POSWR1Entities())
            {
                var s = (from l in e.ReleasedSalesLines
                         join h in e.ReleasedSalesHeaders on l.DocumentNo_ equals h.No_
                         where l.Sell_toCustomerNo == customerNo
                         select new { l, h.PostingDate }).ToList();
                s.ForEach(i =>
                {
                    i.l.ModifiedDate = i.PostingDate;
                });
                return s.Select(i => i.l).OrderByDescending(i => i.ModifiedDate).ThenBy(i => i.LineNo_).ToList();
            }
        }



        public void PostGeneralJournalLines()
        {
            using (var e = new POSWR1Entities())
            {
                var jurnals = e.GenJournalLines.Where(i => i.IsGeneral == true).ToList();
                foreach (var i in jurnals)
                {
                    e.PostedGenJournalLines.Add(new PostedGenJournalLine
                    {
                        DocumentType = i.DocumentType,
                        Amount = i.Amount,
                        PostingDate = i.PostingDate,
                        ResponsibilityCenter = i.ResponsibilityCenter,
                        PaymentMethodCode = i.PaymentMethodCode,
                        DocumentNo = i.DocumentNo,
                        LineNo_ = i.LineNo_,
                        Description = i.Description,
                        ModifiedDate = i.ModifiedDate,
                        Bal_AccountType = i.Bal_AccountType,
                        AccountType = i.AccountType,
                        Bal_AccountNo_ = i.Bal_AccountNo_,
                        AccountNo_ = i.AccountNo_,
                        BatchName = i.BatchName,
                        CurrencyFactor = i.CurrencyFactor,
                        PlasticCardNo = i.PlasticCardNo,
                        CurrencyCode = i.CurrencyCode,
                        Salespers__Purch_Code = i.Salespers__Purch_Code,
                        TemplateName = i.TemplateName,
                        IsSent = true,
                        ModifiedUserID = i.ModifiedUserID,
                        PaymentMethodCodeGeo = i.PaymentMethodCodeGeo,
                        PresentCardNo = i.PresentCardNo
                    });
                }
                e.GenJournalLines.RemoveRange(
                        e.GenJournalLines.Where(i => i.IsGeneral == true));
                e.SaveChanges();
            }
        }

        public void PostSalesHeader(SalesHeader h)
        {
            using (var e = new POSWR1Entities())
            {
                var dp = e.PostedSalesHeaders.FirstOrDefault(i => i.No_ == h.No_ && i.DocumentType == h.DocumentType);
                if (dp != null)
                    e.PostedSalesHeaders.Remove(dp);
                var p = new PostedSalesHeader
                {
                    No_ = h.No_,
                    Sell_toCustomerNo = h.Sell_toCustomerNo,
                    Amount = h.Amount,
                    AmountIncludingVat = h.AmountIncludingVat,
                    CurrencyCode = h.CurrencyCode,
                    CurrencyFactor = h.CurrencyFactor,
                    CustomerPriceGroup = h.CustomerPriceGroup,
                    DocumentType = h.DocumentType,
                    DueDate = h.DueDate,
                    ExternalDocumentNo_ = h.ExternalDocumentNo_,
                    Guid = h.Guid,
                    IsEmployee = h.IsEmployee,
                    InvoiceDiscountAmount = h.InvoiceDiscountAmount,
                    InvoiceDiscountCalc = h.InvoiceDiscountCalc,
                    ModifiedDate = h.ModifiedDate,
                    ModifiedUser = h.ModifiedUser,
                    PaymentMethodCode = h.PaymentMethodCode,
                    PaymentTermsCode = h.PaymentTermsCode,
                    PlasticCardNo = h.PlasticCardNo,
                    PostingDate = h.PostingDate,
                    PostingDescription = h.PostingDescription,
                    PricesIncludingVat = h.PricesIncludingVat,
                    ResponsibilityCenter = h.ResponsibilityCenter,
                    SalespersonCode = h.SalespersonCode,
                    Sell_toCustomerName = h.Sell_toCustomerName,
                    Sell_toCountry = h.Sell_toCountry,
                    Ship_toCity = h.Ship_toCity,
                    Sell_toPostCode = h.Sell_toPostCode,
                    ShipmentDate = h.ShipmentDate,
                    Ship_toAddress = h.Ship_toAddress,
                    Sell_toCity = h.Sell_toCity,
                    Ship_toCountry = h.Ship_toCountry,
                    VatInvoice = h.VatInvoice,
                    Waybill = h.Waybill,
                    VATRegistrationNo_ = h.VATRegistrationNo_,
                    ShippingAgentCode = h.ShippingAgentCode,
                    Status = h.Status,
                    Sell_toAddress = h.Sell_toAddress,
                    ShipmentMethodCode = h.ShipmentMethodCode,
                    Ship_toAddressCode = h.Ship_toAddressCode
                };
                e.PostedSalesHeaders.Add(p);

                e.PostedSalesLines.RemoveRange(
                        e.PostedSalesLines.Where(i => i.DocumentNo_ == h.No_ && i.DocumentType == h.DocumentType));


                e.PostedSalesLines.AddRange(h.SalesLines.Select(i => new PostedSalesLine
                {
                    No_ = i.No_,
                    Sell_toCustomerNo = i.Sell_toCustomerNo,
                    OrderType = i.OrderType,
                    DocumentType = i.DocumentType,
                    Amount = i.Amount,
                    Type = i.Type,
                    DocumentNo_ = i.DocumentNo_,
                    Quantity = i.Quantity,
                    ResponsibilityCenter = i.ResponsibilityCenter,
                    UnitPrice = i.UnitPrice,
                    LineNo_ = i.LineNo_,
                    UnitOfMeasureCode = i.UnitOfMeasureCode,
                    LocationCode = i.LocationCode,
                    ModifiedDate = i.ModifiedDate,
                    AmountIncludingVAT = i.AmountIncludingVAT,
                    Description = i.Description,
                    ModifiedUser = i.ModifiedUser,
                    GrossWeight = i.GrossWeight,
                    Inv_DiscountAmount = i.Inv_DiscountAmount,
                    ItemCategoryCode = i.ItemCategoryCode,
                    LargeDescription = i.LargeDescription,
                    LineDiscountAmount = i.LineDiscountAmount,
                    LineDiscountPercent = i.LineDiscountPercent,
                    NetWeight = i.NetWeight,
                    No2 = i.No2,
                    ProdSubGroupCode = i.ProdSubGroupCode,
                    ProductGroupCode = i.ProductGroupCode,
                    QuantityBase = i.QuantityBase,
                    UnitVolume = i.UnitVolume
                }));

                e.PostedGenJournalLines.RemoveRange(
                    e.PostedGenJournalLines.Where(i => i.DocumentNo == h.No_ && i.DocumentType == h.DocumentType));

                e.PostedGenJournalLines.AddRange(h.JournalLines.Select(i => new PostedGenJournalLine
                {
                    DocumentType = i.DocumentType,
                    Amount = i.Amount,
                    PostingDate = i.PostingDate,
                    ResponsibilityCenter = i.ResponsibilityCenter,
                    PaymentMethodCode = i.PaymentMethodCode,
                    DocumentNo = i.DocumentNo,
                    LineNo_ = i.LineNo_,
                    Description = i.Description,
                    ModifiedDate = i.ModifiedDate,
                    Bal_AccountType = i.Bal_AccountType,
                    AccountType = i.AccountType,
                    Bal_AccountNo_ = i.Bal_AccountNo_,
                    AccountNo_ = i.AccountNo_,
                    BatchName = i.BatchName,
                    CurrencyFactor = i.CurrencyFactor,
                    PlasticCardNo = i.PlasticCardNo,
                    CurrencyCode = i.CurrencyCode,
                    Salespers__Purch_Code = i.Salespers__Purch_Code,
                    TemplateName = i.TemplateName,
                    IsSent = true,
                    ModifiedUserID = i.ModifiedUserID,
                    PaymentMethodCodeGeo = i.PaymentMethodCodeGeo,
                    PresentCardNo = i.PresentCardNo
                }));

                e.PostedPaymentSchedules.RemoveRange(e.PostedPaymentSchedules.Where(i => i.DocumentNo == h.No_));

                e.PostedPaymentSchedules.AddRange(h.PaymentSchedules.Select(i => new PostedPaymentSchedule
                {
                    Amount = i.Amount,
                    DocumentNo = i.DocumentNo,
                    CustomerNo = i.CustomerNo,
                    EntryNo = i.EntryNo,
                    Date = i.Date,
                    EntryType = i.EntryType,
                    RemeiningAmount = i.RemeiningAmount
                }));


                e.SalesHeaders.RemoveRange(e.SalesHeaders.Where(i => i.No_ == h.No_ && i.DocumentType == h.DocumentType));
                e.SalesLines.RemoveRange(
                    e.SalesLines.Where(i => i.DocumentNo_ == h.No_ && i.DocumentType == h.DocumentType));
                e.GenJournalLines.RemoveRange(
                    e.GenJournalLines.Where(i => i.DocumentNo == h.No_ && i.DocumentType == h.DocumentType));
                e.PaymentSchedules.RemoveRange(e.PaymentSchedules.Where(i => i.DocumentNo == h.No_));

                e.SaveChanges();
            }
        }

        public List<BankAccount> GetBankAccounts(int accountType, int type)
        {
            using (var e = new POSWR1Entities())
            {
                return e.BankAccounts.Where(i => i.AccountType == accountType && i.Type == type).ToList();
            }
        }

        public void UpdateImageTest(byte[] mas)
        {
            using (var e = new POSWR1Entities())
            {
                var p = e.ItemPictures.First(i => i.ItemNo_ == "01.11360.0000.0000");
                p.Content = mas;
                e.SaveChanges();
            }

        }

        public void AddNewItemComment(string itemNo, DateTime commentDate, string comment)
        {
            using (var e = new POSWR1Entities())
            {
                int? maxLine = e.Comment_Line.Where(i => i.Table_Name == 27 && i.No_ == itemNo).Max(i => (int?)i.Line_No_);
                if (!maxLine.HasValue) maxLine = 0;
                e.Comment_Line.Add(new Comment_Line
                {
                    Table_Name = 27,
                    No_ = itemNo,
                    Date = commentDate,
                    IsNew = true,
                    Code = "",
                    Line_No_ = maxLine.Value + 1,
                    Comment = comment,
                    IsActive = true
                });
                e.SaveChanges();
            }
        }

        public void RemoveItemComment(string itemNo, int line_no)
        {
            using (var e = new POSWR1Entities())
            {
                var dbComment =
                    e.Comment_Line.FirstOrDefault(i => i.Table_Name == 27 && i.No_ == itemNo && i.Line_No_ == line_no);
                if (dbComment != null)
                {
                    dbComment.IsActive = false;
                    e.SaveChanges();
                }
            }
        }

        public List<Comment_Line> GetItemComments(string itemNo)
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.Comment_Line.Where(i => i.Table_Name == 27 && i.No_ == itemNo && i.IsActive)
                        .OrderBy(i => i.Date)
                        .ThenBy(i => i.Line_No_)
                        .ToList();

            }
        }

        public void UpdateLikeStatus(string itemNo, string customerNo, bool like)
        {
            using (var e = new POSWR1Entities())
            {
                var ic = e.ItemCustomers.FirstOrDefault(i => i.ItemNo_ == itemNo && i.CustomerNo_ == customerNo);
                if (ic != null)
                {
                    ic.Like = like;
                    ic.IsNew = true;
                }
                else
                {
                    e.ItemCustomers.Add(new ItemCustomer
                    {
                        ItemNo_ = itemNo,
                        CustomerNo_ = customerNo,
                        VariantCode = "",
                        IsNew = true,
                        Like = like

                    });
                }
                e.SaveChanges();
            }
        }


        public List<POSMessageEntry> GetNewMessages()
        {
            using (var e = new POSWR1Entities())
            {
                return e.POSMessageEntries.Where(i => i.EntryNo == null).ToList();

            }
        }

        public void UpdateMessageEntryNo(long id, string entryNo)
        {
            using (var e = new POSWR1Entities())
            {
                var m = e.POSMessageEntries.FirstOrDefault(i => i.Id == id);
                if (m != null)
                {
                    m.EntryNo = entryNo;
                    e.SaveChanges();
                }
            }
        }

        public List<string> GetStatusChangeMessages()
        {
            using (var e = new POSWR1Entities())
            {
                return
                    e.POSMessageEntries.Where(i => i.ReadStatusChange == true && i.EntryNo != null)
                        .Select(i => i.EntryNo)
                        .ToList();
            }
        }

        public decimal? GetsalesPrice(string itemNo, string uniTofmeasureCode, string customerNo, string customerGroup)
        {
            using (var e = new POSWR1Entities())
            {
                var slist =
                    e.SalesPrices.Where(
                        i =>
                            i.ItemNo_ == itemNo && i.UnitOfMeasureCode == uniTofmeasureCode &&
                            ((i.SalesType == 0 && i.SalesCode == customerNo) ||
                             (i.SalesType == 1 && i.SalesCode == customerGroup) || i.SalesType == 2))
                        .OrderBy(i => i.SalesType)
                        .ThenByDescending(i => i.StartingDate)
                        .ToList();
                if (slist.Count > 0)
                    return slist.First().UnitPrice;

                return null;
            }
        }

        public void SyncMessages(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("DELETE FROM [dbo].[POSMessageEntry]");
                foreach (DataRow r in dt.Rows)
                {
                    POSMessageEntry a = new POSMessageEntry();
                    a.EntryNo = r["Entry No_"].ToString();
                    a.SenderID = r["Sender ID"].ToString();
                    a.ReceiverID = r["Receiver ID"].ToString();
                    a.Text = r["Text"].ToString();
                    a.SendDateTime = (DateTime)r["Send DateTime"];
                    a.IsRead = r["IsRead"].ToString() == "1";
                    if (a.IsRead)
                        a.ReadDateTime = (DateTime)r["Read DateTime"];
                    a.SenderSalespersCode = r["Sender Salespers_ Code"].ToString();
                    a.ReceiverSalespersCode = r["Receiver Salespers_ Code"].ToString();
                    e.POSMessageEntries.Add(a);
                }
                e.SaveChanges();
            }
        }

        public void GetItemByBarCode(string code, out Item item, out string unitOfMeasure)
        {
            item = null;
            unitOfMeasure = null;
            using (var e = new POSWR1Entities())
            {
                var ic =
                    e.ItemCrossReferences.Where(
                        i => i.Cross_ReferenceNo_ == code && i.Cross_ReferenceType == 3)
                        .FirstOrDefault();
                if (ic != null)
                {
                    item = e.Items.FirstOrDefault(i => i.No_ == ic.ItemNo_);
                    unitOfMeasure = ic.UnitOfMeasure;
                }
            }
        }
        public void updateGenJournalPostingDate(int lineNo, string documentNo, DateTime? postingTime)
        {
            using (var e = new POSWR1Entities())
            {
                var gn =
                    e.GenJournalLines.FirstOrDefault(
                        i =>
                             i.LineNo_ == lineNo &&
                            i.DocumentNo == documentNo);
                if (gn != null && postingTime.HasValue)
                {
                    gn.PostingDate = postingTime;
                    e.SaveChanges();
                }
            }
        }

        public void deletejournal(int lineNo, string documentNo)
        {
            using (var e = new POSWR1Entities())
            {
                e.GenJournalLines.RemoveRange(e.GenJournalLines.Where(
                    i =>
                         i.LineNo_ == lineNo &&
                        i.DocumentNo == documentNo));
                e.SaveChanges();

            }
        }



        public void SyncVendors(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.Vendor");
                foreach (DataRow r in dt.Rows)
                {
                    Vendor a = new Vendor();
                    a.Country_Region_Code = r["Country_Region Code"].ToString();
                    a.No_ = r["No_"].ToString();
                    a.Name = r["Name"].ToString();
                    a.VAT_Registration_No_ = r["VAT Registration No_"].ToString();
                    e.Vendors.Add(a);
                }
                e.SaveChanges();
            }
        }

        public void SyncVehicles(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from dbo.Vehicle");
                foreach (DataRow r in dt.Rows)
                {
                    Vehicle a = new Vehicle();
                    a.Vehicle_No_ = r["Vehicle No_"].ToString();
                    a.Model = r["Model"].ToString();
                    a.Owner_No_= r["Owner No_"].ToString();
                    a.Owner_Name = r["Owner Name"].ToString();

                    a.Ownership = int.Parse( r["Ownership"].ToString() );
                    a.Registration_No_ = r["Registration No_"].ToString();
                    a.Trail_Registration_No_ = r["Trail Registration No_"].ToString();
                    a.Tranzit_Location_Code = r["Tranzit Location Code"].ToString();
                    a.Type = int.Parse( r["Type"].ToString() ) ;
                    a.Driver_Code = r["Driver Code"].ToString();
                    a.Driver_Name = r["Driver Name"].ToString();
                    a.Driver_ID = r["Driver ID"].ToString();
                    a.WIN_Code = r["WIN Code"].ToString();
                    e.Vehicles.Add(a);
                }
                e.SaveChanges();
            }
        }

        public void SyncDimensionValue(DataTable dt)
        {
            using (var e = new POSWR1Entities())
            {
                e.Database.ExecuteSqlCommand("delete from [dbo].[Dimension Value]");
                foreach (DataRow r in dt.Rows)
                {
                    Dimension_Value d = new Dimension_Value();



                    d.Dimension_Code = r["Dimension Code"].ToString();
                    d.Code = r["Code"].ToString();
                    d.Name = r["Name"].ToString();
                    d.Dimension_Value_Type = (int)r["Dimension Value Type"];
                    d.Totaling = r["Totaling"].ToString();
                    d.Blocked = (byte)r["blocked"];
                    d.Consolidation_Code = r["Consolidation Code"].ToString();
                    d.Indentation = (int)r["Indentation"];
                    d.Global_Dimension_No_ = (int)r["Global Dimension No_"];
                    d.Map_to_IC_Dimension_Code = r["Map-to IC Dimension Code"].ToString();
                    d.Map_to_IC_Dimension_Value_Code = r["Map-to IC Dimension Value Code"].ToString();
                    d.Dimension_Value_ID = (int)r["Dimension Value ID"];
                    d.Name_2 = r["Name 2"].ToString();
                    d.DimensionName = r["Code"].ToString() + " " + r["Name"].ToString();

                    e.Dimension_Values.Add(d);
                }
                //try
                //{
                e.SaveChanges();
                //}
                //catch(DbEntityValidationException ex)
                //{

                //}

            }
        }
    }
}
