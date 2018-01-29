using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
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
using Point = System.Drawing.Point;
using Vector = MedicalDiamondSearch.Core.Helpers.Vector;

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

        public int SelectedAlgorithm
        {
            get => _selectedAlgorithm;
            set {
                if (value == _selectedAlgorithm) return;
                _selectedAlgorithm = value;
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
        private int _selectedAlgorithm;

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
                        case "SelectedAlgorithm":
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
            await Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                var refrentImage = new Bitmap(ReferenceFrame);
                var currentImage = new Bitmap(CurrentFrame);
                var resultImage = new Bitmap(refrentImage.Width, refrentImage.Height, PixelFormat.Format32bppRgb);
                Image cImage = null;
                var cImageTask = Task.Run(() =>
                {
                    cImage = new Image(currentImage.GetPixels(), currentImage.Width, currentImage.Height);
                    cImage.GenerateBlocks();
                });
                var rImage = new Image(refrentImage.GetPixels(), refrentImage.Width, refrentImage.Height);
                rImage.GenerateBlocks();
                await cImageTask;

                IDictionary<Point, Vector> result = null;

                resultPage.ResultViewModel.Vectors = new ObservableCollection<string>();
                if (SelectedAlgorithm == 0)
                {
                    Application.Current.Dispatcher.Invoke(() => State = "Executing Medical Diamond Search...");
                    result = Mds.CalculateVectors(rImage, cImage);
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() => State = "Executing Diamond Search...");
                    result = Ds.CalculateVectors(rImage, cImage);
                }
                Application.Current.Dispatcher.Invoke(() => State = "Calculating output image...");
                var errorCount = 0;
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
                            if (cImage.Pixels[
                                    new System.Drawing.Point(pixel.Position.X + vector.Value.X,
                                        pixel.Position.Y + vector.Value.Y)].Color != pixel.Color)
                                errorCount++;
                            resultImage.SetPixel(pixel.Position.X + vector.Value.X, pixel.Position.Y + vector.Value.Y,
                                pixel.Color);
                            refrentImage.SetPixel(pixel.Position.X + vector.Value.X, pixel.Position.Y + vector.Value.Y,
                                pixel.Color);
                        }
                    }
                }
                Application.Current.Dispatcher.Invoke(() => State = "Saving output image...");
                if (!Directory.Exists("output"))
                    Directory.CreateDirectory("output");
                var output = $"output/{Guid.NewGuid().ToString()}.png";
                var output1 = $"output/{Guid.NewGuid().ToString()}.png";
                resultImage.Save(output);
                refrentImage.Save(output1);

                var motionError = (decimal)errorCount / (result.Count * MedicalDiamondSearchSettings.BlockSize * MedicalDiamondSearchSettings.BlockSize);

                Application.Current.Dispatcher.Invoke(() => State = "Calculating relative error...");
                var error = refrentImage.Compare(currentImage);

                stopwatch.Stop();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    resultPage.ResultViewModel.CurrentFrame = new BitmapImage(new Uri(CurrentFrame));
                    resultPage.ResultViewModel.RefrenceFrame = new BitmapImage(new Uri(ReferenceFrame));
                    resultPage.ResultViewModel.MotionFrame = new BitmapImage(new Uri(Path.Combine(Directory.GetCurrentDirectory(), output)));
                    resultPage.ResultViewModel.ResultFrame = new BitmapImage(new Uri(Path.Combine(Directory.GetCurrentDirectory(), output1)));
                    resultPage.ResultViewModel.TimeElapsed = stopwatch.Elapsed.TotalSeconds + " seconds";
                    resultPage.ResultViewModel.Error = error;
                    resultPage.ResultViewModel.MotionError = motionError;
                });
            });
            State = "Done.";
            IsBusy = false;
            App.Window.Frame.Content = resultPage;
            State = string.Empty;
        }

        private bool CanExecute()
        {
            return !string.IsNullOrWhiteSpace(ReferenceFrame) && !string.IsNullOrWhiteSpace(CurrentFrame) && !IsBusy && SelectedBlockSize != -1 && SelectedAlgorithm != -1;
        }

    }
}
