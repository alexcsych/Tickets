using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Tickets.Data;
using Tickets.Infrastructure;

namespace Tickets.ViewModels
{
    public class SignInViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly MainViewModel _mainViewModel;

        private string _name = string.Empty;
        private string _lastName = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        private bool _isNameTouched, _isLastNameTouched, _isEmailTouched, _isPasswordTouched, _isConfirmPasswordTouched;

        public string Name { get => _name; set { _name = value; _isNameTouched = true; OnPropertyChanged(); } }
        public string LastName { get => _lastName; set { _lastName = value; _isLastNameTouched = true; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; _isEmailTouched = true; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; _isPasswordTouched = true; OnPropertyChanged(); OnPropertyChanged(nameof(ConfirmPassword)); } }
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; _isConfirmPasswordTouched = true; OnPropertyChanged(); } }

        public ICommand RegistrationCommand { get; }
        public ICommand OpenLogInCommand { get; }

        public SignInViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            RegistrationCommand = new RelayCommand(
                execute: obj =>
                {
                    try
                    {
                        using var db = new AppDbContext();

                        if (db.Users.Any(u => u.Email == Email))
                        {
                            MessageBox.Show("Користувач з такою поштою вже існує!", "Помилка");
                            return;
                        }

                        var newUser = new Models.User
                        {
                            Name = this.Name,
                            LastName = this.LastName,
                            Email = this.Email,
                            Password = this.Password,
                            Role = Models.UserRole.Customer
                        };

                        db.Users.Add(newUser);
                        db.SaveChanges();

                        _mainViewModel.NavigateTo(new RoutesViewModel(_mainViewModel) { CurrentUser = newUser } );
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при реєстрації: {ex.Message}", "Критична помилка");
                    }
                },
                canExecute: obj => IsFormValid()
            );

            OpenLogInCommand = new RelayCommand(p =>
            {
                _mainViewModel.NavigateTo(new LogInViewModel(_mainViewModel));
            });
        }

        private bool IsFormValid()
        {
            return _isNameTouched && _isLastNameTouched && _isEmailTouched && _isPasswordTouched && _isConfirmPasswordTouched &&
                   string.IsNullOrEmpty(this[nameof(Name)]) &&
                   string.IsNullOrEmpty(this[nameof(LastName)]) &&
                   string.IsNullOrEmpty(this[nameof(Email)]) &&
                   string.IsNullOrEmpty(this[nameof(Password)]) &&
                   string.IsNullOrEmpty(this[nameof(ConfirmPassword)]);
        }

        public string Error => null!;
        public string this[string name]
        {
            get
            {
                string error = string.Empty;
                switch (name)
                {
                    case nameof(Name):
                        if (!_isNameTouched) return null!;
                        if (string.IsNullOrWhiteSpace(Name)) error = "Ім'я обов'язкове!";
                        else if (Name.Length < 2) error = "Мін. 2 символи!";
                        break;
                    case nameof(LastName):
                        if (!_isLastNameTouched) return null!;
                        if (string.IsNullOrWhiteSpace(LastName)) error = "Прізвище обов'язкове!";
                        else if (LastName.Length < 2) error = "Мін. 2 символи!";
                        break;
                    case nameof(Email):
                        if (!_isEmailTouched) return null!;
                        if (string.IsNullOrWhiteSpace(Email)) error = "Пошта обов'язкова!";
                        else if (Email.Length < 5) error = "Мін. 5 символів!";
                        else if (!Email.Contains('@') || !Email.Contains('.')) error = "Невірний формат пошти!";
                        break;
                    case nameof(Password):
                        if (!_isPasswordTouched) return null!;
                        if (string.IsNullOrWhiteSpace(Password)) error = "Введіть пароль!";
                        else if (Password.Length < 6) error = "Мін. 6 символів!";
                        break;
                    case nameof(ConfirmPassword):
                        if (!_isConfirmPasswordTouched) return null!;
                        if (ConfirmPassword.Length < 6) error = "Мін. 6 символів!";
                        else if (ConfirmPassword != Password) error = "Паролі не співпадають!";
                        break;
                }
                return error;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}