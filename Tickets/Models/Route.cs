using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Tickets.Models
{
    public class Route
    {
        public int Id { get; set; }
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public int Price { get; set; }
        public int BusId { get; set; }
        public Bus? Bus { get; set; }
        public ObservableCollection<Ticket> Tickets { get; set; } = [];
    }
}
