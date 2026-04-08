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

        public MainViewModel()
        {
            CurrentView = new LogInViewModel(this);
        }

        public void NavigateTo(object viewModel)
        {
            CurrentView = viewModel;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
