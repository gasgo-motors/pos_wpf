using DataLayer;
using PosClient.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using BusinessLayer;
using DataLayer.Models;
using PosClient.Views;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Media;

namespace PosClient.ViewModels
{
    public class ReservesViewModel : PosViewModel
    {
        public OrderViewModel ParentModel { get; set; }

        public ReservesViewModel()
        {
            _firstMatchFiltering = true;
            datagridLoadingVisibility = Visibility.Collapsed;
        }

        public ReservesViewModel(OrderViewModel parentModel)
        {
            _firstMatchFiltering = true;
            datagridLoadingVisibility = Visibility.Collapsed;
            ParentModel = parentModel;
            _sumVisible = false;
        }

        public List<decimal> ColWidths { get; set; }

        public IEnumerable<ISalesLine> ParentSalesLines
        {
            get { return ParentModel.SalesLines; }
        }

        public Visibility FilterNavigationVisibility
        {
            get { return FirstMatchFiltering ? Visibility.Visible : Visibility.Collapsed; }
        }

        public byte[] SelectedImageBytes { get; set; }

        public List<byte[]> SelectedItemImagesBytes { get; set; }


        private bool _sumVisible;
        public bool SumVisible
        {
            get { return _sumVisible; }
            set
            {
                if (_sumVisible != value)
                {
                    _sumVisible = value;
                    RaisePropertyChanged(() => SumVisible);
                    RaisePropertyChanged(() => SumVisibility);
                }
            }
        }

        public Visibility SumVisibility
        {
            get { return _sumVisible ? Visibility.Visible : Visibility.Collapsed; }
        }


        public Visibility IsSelectedVisibility
        {
            get { return SelectedItem != null ? Visibility.Visible : Visibility.Collapsed; }
        }

        private BitmapImage ConvertBytesToImage(byte[] b)
        {
            if (b == null)
                return null;
            try
            {
                using (var ms = new System.IO.MemoryStream(b))
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad; // here
                    ms.Position = 0;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
            catch
            {
                return null;
            }
        }

        public BitmapImage SelectedItemImage { get; set; }

        public void UpdateSelectedImage()
        {
            RaisePropertyChanged(() => SelectedItemImage);
        }

        public List<BitmapImage> SelectedItemImages
        {
            get;
            set;
        }


        private bool _firstMatchFiltering;

        public bool FirstMatchFiltering
        {
            get { return _firstMatchFiltering; }
            set
            {
                if (_firstMatchFiltering != value)
                {
                    _firstMatchFiltering = value;
                    RaisePropertyChanged(() => FirstMatchFiltering);
                    RaisePropertyChanged(() => FilterNavigationVisibility);
                    RefreshGrid();
                }
            }
        }

        private List<Tuple<string, byte[]>> _sugestedImages { get; set; }

        public List<ItemVehicleModel> VehicleModels
        {
            get;
            set;
        }
        public List<AdditionalParameter> AdditionalParameters { get; set; }
        public List<ProjectedItemReceipt> ProjectedItems { get; set; }

        public List<ShortItemEntry> Items { get; set; }

        private List<ShortItemEntry> _itemsView;

        public List<ShortItemEntry> Itemsview
        {
            get { return _itemsView; }
            set
            {
                if (_itemsView != value)
                {
                    _itemsView = value;
                    RaisePropertyChanged(() => Itemsview);
                }
            }
        }

        public bool _withBalance;
        public bool WithBalance
        {
            get { return _withBalance; }
            set
            {
                if (_withBalance != value)
                {
                    _withBalance = value;
                    RefreshGrid();
                    //Itemsview.Refresh();
                }
            }
        }

        private List<ProjectedItemReceipt> _projectedItemsList;


        public Visibility _datagridLoadingVisibility;
        public Visibility datagridLoadingVisibility
        {
            get { return _datagridLoadingVisibility; }
            set
            {
                if (_datagridLoadingVisibility != value)
                {
                    _datagridLoadingVisibility = value;
                    RaisePropertyChanged(() => datagridLoadingVisibility);
                }
            }
        }



        public bool _loadingisVisible { get; set; }



        public Visibility LoadingVisibility
        {
            get { return _loadingisVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        public List<ProjectedItemReceipt> ProjectedItemsList
        {
            get { return _projectedItemsList; }
            set
            {
                if (_projectedItemsList != value)
                {
                    _projectedItemsList = value;
                    RaisePropertyChanged(() => ProjectedItemsList);
                }
            }
        }

        private List<AdditionalParameter> _additionalParametersList;

        public List<AdditionalParameter> AdditionalParametersList
        {
            get { return _additionalParametersList; }
            set
            {
                if (_additionalParametersList != value)
                {
                    _additionalParametersList = value;
                    RaisePropertyChanged(() => AdditionalParametersList);
                }
            }
        }


        public List<VehicleModel> VehicleModelsList { get; set; }

        internal void goNext()
        {
            var curIndex = -1;
            if (SelectedItem != null)
                curIndex = Itemsview.IndexOf(SelectedItem);
            for (int i = curIndex + 1; i < Itemsview.Count; i++)
            {
                if (itemIsFiltered(Itemsview[i], false))
                {
                    SelectedItem = Itemsview[i];
                    break;
                }
            }
        }

        internal void goNextIndex()
        {
            var curIndex = -1;
            if (SelectedItem != null)
                curIndex = Itemsview.IndexOf(SelectedItem);
            if (curIndex + 1 < Itemsview.Count)
            {
                SelectedItem = Itemsview[curIndex + 1];
            }
        }

        internal void goPrevIndex()
        {
            var curIndex = -1;
            if (SelectedItem != null)
                curIndex = Itemsview.IndexOf(SelectedItem);
            if (curIndex - 1 >= 0)
            {
                SelectedItem = Itemsview[curIndex - 1];
            }
        }

        internal void goPrev()
        {
            var curIndex = -1;
            if (SelectedItem != null)
                curIndex = Itemsview.IndexOf(SelectedItem);
            for (int i = curIndex - 1; i >= 0; i--)
            {
                if (itemIsFiltered(Itemsview[i], false))
                {
                    SelectedItem = Itemsview[i];
                    break;
                }
            }
        }

        private string _filterString;
        public string FilterString
        {
            get { return _filterString; }
            set
            {
                if (_filterString != value)
                {
                    _filterString = value;
                    if (!_firstMatchFiltering)
                        RefreshGrid();
                    else
                    {
                        Reserves.Current.ActiveGrid.Items.Refresh();
                        SetFirstMathingRow();
                    }
                }
            }
        }

        private async void SetFirstMathingRow()
        {
            datagridLoadingVisibility = Visibility.Visible;
            await Task.Factory.StartNew(() =>
            {
                SelectedItem = Items.FirstOrDefault(i => itemIsFiltered(i, false));
            });
            datagridLoadingVisibility = Visibility.Collapsed;
            Reserves.Current.ScrollToSelectedItem(true);
        }


        public List<string> BrandsList { get; set; }
        private string _selectedBrand;
        public string SelectedBrand
        {
            get { return _selectedBrand; }
            set
            {
                if (_selectedBrand != value)
                {
                    _selectedBrand = value;
                    //Itemsview.Refresh();
                    RefreshGrid();
                }
            }
        }
        public List<Manufacturer> ManufactureList { get; set; }
        private string _selectedManufacture;
        public string SelectedManufacture
        {
            get { return _selectedManufacture; }
            set
            {
                if (_selectedManufacture != value)
                {
                    _selectedManufacture = value;
                    //Itemsview.Refresh();
                    RefreshGrid();
                }
            }
        }

        public List<VehicleGroup> VehicleGroupList { get; set; }
        private VehicleGroup _selectedVehicleGroup;
        public VehicleGroup SelectedVehicleGroup
        {
            get { return _selectedVehicleGroup; }
            set
            {
                if (_selectedVehicleGroup != value)
                {
                    _selectedVehicleGroup = value;
                    Reserves.Current.tbx_filter_select();
                    //Itemsview.Refresh();
                    RefreshGrid();
                }
            }
        }


        public List<Tuple<string, string>> ModelNosList { get; set; }
        private string _selectedModelNo;
        public string SelectedModelNo
        {
            get { return _selectedModelNo; }
            set
            {
                if (_selectedModelNo != value)
                {
                    _selectedModelNo = value;
                    //Itemsview.Refresh();
                    RefreshGrid();
                }
            }
        }
        //public List<string> CabTypesList { get; set; }
        //private string _selectedCabType;
        //public string SelectedCabType
        //{
        //    get { return _selectedCabType; }
        //    set
        //    {
        //        if (_selectedCabType != value)
        //        {
        //            _selectedCabType = value;
        //            RaisePropertyChanged(() => SelectedCabType);
        //            Itemsview.Refresh();
        //        }
        //    }
        //}
        //public List<string> EnglinesList { get; set; }
        //private string _selectedEngline;
        //public string SelectedEngline
        //{
        //    get { return _selectedEngline; }
        //    set
        //    {
        //        if (_selectedEngline != value)
        //        {
        //            _selectedEngline = value;
        //            RaisePropertyChanged(() => SelectedEngline);
        //            Itemsview.Refresh();
        //        }
        //    }
        //}
        public List<int?> YearsFrom { get; set; }
        private int? _selectedYearFrom;
        public int? SelectedYearFrom
        {
            get { return _selectedYearFrom; }
            set
            {
                if (_selectedYearFrom != value)
                {
                    _selectedYearFrom = value;
                    //Itemsview.Refresh();
                    RefreshGrid();
                }
            }
        }

        public List<int?> YearsTo { get; set; }
        private int? _selectedYearTo;
        public int? SelectedYearTo
        {
            get { return _selectedYearTo; }
            set
            {
                if (_selectedYearTo != value)
                {
                    _selectedYearTo = value;
                    //Itemsview.Refresh();
                    RefreshGrid();
                }
            }
        }

        private string _selectedOeNumber;
        public string SelectedOeNumber
        {
            get { return _selectedOeNumber; }
            set
            {
                if (_selectedOeNumber != value)
                {
                    _selectedOeNumber = value;
                    RefreshGrid();
                }
            }
        }



        public bool? Like { get; set; }

        public List<BitmapImage> RelatedPicturesImages { get; set; }
        private List<Tuple<string, byte[]>> _RelatedItemPictures;
        public List<Tuple<string, byte[]>> RelatedItemPictures
        {
            get { return _RelatedItemPictures; }
            set
            {
                if (_RelatedItemPictures != value)
                {
                    _RelatedItemPictures = value;
                    RaisePropertyChanged(() => RelatedItemPictures);
                }
            }
        }

        private Task activeTask;
        private Task waitingTask;

        private void UpdateItemInfo(string itemNo)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    _loadingisVisible = true;
                    RaisePropertyChanged(() => LoadingVisibility);
                }));


            var _projectedItemsList = new List<ProjectedItemReceipt>();
            var _itemVehicleModelsList = new List<VehicleModel>();
            var _additionalParametersList = new List<AdditionalParameter>();
            List<Tuple<string, byte[]>> _relatedItemPictures = null;
            byte[] _selectedImageBytes = null;
            List<byte[]> _selectedItemImagesBytes = null;
            BitmapImage _selectedItemImage = null;
            List<BitmapImage> _selectedItemImages = null;
            List<BitmapImage> _relatedPicturesImages = new List<BitmapImage>() { null, null, null };
            bool? _like = null;
            var _selectedItemBox = "";
            if (itemNo != null)
            {
                _projectedItemsList =
                    ProjectedItems.Where(i => i.ItemNo_ == itemNo)
                        .OrderByDescending(i => i.ReceiptDate)
                        .ToList();
                _itemVehicleModelsList = DaoController.Current.GetItemVehicleModelsList(itemNo); //  VehicleModels.Where(i => i.ItemNo_ == itemNo).ToList();
                _additionalParametersList =
                    AdditionalParameters.Where(i => i.TableID == 27 && i.No_ == itemNo).ToList();
                var image = DaoController.Current.GetItemImage(itemNo);
                _selectedImageBytes = image.Count > 0 ? image.First() : null;
                _selectedItemImagesBytes = image;
                if (App.Current.User.UserType == PosUserTypes.Shop)
                    _relatedItemPictures = DaoController.Current.GetRelatedItemImages(itemNo);
                _selectedItemBox = DaoController.Current.GetItemBox(itemNo);
                if (ParentModel != null)
                {
                    _like = DaoController.Current.GetItemCustomerLike(itemNo, ParentModel.Order.Sell_toCustomerNo);
                }
            }
            _selectedItemImage = ConvertBytesToImage(_selectedImageBytes);
            _selectedItemImages = _selectedItemImagesBytes != null ? _selectedItemImagesBytes.Select(ConvertBytesToImage).ToList() : null;
            if (App.Current.User.UserType == PosUserTypes.Shop)
                _relatedPicturesImages = _relatedItemPictures != null ? _relatedItemPictures.Select(i => i != null ? ConvertBytesToImage(i.Item2) : null).ToList() : _relatedPicturesImages;

            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    activeTask = null;
                    if (waitingTask != null)
                    {
                        activeTask = waitingTask;
                        waitingTask = null;
                        activeTask.Start();
                    }
                    else
                    {
                        SelectedImageBytes = _selectedImageBytes;
                        SelectedItemImagesBytes = _selectedItemImagesBytes;
                        SelectedItemImage = _selectedItemImage;
                        ProjectedItemsList = _projectedItemsList;
                        VehicleModelsList = _itemVehicleModelsList;
                        RaisePropertyChanged(() => VehicleModelsList);
                        AdditionalParametersList = _additionalParametersList;
                        if (App.Current.User.UserType == PosUserTypes.Shop)
                        {
                            RelatedItemPictures = _relatedItemPictures;
                            RelatedPicturesImages = _relatedPicturesImages;
                        }
                        RaisePropertyChanged(() => RelatedPicturesImages);
                        RaisePropertyChanged(() => SelectedItemImage);
                        SelectedItemImages = _selectedItemImages;
                        RaisePropertyChanged(() => SelectedItemImages);
                        _loadingisVisible = false;
                        RaisePropertyChanged(() => LoadingVisibility);
                        SelectedItemBox = string.IsNullOrEmpty(_selectedItemBox) ? "" : "BOX " + _selectedItemBox;
                        RaisePropertyChanged(() => SelectedItemBox);
                        Like = _like;
                        RaisePropertyChanged(() => Like);
                        RaisePropertyChanged(() => LikeColor);
                        RaisePropertyChanged(() => UnLikeColor);
                    }
                }));
        }

        public string LikeColor
        {
            get { return Like == true ? "Black" : "#CCA8A8A8"; }
        }

        public string UnLikeColor
        {
            get { return Like == false ? "Black" : "#CCA8A8A8"; }
        }


        public List<ShortItemEntry> SelectedItems { get; set; }

        private ShortItemEntry _selectedItem;
        public ShortItemEntry SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    string _no = _selectedItem?.No_;
                    if (activeTask == null)
                    {
                        activeTask = new Task(() => UpdateItemInfo(_no));
                        activeTask.Start();
                    }
                    else
                    {
                        waitingTask = new Task(() => UpdateItemInfo(_no));
                    }
                    RaisePropertyChanged(() => SelectedItem);
                    RaisePropertyChanged(() => IsSelectedVisibility);
                }
            }
        }

        public Visibility OrderVisibility
        {
            get
            {
                return ParentModel != null ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public string SelectedItemBox { get; set; }

        public bool CalcMustVisible { get; set; }

        public Visibility CalcVisibility
        {
            get { return ParentModel != null && CalcMustVisible ? Visibility.Visible : Visibility.Collapsed; }
        }

        public void UpdateCalcVisibility()
        {
            RaisePropertyChanged(() => CalcVisibility);
            Reserves.Current.grid_cq.Visibility = Visibility.Collapsed;
        }

        public Visibility ReservesVisibility
        {
            get { return ParentModel == null ? Visibility.Visible : Visibility.Collapsed; }
        }

        public void SetOrderType(OrderTypes ot, decimal? unitPriceCq = null)
        {
            if (SelectedItem.SelectedQuantity == null)
                SelectedItem.OrderType = null;
            else
            {
                SelectedItem.OrderType = ot;
                SelectedItem.UnitPriceCQ = unitPriceCq;
            }
            RaisePropertyChanged(() => Itemsview);
            RaisePropertyChanged(() => Summary);
        }

        private void FillFilterValues()
        {
            BrandsList = DaoController.Current.getBrandLists();
            BrandsList.Insert(0, null);
            RaisePropertyChanged(() => BrandsList);

            ManufactureList = DaoController.Current.getManufactureLists();
            ManufactureList.Insert(0, null);
            RaisePropertyChanged(() => ManufactureList);

            VehicleGroupList = DaoController.Current.getVehicleGroupList();
            VehicleGroupList.Insert(0, null);


            RaisePropertyChanged(() => VehicleGroupList);




            ModelNosList = DaoController.Current.getModelNosLists();
            ModelNosList.Insert(0, null);
            RaisePropertyChanged(() => ModelNosList);

            //CabTypesList = DaoController.Current.getCabTypesLists();
            //CabTypesList.Insert(0, null);
            //RaisePropertyChanged(() => CabTypesList);

            YearsFrom =
                DaoController.Current.getManufactureYearsListsFrom().Select(i => (int?)i.Year).ToList().Distinct().ToList();
            YearsFrom.Insert(0, null);
            RaisePropertyChanged(() => YearsFrom);

            YearsTo =
                DaoController.Current.getManufactureYearsListsTo().Where(i => i.HasValue).Select(i => (int?)i.Value.Year).ToList().Distinct().ToList();
            YearsTo.Insert(0, null);
            RaisePropertyChanged(() => YearsTo);

            //EnglinesList = DaoController.Current.getEnginesLists();
            //EnglinesList.Insert(0, null);
            //RaisePropertyChanged(() => EnglinesList);

        }


        public async void GotoSuggestedIndex(int index)
        {
            string itemNo = App.Current.User.UserType == PosUserTypes.Shop
                ? _RelatedItemPictures[index].Item1
                : _suggestedImagesTmp[index].Item1;
            _selectedBrand = null;
            RaisePropertyChanged(() => SelectedBrand);
            _selectedManufacture = null;
            RaisePropertyChanged(() => SelectedManufacture);
            _selectedModelNo = null;
            RaisePropertyChanged(() => SelectedModelNo);
            _selectedYearFrom = null;
            RaisePropertyChanged(() => SelectedYearFrom);
            _selectedYearTo = null;
            RaisePropertyChanged(() => SelectedYearTo);
            _selectedOeNumber = null;
            RaisePropertyChanged(() => SelectedOeNumber);

            _filterString = null;
            //Reserves.Current.tbx_filter.Text = "";
            datagridLoadingVisibility = Visibility.Visible;
            List<ShortItemEntry> items = null;
            await Task.Factory.StartNew(() =>
            {
                items = GetFilteredItemsList(FirstMatchFiltering);
            });
            datagridLoadingVisibility = Visibility.Collapsed;
            Itemsview = items;


            datagridLoadingVisibility = Visibility.Visible;
            await Task.Factory.StartNew(() =>
            {
                SelectedItem = Items.FirstOrDefault(i => i.No_ == itemNo);
            });
            datagridLoadingVisibility = Visibility.Collapsed;
            Reserves.Current.ScrollToSelectedItem(true);
            CalcMustVisible = true;
            UpdateCalcVisibility();
            //Reserves.Current.FocusSelectedItemText();
        }

        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private int _index = 0;
        private List<Tuple<string, byte[]>> _suggestedImagesTmp;
        private void UpdateSuggestedImages()
        {
            var images = new List<Tuple<string, byte[]>>();
            for (int i = 0; i < 3 && _sugestedImages.Count > 0; i++)
            {
                images.Add(_sugestedImages[_index++]);
                if (_index == _sugestedImages.Count) _index = 0;
                if (images.Count == 3 || images.Count == _sugestedImages.Count) break;
            }
            //_index++;
            if (_index == _sugestedImages.Count) _index = 0;
            if (images.Count < 3) images.Add(null);
            if (images.Count < 3) images.Add(null);
            if (images.Count < 3) images.Add(null);
            RelatedPicturesImages = images.Select(i => i != null ? ConvertBytesToImage(i.Item2) : null).ToList();
            _suggestedImagesTmp = images;
            RaisePropertyChanged(() => RelatedPicturesImages);
        }

        public void Refresh()
        {



            VehicleModels = DaoController.Current.GetItemVehicleModels();
            ProjectedItems = DaoController.Current.GetProjectedItemReceipts();
            AdditionalParameters = DaoController.Current.AdditionalParameters();
            VehicleModelsList = DaoController.Current.GetVehicleModelsList();
            RaisePropertyChanged(() => VehicleModelsList);
            FillFilterValues();


            if (App.Current.User.UserType != PosUserTypes.Shop)
            {
                _sugestedImages = DaoController.Current.GetSuggestedImages(ParentModel.Order.Sell_toCustomerNo);
                dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                _index = 0;
                UpdateSuggestedImages();
                dispatcherTimer.Tick += (e, u) =>
                {
                    UpdateSuggestedImages();
                };
                var time = App.Current.PosSetting.Settings_ChangeSuggestionsInMinutes;
                int intTime;
                if (!string.IsNullOrEmpty(time) && int.TryParse(time, out intTime) && intTime > 15)
                {

                }
                else
                {
                    intTime = 20;
                }
                dispatcherTimer.Interval = new TimeSpan(0, 0, intTime);
                dispatcherTimer.Start();
            }
            else
            {
                _sugestedImages = null;
                if (dispatcherTimer != null)
                {
                    dispatcherTimer.Stop();
                    dispatcherTimer = null;
                }
            }

            var selectedItems = new List<ShortItemEntry>();
            if (ParentModel != null)
            {

                Items =
                    new List<ShortItemEntry>(
                        DaoController.Current.GetItems(ParentModel.Order.Sell_toCustomerNo, ParentModel.Order.CustomerPriceGroup, App.Current.PosSetting.Settings_Location).Select(i => new ShortItemEntry
                        {
                            No_ = i.No_,
                            Description = i.LargeDescription,
                            DescriptionForSalesLine = i.Description,
                            commentAs = i.CommentAS,
                            Quantity = i.quantitiy,
                            UnitPrice = i.unitnewprice,
                            ParentModel = this,
                            BaseUnitOfMeasure = i.BaseUnitOfMeasure,
                            ManufacturerCode = i.ManufacturerCode,
                            OeNumbers = DeserializeOeNumbers(i.OeNumbers),
                            VehicleGroups = DeserializeVehicleGroups(i.VehicleGroups)
                        }));
                if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                {
                    foreach (var sl in ParentModel.SalesLines)
                    {
                        var item = Items.FirstOrDefault(i => i.No_ == sl.No_);
                        if (item == null) continue;
                        item.SetQuantity(sl.Quantity.Value);
                        // item.SelectedQuantity = sl.Quantity;
                        item.OrderType = sl.OrderType.HasValue ? (OrderTypes?)sl.OrderType.Value : OrderTypes.Ok;
                        if (item.OrderType == OrderTypes.CQ)
                        {
                            item.UnitPriceCQ = sl.UnitPrice;
                        }
                        selectedItems.Add(item);
                    }
                }
            }
            else
            {
                Items =
                    new List<ShortItemEntry>(
                        DaoController.Current.GetItems("", "", App.Current.PosSetting.Settings_Location).Select(i => new ShortItemEntry
                        {
                            No_ = i.No_,
                            Description = i.LargeDescription,
                            DescriptionForSalesLine = i.Description,
                            commentAs = i.CommentAS,
                            Quantity = i.quantitiy,
                            UnitPrice = i.UnitPrice,
                            ParentModel = this,
                            BaseUnitOfMeasure = i.BaseUnitOfMeasure,
                            ManufacturerCode = i.ManufacturerCode,
                            OeNumbers = DeserializeOeNumbers(i.OeNumbers),
                            VehicleGroups = DeserializeVehicleGroups(i.VehicleGroups)
                        }));
            }
            SelectedItems = selectedItems;
            RaisePropertyChanged(() => Summary);
            RefreshGrid();
        }


        private async void RefreshGrid(bool setWithCurrentRow = false)
        {
            datagridLoadingVisibility = Visibility.Visible;
            List<ShortItemEntry> items = null;
            await Task.Factory.StartNew(() =>
        {
            items = GetFilteredItemsList(FirstMatchFiltering);
        });
            datagridLoadingVisibility = Visibility.Collapsed;
            Itemsview = items;
            if (items.Count > 0)
            {
                Reserves.Current.tbx_filter.Foreground = Brushes.Black;
            }
            else
            {
                Reserves.Current.tbx_filter.Foreground = Brushes.Red;
            }
            if (setWithCurrentRow)
                SetFirstMathingRow();
        }



        private bool itemIsFiltered(ShortItemEntry item, bool withFirstMatching)
        {
            return (withFirstMatching || (string.IsNullOrEmpty(_filterString) || item.Description.ToLower().Contains(_filterString.ToLower()) || item.commentAs.ToLower().Contains(_filterString.ToLower()))) &&
                   (string.IsNullOrEmpty(_selectedBrand) || item.ManufacturerCode == _selectedBrand) &&
                   (string.IsNullOrEmpty(_selectedManufacture) ||
                    VehicleModels.Any(i => i.ItemNo_ == item.No_ && i.ManufacturerCode == _selectedManufacture)) &&
                   (string.IsNullOrEmpty(_selectedModelNo) ||
                    VehicleModels.Any(i => i.ItemNo_ == item.No_ && i.ModelNo_ == _selectedModelNo)) &&
                   //(string.IsNullOrEmpty(_selectedCabType) ||
                   // VehicleModels.Any(i => i.ItemNo_ == item.No_ && i.CabType == _selectedCabType)) &&
                   (!_selectedYearFrom.HasValue ||
                    VehicleModels.Any(i => i.ItemNo_ == item.No_ && i.ManufacturingStartDate.Year == _selectedYearFrom.Value)) &&
                   (string.IsNullOrEmpty(_selectedOeNumber) || (item.OeNumbers != null && item.OeNumbers.Any(i => i.ToUpper() == _selectedOeNumber.ToUpper()))) &&
                   (_selectedVehicleGroup == null || (item.VehicleGroups != null && item.VehicleGroups.Any(i => i.ToUpper() == _selectedVehicleGroup.No.ToUpper()))) &&
                   (!_selectedYearTo.HasValue ||
                    VehicleModels.Any(i => i.ItemNo_ == item.No_ && (i.ManufacturingEndDate.HasValue && i.ManufacturingEndDate.Value.Year == _selectedYearTo.Value))) &&
                    //(string.IsNullOrEmpty(_selectedEngline) ||
                    // VehicleModels.Any(i => i.ItemNo_ == item.No_ && i.Engine == _selectedEngline)) &&
                    (!WithBalance || (WithBalance && item.Quantity > 0));
        }

        private List<ShortItemEntry> GetFilteredItemsList(bool withFirstMatching)
        {
            var res = new List<ShortItemEntry>();
            res = Items.Where(i => itemIsFiltered(i, withFirstMatching)).ToList();
            return res;
        }


        public decimal Summary
        {
            get
            {
                decimal _sum = 0;
                if (SelectedItems == null || SelectedItems.Count == 0) return _sum;
                foreach (var i in SelectedItems.Where(i => i.OrderType == OrderTypes.Ok || i.OrderType == OrderTypes.CQ))
                {
                    if (i.UnitPrice != null && i.SelectedQuantity != null)
                    {
                        var uPrice = i.OrderType == OrderTypes.Ok ? i.UnitPrice.Value : i.UnitPriceCQ.Value;
                        _sum += uPrice * i.SelectedQuantity.Value;
                    }
                }
                return _sum;
            }
        }

        public void UpdateSalesLines()
        {
            SelectedItems.RemoveAll(i => i.SelectedQuantity == null || i.SelectedQuantity == 0 || i.OrderType == null);
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
            {
                (ParentSalesLines as List<SalesLine>).RemoveAll(i => SelectedItems.All(j => j.No_ != i.No_));
                int maxIndex = GetMaxInd();
                foreach (var item in SelectedItems)
                {
                    var sl = ParentModel.SalesLines.FirstOrDefault(i => i.No_ == item.No_);
                    if (sl != null)
                    {
                        sl.Quantity = item.SelectedQuantity;
                        sl.OrderType = (int)item.OrderType.Value;
                        sl.UnitPrice = item.OrderType == OrderTypes.CQ && item.UnitPriceCQ.HasValue
                            ? item.UnitPriceCQ.Value
                            : sl.UnitPrice;
                        sl.AmountIncludingVAT = (item.OrderType == OrderTypes.Ok || item.OrderType == OrderTypes.CQ)
                            ? Math.Round(item.SelectedQuantity.Value * item.UnitPrice.Value, 2, MidpointRounding.AwayFromZero)
                            : 0;
                    }
                    else
                    {
                        maxIndex+=100;
                        AddNewSalesLine(CreateSalesLine(item, maxIndex));
                    }
                }
            }
            else
            {
                int maxIndex = GetMaxInd();
                foreach (var item in SelectedItems)
                {

                    maxIndex+=100;
                    AddNewSalesLine(CreateSalesLine(item, maxIndex));

                }
            }
            ParentModel.Order.AmountIncludingVat = ParentModel.SalesLines.Sum(i => i.AmountIncludingVAT);
            ParentModel.RefreshSummary();
        }

        private void AddNewSalesLine(ISalesLine line)
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                ((List<SalesLine>)ParentModel.SalesLines).Add(line as SalesLine);
            else
            {
                (line as ReleasedSalesLine).IsNew = true;
                ((List<ReleasedSalesLine>)ParentModel.SalesLines).Add(line as ReleasedSalesLine);
            }
        }

        public void UpdateLike(bool like)
        {
            if (SelectedItem == null) return;
            if (Like == like) return;
            DaoController.Current.UpdateLikeStatus(SelectedItem.No_, ParentModel.Order.Sell_toCustomerNo, like);
            Like = like;
            RaisePropertyChanged(() => Like);
            RaisePropertyChanged(() => LikeColor);
            RaisePropertyChanged(() => UnLikeColor);
        }

        private int GetMaxInd()
        {
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
                return ((List<SalesLine>)ParentModel.SalesLines).Count == 0 ? 0 : ParentModel.SalesLines.Max(i => i.LineNo_);
            else
                return ((List<ReleasedSalesLine>)ParentModel.SalesLines).Count == 0 ? 0 : ParentModel.SalesLines.Max(i => i.LineNo_);
        }

        private ISalesLine CreateSalesLine(ShortItemEntry item, int ind)
        {
            var price = ParentModel.Order.CurrentCustomer == null ? null : DaoController.Current.GetsalesPrice(item.No_, item.BaseUnitOfMeasure, ParentModel.Order.Sell_toCustomerNo, ParentModel.Order.CurrentCustomer.CustomerPriceGroup);
            if (price == null)
                price = item.UnitPrice;
            if (item.OrderType == OrderTypes.CQ)
                price = item.UnitPriceCQ;
            if (ParentModel.Order.OrderBaseType == OrderBaseTypes.Current)
            {

                return new SalesLine
                {
                    DocumentNo_ = ParentModel.Order.No_,
                    DocumentType = 1,
                    LineNo_ = ind,
                    Type = 2,
                    No_ = item.No_,
                    Description = item.DescriptionForSalesLine,
                    Quantity = item.SelectedQuantity,
                    UnitPrice = price,
                    Sell_toCustomerNo = ParentModel.Order.Sell_toCustomerNo,
                    LocationCode = App.Current.PosSetting.Settings_Location,
                    AmountIncludingVAT =
                        (item.OrderType == OrderTypes.Ok || item.OrderType == OrderTypes.CQ)
                            ? Math.Round(item.SelectedQuantity.Value * price.Value, 2,
                                MidpointRounding.AwayFromZero)
                            : 0,
                    UnitOfMeasureCode = item.BaseUnitOfMeasure,
                    OrderType = (int)item.OrderType.Value,
                    LineDiscountAmount = 0,
                    LineDiscountPercent = 0,
                    LargeDescription = item.Description
                };
            }
            else
            {
                return new ReleasedSalesLine
                {
                    DocumentNo_ = ParentModel.Order.No_,
                    DocumentType = 1,
                    LineNo_ = ind,
                    Type = 2,
                    No_ = item.No_,
                    Description = item.DescriptionForSalesLine,
                    Quantity = item.SelectedQuantity,
                    UnitPrice = item.OrderType == OrderTypes.CQ ? item.UnitPriceCQ : price,
                    Sell_toCustomerNo = ParentModel.Order.Sell_toCustomerNo,
                    LocationCode = App.Current.PosSetting.Settings_Location,
                    AmountIncludingVAT =
                        (item.OrderType == OrderTypes.Ok || item.OrderType == OrderTypes.CQ)
                            ? Math.Round(item.SelectedQuantity.Value * price.Value, 2,
                                MidpointRounding.AwayFromZero)
                            : 0,
                    UnitOfMeasureCode = item.BaseUnitOfMeasure,
                    OrderType = (int)item.OrderType.Value,
                    LineDiscountAmount = 0,
                    LineDiscountPercent = 0,
                    LargeDescription = item.Description
                };
            }
        }

        public static List<string> DeserializeOeNumbers(string xml)
        {
            var res = new List<string>();
            if (String.IsNullOrEmpty(xml))
                return res;
            XmlSerializer serializer = new XmlSerializer(typeof(OeNumbersXml));

            xml = string.Format("<?xml version='1.0'?><OeNumbersXml>{0}</OeNumbersXml>", xml);

            using (TextReader reader = new StringReader(xml))
            {
                var obj = (OeNumbersXml)serializer.Deserialize(reader);
                return obj.ParameterValueText;
            }
        }


        public static List<string> DeserializeVehicleGroups(string xml)
        {
            var res = new List<string>();
            if (String.IsNullOrEmpty(xml))
                return res;
            XmlSerializer serializer = new XmlSerializer(typeof(VehicleGroupsXml));

            xml = string.Format("<?xml version='1.0'?><groups>{0}</groups>", xml);

            using (TextReader reader = new StringReader(xml))
            {
                var obj = (VehicleGroupsXml)serializer.Deserialize(reader);
                return obj.VehicleGroupNo;
            }
        }

    }

    public enum OrderTypes
    {
        Ok,
        Q,
        SP,
        CQ
    }

    [XmlRoot("OeNumbersXml")]
    public class OeNumbersXml
    {
        [XmlElement("ParameterValueText")]
        public List<string> ParameterValueText = new List<string>();
    }

    [XmlRoot("groups")]
    public class VehicleGroupsXml
    {
        [XmlElement("VehicleGroupNo")]
        public List<string> VehicleGroupNo = new List<string>();
    }


    public class ShortItemEntry
    {
        public string SearchFilter
        {
            get { return ParentModel.FilterString; }
        }

        public List<string> OeNumbers { get; set; }
        public List<string> VehicleGroups { get; set; }

        public ReservesViewModel ParentModel;
        public string No_ { get; set; }
        public string Description { get; set; }
        public string DescriptionForSalesLine { get; set; }

        public Visibility CommentsVisibility
        {
            get { return !string.IsNullOrEmpty(commentAs) ? Visibility.Visible : Visibility.Collapsed; }
        }


        public string DescriptionFirst
        {
            get
            {
                if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(SearchFilter)) return Description;
                var ind = Description.ToLower().IndexOf(SearchFilter.ToLower());
                if (ind == -1)
                    return Description;
                return Description.Substring(0, ind);
            }
        }

        public string DescriptionHighLight
        {
            get
            {
                if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(SearchFilter)) return "";
                var ind = Description.ToLower().IndexOf(SearchFilter.ToLower());
                if (ind == -1)
                    return "";
                return Description.Substring(ind, SearchFilter.Length);
            }
        }

        public string DescriptionSecond
        {
            get
            {
                if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(SearchFilter)) return "";
                var ind = Description.ToLower().IndexOf(SearchFilter.ToLower());
                if (ind == -1)
                    return "";
                return Description.Substring(ind + SearchFilter.Length);
            }
        }



        public string CommentAsFirst
        {
            get
            {
                if (string.IsNullOrEmpty(commentAs) || string.IsNullOrEmpty(SearchFilter)) return commentAs;
                var ind = commentAs.ToLower().IndexOf(SearchFilter.ToLower());
                if (ind == -1)
                    return commentAs;
                return commentAs.Substring(0, ind);
            }
        }

        public string CommentAsHighLight
        {
            get
            {
                if (string.IsNullOrEmpty(commentAs) || string.IsNullOrEmpty(SearchFilter)) return "";
                var ind = commentAs.ToLower().IndexOf(SearchFilter.ToLower());
                if (ind == -1)
                    return "";
                return commentAs.Substring(ind, SearchFilter.Length);
            }
        }

        public string CommentAsSecond
        {
            get
            {
                if (string.IsNullOrEmpty(commentAs) || string.IsNullOrEmpty(SearchFilter)) return "";
                var ind = commentAs.ToLower().IndexOf(SearchFilter.ToLower());
                if (ind == -1)
                    return "";
                return commentAs.Substring(ind + SearchFilter.Length);
            }
        }

        public decimal? UnitPrice { get; set; }

        public decimal? UnitPriceCQ { get; set; }
        public decimal? Quantity { get; set; }

        public double? QuantityF2
        {
            get { return Quantity.HasValue ? (double?)Math.Round(Quantity.Value, 2) : null; }
        }

        private decimal? _selectedQuantity { get; set; }
        public string BaseUnitOfMeasure { get; set; }
        public string ManufacturerCode { get; set; }
        public string commentAs { get; set; }

        public void SetQuantity(decimal d)
        {
            _selectedQuantity = d;
        }
        public decimal? SelectedQuantity
        {
            get { return _selectedQuantity; }
            set
            {
                if (_selectedQuantity != value)
                {
                    _selectedQuantity = value;
                    if (_selectedQuantity == null)
                    {
                        OrderType = null;
                        ParentModel.SelectedItems.Remove(this);
                    }
                    else
                    {
                        if (OrderType == null)
                            OrderType = OrderTypes.Ok;
                        if (!ParentModel.SelectedItems.Contains(this))
                            ParentModel.SelectedItems.Add(this);
                    }
                    ParentModel.RaisePropertyChanged(() => ParentModel.Summary);
                }
            }
        }

        public OrderTypes? OrderType { get; set; }

        public string OrderTypeString
        {
            get { return (OrderType == null || OrderType.Value == OrderTypes.Ok) ? "" : OrderType.ToString(); }
        }
    }
}