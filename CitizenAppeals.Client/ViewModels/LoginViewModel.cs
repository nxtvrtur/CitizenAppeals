using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CitizenAppeals.Client.ViewModels.Base;
using CitizenAppeals.Client.ViewModels.Command;
using CitizenAppeals.Client.Views;
using CitizenAppeals.Shared.Dto;
using Newtonsoft.Json;

namespace CitizenAppeals.Client.ViewModels;

public class LoginViewModel : ViewModelBase
{
    private readonly HttpClient _httpClient;
    private readonly PasswordBox _passwordBox;
    private string? _username;
    private bool _isLoading;
    private string? _token;

    public LoginViewModel(PasswordBox passwordBox)
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
        };
        _httpClient = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:5202/api/") };
        _passwordBox = passwordBox;
        LoginCommand = new RelayCommand(async () => await LoginAsync());
    }

    public string? Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        IsLoading = true;
        try
        {
            var loginDto = new LoginDto { Username = Username, Password = _passwordBox.Password };
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var tokenDto = JsonConvert.DeserializeObject<TokenDto>(await response.Content.ReadAsStringAsync());
                _token = tokenDto.Token?.Trim();
                if (string.IsNullOrEmpty(_token))
                {
                    MessageBox.Show("Получен пустой токен от сервера");
                    return;
                }

                var mainWindow = new MainWindow(_httpClient, _token);
                mainWindow.Show();
                if (Application.Current.MainWindow != null)
                    Application.Current.MainWindow.Close();
                Application.Current.MainWindow = mainWindow;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                MessageBox.Show($"Ошибка входа: {response.StatusCode} - {errorContent}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка входа: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}