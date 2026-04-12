using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Tickets.Data;
using Tickets.Infrastructure;
using Tickets.Models;

namespace Tickets.ViewModels
{
    public class TicketsViewModel : BaseViewModel
    {
        private User? _currentUser;
        private string _from = string.Empty;
        private string _to = string.Empty;
        private string _busModel = string.Empty;
        private bool _isAscending = true;
        private ObservableCollection<Ticket> _visibleTickets = [];

        public User? CurrentUser { get => _currentUser; set { _currentUser = value; OnPropertyChanged(); LoadTickets(); } }
        public string From { get => _from; set { _from = value; OnPropertyChanged(); } }
        public string To { get => _to; set { _to = value; OnPropertyChanged(); } }
        public string BusModel { get => _busModel; set { _busModel = value; OnPropertyChanged(); } }
        public bool IsAscending { get => _isAscending; set { _isAscending = value; OnPropertyChanged(); LoadTickets(); } }
        public ObservableCollection<Ticket> VisibleTickets { get => _visibleTickets; set { _visibleTickets = value; OnPropertyChanged(); } }

        public ICommand SearchCommand { get; }
        public ICommand ReturnTicketCommand { get; }
        public ICommand LogOutCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenRoutesCommand { get; }

        public TicketsViewModel(MainViewModel mainViewModel) : base(mainViewModel)
        {
            SearchCommand = new RelayCommand(obj => Search(obj));

            ReturnTicketCommand = new RelayCommand(obj => ReturnTicket(obj));

            LogOutCommand = new RelayCommand(_ => LogOut());

            OpenProfileCommand = new RelayCommand(_ => OpenProfile());

            OpenRoutesCommand = new RelayCommand(_ => OpenRoutes());
        }

        private void Search(object obj)
        {
            if (obj?.ToString() == "Clear")
            {
                From = string.Empty;
                To = string.Empty;
                BusModel = string.Empty;
                IsAscending = true;
            }

            LoadTickets();
        }

        private void ReturnTicket(object obj)
        {
            if (obj is Ticket ticket)
            {
                var result = MessageBox.Show($"Ви впевнені, що хочете повернути квиток на рейс {ticket.Route?.To}?",
                    "Підтвердження", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var db = new AppDbContext();
                        db.Tickets.Remove(ticket);
                        db.SaveChanges();

                        VisibleTickets.Remove(ticket);
                        MessageBox.Show("Квиток успішно повернуто!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при поверненні: {ex.Message}");
                    }
                }
            }
        }

        private void LogOut()
        {
            MainViewModel.NavigateTo(new LogInViewModel(MainViewModel));
        }

        private void OpenProfile()
        {
            MainViewModel.NavigateTo(new ProfileViewModel(MainViewModel) { CurrentUser = this.CurrentUser });
        }

        private void OpenRoutes()
        {
            MainViewModel.NavigateTo(new RoutesViewModel(MainViewModel) { CurrentUser = this.CurrentUser });
        }

        private void LoadTickets()
        {
            if (CurrentUser == null) return;

            try
            {
                using var db = new AppDbContext();

                var query = db.Tickets
                    .Include(t => t.Route)
                        .ThenInclude(t => t!.Bus)
                    .Where(t => t.UserId == CurrentUser.Id)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(From))
                {
                    var searchFrom = From.Trim().ToLower();
                    query = query.Where(t => t.Route!.From != null && EF.Functions.Like(t.Route!.From, $"%{searchFrom}%"));
                }

                if (!string.IsNullOrWhiteSpace(To))
                {
                    var searchTo = To.Trim().ToLower();
                    query = query.Where(t => t.Route!.To != null && EF.Functions.Like(t.Route!.To, $"%{searchTo}%"));
                }

                if (!string.IsNullOrWhiteSpace(BusModel))
                {
                    var searchBus = BusModel.Trim().ToLower();
                    query = query.Where(t => t.Route!.Bus != null && EF.Functions.Like(t.Route!.Bus.Model, $"%{searchBus}%"));
                }

                if (IsAscending)
                    query = query.OrderBy(t => t.Route!.Price);
                else
                    query = query.OrderByDescending(t => t.Route!.Price);

                var result = query.ToList();

                VisibleTickets.Clear();
                foreach (var ticket in result)
                {
                    VisibleTickets.Add(ticket);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження квитків: {ex.Message}");
            }
        } 
    }
}
