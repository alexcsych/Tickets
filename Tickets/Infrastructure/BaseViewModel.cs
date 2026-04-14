using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Tickets.ViewModels;

namespace Tickets.Infrastructure
{
    public abstract class BaseViewModel(MainViewModel mainViewModel) : PropertyHandler
    {
        protected readonly MainViewModel MainViewModel = mainViewModel;
    }
}
