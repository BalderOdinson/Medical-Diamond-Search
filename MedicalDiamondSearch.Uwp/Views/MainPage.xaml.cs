using MedicalDiamondSearch.Uwp.ViewModels;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MedicalDiamondSearch.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            MainViewModel = new MainViewModel { ParameterP = 7 };
            this.InitializeComponent();
            DataContext = MainViewModel;
            BlockSizeComboBox.ItemsSource = new List<string> { "4x4", "8x8", "16x16", "32x32", "64x64" };
        }

        public MainViewModel MainViewModel { get; set; }

        private async void ReferenceFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync().AsTask();
            if (file != null)
            {
                MainViewModel.ReferenceFrame = file.Path;
                MainViewModel.ReferenceFileAccessToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
            }
        }

        private async void CurrentFrameButton_OnClick(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            };
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync().AsTask();
            if (file != null)
            {
                MainViewModel.CurrentFrame = file.Path;
                MainViewModel.CurrentFileAccessToken = Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
            }
        }
    }
}
