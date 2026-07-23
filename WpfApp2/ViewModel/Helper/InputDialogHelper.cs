using Vuzol.View;

namespace Vuzol.ViewModel.Helper
{
    public static class InputDialogHelper
    {
        public static string GetDialogAnswer(string title, string defaultName = "")
        {
            InputDialogSample inputDialog =
                       new InputDialogSample(title, defaultName);
            inputDialog.Title = title;
            if (inputDialog.ShowDialog() == false
                || string.IsNullOrEmpty(inputDialog.Answer)) return string.Empty;

            return inputDialog.Answer;
        }
    }
}
