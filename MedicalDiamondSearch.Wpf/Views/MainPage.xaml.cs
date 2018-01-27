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
using Microsoft.Win32;

namespace MedicalDiamondSearch.Wpf.Views
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            MainViewModel = new MainViewModel { ParameterP = 7 };
            this.InitializeComponent();
            DataContext = MainViewModel;
            BlockSizeComboBox.ItemsSource = new List<string> { "4x4", "8x8", "16x16", "32x32", "64x64" };
        }

        public MainViewModel MainViewModel { get; set; }

        private void ReferenceFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a reference frame",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png"
            };
            if (openFileDialog.ShowDialog() != true) return;
            MainViewModel.ReferenceFrame = openFileDialog.FileName;
        }

        private void CurrentFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Title = "Select a current frame",
                Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                         "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                         "Portable Network Graphic (*.png)|*.png"
            };
            if (openFileDialog.ShowDialog() != true) return;
            MainViewModel.CurrentFrame = openFileDialog.FileName;
        }
    }
}
