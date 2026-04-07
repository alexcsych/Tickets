using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Tickets.Models
{
    public enum UserRole
    {
        Customer,
        Admin
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public ObservableCollection<Ticket> Tickets { get; set; } = [];
    }
}
