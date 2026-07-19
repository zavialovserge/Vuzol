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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
    }
}
