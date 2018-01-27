using Windows.UI.Xaml.Controls;
using MedicalDiamondSearch.Uwp.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MedicalDiamondSearch.Uwp.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ResultPage : Page
    {
        public ResultViewModel ResultViewModel { get; }

        public ResultPage()
        {
            this.InitializeComponent();
            ResultViewModel = new ResultViewModel();
            DataContext = ResultViewModel;
        }

    }
}
