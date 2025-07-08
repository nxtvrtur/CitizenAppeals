using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using CitizenAppeals.Client.ViewModels;

namespace CitizenAppeals.Client.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(HttpClient httpClient, string? token)
        {
            InitializeComponent();
            if (token != null) DataContext = new MainViewModel(httpClient, token);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.OnSelectionChanged(sender, e);
            }
        }
    }
}