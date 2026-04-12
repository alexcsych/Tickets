using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Tickets.ViewModels;

namespace Tickets.Infrastructure
{
    public abstract class BaseViewModel(MainViewModel mainViewModel) : INotifyPropertyChanged
    {
        protected readonly MainViewModel MainViewModel = mainViewModel;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
