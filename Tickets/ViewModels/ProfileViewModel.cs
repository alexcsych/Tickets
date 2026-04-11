using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Tickets.Infrastructure;
using Tickets.Models;
using System.Windows;
using Tickets.Data;

namespace Tickets.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private readonly MainViewModel _mainViewModel;

        private User? _currentUser;
        public User? CurrentUser { get => _currentUser; set { _currentUser = value; OnPropertyChanged(); Name = _currentUser?.Name ?? string.Empty; LastName = _currentUser?.LastName ?? string.Empty; Email = _currentUser?.Email ?? string.Empty; } }

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

        public ICommand UpdateProfileCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand GoBackCommand { get; }

        public ProfileViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            UpdateProfileCommand = new RelayCommand(
                execute: obj =>
                {
                    try
                    {
                        if (CurrentUser == null) return;
                        if (Name == CurrentUser.Name && LastName == CurrentUser.LastName && Email == CurrentUser.Email)
                        {
                            MessageBox.Show("Ви не внесли жодних змін.", "Інформація");
                            return;
                        }

                        using var db = new AppDbContext();
                        var userInDb = db.Users.FirstOrDefault(u => u.Id == CurrentUser.Id);

                        if (userInDb != null)
                        {
                            userInDb.Name = Name;
                            userInDb.LastName = LastName;
                            userInDb.Email = Email;

                            db.SaveChanges();

                            CurrentUser.Name = Name;
                            CurrentUser.LastName = LastName;
                            CurrentUser.Email = Email;

                            MessageBox.Show("Профіль успішно оновлено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка оновлення: {ex.Message}", "Помилка");
                    }
                },
                canExecute: obj => IsProfileValid()
            );

            ChangePasswordCommand = new RelayCommand(
                execute: obj =>
                {
                    try
                    {
                        if (CurrentUser == null) return;
                        if (Password == CurrentUser.Password)
                        {
                            MessageBox.Show("Новий пароль не може збігатися зі старим!", "Увага");
                            return;
                        }

                        using var db = new AppDbContext();
                        var userInDb = db.Users.FirstOrDefault(u => u.Id == CurrentUser.Id);

                        if (userInDb != null)
                        {
                            userInDb.Password = Password;
                            db.SaveChanges();

                            CurrentUser.Password = Password;

                            MessageBox.Show("Пароль змінено!", "Успіх");

                            Password = string.Empty;
                            ConfirmPassword = string.Empty;
                            _isPasswordTouched = false;
                            _isConfirmPasswordTouched = false;
                            OnPropertyChanged(nameof(Password));
                            OnPropertyChanged(nameof(ConfirmPassword));
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка: {ex.Message}");
                    }
                },
                canExecute: obj => IsPasswordValid()
            );

            GoBackCommand = new RelayCommand(_ =>
            {
                _mainViewModel.GoBack();
            });
        }

        private bool IsProfileValid()
        {
            return _isNameTouched && _isLastNameTouched && _isEmailTouched &&
                   string.IsNullOrEmpty(this[nameof(Name)]) &&
                   string.IsNullOrEmpty(this[nameof(LastName)]) &&
                   string.IsNullOrEmpty(this[nameof(Email)]);
        }

        private bool IsPasswordValid()
        {
            return _isPasswordTouched && _isConfirmPasswordTouched &&
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
