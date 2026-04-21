using System.Collections.ObjectModel;
using Tickets.Models;
using Tickets.ViewModels;

namespace Tickets.Tests;
public class RoutesViewModelTests
{
    [Fact]
    public void BuyTicket_ShouldNotAllowPurchase_WhenBusIsFull()
    {
        var routesVM = new RoutesViewModel(null!);

        var fullRoute = new Route
        {
            Id = 1,
            To = "Київ",
            Bus = new Bus { Capacity = 1 },
            Tickets = new System.Collections.ObjectModel.ObservableCollection<Ticket> { new Ticket() }
        };

        routesVM.BuyTicket(fullRoute);

        Assert.Single(fullRoute.Tickets);
    }
}
