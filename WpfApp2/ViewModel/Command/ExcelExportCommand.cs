using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Vuzol.ViewModel.Model;
using Excel = Microsoft.Office.Interop.Excel;

namespace Vuzol.ViewModel.Command
{
    /// <summary>
    /// Команда для експорту даних з DataGrid в Excel файл з заголовками та форматуванням
    /// </summary>
    public class ExcelExportCommand : CommandBase
    {
        private readonly List<Property> _dataSource;

        public ExcelExportCommand(List<Property> dataSource)
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

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel files (*.xlsx)|*.xlsx|Excel 2003 (*.xls)|*.xls",
                FilterIndex = 1,
                DefaultExt = ".xlsx",
                FileName = $"Export_{DateTime.Now:yyyyMMdd_HHmmss}"
            };

            if (saveFileDialog.ShowDialog() != true)
                return;

            string filePath = saveFileDialog.FileName;
            Excel.Application xlApp = null;
            Excel.Workbook xlWorkbook = null;
            Excel._Worksheet xlWorksheet = null;

            try
            {
                // Ініціалізація Excel
                xlApp = new Excel.Application();
                xlWorkbook = xlApp.Workbooks.Add();
                xlWorksheet = (Excel._Worksheet)xlWorkbook.Sheets[1];
                xlWorksheet.Name = "Дані";

                // Експорт даних
                ExportDataToWorksheet(xlWorksheet, _dataSource);

                // Автоматичне налаштування ширини колонок
                xlWorksheet.Columns.AutoFit();

                // Збереження файлу
                xlWorkbook.SaveAs(filePath);

                MessageBox.Show($"Дані успішно експортовано в:\n{filePath}", "Готово",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при експорті даних:\n{ex.Message}", "Помилка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Очищення ресурсів
                try
                {
                    if (xlWorksheet != null) Marshal.ReleaseComObject(xlWorksheet);
                    if (xlWorkbook != null)
                    {
                        xlWorkbook.Close(false);
                        Marshal.ReleaseComObject(xlWorkbook);
                    }
                    if (xlApp != null)
                    {
                        xlApp.Quit();
                        Marshal.ReleaseComObject(xlApp);
                    }
                }
                catch { /* Ігноруємо помилки при очищенні */ }
            }
        }

        /// <summary>
        /// Експортує дані в робочий лист Excel з заголовками та стилізацією
        /// </summary>
        private void ExportDataToWorksheet(Excel._Worksheet worksheet, List<Property> properties)
        {
            // Визначення заголовків колонок
            var headers = new[]
            {
                "Заводський номер",
                "Найменування",
                "Інвентарний номер",
                "Номер накладної",
                "Дата накладної",
                "Книга обліку",
                "Сторінка книги",
                "Книга закріплень",
                "Сторінка закріплень",
                "Номер формуляру",
                "Дата формуляру",
                "Наказ на введення",
                "Дата наказу",
                "Мітка",
                "Статус",
                "Ціна",
                "Кількість",
                "Відповідальна особа",
                "Відповідальний підрозділ",
                "Додаткова інформація"
            };

            // Запис заголовків в першому рядку
            for (int col = 0; col < headers.Length; col++)
            {
                Excel.Range headerCell = (Excel.Range)worksheet.Cells[1, col + 1];
                headerCell.Value = headers[col];

                // Стилізація заголовків
                FormatHeaderCell(headerCell);
            }

            // Запис даних
            for (int row = 0; row < properties.Count; row++)
            {
                var property = properties[row];
                int excelRow = row + 2; // +2 тому що перший рядок - заголовки

                try
                {
                    // Запис кожного поля з безпечною обробкою null значень
                    worksheet.Cells[excelRow, 1] = property.FactoryNumber;
                    worksheet.Cells[excelRow, 2] = property.Name ?? string.Empty;
                    worksheet.Cells[excelRow, 3] = property.InventoryNumber;
                    worksheet.Cells[excelRow, 4] = property.InvoiceId;
                    worksheet.Cells[excelRow, 5] = property.InvoiceDate == DateTime.MinValue ? 
                        string.Empty : property.InvoiceDate.ToString("dd.MM.yyyy");
                    worksheet.Cells[excelRow, 6] = property.BookId;
                    worksheet.Cells[excelRow, 7] = property.BookPage;
                    worksheet.Cells[excelRow, 8] = property.OrderBookId;
                    worksheet.Cells[excelRow, 9] = property.OrderBookPage;
                    worksheet.Cells[excelRow, 10] = property.FormId ?? string.Empty;
                    worksheet.Cells[excelRow, 11] = property.FormDate == DateTime.MinValue ? 
                        string.Empty : property.FormDate.ToString("dd.MM.yyyy");
                    worksheet.Cells[excelRow, 12] = property.OrderId;
                    worksheet.Cells[excelRow, 13] = property.OrderDate == DateTime.MinValue ? 
                        string.Empty : property.OrderDate.ToString("dd.MM.yyyy");
                    worksheet.Cells[excelRow, 14] = property.PropertyTypeName ?? string.Empty;
                    worksheet.Cells[excelRow, 15] = property.StatusName ?? string.Empty;
                    worksheet.Cells[excelRow, 16] = property.Price.ToString("F2");
                    worksheet.Cells[excelRow, 17] = property.Quantity.ToString("F2");
                    worksheet.Cells[excelRow, 18] = property.FIO_R_STR ?? string.Empty;
                    worksheet.Cells[excelRow, 19] = property.UnitName ?? string.Empty;
                    worksheet.Cells[excelRow, 20] = property.Additionalnfo ?? string.Empty;

                    // Альтернативне фарбування рядків для кращої читабельності
                    if (row % 2 == 1)
                    {
                        Excel.Range rowRange = worksheet.Range[$"A{excelRow}:T{excelRow}"];
                        rowRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(
                            System.Drawing.Color.FromArgb(240, 240, 240));
                    }
                }
                catch (Exception ex)
                {
                    // Логуємо помилку при записі рядка, але продовжуємо
                    System.Diagnostics.Debug.WriteLine($"Помилка при експорті рядка {excelRow}: {ex.Message}");
                }
            }

            // Установлення мінімальної ширини першої колонки для заголовків
            worksheet.Columns[1].ColumnWidth = 15;
        }

        /// <summary>
        /// Форматує комірку заголовка (колір, шрифт, вирівнювання)
        /// </summary>
        private void FormatHeaderCell(Excel.Range cell)
        {
            try
            {
                // Фон темно-сірий (відповідає стилю DataGridColumnHeaderStyle з App.xaml)
                cell.Interior.Color = System.Drawing.ColorTranslator.ToOle(
                    System.Drawing.Color.FromArgb(44, 62, 80));

                // Текст білий, жирний
                cell.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White);
                cell.Font.Bold = true;
                cell.Font.Size = 11;

                // Вирівнювання по центру
                cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                cell.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

                // Перенесення тексту для довгих заголовків
                cell.WrapText = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Помилка при форматуванні заголовка: {ex.Message}");
            }
        }
    }
}   