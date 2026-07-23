using System.Windows;

namespace Vuzol.View
{
    /// <summary>
    /// Interaction logic for InputDialogSample.xaml
    /// </summary>
    public partial class InputDialogSample : Window
    {
        public InputDialogSample(string question, string defaultAnswer = "", bool IsQuantityvisible = false, double quantity = 0)
        {
            InitializeComponent();
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer;
            txtQuantity.Text = quantity.ToString();
            txtQuantity.IsEnabled = IsQuantityvisible;
            txtQuantity.Visibility = IsQuantityvisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            txtAnswer.SelectAll();
            txtAnswer.Focus();
        }

        public string Answer
        {
            get { return txtAnswer.Text; }
        }
        public double Quantity
        {
            get { return Double.Parse(txtQuantity.Text.ToString().Replace('.', ',')); }
        }
    }
}
