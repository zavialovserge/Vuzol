using System.Windows;

namespace Vuzol.View
{
    /// <summary>
    /// Interaction logic for PersonalChoose.xaml
    /// </summary>
    public partial class PersonalChoose : Window
    {
        public PersonalChoose(List<ViewModel.Model.Employee> employees)
        {
            InitializeComponent();
            foreach (var employee in employees)
            {
                cbAnswer.Items.Add(employee.RankDescription + " " + employee.FirstName + " " + employee.LastName + " " + employee.Position);
            }
            cbAnswer.SelectedValue = cbAnswer.Items[0];
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }      

        public string Answer
        {
            get { return cbAnswer.SelectedValue.ToString(); }
        }
    }
}
