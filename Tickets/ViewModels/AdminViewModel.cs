using System;
using System.Collections;
using System.Collections.Generic;
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
    public class AdminViewModel : INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;

        public List<string> Tables { get; } = ["Користувачі", "Рейси", "Автобуси", "Квитки"];

        private object? _selectedItem;
        public object? SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged(); } }

        private string _selectedTable = string.Empty;
        public string SelectedTable { get => _selectedTable; set { _selectedTable = value; OnPropertyChanged(); LoadCurrentTable(); } }

        private IEnumerable _currentData = new List<object>();
        public IEnumerable CurrentData { get => _currentData; set { _currentData = value; OnPropertyChanged(); } }

        public ICommand SaveChangesCommand { get; }
        public ICommand LogOutCommand { get; }
        public ICommand DeleteCommand { get; }

        public AdminViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            SelectedTable = Tables[0];

            SaveChangesCommand = new RelayCommand(_ => SaveAll());
            LogOutCommand = new RelayCommand(_ => _mainViewModel.NavigateTo(new LogInViewModel(_mainViewModel)));
            DeleteCommand = new RelayCommand(param => DeleteItem(param));
        }

        private void LoadCurrentTable()
        {
            using var db = new AppDbContext();

            CurrentData = SelectedTable switch
            {
                "Користувачі" => db.Users.ToList(),
                "Рейси" => db.Routes.ToList(),
                "Автобуси" => db.Buses.ToList(),
                "Квитки" => db.Tickets.ToList(),
                _ => new List<object>()
            };
        }

        private void SaveAll()
        {
            try
            {
                using var db = new AppDbContext();

                foreach (var item in CurrentData)
                {
                    if (item is User u) { if (u.Id == 0) db.Users.Add(u); else db.Users.Update(u); }
                    else if (item is Route r) { if (r.Id == 0) db.Routes.Add(r); else db.Routes.Update(r); }
                    else if (item is Bus b) { if (b.Id == 0) db.Buses.Add(b); else db.Buses.Update(b); }
                    else if (item is Ticket t) { if (t.Id == 0) db.Tickets.Add(t); else db.Tickets.Update(t); }
                }

                db.SaveChanges();
                MessageBox.Show("Всі зміни (включаючи нові записи) збережено!");

                LoadCurrentTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}");
            }
        }
        private void DeleteItem(object? param)
        {
            if (param == null) return;

            try
            {
                using var db = new AppDbContext();
                db.Remove(param);
                db.SaveChanges();
                LoadCurrentTable();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося видалити: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
