using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using Tickets.Infrastructure;

namespace Tickets.ViewModels
{
    public class LogInViewModel : INotifyPropertyChanged, IDataErrorInfo
    {
        private string _login = string.Empty;
        private string _password = string.Empty;
        private bool _isLoginTouched = false;
        private bool _isPasswordTouched = false;

        public string Login
        {
            get => _login;
            set
            {
                _login = value;
                _isLoginTouched = true;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                _isPasswordTouched = true;
                OnPropertyChanged();
            }
        }

        public ICommand LogInCommand { get; }
        public ICommand OpenSignInCommand { get; }

        public LogInViewModel()
        {
            LogInCommand = new RelayCommand(
                execute: obj =>
                {
                    MessageBox.Show("Вхід виконано успішно!");
                },
                canExecute: obj => IsFormValid()
            );

            OpenSignInCommand = new RelayCommand(p =>
            {
                var signInWindow = new Views.SignIn();
                signInWindow.Show();
                if (p is Window currentWindow) currentWindow.Close();
            });
        }

        private bool IsFormValid()
        {
            return _isLoginTouched && _isPasswordTouched &&
                   string.IsNullOrEmpty(this[nameof(Login)]) &&
                   string.IsNullOrEmpty(this[nameof(Password)]);
        }

        public string Error => null!;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;

                if (columnName == nameof(Login))
                {
                    if (!_isLoginTouched) return string.Empty;

                    if (string.IsNullOrWhiteSpace(Login))
                        error = "Логін обов'язковий!";
                    else if (Login.Length < 3)
                        error = "Логін має бути не менше 3 символів!";
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
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}