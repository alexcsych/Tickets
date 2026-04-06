using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using Tickets.Infrastructure;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Tickets.ViewModels
{
    public class SignInViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _login = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;

        private bool _isLoginTouched, _isEmailTouched, _isPasswordTouched, _isConfirmPasswordTouched;

        public string Login { get => _login; set { _login = value; _isLoginTouched = true; OnPropertyChanged(); } }
        public string Email { get => _email; set { _email = value; _isEmailTouched = true; OnPropertyChanged(); } }
        public string Password { get => _password; set { _password = value; _isPasswordTouched = true; OnPropertyChanged(); OnPropertyChanged(nameof(ConfirmPassword)); } }
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; _isConfirmPasswordTouched = true; OnPropertyChanged(); } }

        public ICommand RegisterCommand { get; }
        public ICommand OpenLogInCommand { get; }

        public SignInViewModel()
        {
            RegisterCommand = new RelayCommand(
                execute: obj =>
                {
                    MessageBox.Show("Аккаунт створено!");
                },
                canExecute: obj => IsFormValid()
            );

            OpenLogInCommand = new RelayCommand(p =>
            {
                var logInWindow = new Views.LogIn();
                logInWindow.Show();
                if (p is Window currentWindow) currentWindow.Close();
            });
        }

        private bool IsFormValid()
        {
            return _isLoginTouched && _isEmailTouched && _isPasswordTouched && _isConfirmPasswordTouched &&
                   string.IsNullOrEmpty(this[nameof(Login)]) &&
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
                    case nameof(Login):
                        if (!_isLoginTouched) return null!;
                        if (string.IsNullOrWhiteSpace(Login)) error = "Логін обов'язковий!";
                        else if (Login.Length < 3) error = "Мін. 3 символи!";
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