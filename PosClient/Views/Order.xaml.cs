using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLayer;
using DataLayer;
using DataLayer.Models;
using MahApps.Metro.Controls.Dialogs;
using PosClient.ViewModels;
using PosClient.Helpers;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Font;
using iText.IO.Font.Constants;
using iText.IO.Font;
using iText.Kernel.Pdf.Canvas.Draw;

namespace PosClient.Views
{
    public abstract class OrderController : PosUserControl<Order, OrderViewModel>
    {

    }
    public partial class Order : OrderController
    {
        public Order()
        {
            InitializeComponent();
            pdfWebViewer.Navigate(new Uri("about:blank"));
        }




        public UserControlTypes PrevUserControl { get; set; }
        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Order;
            }
        }

        public override ScrollBarVisibility ScrollBarVis
        {
            get
            {
                return ScrollBarVisibility.Disabled;
            }
        }

        public override void Refresh()
        {
            CurrentModel.Refresh();
            Keyboard.Focus(tbx_bar_code);
            tbx_bar_code.Text = "";
            tbx_bar_code.Focus();
            //FocusManager.SetFocusedElement(this, tbx_bar_code);
            
        }

        public override bool ShowHomeBtn
        {
            get { return true; }
        }

        private string _headerText;
        private List<string> _prevHeaders; 

        public void SetHeaders(string text, List<string> path)
        {
            _headerText = text;
            _prevHeaders = path;
        }
        public override string HeaderText
        {
            get { return _headerText; }
        }

        public override List<string> PrevHeaders
        {
            get { return _prevHeaders; }
        }

        private void Btn_products_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Reserves, new ReservesViewModel(CurrentModel));
        }

        private void Btn_payment_schedule_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.PaymentSchedule, new PaymentScheduleViewModel(CurrentModel));
        }

        private void Btn_payment_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(UserControlTypes.Payment, new PaymentViewModel(CurrentModel));
        }

        private async void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ შენახვა?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                try
                {
                    CurrentModel.CreateOrder();
                    //if( CurrentModel.IsNew)
                    //    await App.Current.CurrentMainWindow.ShowMessageAsync("ახალი ორდერი შეიქმნა წარმატებით!", "");
                    if (CurrentModel.Order.Sell_toCustomerNo == App.Current.PosSetting.Settings_GenCustomerCode)
                        OpenNewOrder();
                    else
                        NavigateToControl(PrevUserControl);
                }
                catch (Exception ex)
                {
                    App.Current.ShowErrorDialog("შეცდომა შენახვისას!", ex.Message);
                }
            }
        }

        private async void OpenNewOrder()
        {
            var customer = DaoController.Current.GetCustomer(App.Current.PosSetting.Settings_GenCustomerCode);
            if (customer == null)
                App.Current.ShowErrorDialog("საცალო კლიენტი ვერ მოიძებნა ბაზაში!", "");
            else
            {

                var order = new SalesHeader
                {
                    No_ = DaoController.Current.GenerateNewKey(App.Current.PosSetting.Settings_SalesHeaderNumberCount),
                    DocumentType = 1,
                    PostingDate = DateTime.Now,
                    Sell_toCustomerNo = customer.No_,
                    Sell_toCustomerName = customer.Name,
                    CustomerPriceGroup = customer.CustomerPriceGroup,
                    SalespersonCode = App.Current.PosSetting.Settings_SalesPersonCode,
                    SalesLines = new List<SalesLine>(),
                    JournalLines = new List<GenJournalLine>(),
                    PaymentSchedules = new List<DataLayer.PaymentSchedule>(),
                    CurrentCustomer = customer
                };
                // Order.Current.SetHeaders("ახალი შეკვეთა", new List<string>() { "დღის განრიგი" });
                Order.Current.PrevUserControl = UserControlTypes.Customers;
                NavigateToControl(UserControlTypes.Order, new OrderViewModel(order, true));
            }

        }

        private async void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ ორდერის გაუქმება?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                DaoController.Current.RemoveSalesHeader(CurrentModel.Order);
                NavigateToControl(PrevUserControl);
            }


        }

        private void BtnEdit_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.IsEditable = true;
            CurrentModel.Refresh();
            CurrentModel.LoadOrderFromNav();
        }

        private void BtnGoBack_OnClick(object sender, RoutedEventArgs e)
        {
            NavigateToControl(PrevUserControl);
        }

        private void Del_comment_OnClick(object sender, RoutedEventArgs e)
        {
            int line_no = ((sender as FrameworkElement).DataContext as SalesLineShortEntry).LineNo;
            CurrentModel.DeleteSalesLine(line_no);
            CurrentModel.Refresh();
        }

        private async void BtnQuete_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ ორდერის ქვოტირება?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                CurrentModel.QueteOrders();
                NavigateToControl(PrevUserControl);
            }
        }

        private void Tbx_bar_code_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (!string.IsNullOrEmpty(tbx_bar_code.Text))
                {
                    CurrentModel.AddItemByBarCode(tbx_bar_code.Text);
                    tbx_bar_code.Text = "";
                }
            }
        }

        private void OrderController_Loaded(object sender, RoutedEventArgs e)
        {
            tbx_bar_code.Focus();
            Keyboard.Focus(tbx_bar_code);
        }

        private async void BtnSaveAndSend_OnClick(object sender, RoutedEventArgs e)
        {
            if (
                await
                    App.Current.CurrentMainWindow.ShowMessageAsync("გსურთ შენახვა და გადაგზავნა?", "",
                        MessageDialogStyle.AffirmativeAndNegative) != MessageDialogResult.Negative)
            {
                List<string> orders;
                try
                {
                    orders = CurrentModel.CreateOrder();
                }
                catch (Exception ex)
                {
                    App.Current.ShowErrorDialog("შეცდომა შენახვისას!", ex.Message);
                    return;
                }



                var mySettings = new MetroDialogSettings()
                {
                    AnimateShow = false,
                    AnimateHide = false
                };
                var controller = await App.Current.CurrentMainWindow.ShowProgressAsync("მიმდინარეობს ორდერის გადაგზავნა...", "გთხოვთ მოიცადოთ!", settings: mySettings);
                controller.SetIndeterminate();
                string errorNo = "", errorMessage = "";
                int sCount = 0;
                List<RemainingItemEntry> rList = new List<RemainingItemEntry>();

                var orderNo = CurrentModel.Order.No_;
                await Task.Factory.StartNew(() =>
                {
                    sCount =
                        SendServiceManager.Current.SendOrders(orders
                             , true,
                            out errorNo, out errorMessage, out rList);
                });





                await controller.CloseAsync();
                if (CurrentModel.Order.Sell_toCustomerNo == App.Current.PosSetting.Settings_GenCustomerCode)
                    OpenNewOrder();
                else
                    NavigateToControl(PrevUserControl);

            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var order = (this.DataContext as OrderViewModel).Order;


            byte[] result;

            using (var memoryStream = new MemoryStream())
            {
                var pdfWriter = new PdfWriter(memoryStream);
                var pdfDocument = new PdfDocument(pdfWriter);
                var document = new Document(pdfDocument, new PageSize(250, 500) ); //  , PageSize.A4, true);
                document.SetMargins(5, 5, 5, 5);
                PdfFont sylfaenfont = PdfFontFactory.CreateFont(@"c:\\windows\fonts\Sylfaen.ttf", PdfEncodings.IDENTITY_H);
                document.SetFont(sylfaenfont);
                document.Add(new Paragraph("შპს გასგო მოტორსი"));
                document.Add(new Paragraph(order.Ship_toAddress));
                document.Add(new Paragraph(order.PostingDate.HasValue ? order.PostingDate.Value.ToString("dd/MM/yyyy HH:mm") : ""));
                document.Add(new Paragraph(order.No_));

                document.Add(new LineSeparator(new DashedLine()));

                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 12,6 }))
                .UseAllAvailableWidth();
                var count = 0;
                table.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                table.SetFontSize(10);
                foreach(var l in order.SalesLines)
                {
                    count++;
                    table.AddCell(new Cell().Add(new Paragraph(count.ToString()) ).SetBorder(iText.Layout.Borders.Border.NO_BORDER) );
                    table.AddCell(new Cell().Add(new Paragraph(  $"{(string.IsNullOrEmpty(l.LargeDescription) ? "" : l.LargeDescription)}\n{l.Service_Provider_Name}\n{l.Customer_Vehicle}" ) ).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph( $"{l.Quantity:N0}*{l.UnitPrice:F2}={l.AmountIncludingVAT:F2} ₾"  ).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT) );
                }
                document.Add(table);

                DashedLine line = new DashedLine();
                document.Add(new LineSeparator(line));

                document.Add(new Paragraph($"ჯამი    {order.AmountIncludingVat:F2} ₾").SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetFontSize(15).SetBold() );

                document.Close();

                result = memoryStream.ToArray();

                pdfWebViewer.NavigateToStream(memoryStream);
            }
            var fileName = $@"C:\wr\check_{order.No_}_{Guid.NewGuid()}.pdf";
            File.WriteAllBytes(fileName, result);

            pdfWebViewer.Navigate(fileName);

            // pdfWebViewer.Visibility = Visibility.Visible;

        }
    }
}
