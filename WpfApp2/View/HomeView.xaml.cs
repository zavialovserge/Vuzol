using System.Windows;
using System.Windows.Controls;

namespace Vuzol.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            // Синхронізація горизонтального скролу між фільтрами та DataGrid
            DataGridScrollViewer.ScrollChanged += (s, e) =>
            {
                if (e.HorizontalChange != 0)
                {
                    FilterScrollViewer.ScrollToHorizontalOffset(DataGridScrollViewer.HorizontalOffset);
                }
            };

            FilterScrollViewer.ScrollChanged += (s, e) =>
            {
                if (e.HorizontalChange != 0)
                {
                    DataGridScrollViewer.ScrollToHorizontalOffset(FilterScrollViewer.HorizontalOffset);
                }
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
