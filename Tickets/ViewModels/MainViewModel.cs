using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Tickets.Infrastructure;

namespace Tickets.ViewModels
{
    public class MainViewModel : PropertyHandler
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
    }
}
