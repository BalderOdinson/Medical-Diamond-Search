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
using MedicalDiamondSearch.Wpf.ViewModels;

namespace MedicalDiamondSearch.Wpf.Views
{
    /// <summary>
    /// Interaction logic for ResultPage.xaml
    /// </summary>
    public partial class ResultPage : Page
    {
        public ResultViewModel ResultViewModel { get; }

        public ResultPage()
        {
            this.InitializeComponent();
            ResultViewModel = new ResultViewModel();
            DataContext = ResultViewModel;
        }

        private void OverflowButton_OnClick(object sender, RoutedEventArgs e)
        {
            VectorPopup.IsOpen = !VectorPopup.IsOpen;
        }
    }
}
