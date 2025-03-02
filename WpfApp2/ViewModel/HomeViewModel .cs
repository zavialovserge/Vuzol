using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Vuzol.Navigation;
using Vuzol.Services;
using Vuzol.ViewModel.Command;
using Vuzol.ViewModel.Model;
using Excel = Microsoft.Office.Interop.Excel;
using Word = Microsoft.Office.Interop.Word;
namespace Vuzol.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        private RelayCommand _addNewProperty;
        private RelayCommand _delCommand;
        private Property _selectedProperty;
        private RelayCommand _excelCommand;
        private RelayCommand _printForm;
        private RelayCommand _printAccountingForm;
        private NavigationProperty _navigationProperty;
        private string _factoryNumberFilter { get; set; }
        private string _nameFilter { get; set; }
        private string _inventoryNumberFilter { get; set; }
        private string _invoiceIdFilter { get; set; }
        private string _orderIdFilter { get; set; }
        private string _bookIdFilter { get; set; }
        private string _orderBookIdFilter { get; set; }
        private string _formIdFilter { get; set; }
        private string _statusNameFilter { get; set; }
        private string _fIO_R_STRFilter { get; set; }
        private string _unitNameFilter { get; set; }
        private string _quantityFilter { get; set; }
        private string _priceFilter { get; set; }
        private string _additionalnfoFilter { get; set; }
        public HomeViewModel(NavigationProperty NavigationProperty)
        {
            SelectedList = new ObservableCollection<Property>(PropertyData.GetAllProperty());
            SelectedProperty = SelectedList.First();
            AddCommand = new NavigateCommand<AddViewModel>(NavigationProperty,
                                                ()=>new AddViewModel(NavigationProperty, SelectedProperty));
            EditCommand = new NavigateCommand<AddViewModel>(NavigationProperty,
                                                () => new AddViewModel(NavigationProperty,SelectedProperty,true));
            ShowEmployees = new NavigateCommand<EmployeeViewModel>(NavigationProperty,
                                                () => new EmployeeViewModel(NavigationProperty));
            ShowUnits = new NavigateCommand<UnitViewModel>(NavigationProperty,
                                               () => new UnitViewModel(NavigationProperty));
            ShowRanks = new NavigateCommand<RankViewModel>(NavigationProperty,
                                               () => new RankViewModel(NavigationProperty));
            ShowPropertyType = new NavigateCommand<PropertyTypeViewModel>(NavigationProperty,
                                               () => new PropertyTypeViewModel(NavigationProperty));
            ShowPropertyStatus = new NavigateCommand<PropertyStatusViewModel>(NavigationProperty,
                                               () => new PropertyStatusViewModel(NavigationProperty));
            SelectedListSource = (CollectionView)CollectionViewSource.GetDefaultView(SelectedList);
            SelectedListSource.Filter =new Predicate<object>(o=> Filters(o as Property));
        }
        
        public CollectionView SelectedListSource { get;  set; }
        public Property SelectedProperty
        {
            get { return _selectedProperty; }
            set
            {
                _selectedProperty = value;
                
                OnPropertyChanged(nameof(_selectedProperty));
            }
        }
        public ObservableCollection<Property> SelectedList { get; set; }
       
        public ICommand Complectness { get; }
        public ICommand ComplectnessSoftWare { get; }
        public ICommand ShowEmployees { get; }
        public ICommand ShowUnits { get; }
        public ICommand ShowPropertyType { get; }
        public ICommand ShowRanks { get; }
        public ICommand ShowPropertyStatus { get; }
        public ICommand AddNewProperty
        {
            get
            {
                return _addNewProperty ?? (_addNewProperty = new RelayCommand(
                   property =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете додати елемент?", "Додати елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       Property prop = property as Property;
                       if (prop == null) return;
                       SelectedList.Clear();
                       if (!PropertyData.InsertIntoDb(prop)) return;
                       List<Property> allPropertyFromDb = PropertyData.GetAllProperty();
                       foreach (var propertyFromDb in allPropertyFromDb)
                       {
                           SelectedList.Add(propertyFromDb);
                       }
                   }));
            }
        }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DelCommand
        {
            get
            {
                return _delCommand ?? (_delCommand = new RelayCommand(
                   x =>
                   {
                       if (MessageBox.Show("Ви дійсно хочете видалити елемент?", "Видалити елемент",
                           MessageBoxButton.YesNo) != MessageBoxResult.Yes)
                       {
                           return;
                       }
                       if (SelectedProperty != null)
                       {
                           if (!PropertyData.DeletefFromDb(SelectedProperty)) return;
                           SelectedList.Clear();
                           List<Property> allPropertyFromDb = PropertyData.GetAllProperty();
                           foreach (var propertyFromDb in allPropertyFromDb)
                           {
                               SelectedList.Add(propertyFromDb);
                           }
                       }
                   }));
            }
        }
        private bool Filters(Property prop)
        {
            if (prop == null) return true;
            int FactoryNumberFilterInt = 0;
            int InventoryNumberFilterInt = 0;
            int InvoiceIdFilterInt = 0;
            int OrderIdFilterInt = 0;
            int BookIdFilterInt = 0;
            int OrderBookIdFilterInt = 0;

            bool canFactoryNumberFilter = int.TryParse(FactoryNumberFilter, out FactoryNumberFilterInt);
            bool canInventoryNumberFilter = int.TryParse(InventoryNumberFilter, out InventoryNumberFilterInt);
            bool canInvoiceIdFilter = (int.TryParse(InvoiceIdFilter, out InvoiceIdFilterInt));
            bool canOrderIdFilter = int.TryParse(OrderIdFilter, out OrderIdFilterInt);
            bool canBookIdFilter = int.TryParse(BookIdFilter, out BookIdFilterInt);
            bool canOrderBookIdFilter = int.TryParse(OrderBookIdFilter, out OrderBookIdFilterInt);
            
            if (!canFactoryNumberFilter
                && string.IsNullOrEmpty(NameFilter)
                && !canInventoryNumberFilter
                && string.IsNullOrEmpty(StatusNameFilter)
                && !canInvoiceIdFilter
                && !canOrderIdFilter
                && !canBookIdFilter
                && !canOrderBookIdFilter
                && string.IsNullOrEmpty(FormIdFilter)
                && string.IsNullOrEmpty(FIO_R_STRFilter)
                && string.IsNullOrEmpty(UnitNameFilter)
                )
                return true;

           if (FactoryNumberFilterInt != 0 || !string.IsNullOrEmpty(NameFilter) 
                || InventoryNumberFilterInt != 0 || !string.IsNullOrEmpty(StatusNameFilter)
                || InvoiceIdFilterInt != 0 || OrderIdFilterInt!=0 || BookIdFilterInt!=0 
                || OrderBookIdFilterInt != 0 || !string.IsNullOrEmpty(FormIdFilter)
                || !string.IsNullOrEmpty(FIO_R_STRFilter) || !string.IsNullOrEmpty(UnitNameFilter)
                )
                return (   (FactoryNumberFilterInt == 0 || FactoryNumberFilterInt == prop.FactoryNumber)
                        && (NameFilter == null || prop.Name.Contains(NameFilter))
                        && (InventoryNumberFilterInt == 0 || InventoryNumberFilterInt == prop.InventoryNumber)
                        && (StatusNameFilter == null || prop.StatusName.Contains(StatusNameFilter))
                        && (InvoiceIdFilterInt == 0 || InvoiceIdFilterInt == prop.InvoiceId)
                        && (OrderIdFilterInt == 0 || OrderIdFilterInt == prop.OrderId)
                        && (BookIdFilterInt == 0 || BookIdFilterInt == prop.BookId)
                        && (OrderBookIdFilterInt == 0 || OrderBookIdFilterInt == prop.OrderBookId)
                        && (FormIdFilter == null || prop.FormId.Contains(FormIdFilter))
                        && (FIO_R_STRFilter == null  || prop.FIO_R_STR.Contains(FIO_R_STRFilter))
                        && (UnitNameFilter == null || prop.UnitName.Contains(UnitNameFilter))
                        );

            return FactoryNumberFilterInt == prop.FactoryNumber
                   || prop.Name.Contains(NameFilter)
                   || InventoryNumberFilterInt == prop.InventoryNumber
                   || prop.StatusName.Contains(StatusNameFilter)
                   || InvoiceIdFilterInt == prop.InvoiceId
                   || OrderIdFilterInt == prop.OrderId
                   || BookIdFilterInt == prop.BookId
                   || OrderBookIdFilterInt == prop.OrderBookId
                   || prop.FormId.Contains(FormIdFilter)
                   || prop.FIO_R_STR.Contains(FIO_R_STRFilter)
                   || prop.FIO_R_STR.Contains(UnitNameFilter)
                   ;
        }
        public string FactoryNumberFilter
        {
            get { return _factoryNumberFilter; }
            set
            {
                _factoryNumberFilter = value;                
                OnPropertyChanged(nameof(_factoryNumberFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string NameFilter
        {
            get { return _nameFilter; }
            set
            {
                _nameFilter = value;
                OnPropertyChanged(nameof(_nameFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string InventoryNumberFilter
        {
            get { return _inventoryNumberFilter; }
            set
            {
                _inventoryNumberFilter = value;
                OnPropertyChanged(nameof(_inventoryNumberFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string StatusNameFilter
        {
            get { return _statusNameFilter; }
            set
            {
                _statusNameFilter = value;
                OnPropertyChanged(nameof(_statusNameFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string InvoiceIdFilter
        {
            get { return _invoiceIdFilter; }
            set
            {
                _invoiceIdFilter = value;
                OnPropertyChanged(nameof(_invoiceIdFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string OrderIdFilter
        {
            get { return _orderIdFilter; }
            set
            {
                _orderIdFilter = value;
                OnPropertyChanged(nameof(_orderIdFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string BookIdFilter
        {
            get { return _bookIdFilter; }
            set
            {
                _bookIdFilter = value;
                OnPropertyChanged(nameof(_bookIdFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string OrderBookIdFilter
        {
            get { return _orderBookIdFilter; }
            set
            {
                _orderBookIdFilter = value;
                OnPropertyChanged(nameof(_orderBookIdFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string FormIdFilter
        {
            get { return _formIdFilter; }
            set
            {
                _formIdFilter = value;
                OnPropertyChanged(nameof(_formIdFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string FIO_R_STRFilter
        {
            get { return _fIO_R_STRFilter; }
            set
            {
                _fIO_R_STRFilter = value;
                OnPropertyChanged(nameof(_fIO_R_STRFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string UnitNameFilter
        {
            get { return _unitNameFilter; }
            set
            {
                _unitNameFilter = value;
                OnPropertyChanged(nameof(_unitNameFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string QuantityFilter
        {
            get { return _quantityFilter; }
            set
            {
                _quantityFilter = value;
                OnPropertyChanged(nameof(_quantityFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string PriceFilter
        {
            get { return _priceFilter; }
            set
            {
                _priceFilter = value;
                OnPropertyChanged(nameof(_priceFilter));
                SelectedListSource?.Refresh();
            }
        }
        public string AdditionalnfoFilter
        {
            get { return _additionalnfoFilter; }
            set
            {
                _additionalnfoFilter = value;
                OnPropertyChanged(nameof(_additionalnfoFilter));
                SelectedListSource?.Refresh();
            }
        }
        public ICommand ExcelCommand
        {
            get
            {
                return _excelCommand ?? (_excelCommand = new RelayCommand(
                   x =>
                   {
                       OpenFileDialog fileDialog = new OpenFileDialog();
                       fileDialog.Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx";
                       if (fileDialog.ShowDialog() != true) return;
                       Excel.Application xlApp = new Excel.Application();
                       Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(fileDialog.FileName);
                       Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[1];
                       Excel.Range xlRange = xlWorksheet.UsedRange;
                      
                       int rCnt;
                       int rw = 0;
                       int cl = 0;
                       List<Property> properties = new List<Property>();

                       rw = xlRange.Rows.Count;
                       cl = xlRange.Columns.Count;
                       try
                       {                
                           
                           for (rCnt = 2; rCnt <= rw; rCnt++)
                           {
                               int factoryNumber = Convert.ToInt32((xlRange.Cells[rCnt, 1] as Excel.Range).Value);
                               string? Name = Convert.ToString((xlRange.Cells[rCnt, 2] as Excel.Range).Value);
                               int InventoryNumber = Convert.ToInt32((xlRange.Cells[rCnt, 3] as Excel.Range).Value);
                               int InvoiceId = Convert.ToInt32((xlRange.Cells[rCnt, 4] as Excel.Range).Value); ;
                               int BookId = Convert.ToInt32((xlRange.Cells[rCnt, 5] as Excel.Range).Value); ;
                               string? FormId =  Convert.ToString((xlRange.Cells[rCnt, 6] as Excel.Range).Value);
                               int OrderId = Convert.ToInt32((xlRange.Cells[rCnt, 7] as Excel.Range).Value); ;
                               int PropertyTypeId =  Convert.ToInt32((xlRange.Cells[rCnt, 8] as Excel.Range).Value);
                               string? Additionalnfo =  Convert.ToString((xlRange.Cells[rCnt, 9] as Excel.Range).Value);
                               Property property = new Property(factoryNumber, Name,
                                             InventoryNumber,
                                             InvoiceId, BookId, BookId, FormId,
                                             string.Empty, DateTime.Now,
                                             OrderId, PropertyTypeId,
                                             Additionalnfo, 
                                             DateTime.Now,0,0,"","","",0,0,DateTime.Now,0);
                               properties.Add(property);
                           }


                       }
                       catch { }
                       finally
                       {
                           xlWorkbook.Close(true, null, null);
                           xlApp.Quit();

                           Marshal.ReleaseComObject(xlWorkbook);
                           Marshal.ReleaseComObject(xlWorksheet);
                           Marshal.ReleaseComObject(xlApp);
                       }
                       foreach (var property in properties)
                       {
                           if(PropertyData.InsertIntoDb(property)) SelectedList.Add(property);

                       }
                   }));
            }
        }        
        public ICommand PrintForm
        {
            get
            {
                return _printForm ?? (_printForm = new RelayCommand(
                   x =>
                   {
                       Dictionary<string, string> items = new Dictionary<string, string>()
                           {
                               {  "FormNumber", SelectedProperty.FormId },
                               {  "Name", SelectedProperty.Name },
                               {  "InventoryNumber", SelectedProperty.InventoryNumber.ToString() },
                               {  "AdditionalInfo", SelectedProperty.Additionalnfo },
                               {  "DateD", SelectedProperty.FormDate.Day.ToString() },
                               {  "DateM", SelectedProperty.FormDate.Month.ToString() },
                               {  "DateY", SelectedProperty.FormDate.Year.ToString() }
                           };
                       string path = Directory.GetCurrentDirectory() + "\\Form for print\\Form.docx";
                       PrintWordDoc(items, path);
                       
                   }));
            }
        }
        private void PrintWordDoc(Dictionary<string, string> items, string path)
        {
            Word.Application app = null;

            FileInfo file = new FileInfo(path);

            try
            {
                string fileName = file.FullName;
                app = new Microsoft.Office.Interop.Word.Application { Visible = true };
                Microsoft.Office.Interop.Word.Document aDoc =
                    app.Documents.Open(fileName, ReadOnly: false, Visible: true);
                aDoc.Activate();

                foreach (var item in items)
                {
                    FindAndReplace(app, item.Key, item.Value);

                }
                string newFileName =
                Path.Combine(file.DirectoryName, DateTime.Now.ToString("yyyyMMdd HHmmss ") + file.Name);
                app.ActiveDocument.SaveAs2(newFileName);
                
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                
            }
        }
        private void FindAndReplace(Microsoft.Office.Interop.Word.Application doc, object findText, object replaceWithText)
        {
            //options
            object matchCase = false;
            object matchWholeWord = true;
            object matchWildCards = false;
            object matchSoundsLike = false;
            object matchAllWordForms = false;
            object forward = true;
            object format = false;
            object matchKashida = false;
            object matchDiacritics = false;
            object matchAlefHamza = false;
            object matchControl = false;
            object read_only = false;
            object visible = true;
            object replace = 2;
            object wrap = 1;
            //execute find and replace
            doc.Selection.Find.Execute(ref findText, ref matchCase, ref matchWholeWord,
                ref matchWildCards, ref matchSoundsLike, ref matchAllWordForms, ref forward, ref wrap, ref format, ref replaceWithText, ref replace,
                ref matchKashida, ref matchDiacritics, ref matchAlefHamza, ref matchControl);

        }
        public ICommand PrintAccountingForm
        {
            get
            {
                return _printAccountingForm ?? (_printAccountingForm = new RelayCommand(
                   x =>
                   {
                       Dictionary<string, string> items = new Dictionary<string, string>()
                           {
                               {  "FormNumber", SelectedProperty.FormId },
                               {  "Name", SelectedProperty.Name+ " " 
                                           + SelectedProperty.InventoryNumber.ToString() },
                               {  "UnitFio", SelectedProperty.UnitName + " " + SelectedProperty.FIO_R_STR },
                               {  "ConBook", SelectedProperty.BookId.ToString() },
                               {  "PageConBook", SelectedProperty.BookPage.ToString() },
                               {  "OrderBook", SelectedProperty.OrderBookId.ToString() },
                               {  "PageOrderBook", SelectedProperty.OrderBookPage.ToString() }

                           };
                       string path = Directory.GetCurrentDirectory() + "\\Form for print\\RegistraionCard.docx";
                       PrintWordDoc(items, path);
                   }));
            }
        }

    }
}
