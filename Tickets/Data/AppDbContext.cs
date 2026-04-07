using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Tickets.Models;

namespace Tickets.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=tickets.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "Admin",
                    LastName = "System",
                    Email = "admin@tickets.com",
                    Password = "admin_password",
                    Role = UserRole.Admin
                },
                new User
                {
                    Id = 2,
                    Name = "User",
                    LastName = "User",
                    Email = "User@mail.com",
                    Password = "user_password",
                    Role = UserRole.Customer
                }
            );

            modelBuilder.Entity<Bus>().HasData(
                new Bus { Id = 1, Model = "Mercedes-Benz Sprinter", Number = "AP 1234 BT", Capacity = 18 },
                new Bus { Id = 2, Model = "Setra S 515 HD", Number = "AA 7777 XX", Capacity = 25 }
            );

            modelBuilder.Entity<Route>().HasData(
                new Route
                {
                    Id = 1,
                    From = "Zaporozhye",
                    To = "Kyiv",
                    DepartureTime = new DateTime(2026, 5, 10, 08, 00, 00),
                    Price = 850,
                    BusId = 1
                },
                new Route
                {
                    Id = 2,
                    From = "Kyiv",
                    To = "Lviv",
                    DepartureTime = new DateTime(2026, 5, 11, 14, 30, 00),
                    Price = 600,
                    BusId = 2
                },
                new Route
                {
                    Id = 3,
                    From = "Kyiv",
                    To = "Zaporozhye",
                    DepartureTime = new DateTime(2026, 5, 21, 12, 30, 00),
                    Price = 800,
                    BusId = 2
                },
                new Route
                {
                    Id = 4,
                    From = "Zaporozhye",
                    To = "Lviv",
                    DepartureTime = new DateTime(2026, 5, 21, 18, 00, 00),
                    Price = 1300,
                    BusId = 1
                }
            );
        }
    }
}
