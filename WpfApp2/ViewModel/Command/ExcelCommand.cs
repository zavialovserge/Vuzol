using Microsoft.Win32;

namespace Vuzol.ViewModel.Command
{
    public class ExcelCommand : CommandBase
    {

        public override void Execute(object parameter)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "xls files (*.xls)|*.xls|xlsx files (*.xlsx)|*.xlsx";
            if (fileDialog.ShowDialog() == true)
            {
                var path = fileDialog.FileName;
                GetDataFromExcel(path);
                InsertDataIntoDb();
            }
        }

        private void InsertDataIntoDb()
        {

        }

        private void GetDataFromExcel(string path)
        {
            var p = path;
        }
    }
}
