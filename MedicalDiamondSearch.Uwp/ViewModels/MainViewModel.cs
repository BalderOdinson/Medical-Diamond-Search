using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using MedicalDiamondSearch.Core;
using MedicalDiamondSearch.Core.Settings;
using MedicalDiamondSearch.Uwp.Annotations;
using MedicalDiamondSearch.Uwp.Extensions;
using MedicalDiamondSearch.Uwp.Helpers;
using MedicalDiamondSearch.Uwp.Views;
using Image = MedicalDiamondSearch.Core.Helpers.Image;
using System.Drawing;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Core;

namespace MedicalDiamondSearch.Uwp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _referenceFrame;
        private string _currentFrame;
        private bool _isBusy;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ReferenceFileAccessToken { get; set; }
        public string CurrentFileAccessToken { get; set; }

        public string ReferenceFrame
        {
            get => _referenceFrame;
            set
            {
                if (value == _referenceFrame) return;
                _referenceFrame = value;
                OnPropertyChanged();
            }
        }

        public string CurrentFrame
        {
            get => _currentFrame;
            set
            {
                if (value == _currentFrame) return;
                _currentFrame = value;
                OnPropertyChanged();
            }
        }

        public int SelectedBlockSize
        {
            get
            {
                switch (MedicalDiamondSearchSettings.BlockSize)
                {
                    case 4:
                        return 0;
                    case 8:
                        return 1;
                    case 16:
                        return 2;
                    case 32:
                        return 3;
                    case 64:
                        return 4;
                    default:
                        return -1;
                }
            }
            set
            {
                switch (value)
                {
                    case 0:
                        MedicalDiamondSearchSettings.BlockSize = 4;
                        break;
                    case 1:
                        MedicalDiamondSearchSettings.BlockSize = 8;
                        break;
                    case 2:
                        MedicalDiamondSearchSettings.BlockSize = 16;
                        break;
                    case 3:
                        MedicalDiamondSearchSettings.BlockSize = 32;
                        break;
                    case 4:
                        MedicalDiamondSearchSettings.BlockSize = 64;
                        break;
                    default:
                        MedicalDiamondSearchSettings.BlockSize = 16;
                        break;
                }
                OnPropertyChanged();
            }
        }

        public int ParameterP
        {
            get => MedicalDiamondSearchSettings.SearchParameterP;
            set
            {
                if (value == MedicalDiamondSearchSettings.SearchParameterP) return;
                MedicalDiamondSearchSettings.SearchParameterP = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfThreads
        {
            get => MedicalDiamondSearchSettings.NumberOfThreads;
            set
            {
                if (value == MedicalDiamondSearchSettings.NumberOfThreads) return;
                MedicalDiamondSearchSettings.NumberOfThreads = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (value == _isBusy) return;
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private AsyncCommand _calculateCommand;

        public ICommand CalculateCommand
        {
            get
            {
                if (_calculateCommand != null)
                    return _calculateCommand;
                _calculateCommand = new AsyncCommand(Calculate, CanExecute);
                this.PropertyChanged += (sender, args) =>
                {
                    switch (args.PropertyName)
                    {
                        case "ReferenceFrame":
                        case "CurrentFrame":
                        case "IsBusy":
                        case "SelectedBlockSize":
                            _calculateCommand.OnCanExecuteChanged();
                            break;
                    }
                };
                return _calculateCommand;
            }
        }

        private async Task Calculate()
        {
            IsBusy = true;
            var resultPage = new ResultPage();
            WriteableBitmap refrentImage = null;
            WriteableBitmap currentImage = null;
            using (var refImage =
                await (await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(ReferenceFileAccessToken)).OpenStreamForWriteAsync())
            using (var curImage =
                await (await Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.GetFileAsync(CurrentFileAccessToken)).OpenStreamForWriteAsync())
            {
                refrentImage = await BitmapFactory.FromStream(refImage);
                currentImage = await BitmapFactory.FromStream(curImage);
            }

            Stopwatch stopwatch = null;
            await Task.Run(async () =>
            {
                Image rImage = null;
                Image cImage = null;
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                     () =>
                     {
                         rImage = new Image(refrentImage.GetPixels(), refrentImage.PixelWidth, refrentImage.PixelHeight);
                         rImage.GenerateBlocks();
                         cImage = new Image(currentImage.GetPixels(), currentImage.PixelWidth, currentImage.PixelHeight);
                         cImage.GenerateBlocks();
                     }
                 );
                resultPage.ResultViewModel.Vectors = new ObservableCollection<string>();
                stopwatch = Stopwatch.StartNew();
                var result = Mds.CalculateVectors(rImage, cImage);
                stopwatch.Stop();
                foreach (var vector in result)
                {
                    if (vector.Value.X != 0 || vector.Value.Y != 0)
                    {
                        resultPage.ResultViewModel.Vectors.Add(
                            $"Block({vector.Key.X},{vector.Key.Y}), vector({vector.Value.X},{vector.Value.Y})");
                        foreach (var pixel in rImage.Blocks[vector.Key].Pixels)
                        {
                            refrentImage.SetPixel(pixel.Position.X + vector.Value.X, pixel.Position.Y + vector.Value.Y,
                                pixel.Color.ToWinColor());
                        }
                    }
                }
            });

            resultPage.ResultViewModel.CurrentFrame = new BitmapImage(new Uri(CurrentFrame));
            resultPage.ResultViewModel.RefrenceFrame = currentImage;
            resultPage.ResultViewModel.ResultFrame = refrentImage;
            resultPage.ResultViewModel.TimeElapsed = stopwatch.Elapsed.TotalSeconds + " seconds";
            resultPage.ResultViewModel.Error = refrentImage.Compare(currentImage);

            IsBusy = false;

            if (Window.Current.Content is Frame frame)
                frame.Content = resultPage;
        }

        private bool CanExecute()
        {
            return !string.IsNullOrWhiteSpace(ReferenceFrame) && !string.IsNullOrWhiteSpace(CurrentFrame) && !IsBusy && SelectedBlockSize != -1;
        }

    }
}
