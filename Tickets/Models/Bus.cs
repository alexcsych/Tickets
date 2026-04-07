using System.Collections.ObjectModel;

namespace Tickets.Models
{
    public class Bus
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public ObservableCollection<Route> Routes { get; set; } = [];
    }
}
