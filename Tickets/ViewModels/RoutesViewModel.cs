using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Tickets.Data;
using Tickets.Infrastructure;
using Tickets.Models;

namespace Tickets.ViewModels
{
    public class RoutesViewModel : BaseViewModel
    {
        private User? _currentUser;
        private string _from = string.Empty;
        private string _to = string.Empty;
        private string _busModel = string.Empty;
        private bool _isAscending = true;
        private ObservableCollection<Route> _visibleRoutes = [];

        public User? CurrentUser { get => _currentUser; set { _currentUser = value; OnPropertyChanged(); LoadRoutes(); } }
        public string From { get => _from; set { _from = value; OnPropertyChanged(); } }
        public string To { get => _to; set { _to = value; OnPropertyChanged(); } }
        public string BusModel { get => _busModel; set { _busModel = value; OnPropertyChanged(); } }
        public bool IsAscending { get => _isAscending; set { _isAscending = value; OnPropertyChanged(); LoadRoutes(); } }
        public ObservableCollection<Route> VisibleRoutes { get => _visibleRoutes; set { _visibleRoutes = value; OnPropertyChanged(); } }

        public ICommand SearchCommand { get; }
        public ICommand BuyTicketCommand { get; }
        public ICommand LogOutCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenTicketsCommand { get; }

        public RoutesViewModel(MainViewModel mainViewModel) : base(mainViewModel)
        {
            SearchCommand = new RelayCommand(obj => Search(obj));

            BuyTicketCommand = new RelayCommand(obj => BuyTicket(obj));

            LogOutCommand = new RelayCommand(_ => LogOut());

            OpenProfileCommand = new RelayCommand(_ => OpenProfile());

            OpenTicketsCommand = new RelayCommand(_ => OpenTickets());
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

            LoadRoutes();
        }

        public void BuyTicket(object obj)
        {
            if (obj is Route selectedRoute && CurrentUser != null)
            {
                if (selectedRoute.Tickets.Count >= (selectedRoute.Bus?.Capacity ?? 0))
                {
                    MessageBox.Show("Місць більше немає!", "Увага");
                    return;
                }

                try
                {
                    using var db = new AppDbContext();

                    var newTicket = new Ticket
                    {
                        RouteId = selectedRoute.Id,
                        UserId = CurrentUser.Id,
                        PurchaseDate = DateTime.Now
                    };

                    db.Tickets.Add(newTicket);
                    db.SaveChanges();

                    selectedRoute.Tickets.Add(newTicket);

                    OnPropertyChanged(nameof(VisibleRoutes));

                    MessageBox.Show($"Квиток до міста {selectedRoute.To} успішно куплено!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка при купівлі: {ex.Message}");
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

        private void OpenTickets()
        {
            MainViewModel.NavigateTo(new TicketsViewModel(MainViewModel) { CurrentUser = this.CurrentUser });
        }

        private void LoadRoutes()
        {
            try
            {
                using var db = new AppDbContext();

                var query = db.Routes
                    .Include(r => r.Bus)
                    .Include(r => r.Tickets)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(From))
                {
                    var searchFrom = From.Trim().ToLower();
                    query = query.Where(r => r.From != null && EF.Functions.Like(r.From, $"%{searchFrom}%"));
                }

                if (!string.IsNullOrWhiteSpace(To))
                {
                    var searchTo = To.Trim().ToLower();
                    query = query.Where(r => r.To != null && EF.Functions.Like(r.To, $"%{searchTo}%"));
                }

                if (!string.IsNullOrWhiteSpace(BusModel))
                {
                    var searchBus = BusModel.Trim().ToLower();
                    query = query.Where(r => r.Bus != null && EF.Functions.Like(r.Bus.Model, $"%{searchBus}%"));
                }

                if (IsAscending)
                    query = query.OrderBy(r => r.Price);
                else
                    query = query.OrderByDescending(r => r.Price);

                var result = query.ToList();
                VisibleRoutes.Clear();
                foreach (var route in result)
                {
                    VisibleRoutes.Add(route);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка пошуку: {ex.Message}", "Помилка бази даних");
            }
        }
    }
}