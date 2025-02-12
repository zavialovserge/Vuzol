using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Vuzol.ViewModel.Model;

namespace Vuzol.Converteres
{
    public class PropertyConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int factoryNumber;
            int.TryParse(values[0].ToString(),out factoryNumber);
            string? Name = values[1].ToString();
            int InventoryNumber;
            int.TryParse(values[2].ToString(), out InventoryNumber);
            int InvoiceId;
            int.TryParse(values[3].ToString(), out InvoiceId);
            int BookId;
            int.TryParse(values[4].ToString(), out BookId);
            string? FormId = values[5].ToString();
            string? FormName = values[6].ToString();
            DateTime FormDate = DateTime.Now;
            int OrderId;
            int.TryParse(values[7].ToString(), out OrderId);
            int PropertyTypeId = 0;
            int CompletnessId;
            int.TryParse(values[8].ToString(), out CompletnessId);
            string? Additionalnfo = values[9].ToString();            
            string? FIO_R_STR = values[10].ToString();
            string? FIO_I_STR = values[10].ToString();
            DateTime DLM =DateTime.Now;
            return new Property(factoryNumber, Name, InventoryNumber,
                                InvoiceId, BookId, BookId, FormId,
                                FormName, FormDate, OrderId, 
                                PropertyTypeId, CompletnessId, Additionalnfo,
                                DLM,0,1, FIO_R_STR, FIO_I_STR,"",0,0,DateTime.Now,0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
