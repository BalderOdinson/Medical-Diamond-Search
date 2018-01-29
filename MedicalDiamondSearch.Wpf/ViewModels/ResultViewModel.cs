using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace MedicalDiamondSearch.Wpf.ViewModels
{
    public class ResultViewModel : INotifyPropertyChanged
    {
        private ImageSource _refrenceFrame;
        private ImageSource _currentFrame;
        private ImageSource _resultFrame;
        private string _timeElapsed;
        private decimal _error;
        private ObservableCollection<string> _vectors;
        private ImageSource _motionFrame;
        private decimal _motionError;
        private string _totalTimeElapsed;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ImageSource RefrenceFrame
        {
            get => _refrenceFrame;
            set
            {
                if (Equals(value, _refrenceFrame)) return;
                _refrenceFrame = value;
                OnPropertyChanged();
            }
        }

        public ImageSource CurrentFrame
        {
            get => _currentFrame;
            set
            {
                if (Equals(value, _currentFrame)) return;
                _currentFrame = value;
                OnPropertyChanged();
            }
        }

        public ImageSource ResultFrame
        {
            get => _resultFrame;
            set
            {
                if (Equals(value, _resultFrame)) return;
                _resultFrame = value;
                OnPropertyChanged();
            }
        }

        public ImageSource MotionFrame
        {
            get => _motionFrame;
            set
            {
                if (Equals(value, _motionFrame)) return;
                _motionFrame = value;
                OnPropertyChanged();
            }
        }

        public string TimeElapsed
        {
            get => _timeElapsed;
            set
            {
                if (value == _timeElapsed) return;
                _timeElapsed = value;
                OnPropertyChanged();
            }
        }

        public string TotalTimeElapsed
        {
            get => _totalTimeElapsed;
            set
            {
                if (value == _totalTimeElapsed) return;
                _totalTimeElapsed = value;
                OnPropertyChanged();
            }
        }

        public decimal Error
        {
            get => _error;
            set
            {
                if (value == _error) return;
                _error = value;
                OnPropertyChanged();
            }
        }

        public decimal MotionError
        {
            get => _motionError;
            set {
                if (value == _motionError) return;
                _motionError = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Vectors
        {
            get => _vectors;
            set
            {
                if (Equals(value, _vectors)) return;
                _vectors = value;
                OnPropertyChanged();
            }
        }
    }
}
