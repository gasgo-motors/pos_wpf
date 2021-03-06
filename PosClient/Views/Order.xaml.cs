﻿using System;
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
using System.Threading;
using RawPrint;
using System.Drawing.Printing;
using Aspose.Pdf.Facades;

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
                {
                    if (!string.IsNullOrEmpty(errorNo) || !string.IsNullOrEmpty(errorMessage) || (rList != null &&  rList.Count > 0 ) )
                    {
                        var model = new SendToNavResultViewModel(sCount, errorNo, errorMessage, rList);
                        var dialog = (BaseMetroDialog) this.Resources["SendDetail"];
                        dialog.DataContext = model;
                        await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
                    }

                    NavigateToControl(PrevUserControl);
                }

            }
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            var order = (this.DataContext as OrderViewModel).Order;
            var salesLines = (this.DataContext as OrderViewModel).SalesLines;

            byte[] result;

            using (var memoryStream = new MemoryStream())
            {
                var pdfWriter = new PdfWriter(memoryStream);
                var pdfDocument = new PdfDocument(pdfWriter);
                var document = new Document(pdfDocument, new PageSize(270, 500) ); //  , PageSize.A4, true);
                document.SetMargins(2, 2, 2, 2);
                PdfFont sylfaenfont = PdfFontFactory.CreateFont(@"c:\\windows\fonts\Sylfaen.ttf", PdfEncodings.IDENTITY_H);
                document.SetFont(sylfaenfont);
                document.Add(new Paragraph("შპს გასგო მოტორსი"));
                document.Add(new Paragraph(order.Ship_toAddress != null ? order.Ship_toAddress : "" ));
                document.Add(new Paragraph(order.PostingDate.HasValue ? order.PostingDate.Value.ToString("dd/MM/yyyy HH:mm") : ""));
                document.Add(new Paragraph(order.No_));
                
                document.Add(new LineSeparator(new DashedLine()));

                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 1, 12,6 }))
                .UseAllAvailableWidth();
                var count = 0;
                table.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                table.SetFontSize(10);
                foreach(var l in salesLines)
                {
                    count++;
                    table.AddCell(new Cell().Add(new Paragraph(count.ToString()) ).SetBorder(iText.Layout.Borders.Border.NO_BORDER) );
                    var desc = $"{(string.IsNullOrEmpty(l.LargeDescription) ? "" : l.LargeDescription)}";
                    if(!string.IsNullOrEmpty(l.Service_Provider_Name))
                        desc = $"{desc}\n{l.Service_Provider_Name}";
                    if (!string.IsNullOrEmpty(l.Customer_Vehicle))
                        desc = $"{desc}\n{l.Customer_Vehicle}";
                    if (App.Current.User.UserType == PosUserTypes.Shop)
                    {
                        var shelfno = DaoController.Current.GetStockShelfNoByItemId(l.No_);
                        desc = $"{desc}\n{shelfno}";
                    }
                    table.AddCell(new Cell().Add(new Paragraph( desc ) ).SetBorder(iText.Layout.Borders.Border.NO_BORDER));
                    table.AddCell(new Cell().Add(new Paragraph( $"{l.Quantity:N0}*{l.UnitPrice:F2}={l.AmountIncludingVAT:F2} ₾"  ).SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT)).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.RIGHT) );
                }
                document.Add(table);

                DashedLine line = new DashedLine();
                document.Add(new LineSeparator(line));

                document.Add(new Paragraph($"ჯამი    {order.AmountIncludingVat:F2} ₾").SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT).SetFontSize(15).SetBold().SetMarginBottom(20) );


                document.Close();

                result = memoryStream.ToArray();


                pdfWebViewer.NavigateToStream(memoryStream);
            }



            var fileName = $@"C:\wr\check_{order.No_}_{Guid.NewGuid()}.pdf";
            File.WriteAllBytes(fileName, result);

            

            if (!string.IsNullOrEmpty(App.Current.PosSetting.Settings_Printers))
            {
                //var pr = "";
                //foreach (String printer in PrinterSettings.InstalledPrinters)
                //{
                //    pr =   printer.ToString();
                //}

                foreach (var printerName in App.Current.PosSetting.Settings_Printers.Split(',').ToList())
                {
                    printToPrinter(printerName, fileName);
                    //print1(printerName, fileName);
                    Thread.Sleep(4000);
                }
                return;
            }
            else
                pdfWebViewer.Navigate(fileName);


        }

        private void printToPrinter(string printerName, string filePath)
        {
            Spire.Pdf.PdfDocument pdfdocument = new Spire.Pdf.PdfDocument();
            pdfdocument.LoadFromFile(filePath);
            pdfdocument.PrinterName = printerName;

            PaperSize paperSize = new PaperSize();
            paperSize.Width = 283;//inch*100
            paperSize.Height = 826;//inch*100
            paperSize.RawKind = (int)PaperKind.Custom;
          //  pdfdocument.PrinterSettings.PaperSize = paperSize;

            //pdfdocument.war
            pdfdocument.PrintDocument.PrinterSettings.Copies = 1;
            pdfdocument.PrintDocument.Print();
            pdfdocument.Dispose();
        }

        public void print1(string printerName, string filePath)
        {
           // string dataDir = RunExamples.GetDataDir_AsposePdfFacades_Printing();
            // Create PdfViewer object
            PdfViewer viewer = new PdfViewer();
            // Open input PDF file
            viewer.BindPdf(filePath);
            // Set attributes for printing
            viewer.AutoResize = true;         // Print the file with adjusted size
            viewer.AutoRotate = true;         // Print the file with adjusted rotation
            viewer.PrintPageDialog = false;   // Do not produce the page number dialog when printing
            // Create objects for printer and page settings and PrintDocument
            System.Drawing.Printing.PrinterSettings ps = new System.Drawing.Printing.PrinterSettings();
            System.Drawing.Printing.PageSettings pgs = new System.Drawing.Printing.PageSettings();
            System.Drawing.Printing.PrintDocument prtdoc = new System.Drawing.Printing.PrintDocument();
            // Set printer name
            ps.PrinterName = prtdoc.PrinterSettings.PrinterName;
            // Set PageSize (if required)
            pgs.PaperSize = new System.Drawing.Printing.PaperSize("Custom", 280, 826);
            // Set PageMargins (if required)
            pgs.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            // ExStart:PrintDialog
            //System.Windows.Forms.PrintDialog printDialog = new System.Windows.Forms.PrintDialog();
            //if (printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //{
                // Document printing code goes here
                // Print document using printer and page settings
                viewer.PrintDocumentWithSettings(pgs, ps);
            //}
            // ExEnd:PrintDialog
            // Close the PDF file after priting
            viewer.Close();
        }


    }
}
