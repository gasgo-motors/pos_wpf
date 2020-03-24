using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace BusinessLayer
{
    public class SendServiceManager : PosManager<SendServiceManager>
    {
        public void SendCustomers()
        {
            var customers = DaoController.Current.GetNewCustomers();
            POSMng.POSMng pc = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            foreach (var customer in customers)
            {
                var newCode = pc.CreateNewCustomerNew(customer.Name, customer.VATRegistrationNo_, customer.NeedsVATInvoice.Value, customer.Contact,
                    customer.PhoneNo_, customer.Address, customer.ShipToAddress, customer.City, customer.Country_RegionCode, PosSetting.Settings_SalesPersonCode, customer.PostCode, customer.Mobile_, customer.AreaCode, customer.Customer_Posting_Group);
                DaoController.Current.UpdateCustomerStatus(customer.No_, newCode);
            }
        }

        public void SendGeneralGenJurnals()
        {
            var jurnals = DaoController.Current.GetGeneralGenJournalLines();
            POSMng.POSMng pc = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            foreach (var g in jurnals)
            {
                pc.CreatePaymentLine(1, g.DocumentNo, g.Amount.Value, PosSetting.Settings_JnlTemplateName,
                    PosSetting.Settings_JnlBatchName, g.PostingDate.Value, g.AccountType.Value, g.AccountNo_,
                    g.Bal_AccountType.Value, g.Bal_AccountNo_, PosSetting.Settings_ResponsibilityCenter,
                    g.PaymentMethodCode,
                    PosSetting.Settings_SalesPersonCode);
            }
            DaoController.Current.PostGeneralJournalLines();
        }

        public void SendLikes()
        {
            var likes = DaoController.Current.GetNewLikes();
            POSMng.POSMng pc = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            foreach (var like in likes)
            {
                pc.CreateCustItemLike(like.CustomerNo_, like.ItemNo_, like.Like == true);
                DaoController.Current.SetLikeSent(like.CustomerNo_, like.ItemNo_);
            }
        }

        public void SendNewMessages()
        {
            var newMessages = DaoController.Current.GetNewMessages();
            POSMng.POSMng pc = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            foreach (var message in newMessages)
            {
                var entryNo = pc.CreatePOSMessage(message.SenderID, message.ReceiverID, message.Text);
                DaoController.Current.UpdateMessageEntryNo(message.Id, entryNo);
            }
        }


        private List<RemainingItemEntry> SendOrder(SalesHeader h)
        {
            List<RemainingItemEntry> remainingItems = new List<RemainingItemEntry>();
            POSMng.POSMng pc = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };

            salesorderservice.SalesOrder_Service client = new salesorderservice.SalesOrder_Service
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl1
            };

            salesquoteservice.SalesQuote_Service qClient = new salesquoteservice.SalesQuote_Service
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl3
            };

            salesordersubform.SalesOrderLine_Service lineclient = new salesordersubform.SalesOrderLine_Service
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl2
            };

            if (h.OkSalesLines.Count > 0)
            {
                bool shemovida = false;
                foreach (var l in h.OkSalesLines)
                {
                    var quantity = pc.CalcItemInventoryByLocation(l.No_, l.LocationCode);
                    if (l.Quantity > quantity)
                    {
                        remainingItems.Add(new RemainingItemEntry
                        {
                            OrderNo = h.No_,
                            ItemNo = l.No_,
                            ItemDesc = l.LargeDescription,
                            RequestedQuantity = l.Quantity.Value,
                            RemainingQuantity = l.Quantity.Value - quantity
                        });


                        h.SalesLines.Add(new SalesLine
                        {
                            DocumentNo_ = h.No_,
                            DocumentType = 1,
                            LineNo_ = h.SalesLines.Max(i => i.LineNo_) + 1,
                            Type = 2,
                            No_ = l.No_,
                            Description = l.Description,
                            Quantity = l.Quantity.Value - quantity,
                            UnitPrice = l.UnitPrice,
                            Sell_toCustomerNo = h.Sell_toCustomerNo,
                            LocationCode = l.LocationCode,
                            AmountIncludingVAT = 0,
                            UnitOfMeasureCode = l.UnitOfMeasureCode,
                            OrderType = 1,
                            LineDiscountAmount = 0,
                            LineDiscountPercent = 0
                        });
                        DaoController.Current.AddSalesLine(h.SalesLines.Last());


                        l.Quantity = quantity;
                        l.AmountIncludingVAT = Math.Round(l.Quantity.Value * l.UnitPrice.Value, 2, MidpointRounding.AwayFromZero);
                    }
                    if (l.Quantity > 0)
                    {
                        if (!shemovida)
                        {
                            salesorderservice.SalesOrder s = new salesorderservice.SalesOrder
                            {
                                No = h.No_
                            };

                            client.Create(ref s);
                            //s.Document_Type = salesorderservice.Document_Type.Order;
                            s.Sell_to_Customer_No = h.Sell_toCustomerNo;
                            s.Salesperson_Code = PosSetting.Settings_SalesPersonCode;
                            s.Responsibility_Center = PosSetting.Settings_ResponsibilityCenter;
                            s.Posting_Date = h.PostingDate.Value.Date;
                            s.POS_Order_No = h.No_;
                            s.POS_Order_Type = salesorderservice.POS_Order_Type.Order;
                            s.Ship_to_Code = h.Ship_toAddressCode;
                            client.Update(ref s);
                            shemovida = true;
                        }

                        pc.CreateSalesLine(l.DocumentType, h.No_, l.LineNo_, l.Type.Value, l.No_, l.LocationCode,
                            l.UnitOfMeasureCode, l.UnitPrice.Value, l.Quantity.Value,
                            l.LineDiscountPercent.Value, l.LineDiscountAmount.Value,  Convert.ToDateTime(h.OrderStartDate).ToUniversalTime(), Convert.ToDateTime(h.OrderClosedDate).ToUniversalTime(), h.OrderDuraction,
                            l.Service_Provider, l.Customer_Vehicle);

                    }
                }
                if (shemovida)
                {
                    pc.ReleaseSalesOrder(1, h.No_);
                    pc.CreateWhsShipment(1, h.No_);

                    foreach (var g in h.PaymentSchedules)
                    {
                        pc.CreatePaySchedLine(h.Sell_toCustomerNo, h.No_, g.Date.Value, g.Amount.Value);
                    }

                    foreach (var g in h.JournalLines)
                    {
                        pc.CreatePaymentLine(1, h.No_, g.Amount.Value, PosSetting.Settings_JnlTemplateName,
                            PosSetting.Settings_JnlBatchName, g.PostingDate.Value, g.AccountType.Value, g.AccountNo_, g.Bal_AccountType.Value, g.Bal_AccountNo_, PosSetting.Settings_ResponsibilityCenter, g.PaymentMethodCode,
                            PosSetting.Settings_SalesPersonCode);
                    }

                }

            }
            if (h.QSalesLines.Count > 0)
            {
                var no = DaoController.Current.GenerateNewKey(PosSetting.Settings_SalesHeaderNumberCount, PosSetting.Settings_QoutePrefix);
                salesquoteservice.SalesQuote q = new salesquoteservice.SalesQuote
                {
                    No = no
                };

                qClient.Create(ref q);
                q.Sell_to_Customer_No = h.Sell_toCustomerNo;
                q.Salesperson_Code = PosSetting.Settings_SalesPersonCode;
                q.Responsibility_Center = PosSetting.Settings_ResponsibilityCenter;
                q.Posting_Date = h.PostingDate.Value.Date;
                q.POS_Order_No = no;
                q.POS_Order_Type = salesquoteservice.POS_Order_Type.Order;
                q.Ship_to_Code = h.Ship_toAddressCode;
                qClient.Update(ref q);

                foreach (var l in h.QSalesLines)
                {
                    pc.CreateSalesLine(0, no, l.LineNo_, l.Type.Value, l.No_, l.LocationCode,
                        l.UnitOfMeasureCode, l.UnitPrice.Value, l.Quantity.Value, l.LineDiscountPercent.Value,
                        l.LineDiscountAmount.Value, Convert.ToDateTime(h.OrderStartDate).ToUniversalTime(), Convert.ToDateTime(h.OrderClosedDate).ToUniversalTime(), h.OrderDuraction,
                        l.Service_Provider, l.Customer_Vehicle
                        );

                    var quantity = pc.CalcItemInventoryByLocation(l.No_, l.LocationCode);
                    remainingItems.Add(new RemainingItemEntry
                    {
                        OrderNo = h.No_,
                        ItemNo = l.No_,
                        ItemDesc = l.Description,
                        RequestedQuantity = l.Quantity.Value,
                        RemainingQuantity = l.Quantity.Value - quantity
                    });


                }

                pc.ReleaseSalesOrder(0, no);

            }
            if (h.CQSalesLines.Count > 0)
            {
                var no = DaoController.Current.GenerateNewKey(PosSetting.Settings_SalesHeaderNumberCount, PosSetting.Settings_ConcurrentQoutePrefix);
                salesquoteservice.SalesQuote q = new salesquoteservice.SalesQuote
                {
                    No = no
                };

                qClient.Create(ref q);
                q.Sell_to_Customer_No = h.Sell_toCustomerNo;
                q.Salesperson_Code = PosSetting.Settings_SalesPersonCode;
                q.Responsibility_Center = PosSetting.Settings_ResponsibilityCenter;
                q.Posting_Date = h.PostingDate.Value.Date;
                q.POS_Order_No = no;
                q.POS_Order_Type = salesquoteservice.POS_Order_Type.Order;
                q.Ship_to_Code = h.Ship_toAddressCode;
                qClient.Update(ref q);

                foreach (var l in h.CQSalesLines)
                {
                    pc.CreateSalesLine(0, no, l.LineNo_, l.Type.Value, l.No_, l.LocationCode,
                        l.UnitOfMeasureCode, l.UnitPrice.Value, l.Quantity.Value, l.LineDiscountPercent.Value,
                        l.LineDiscountAmount.Value, Convert.ToDateTime(h.OrderStartDate).ToUniversalTime(), Convert.ToDateTime(h.OrderClosedDate).ToUniversalTime(), h.OrderDuraction,
                        l.Service_Provider, l.Customer_Vehicle);
                }

                pc.ReleaseSalesOrder(0, no);

            }
            if (h.SPSalesLines.Count > 0)
            {
                var no = DaoController.Current.GenerateNewKey(PosSetting.Settings_SalesHeaderNumberCount, PosSetting.Settings_SpecOrderPrefix);
                salesorderservice.SalesOrder s = new salesorderservice.SalesOrder
                {
                    No = no
                };

                client.Create(ref s);
                //s.Document_Type = salesorderservice.Document_Type.Order;
                s.Sell_to_Customer_No = h.Sell_toCustomerNo;
                s.Salesperson_Code = PosSetting.Settings_SalesPersonCode;
                s.Responsibility_Center = PosSetting.Settings_ResponsibilityCenter;
                s.Posting_Date = h.PostingDate.Value.Date;
                s.POS_Order_No = no;
                s.POS_Order_Type = salesorderservice.POS_Order_Type.Order;
                s.Ship_to_Code = h.Ship_toAddressCode;
                s.Transport_Method = "1";
                client.Update(ref s);

                foreach (var l in h.SPSalesLines)
                {
                    pc.CreateSalesLine(l.DocumentType, no, l.LineNo_, l.Type.Value, l.No_, l.LocationCode,
                        l.UnitOfMeasureCode, l.UnitPrice.Value, l.Quantity.Value, l.LineDiscountPercent.Value,
                        l.LineDiscountAmount.Value, Convert.ToDateTime(h.OrderStartDate).ToUniversalTime(), Convert.ToDateTime(h.OrderClosedDate).ToUniversalTime(), h.OrderDuraction,
                        l.Service_Provider, l.Customer_Vehicle);
                }

                pc.ReleaseSalesOrder(1, no);
            }

            DaoController.Current.PostSalesHeader(h);

            return remainingItems;
        }

        public void SendComments()
        {
            POSMng.POSMng pc = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            var list = DaoController.Current.GetNewComments();
            foreach (var c in list)
            {
                pc.CreateItemComment(c.No_, c.Comment, c.Date);
            }
            DaoController.Current.UpdateNewCommentStatuses();
        }


        public int SendOrders(List<string> orderNos, bool withCustomers, out string errorHeader, out string errorMessage, out List<RemainingItemEntry> RemainingItems)
        {
            int res = 0;
            errorHeader = null;
            errorMessage = null;
            RemainingItems = new List<RemainingItemEntry>();
            try
            {
                // კლიენტების გადაგზავნა
                if (withCustomers)
                    SendCustomers();
                // კომენტარების გადაგზავნა
                SendComments();
                SendLikes();
                SendGeneralGenJurnals();
                var salesHeaders = DaoController.Current.GetSalesHeaderList(orderNos);
                var salesLines = DaoController.Current.GetSalesLinesList();
                var journalLines = DaoController.Current.GetJournalLines();
                var paymentScheduels = DaoController.Current.GetPaymentSchedules();
                foreach (var h in salesHeaders)
                {
                    h.SalesLines = salesLines.Where(i => i.DocumentNo_ == h.No_).ToList();
                    h.JournalLines = journalLines.Where(i => i.DocumentNo == h.No_).ToList();
                    h.PaymentSchedules = paymentScheduels.Where(i => i.DocumentNo == h.No_).ToList();
                }
                foreach (var h in salesHeaders)
                {
                    try
                    {
                        var list = SendOrder(h);
                        res++;
                        RemainingItems.AddRange(list);
                    }
                    catch (Exception ex)
                    {
                        errorHeader = h.No_;
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }
            return res;
        }
    }
}
