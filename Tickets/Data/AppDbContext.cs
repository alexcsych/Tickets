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
        }
    }
}
