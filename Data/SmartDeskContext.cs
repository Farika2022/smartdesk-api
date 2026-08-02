// DbContext is the bridge between C# and PostgreSQL.
// It tells Entity Framework: here are my models,
// here is the database connection, create the tables for me.
// ============================================================

using Microsoft.EntityFrameworkCore;
using SmartDesk.Api.Models;

namespace SmartDesk.Api.Data;

// DbContext is the Entity Framework base class.
// It provides all the database operations:
// SaveChanges(), Find(), Add(), Remove() etc.

public class SmartDeskContext: DbContext
{
    // ASP.NET passes database options (connection string, provider)
    // through dependency injection. This constructor receives them.
    public SmartDeskContext(DbContextOptions<SmartDeskContext>options)
        : base(options)
        {

        }
        // DbSet = a table in the database.
        // DbSet<Ticket> = the tickets table.

        public DbSet<Ticket> Tickets {get;set;}
        public DbSet<User> Users {get;set;}

        //OnModelCreating => Customise how Entity Framework maps models to tables.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>().HasData(
                new Ticket
                {
                    Id = 10001,
                    Customer = "Lars Nielsen",
                    Email = "lars@example.com",
                    Subject = "Front wheel is making a grinding noise",
                    Status = "open",
                    Urgency = "HIGH",
                    CreatedAt = new DateTime(2026, 7, 10, 8, 23, 0, DateTimeKind.Utc)
                },
                new Ticket
                {
                    Id =10002,
                Customer = "Sofia Berg",
                Email = "sofia@example.com",
                Subject = "Invoice shows wrong VAT amount",
                Status = "open",
                Urgency = "LOW",
                CreatedAt = new DateTime(2026, 7, 11, 10, 5, 0, DateTimeKind.Utc)
            },
            new Ticket
            {
                Id = 10003,
                Customer = "Mikkel Holm",
                Email = "mikkel@example.com",
                Subject = "Battery not charging past 40 percent",
                Status = "resolved",
                Urgency = "MEDIUM",
                CreatedAt = new DateTime(2026, 7, 12, 14, 30, 0, DateTimeKind.Utc)
            }
                
            );

        modelBuilder.Entity <User>().HasData(
            new User
            {
                Id = 1,
                Email = "staff@smartdesk.com",
                PasswordHash = "$2b$12$A.3RKBS0s/IlMceFJOGoPOdxdwPhbM.RFs7yHfuXJKFQE1jYmRkgu",
                Role = "staff"
            }
         );
        }
}
