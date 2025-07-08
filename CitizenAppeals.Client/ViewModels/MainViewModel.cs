using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CitizenAppeals.Client.ViewModels.Base;
using CitizenAppeals.Client.ViewModels.Command;
using CitizenAppeals.Client.Views;
using CitizenAppeals.Shared.Dto;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace CitizenAppeals.Client.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private ObservableCollection<AppealDto> _appeals;
    private ObservableCollection<AppealDto> _selectedAppeals;
    private bool _isLoading;

    public MainViewModel(HttpClient httpClient, string token)
    {
        _httpClient = httpClient;
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        _appeals = new ObservableCollection<AppealDto>();
        _selectedAppeals = new ObservableCollection<AppealDto>();
        SetDetectedCommand = new RelayCommand(async () => await UpdateCheckResultAsync("Выявлено"),
            () => !IsLoading && SelectedAppeals.Any());
        SetNotDetectedCommand = new RelayCommand(async () => await UpdateCheckResultAsync("Не выявлено"),
            () => !IsLoading && SelectedAppeals.Any());
        GenerateReportCommand = new RelayCommand(async () => await GenerateReportAsync());

        Task.Run(LoadAppealsAsync);
    }

    public ObservableCollection<AppealDto> Appeals
    {
        get => _appeals;
        set => SetProperty(ref _appeals, value);
    }

    public ObservableCollection<AppealDto> SelectedAppeals
    {
        get => _selectedAppeals;
        set => SetProperty(ref _selectedAppeals, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand SetDetectedCommand { get; }
    public ICommand SetNotDetectedCommand { get; }
    public ICommand GenerateReportCommand { get; }

    public void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid dataGrid) return;
        SelectedAppeals.Clear();
        foreach (AppealDto item in dataGrid.SelectedItems) SelectedAppeals.Add(item);
        (SetDetectedCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (SetNotDetectedCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private async Task LoadAppealsAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _httpClient.GetAsync("Appeals");
            if (response.IsSuccessStatusCode)
            {
                var appeals = JsonConvert.DeserializeObject<List<AppealDto>>(await response.Content.ReadAsStringAsync());
            
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    Appeals.Clear();
                    if (appeals != null)
                    {
                        foreach (var appeal in appeals)
                        {
                            Appeals.Add(appeal);
                        }
                    }
                });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MessageBox.Show($"Ошибка загрузки обращений: {response.StatusCode} - {errorContent}");
                });
            }
        }
        catch (Exception ex)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
            });
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task UpdateCheckResultAsync(string result)
    {
        if (!SelectedAppeals.Any()) return;

        IsLoading = true;
        try
        {
            var ids = SelectedAppeals.Select(a => a.Id).ToList();
            var updateDto = new UpdateCheckResultDto { Ids = ids, CheckResult = result };
            var content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("appeals/update", content);
            if (response.IsSuccessStatusCode)
            {
                await LoadAppealsAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Ошибка обновления результата: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task GenerateReportAsync()
    {
        var dialog = new ReportDialog();
        if (dialog.ShowDialog() == true)
        {
            IsLoading = true;
            try
            {
                var request = new ReportRequestDto
                {
                    StartDate = dialog.StartDate,
                    EndDate = dialog.EndDate
                };
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8,
                    "application/json");
                var response = await _httpClient.PostAsync("appeals/report", content);
                if (response.IsSuccessStatusCode)
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "Word Document (*.docx)|*.docx",
                        FileName = "report.docx"
                    };
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        await File.WriteAllBytesAsync(saveFileDialog.FileName,
                            await response.Content.ReadAsByteArrayAsync());
                        MessageBox.Show("Отчет сохранен!");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Ошибка генерации отчета: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}