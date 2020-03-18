using PosClient.Helpers;
using PosClient.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLayer;
using DataLayer;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Button = System.Windows.Controls.Button;
using DataGrid = System.Windows.Controls.DataGrid;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Orientation = System.Windows.Controls.Orientation;
using ScrollBar = System.Windows.Controls.Primitives.ScrollBar;
using TextBox = System.Windows.Controls.TextBox;

namespace PosClient.Views
{
    public abstract class ReservesController : PosUserControl<Reserves, ReservesViewModel>
    {
         
    }

    public partial class Reserves : ReservesController
    {
        public Reserves()
        {
            InitializeComponent();
        }

        public override string HeaderText
        {
            get
            {
                return "მარაგები";
            }
        }

        public override List<string> PrevHeaders
        {
            get { return CurrentModel.ParentModel == null ? new List<string>() :  new List<string> { "დღის განრიგი", "ახალი შეკვეთა" }; }
        }

        public override bool ShowHomeBtn
        {
            get { return CurrentModel.ParentModel == null; }
        }

        public override UserControlTypes UserControlType
        {
            get
            {
                return UserControlTypes.Reserves;
            }
        }

        public override ScrollBarVisibility ScrollBarVis
        {
            get
            {
                return ScrollBarVisibility.Disabled;
            }
        }


        private void SetDataGridWidths()
        {
            try
            {
                if (File.Exists(@"C:\wr\reservesgrid_column_widths.txt"))
                {
                    string text = File.ReadAllText(@"C:\wr\reservesgrid_column_widths.txt");
                    var widths = text.Split(',').Select(i => double.Parse(i)).ToList();
                    int cnt = 0;
                    foreach (var c in ReservesGrid.Columns)
                    {
                        c.Width = new DataGridLength(widths[cnt++]);
                    }
                }
            }
            catch { }
        }

        private void SaveDataGridWidths()
        {
            try
            {
                var text = string.Join(",", ReservesGrid.Columns.Select(i => i.Width.Value).ToList());
                File.WriteAllText(@"C:\wr\reservesgrid_column_widths.txt", text);
            }
            catch { }
        }


        public override void Refresh()
        {

            SetDataGridWidths();

            tbx_filter.Text = "";
            CurrentModel.Refresh();


            
        }

        public DataGrid ActiveGrid
        {
            get { return CurrentModel.ParentModel != null ? ReservesGrid : ReservesGrid1; }
        }

        public override int HomeButtonSize
        {
            get { return 30; }
        }

        public override int HomeIconSize
        {
            get { return 15; }
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveDataGridWidths();
            CurrentModel.UpdateSalesLines();
            NavigateToControl(UserControlTypes.Order);
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            SaveDataGridWidths();
            NavigateToControl(UserControlTypes.Order);
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.SetOrderType(OrderTypes.Ok);
            CurrentModel.CalcMustVisible = false;
            CurrentModel.UpdateCalcVisibility();
            tbx_filter.SelectAll();
            tbx_filter.Focus();

            ReservesGrid.Items.Refresh();
        }

        public void tbx_filter_select()
        {
            tbx_filter.SelectAll();
            tbx_filter.Focus();
        }

        private void BtnQ_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.SetOrderType(OrderTypes.Q);
            CurrentModel.CalcMustVisible = false;
            CurrentModel.UpdateCalcVisibility();
            tbx_filter.SelectAll();
            tbx_filter.Focus();
            ReservesGrid.Items.Refresh();
        }

        private void BtnSP_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.SetOrderType(OrderTypes.SP);
            CurrentModel.CalcMustVisible = false;
            CurrentModel.UpdateCalcVisibility();
            tbx_filter.SelectAll();
            tbx_filter.Focus();
            ReservesGrid.Items.Refresh();
        }

        public void UpdateSelectedTextbox()
        {
            var s = GetSelectedTextbox;
            if (s != null && s != App.Current.TargetTextBox)
                App.Current.TargetTextBox = s;
        }

        private void BtnC_OnClick(object sender, RoutedEventArgs e)
        {
            UpdateSelectedTextbox();
            if (App.Current.TargetTextBox == null ) return;
            var c = (sender as Button).CommandParameter.ToString();
            var routedEvent = TextCompositionManager.TextInputEvent;
            App.Current.TargetTextBox.RaiseEvent(
              new TextCompositionEventArgs(
                InputManager.Current.PrimaryKeyboardDevice,
                new TextComposition(InputManager.Current, App.Current.TargetTextBox, c))
              { RoutedEvent = routedEvent }
            );
            App.Current.TargetTextBox.Focus();
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (_fromRowEnter)
                {
                    _fromRowEnter = false;
                    return;
                }
                else
                {
                    tbx_filter.SelectAll();
                    tbx_filter.Focus();
                    return;
                }
            }
            if (e.Key == Key.Down && ReservesGrid.SelectedIndex + 1 < ReservesGrid.Items.Count)
            {
                ReservesGrid.SelectedIndex++;
                var cell = PosTools.GetCell(ReservesGrid, ReservesGrid.SelectedIndex, 3);
                // ReservesGrid.CurrentCell = new DataGridCellInfo(ReservesGrid.CurrentItem, ReservesGrid.Columns[0]);
                var tbx = PosTools.GetChildOfType<TextBox>(cell);
                tbx.Focus();
                // tbx.SelectAll();
            }
            else if (e.Key == Key.Up && ReservesGrid.SelectedIndex > 0)
            {
                ReservesGrid.SelectedIndex--;
                FocusSelectedItemText();
            }
            e.Handled = true;
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
        }

        private async void Btn_next_OnClick(object sender, RoutedEventArgs e)
        {
            GoNext();
        }

        private void Btn_prev_OnClick(object sender, RoutedEventArgs e)
        {
            GoPrev();
        }

        public void FocusSelectedItemText()
        {
            if(ActiveGrid == ReservesGrid1) return;
            if (ActiveGrid.SelectedIndex >= 0)
            {
                var cell = PosTools.GetCell(ActiveGrid, ActiveGrid.SelectedIndex, 3);
                var tbx = PosTools.GetChildOfType<TextBox>(cell);
                if( tbx != null)
                    tbx.Focus();
            }
        }

        private FrameworkElement GetSelectedTextbox
        {
            get
            {
                if (grid_cq.Visibility == Visibility.Visible)
                {
                    return tbx_cq;
                }
                if (ActiveGrid == ReservesGrid1) return null;
                if (ActiveGrid.SelectedIndex >= 0)
                {
                    var cell = PosTools.GetCell(ActiveGrid, ActiveGrid.SelectedIndex, 3);
                    var tbx = PosTools.GetChildOfType<TextBox>(cell);
                    return tbx;
                }
                return null;
            }
        }

        private static ScrollBar GetScrollbar(DependencyObject dep, Orientation orientation)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dep); i++)
            {
                var child = VisualTreeHelper.GetChild(dep, i);
                var bar = child as ScrollBar;
                if (bar != null && bar.Orientation == orientation)
                    return bar;
                else
                {
                    ScrollBar scrollBar = GetScrollbar(child, orientation);
                    if (scrollBar != null)
                        return scrollBar;
                }
            }
            return null;
        }

        public void ScrollToSelectedItem(bool up = false)
        {
            if (CurrentModel.SelectedItem != null)
            {
                tbx_filter.Foreground = Brushes.Black;
                ActiveGrid.ScrollIntoView(CurrentModel.SelectedItem);
                if (up)
                {
                    //var verticalScrollBar = GetScrollbar(ActiveGrid, Orientation.Vertical);
                    //var firstRow = verticalScrollBar.Value;
                    //if (ActiveGrid.SelectedIndex - firstRow > 5)
                    //{
                    //    var index = ActiveGrid.SelectedIndex + ActiveGrid.SelectedIndex - firstRow - 2;
                    //    if (CurrentModel.Items.Count > index && index >= 0)
                    //    {
                    //        ActiveGrid.ScrollIntoView(CurrentModel.Items[(int)index]);
                    //    }
                    //}

                }
            }
            else
            {
                tbx_filter.Foreground = Brushes.Red;
            }

        }

        private async void TextBox_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                FocusSelectedItemText();
                ScrollToSelectedItem();
            }
            else if (e.Key == Key.F3)
            {
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                {
                    GoPrev();
                }
                else
                {
                    GoNext();
                }
            }
            else if (e.Key == Key.Up)
            {
                GoPrevIndex();
                tbx_filter.SelectAll();
            }
            else if (e.Key == Key.Down)
            {
                GoNextIndex();
                tbx_filter.SelectAll();
            }
            else if (e.Key == Key.Delete)
            {
                // delete key
                tbx_filter.Text = "";
            }
        }

        private async void GoNext()
        {
            var cr = CurrentModel;
            await Task.Factory.StartNew(() => { cr.goNext(); });
            ScrollToSelectedItem();
        }

        private async void GoPrev()
        {
            var cr = CurrentModel;
            await Task.Factory.StartNew(() => { cr.goPrev(); });
            ScrollToSelectedItem();
        }

        private  void GoNextIndex()
        {
            CurrentModel.goNextIndex();
            ScrollToSelectedItem();
        }

        private  void GoPrevIndex()
        {
            CurrentModel.goPrevIndex();
            ScrollToSelectedItem();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
             CurrentModel.SelectedItemImage = (sender as Image).DataContext as BitmapImage;
             CurrentModel.UpdateSelectedImage();
        }

        private void ImageMain_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            CurrentModel.CalcMustVisible = true;
            CurrentModel.UpdateCalcVisibility();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            CurrentModel.CalcMustVisible = true;
            CurrentModel.UpdateCalcVisibility();
        }

        private void BtnCloseCalculator_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.CalcMustVisible = false;
            CurrentModel.UpdateCalcVisibility();
        }

        private async void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if (CurrentModel.SelectedItem != null)
            {
                try
                {
                    string s = OrdersManager.Current.GetItemInventoryByLocations(CurrentModel.SelectedItem.No_);
                    s = s.Replace(@"\n", "\n");
                    var dialog = (BaseMetroDialog) this.Resources["itemInventoriesDetail"];

                    dialog.DataContext = s;
                    currentInventoryDialog = dialog;
                    await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
                    // await App.Current.CurrentMainWindow.ShowMessageAsync(s, s);

                }
                catch (Exception ex)
                {
                    App.Current.ShowErrorDialog("კავშირის პრობლემა", ex.Message);
                }
            }
        }


        private BaseMetroDialog currentInventoryDialog;

        private async void Hp_comments_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = (BaseMetroDialog)this.Resources["itemCommentsDetail"];
            dialog.DataContext = new ItemCommentsDetailViewModel(CurrentModel.SelectedItem.No_);
            await App.Current.CurrentMainWindow.ShowMetroDialogAsync(dialog);
            
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            await App.Current.CurrentMainWindow.HideMetroDialogAsync(currentInventoryDialog);
        }

        private void Tbx_filter_OnGotFocus(object sender, RoutedEventArgs e)
        {
            CurrentModel.CalcMustVisible = false;
            CurrentModel.UpdateCalcVisibility();
        }

        private void ReservesGrid1_KeyUp(object sender, KeyEventArgs e)
        {
            if ( (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
            {
                
                e.Handled = true;
                tbx_filter.RaiseEvent(
                  new TextCompositionEventArgs(
                    InputManager.Current.PrimaryKeyboardDevice,
                    new TextComposition(InputManager.Current, tbx_filter, e.Key.ToString().Substring(e.Key.ToString().Length - 1) ))
                  { RoutedEvent = TextCompositionManager.TextInputEvent }
                );
                tbx_filter.Focus();
            }
        }

        private void Image_MouseLeftButtonDown1(object sender, MouseButtonEventArgs e)
        {
            CurrentModel.GotoSuggestedIndex(0);


        }

        private void Image_MouseLeftButtonDown2(object sender, MouseButtonEventArgs e)
        {
            CurrentModel.GotoSuggestedIndex(1);
        }

        private void Image_MouseLeftButtonDown3(object sender, MouseButtonEventArgs e)
        {
            CurrentModel.GotoSuggestedIndex(2);
        }

        private bool _fromRowEnter;
        private void ReservesGrid_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                if (ActiveGrid.SelectedIndex >= 0)
                {
                    var cell = PosTools.GetCell(ActiveGrid, ActiveGrid.SelectedIndex, 3);
                    var tbx = PosTools.GetChildOfType<TextBox>(cell);
                    if (!tbx.IsFocused)
                    {
                        _fromRowEnter = true;
                        FocusSelectedItemText();
                    }
                }
            }
        }

        private void ReservesGrid1_OnKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.A && e.Key <= Key.Z)  )
            {
                //e.Handled = true;
                //tbx_filter.RaiseEvent(
                //  new TextCompositionEventArgs(
                //    InputManager.Current.PrimaryKeyboardDevice,
                //    new TextComposition(InputManager.Current, tbx_filter, e.Key.ToString()))
                //  { RoutedEvent = TextCompositionManager.TextInputEvent }
                //);
                tbx_filter.Focus();
            }
        }

        private void Btn_next_grid_OnClick(object sender, RoutedEventArgs e)
        {
            if (ActiveGrid.SelectedIndex + 1 < ActiveGrid.Items.Count)
            {
                ActiveGrid.SelectedIndex++;
                ScrollToSelectedItem();
            }
        }

        private void Btn_prev_grid_OnClick(object sender, RoutedEventArgs e)
        {
            if ( ActiveGrid.SelectedIndex > 0)
            {
                ActiveGrid.SelectedIndex--;
                ScrollToSelectedItem();
            }
        }

        private void VehicleModels1_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(System.DateTime))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
            if (e.PropertyType == typeof(Nullable<System.DateTime>))
                (e.Column as DataGridTextColumn).Binding.StringFormat = "dd/MM/yyyy";
        }

        private void Btn_like_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.UpdateLike(true);
        }

        private void Btn_unlike_OnClick(object sender, RoutedEventArgs e)
        {
            CurrentModel.UpdateLike(false);
        }

        private async void tbx_filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            int startLength = tb.Text.Length;

            await Task.Delay(200);
            if (startLength == tb.Text.Length)
                CurrentModel.FilterString = tb.Text;

        }

        private void BtnCQ_OnClick(object sender, RoutedEventArgs e)
        {
            if (grid_cq.Visibility == Visibility.Collapsed)
            {
                tbx_cq.Text = "";
                grid_cq.Visibility = Visibility.Visible;
                tbx_cq.Focus();
            }
            else
            {
                grid_cq.Visibility = Visibility.Collapsed;
                tbx_cq.Text = "";
            }
        }

        private void ClearCQ()
        {
            grid_cq.Visibility = Visibility.Collapsed;
            tbx_cq.Text = "";
        }

        private void BtnOk_cq_OnClick(object sender, RoutedEventArgs e)
        {
            decimal res;
            if (decimal.TryParse(tbx_cq.Text, out res))
            {

                CurrentModel.SetOrderType(OrderTypes.CQ, res);
                CurrentModel.CalcMustVisible = false;
                CurrentModel.UpdateCalcVisibility();
                tbx_filter.SelectAll();
                tbx_filter.Focus();
                ReservesGrid.Items.Refresh();
            }
            else
            {
                ClearCQ();
            }
        }

        private void Tbx_cq_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                BtnOk_cq_OnClick(null, null);
            }
        }

    }
}
