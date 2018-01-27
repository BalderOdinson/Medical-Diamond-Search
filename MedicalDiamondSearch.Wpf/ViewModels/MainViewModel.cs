using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MedicalDiamondSearch.Core;
using MedicalDiamondSearch.Core.Settings;
using MedicalDiamondSearch.Wpf.Extensions;
using MedicalDiamondSearch.Wpf.Helpers;
using MedicalDiamondSearch.Wpf.Views;
using Image = MedicalDiamondSearch.Core.Helpers.Image;

namespace MedicalDiamondSearch.Wpf.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _referenceFrame;
        private string _currentFrame;
        private bool _isBusy;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string State
        {
            get => _state;
            set
            {
                if (value == _state) return;
                _state = value;
                OnPropertyChanged();
            }
        }

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
        private string _state;

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
            var resultPage = new ResultPage();
            IsBusy = true;
            State = "Loading images...";
            await Task.Run(() =>
            {
                var refrentImage = new Bitmap(ReferenceFrame);
                var currentImage = new Bitmap(CurrentFrame);

                var rImage = new Image(refrentImage.GetPixels(), refrentImage.Width, refrentImage.Height);
                rImage.GenerateBlocks();
                var cImage = new Image(currentImage.GetPixels(), currentImage.Width, currentImage.Height);
                cImage.GenerateBlocks();

                resultPage.ResultViewModel.Vectors = new ObservableCollection<string>();
                Application.Current.Dispatcher.Invoke(() => State = "Executing Medical Diamond Search...");
                var stopwatch = Stopwatch.StartNew();
                var result = Mds.CalculateVectors(rImage, cImage);
                stopwatch.Stop();
                Application.Current.Dispatcher.Invoke(() => State = "Calculating output image...");
                foreach (var vector in result)
                {
                    if (vector.Value.X != 0 || vector.Value.Y != 0)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            resultPage.ResultViewModel.Vectors.Add(
                                $"Block({vector.Key.X},{vector.Key.Y}), vector({vector.Value.X},{vector.Value.Y})");
                        });
                        foreach (var pixel in rImage.Blocks[vector.Key].Pixels)
                        {
                            refrentImage.SetPixel(pixel.Position.X + vector.Value.X, pixel.Position.Y + vector.Value.Y,
                                pixel.Color);
                        }
                    }
                }
                Application.Current.Dispatcher.Invoke(() => State = "Saving output image...");
                if (!Directory.Exists("output"))
                    Directory.CreateDirectory("output");
                var output = $"output/{Guid.NewGuid().ToString()}.png";
                refrentImage.Save(output);

                Application.Current.Dispatcher.Invoke(() => State = "Calculating relative error...");
                var error = refrentImage.Compare(currentImage);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    resultPage.ResultViewModel.CurrentFrame = new BitmapImage(new Uri(CurrentFrame));
                    resultPage.ResultViewModel.RefrenceFrame = new BitmapImage(new Uri(ReferenceFrame));
                    resultPage.ResultViewModel.ResultFrame = new BitmapImage(new Uri(Path.Combine(Directory.GetCurrentDirectory(),output)));
                    resultPage.ResultViewModel.TimeElapsed = stopwatch.Elapsed.TotalSeconds + " seconds";
                    resultPage.ResultViewModel.Error = error;
                });
            });
            State = "Done.";
            IsBusy = false;
            App.Window.Frame.Content = resultPage;
            State = string.Empty;
        }

        private bool CanExecute()
        {
            return !string.IsNullOrWhiteSpace(ReferenceFrame) && !string.IsNullOrWhiteSpace(CurrentFrame) && !IsBusy && SelectedBlockSize != -1;
        }

    }
}
