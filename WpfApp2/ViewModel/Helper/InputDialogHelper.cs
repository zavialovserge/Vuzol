using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vuzol.View;

namespace Vuzol.ViewModel.Helper
{
    public static class InputDialogHelper
    {
        public static string GetDialogAnswer(string title,string defaultName = "")
        {
            InputDialogSample inputDialog =
                       new InputDialogSample(title,defaultName);
            inputDialog.Title = title;
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
    }
}
