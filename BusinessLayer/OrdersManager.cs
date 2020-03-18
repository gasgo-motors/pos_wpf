using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;

namespace BusinessLayer
{
    public class OrdersManager : PosManager<OrdersManager>
    {
        public string GetItemInventoryByLocations(string itemNo)
        {
            POSMng.POSMng pc = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            return pc.GetItemInventoryByLocations(itemNo);
        }

        public salesorderservice.SalesOrder GetNavOrder(string documentType, string orderNo)
        {
            BusinessLayer.salesorderservice.SalesOrder_Service client = new BusinessLayer.salesorderservice.SalesOrder_Service
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl1
            };
            return  client.Read(documentType, orderNo );
        }

        public bool IsOrderWaybillUploaded(int orderType, string orderNo)
        {
            POSMng.POSMng pc = new POSMng.POSMng
            {
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(PosSetting.Settings_DomainUserName, PosSetting.Settings_DomainPassword, PosSetting.Settings_DomainName),
                Url = PosSetting.Settings_PosServiceUrl
            };
            return pc.OrderWaybillUploaded(orderType, orderNo);
        }

        public void SaveNavOrder(ReleasedSalesHeader h/*, List<ReleasedSalesLine> lines, List<ReleasedPaymentSchedule> schedules,List<ReleasedGenJournalLine> genjournal*/ )
        {
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
            pc.ReopenSalesOrder(h.DocumentType, h.No_);
            //pc.DeleteSalesOrder(1, h.No_);
            //salesorderservice.SalesOrder s = new salesorderservice.SalesOrder
            //{
            //    No = h.No_
            //};
            //client.Create(ref s);
            //s.Document_Type = salesorderservice.Document_Type.Order;
            //s.Sell_to_Customer_No = h.Sell_toCustomerNo;
            //s.Salesperson_Code = PosSetting.Settings_SalesPersonCode;
            //s.Responsibility_Center = PosSetting.Settings_ResponsibilityCenter;
            //s.Posting_Date = h.PostingDate.Value.Date;
            //s.POS_Order_No = h.No_;
            //s.POS_Order_Type = salesorderservice.POS_Order_Type.Order;
            //client.Update(ref s);
            List<RemainingItemEntry> RemainingItems = new List<RemainingItemEntry>();
            foreach (var l in h.SalesLines.Where(i => i.IsNew).ToList())
            {
                var quantity = pc.CalcItemInventoryByLocation(l.No_, l.LocationCode);
                if (l.Quantity > quantity)
                {
                    RemainingItems.Add(new RemainingItemEntry
                    {
                        OrderNo = h.No_,
                        ItemNo = l.No_,
                        ItemDesc = l.Description,
                        RequestedQuantity = l.Quantity.Value,
                        RemainingQuantity = l.Quantity.Value - quantity
                    });
                    l.Quantity = quantity;
                }
                if (l.Quantity > 0)
                    pc.CreateSalesLine(l.DocumentType, h.No_, l.LineNo_, l.Type.Value, l.No_, l.LocationCode, l.UnitOfMeasureCode, l.UnitPrice.Value, l.Quantity.Value, l.LineDiscountPercent.Value, l.LineDiscountAmount.Value);
            }
            pc.ReleaseSalesOrder(h.DocumentType, h.No_);
            pc.CreateWhsShipment(h.DocumentType, h.No_);
            //foreach(var g in h.PaymentSchedules)
            //{
            //    pc.CreatePaySchedLine(h.Sell_toCustomerNo, h.No_, g.Date.Value, g.Amount.Value);
            //}

            //foreach (var g in h.PaymentLines)
            //{
            //    pc.CreatePaymentLine(1, h.No_, g.Amount.Value, PosSetting.Settings_JnlTemplateName, PosSetting.Settings_JnlBatchName, g.PostingDate.Value, g.AccountType.Value, g.AccountNo_, g.Bal_AccountType.Value, g.Bal_AccountNo_, PosSetting.Settings_ResponsibilityCenter, g.PaymentMethodCode, PosSetting.Settings_SalesPersonCode);
            //}


        }


    }

}
