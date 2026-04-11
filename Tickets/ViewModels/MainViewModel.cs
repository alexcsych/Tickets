using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tickets.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private object? _currentView;
        public object? CurrentView { get => _currentView; set { _currentView = value; OnPropertyChanged(); } }

        private object? _previousView;

        public MainViewModel()
        {
            CurrentView = new LogInViewModel(this);
        }

        public void NavigateTo(object nextView)
        {
            if (CurrentView is not null and not LogInViewModel and not SignInViewModel)
            {
                _previousView = CurrentView;
            }
            CurrentView = nextView;
        }

        public void GoBack()
        {
            if (_previousView != null)
                CurrentView = _previousView;
                _previousView = null;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
