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
    public class RoutesViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;

        private User? _currentUser;
        public User? CurrentUser { get => _currentUser; set { _currentUser = value; OnPropertyChanged(); LoadRoutes(); } }

        private string _from = string.Empty;
        private string _to = string.Empty;
        private string _busModel = string.Empty;

        private bool _isAscending = true;
        public bool IsAscending { get => _isAscending; set { _isAscending = value; OnPropertyChanged(); LoadRoutes(); } }

        public string From { get => _from; set { _from = value; OnPropertyChanged(); } }
        public string To { get => _to; set { _to = value; OnPropertyChanged(); } }
        public string BusModel { get => _busModel; set { _busModel = value; OnPropertyChanged(); } }

        private ObservableCollection<Route> _visibleRoutes = [];
        public ObservableCollection<Route> VisibleRoutes { get => _visibleRoutes; set { _visibleRoutes = value; OnPropertyChanged(); } }

        public ICommand SearchCommand { get; }
        public ICommand BuyTicketCommand { get; }
        public ICommand LogOutCommand { get; }
        public ICommand OpenProfileCommand { get; }
        public ICommand OpenTicketsCommand { get; }

        public RoutesViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            SearchCommand = new RelayCommand(obj =>
            {
                if (obj?.ToString() == "Clear")
                {
                    From = string.Empty;
                    To = string.Empty;
                    BusModel = string.Empty;
                    IsAscending = true;
                }

                LoadRoutes();
            });

            BuyTicketCommand = new RelayCommand(obj =>
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
            });

            LogOutCommand = new RelayCommand(obj =>
            {
                _mainViewModel.NavigateTo(new LogInViewModel(_mainViewModel));
            });

            OpenProfileCommand = new RelayCommand(obj =>
            {
                _mainViewModel.NavigateTo(new ProfileViewModel(_mainViewModel) { CurrentUser = this.CurrentUser });
            });

            OpenTicketsCommand = new RelayCommand(_ =>
            {
                _mainViewModel.NavigateTo(new TicketsViewModel(_mainViewModel) { CurrentUser = this.CurrentUser });
            });
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}