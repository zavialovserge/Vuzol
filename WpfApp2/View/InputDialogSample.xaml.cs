using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Vuzol.View
{
    /// <summary>
    /// Interaction logic for InputDialogSample.xaml
    /// </summary>
    public partial class InputDialogSample : Window
    {
        public InputDialogSample(string question, string defaultAnswer = "",double quantity  =0)
        {
            InitializeComponent();
            lblQuestion.Content = question;
            txtAnswer.Text = defaultAnswer; 
            txtQuantity.Text = quantity.ToString();
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
