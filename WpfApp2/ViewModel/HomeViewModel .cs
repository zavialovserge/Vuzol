using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
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
            Complectness = new NavigateCommand<ComplectnessViewModel>(NavigationProperty,
                                               () => new ComplectnessViewModel(NavigationProperty));
            ComplectnessSoftWare = new NavigateCommand<ComplectnessViewModel>(NavigationProperty,
                                               () => new ComplectnessViewModel(NavigationProperty,false));
        }
        
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
                               int CompletnessId = Convert.ToInt32((xlRange.Cells[rCnt, 9] as Excel.Range).Value); 
                               string? Additionalnfo =  Convert.ToString((xlRange.Cells[rCnt, 10] as Excel.Range).Value);
                               Property property = new Property(factoryNumber, Name,
                                             InventoryNumber,
                                             InvoiceId, BookId, BookId, FormId,
                                             string.Empty, DateTime.Now,
                                             OrderId, PropertyTypeId,
                                             CompletnessId,
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
                               {  "AdditionalInfo", SelectedProperty.Additionalnfo }
                           };
                       string path = "C:\\Users\\szavia\\source\\repos\\WpfApp2\\WpfApp2\\Form for print\\Form.docx";
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
                app = new Microsoft.Office.Interop.Word.Application { Visible = false };
                Microsoft.Office.Interop.Word.Document aDoc =
                    app.Documents.Open(fileName, ReadOnly: false, Visible: false);
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
                app.ActiveDocument.Close();
                app.Quit();

                Marshal.ReleaseComObject(app);
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
                       string path = "C:\\Users\\szavia\\source\\repos\\WpfApp2\\WpfApp2\\Form for print\\RegistraionCard.docx";
                       PrintWordDoc(items, path);
                   }));
            }
        }

    }
}
