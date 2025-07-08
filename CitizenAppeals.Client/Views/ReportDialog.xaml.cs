using DateTime = System.DateTime;
using RoutedEventArgs = System.Windows.RoutedEventArgs;
using Window = System.Windows.Window;
using System;
using System.Windows;

namespace CitizenAppeals.Client.Views
{
    public partial class ReportDialog : Window
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ReportDialog()
        {
            InitializeComponent();
            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today;
            DataContext = this;
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}