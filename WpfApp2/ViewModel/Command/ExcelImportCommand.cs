using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows;
using Vuzol.Services;
using Vuzol.ViewModel.Model;
using WpfApp2.Services;
using Excel = Microsoft.Office.Interop.Excel;

namespace Vuzol.ViewModel.Command
{
    public class ExcelImportCommand : CommandBase
    {
        private readonly List<Property> _dataSource;

        public ExcelImportCommand(List<Property> dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        }

        public override void Execute(object parameter)
        {
            if (_dataSource.Count == 0)
            {
                MessageBox.Show("Немає даних для експорту.", "Увага",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|Excel 2003 (*.xls)|*.xls",
                FilterIndex = 1,
                DefaultExt = ".xls",
                FileName = $"Import_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (openFileDialog.ShowDialog() != true)
                return;

            string filePath = openFileDialog.FileName;
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;
            try
            {
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Open(openFileDialog.FileName);
                xlWorksheet = xlWorkbook.Sheets[1];
                Excel.Range xlRange = xlWorksheet.UsedRange;

                int rw = xlRange.Rows.Count;
                int cl = xlRange.Columns.Count;
                List<Property> properties = new List<Property>();
                List<string> validationErrors = new List<string>();

                // Перевірка мінімальної кількості колонок
                if (cl != 24 )
                {
                    MessageBox.Show("Помилка: Excel файл повинен мати  24 колонки.",
                        "Неправильний формат файлу", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                for (int rCnt = 2; rCnt <= rw; rCnt++)
                {
                    try
                    {
                        var rowErrors = ValidateAndParseExcelRow(xlRange, rCnt);

                        if (rowErrors.Item1 != null)
                        {
                            properties.Add(rowErrors.Item1);
                        }
                        else
                        {
                            validationErrors.AddRange(rowErrors.Item2);
                        }
                    }
                    catch (Exception ex)
                    {
                        validationErrors.Add($"Рядок {rCnt}: {ex.Message}");
                        ErrorLogger.LogError(ex, $"Помилка при обробці рядка {rCnt} в Excel файлі.");
                    }
                }

                // Показати помилки валідації якщо вони є
                if (validationErrors.Count > 0)
                {
                    string errorMessage = string.Join("\n", validationErrors.Take(10));
                    if (validationErrors.Count > 10)
                        errorMessage += $"\n... та ще {validationErrors.Count - 10} помилок";

                    MessageBox.Show(errorMessage, "Помилки при завантаженні даних",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                // Вставити у БД
                if (properties.Count > 0)
                {
                    foreach (var property in properties)
                    {
                        PropertyData.InsertFromExcelIntoDb(property);
                    }

                    _dataSource.Clear();
                    var newList = PropertyData.GetAllProperty();
                    foreach (var prop in newList)
                    {
                        _dataSource.Add(prop);
                    }
                    

                    MessageBox.Show($"Успішно завантажено {properties.Count} записів.",
                        "Готово", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Помилка при читанні Excel файлу: {ex.Message}";
                ErrorLogger.LogError(ex, errorMessage);
                MessageBox.Show(errorMessage,
                    "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (xlWorkbook != null)
                {
                    xlWorkbook.Close(true, null, null);
                    Marshal.ReleaseComObject(xlWorkbook);
                }
                if (xlApp != null)
                {
                    xlApp.Quit();
                    Marshal.ReleaseComObject(xlApp);
                }
            }
        }
        /// <summary>
        /// Валідує та парсить один рядок з Excel
        /// </summary>
        private (Property property, List<string> errors) ValidateAndParseExcelRow(Excel.Range xlRange, int rowNumber)
        {
            var errors = new List<string>();

            try
            {
                // Допоміжна функція для безпечного читання значення
                object GetCellValue(int row, int col) => (xlRange.Cells[row, col] as Excel.Range).Value;

                string? GetStringValue(int row, int col)
                {
                    var value = GetCellValue(row, col);
                    return value?.ToString();
                }

                bool TryParseInt(int row, int col, string fieldName, out int result)
                {
                    result = 0;
                    var value = GetCellValue(row, col);

                    if (value == null)
                    {
                        return true;
                    }

                    if (!int.TryParse(value.ToString(), out result))
                    {
                        errors.Add($"Рядок {rowNumber}, колонка {col} ({fieldName}): '{value}' не є цілим числом");
                        return false;
                    }

                    if (result < 0)
                    {
                        errors.Add($"Рядок {rowNumber}, колонка {col} ({fieldName}): число не може бути від'ємним ({result})");
                        return false;
                    }

                    return true;
                }

                bool TryParseDecimal(int row, int col, string fieldName, out decimal result)
                {
                    result = 0;
                    var value = GetCellValue(row, col);

                    if (value == null)
                    {
                        return true;
                    }

                    if (!decimal.TryParse(value.ToString(), out result))
                    {
                        errors.Add($"Рядок {rowNumber}, колонка {col} ({fieldName}): '{value}' не є числом");
                        return false;
                    }

                    if (result < 0)
                    {
                        errors.Add($"Рядок {rowNumber}, колонка {col} ({fieldName}): число не може бути від'ємним ({result})");
                        return false;
                    }

                    return true;
                }
                bool TryParseDate(string dateStr, string fieldName, out DateTime result)
                {
                    result = DateTime.Now;

                    if (string.IsNullOrWhiteSpace(dateStr))
                    {
                        return true; 
                    }

                    if (!DateTime.TryParse(dateStr, out result))
                    {
                        errors.Add($"Рядок {rowNumber}: '{dateStr}' ({fieldName}) має неправильний формат дати");
                        return false;
                    }

                    return true;
                }

                string factoryNumber = GetStringValue(rowNumber, 1);

                string name = GetStringValue(rowNumber, 2);
                if (string.IsNullOrWhiteSpace(name))
                {
                    errors.Add($"Рядок {rowNumber}, колонка 2 (Найменування): значення не може бути пусте");
                    return (null, errors);
                }

                string shortName = GetStringValue(rowNumber, 3);
                string inventoryNumber = GetStringValue(rowNumber, 4);

                if (string.IsNullOrWhiteSpace(inventoryNumber))
                {
                    errors.Add($"Рядок {rowNumber}, колонка 4 (Інвентарний номер): значення не може бути пусте");
                    return (null, errors);
                }
                if (PropertyData.ExistProperty(inventoryNumber))
                {
                    errors.Add($"Рядок {rowNumber}, колонка 4 (Інвентарний номер): '{inventoryNumber}' вже існує в базі даних");
                    return (null, errors);
                }
                string materialResourcesDescription = GetStringValue(rowNumber, 5);
                string invoiceId = GetStringValue(rowNumber, 6);


                string invoiceDate = GetStringValue(rowNumber, 7);
                if (!TryParseDate(invoiceDate, "Дата накладної", out DateTime invoiceDateD))
                    return (null, errors);            

                if (!TryParseInt(rowNumber, 8, "Книга обліку", out int bookId))
                    return (null, errors);

                if (!TryParseInt(rowNumber, 9, "Сторінка книги", out int bookPage))
                    return (null, errors);

                if (!TryParseInt(rowNumber, 10, "Книга закріплень", out int orderBookId))
                    return (null, errors);

                if (!TryParseInt(rowNumber, 11, "Сторінка закріплень", out int orderBookPage))
                    return (null, errors);

                string category = GetStringValue(rowNumber, 12);

                if (!TryParseInt(rowNumber, 13, "Номер формуляру", out int formId))
                    return (null, errors);

                string formDate = GetStringValue(rowNumber, 14);
                if (!TryParseDate(formDate, "Дата формуляру", out DateTime formDateD))
                    return (null, errors);

                if (!TryParseInt(rowNumber, 15, "Наказ на введення", out int orderId))
                    return (null, errors);

                string orderDate = GetStringValue(rowNumber, 16);
                if (!TryParseDate(orderDate, "Дата наказу", out DateTime orderDateD))
                    return (null, errors);

                string statusName = GetStringValue(rowNumber, 17);

                string quantityTypeDescription = GetStringValue(rowNumber, 18);

                if (!TryParseDecimal(rowNumber, 19, "Кількість", out decimal quantity))
                    return (null, errors);

                if (!TryParseDecimal(rowNumber, 20, "Ціна", out decimal price))
                    return (null, errors);

                string fio_r= GetStringValue(rowNumber, 21);
                string unitName = GetStringValue(rowNumber, 22);
                string fio_v = GetStringValue(rowNumber, 23);

                string additionalInfo = GetStringValue(rowNumber, 24) ?? string.Empty;

                // Якщо помилок немає, повертаємо Property
                if (errors.Count == 0)
                {
                    var property = new Property
                    {
                        FactoryNumber = factoryNumber,
                        Name = name,
                        PropertyTypeName = shortName,
                        InventoryNumber = inventoryNumber,
                        MaterialResourcesDescription = materialResourcesDescription,
                        InvoiceId = invoiceId,
                        InvoiceDate = invoiceDateD,
                        BookId = bookId,
                        BookPage = bookPage,
                        OrderBookId = orderBookId,
                        OrderBookPage = orderBookPage,
                        CategoryDescription = category,
                        FormId = formId,
                        FormDate= formDateD,
                        OrderId = orderId,
                        OrderDate = orderDateD,                        
                        StatusName = statusName,  
                        QuantityTypeDescription = quantityTypeDescription,
                        Quantity = quantity,
                        Price = price,       
                        FIO_R_STR = fio_r,
                        FIO_V_STR = fio_v,
                        UnitName = unitName,
                        AdditionalInfo = additionalInfo
                    };

                    return (property, errors);
                }

                return (null, errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Рядок {rowNumber}: Непередбачена помилка - {ex.Message}");
                return (null, errors);
            }
        }
    }
}
