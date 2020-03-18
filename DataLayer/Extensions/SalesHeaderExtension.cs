using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Models;

namespace DataLayer
{
    public partial class SalesHeader : ISalesHeader
    {
        public List<SalesLine> SalesLines { get; set; }

        public List<SalesLine> OkSalesLines
        {
            get { return SalesLines != null ? SalesLines.Where(i => i.OrderType == 0).ToList() : null; }
        }

        public List<SalesLine> QSalesLines
        {
            get { return SalesLines != null ? SalesLines.Where(i => i.OrderType == 1).ToList() : null; }
        }

        public List<SalesLine> SPSalesLines
        {
            get { return SalesLines != null ? SalesLines.Where(i => i.OrderType == 2).ToList() : null; }
        }

        public List<SalesLine> CQSalesLines
        {
            get { return SalesLines != null ? SalesLines.Where(i => i.OrderType == 3).ToList() : null; }
        }

        public List<PaymentSchedule> PaymentSchedules { get; set; }
        public List<GenJournalLine> JournalLines { get; set; }

        public OrderBaseTypes OrderBaseType
        {
            get
            {
                return OrderBaseTypes.Current;
            }
        }

        public Customer CurrentCustomer { get; set; }

        IEnumerable<ISalesLine> ISalesHeader.SalesLines
        {
            get { return SalesLines; }

            set { SalesLines = value.Select(i => (SalesLine)i).ToList(); }
        }

        IEnumerable<IPaymentSchedule> ISalesHeader.PaymentSchedules
        {
            get { return PaymentSchedules; }

            set { PaymentSchedules = value.Select(i => (PaymentSchedule) i).ToList(); }
        }

        IEnumerable<IGenJournalLine> ISalesHeader.JournalLines
        {
            get { return JournalLines; }

            set { JournalLines = value.Select(i => (GenJournalLine)i).ToList(); }
        }
    }

    public partial class ReleasedSalesHeader : ISalesHeader
    {
        public List<ReleasedSalesLine> SalesLines { get; set; }
        public List<ReleasedPaymentSchedule> PaymentSchedules { get; set; }

        public List<ReleasedGenJournalLine> PaymentLines { get; set; }
        public Customer CurrentCustomer { get; set; }
        IEnumerable<ISalesLine> ISalesHeader.SalesLines
        {
            get { return SalesLines; }

            set { SalesLines = value.Select(i => (ReleasedSalesLine)i).ToList(); }
        }

        IEnumerable<IPaymentSchedule> ISalesHeader.PaymentSchedules
        {
            get { return PaymentSchedules; }

            set { PaymentSchedules = value.Select(i => (ReleasedPaymentSchedule)i).ToList(); }
        }

        IEnumerable<IGenJournalLine> ISalesHeader.JournalLines
        {
            get { return PaymentLines; }

            set { PaymentLines = value.Select(i => (ReleasedGenJournalLine)i).ToList(); }
        }
        public OrderBaseTypes OrderBaseType
        {
            get
            {
                return OrderBaseTypes.Released;
            }
        }
    }

    public partial class PostedSalesHeader : ISalesHeader
    {
        public Customer CurrentCustomer { get; set; }
        public List<PostedSalesLine> SalesLines { get; set; }
        public List<PostedPaymentSchedule> PaymentSchedules { get; set; }
        public List<PostedGenJournalLine> JournalLines { get; set; }

        IEnumerable<ISalesLine> ISalesHeader.SalesLines
        {
            get { return SalesLines; }

            set { SalesLines = value.Select(i => (PostedSalesLine)i).ToList(); }
        }

        IEnumerable<IPaymentSchedule> ISalesHeader.PaymentSchedules
        {
            get { return PaymentSchedules; }

            set { PaymentSchedules = value.Select(i => (PostedPaymentSchedule)i).ToList(); }
        }

        IEnumerable<IGenJournalLine> ISalesHeader.JournalLines
        {
            get { return JournalLines; }

            set { JournalLines = value.Select(i => (PostedGenJournalLine)i).ToList(); }
        }
        public OrderBaseTypes OrderBaseType
        {
            get
            {
                return OrderBaseTypes.Posted;
            }
        }
    }
}
