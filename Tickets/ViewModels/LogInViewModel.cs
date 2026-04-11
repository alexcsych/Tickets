using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Tickets.Data;
using Tickets.Infrastructure;

namespace Tickets.ViewModels
{
    public class LogInViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly MainViewModel _mainViewModel;

        private string _email = string.Empty;
        private string _password = string.Empty;

        private bool _isEmailTouched, _isPasswordTouched;

        public string Email { get => _email; set { _email = value; _isEmailTouched = true; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; _isPasswordTouched = true; OnPropertyChanged(); } }

        public ICommand LogInCommand { get; }
        public ICommand OpenSignInCommand { get; }

        public LogInViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            LogInCommand = new RelayCommand(
                execute: obj =>
                {
                    try
                    {
                        using var db = new AppDbContext();

                        var user = db.Users.FirstOrDefault(u => u.Email == Email && u.Password == Password);

                        if (user != null)
                        {
                            var routesVM = new RoutesViewModel(_mainViewModel)
                            {
                                CurrentUser = user
                            };
                            _mainViewModel.NavigateTo(routesVM);
                        }
                        else
                        {
                            this.Password = string.Empty;
                            this._isPasswordTouched = false;
                            OnPropertyChanged(nameof(Password));
                            MessageBox.Show("Невірна пошта або пароль!", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка бази даних: {ex.Message}\n{ex.InnerException?.Message}", "Критична помилка");
                    }
                },
                canExecute: obj => IsFormValid()
            );

            OpenSignInCommand = new RelayCommand(p =>
            {
                _mainViewModel.NavigateTo(new SignInViewModel(_mainViewModel));
            });
        }

        private bool IsFormValid()
        {
            return _isEmailTouched && _isPasswordTouched &&
                   string.IsNullOrEmpty(this[nameof(Email)]) &&
                   string.IsNullOrEmpty(this[nameof(Password)]);
        }

        public string Error => null!;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                if (columnName == nameof(Email))
                {
                    if (!_isEmailTouched) return string.Empty;

                    if (string.IsNullOrWhiteSpace(Email))
                        error = "Пошта обов'язкова!";
                    else if (Email.Length < 5)
                        error = "Пошта занадто коротка (мін. 5)!";
                }

                if (columnName == nameof(Password))
                {
                    if (!_isPasswordTouched) return string.Empty;

                    if (string.IsNullOrWhiteSpace(Password))
                        error = "Пароль не може бути порожнім!";
                    else if (Password.Length < 6)
                        error = "Пароль занадто короткий (мін. 6)!";
                }

                return error;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}