using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Windows;

namespace CitizenAppeals.Client.ViewModels
{
    public partial class ReportDialogViewModel : ObservableObject
    {
        [ObservableProperty] public DateTime? startDate = DateTime.Today.AddMonths(-1);

        [ObservableProperty] public DateTime? endDate = DateTime.Today;

        [RelayCommand]
        private void Ok()
        {
            if (StartDate == null || EndDate == null)
            {
                MessageBox.Show("Выберите обе даты", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is Views.ReportDialog)?.Close();
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is Views.ReportDialog)!.DialogResult = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is Views.ReportDialog)?.Close();
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w is Views.ReportDialog)!.DialogResult = false;
        }
    }
}