using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClickToLoadMore.Models
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) 
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

    public static class DbInitializer
    {
        public static void Initialize(DatabaseContext context)
        {
            context.Database.EnsureCreated();

            if (context.Items.Any())
                return;

            //Populate 20 items
            var items = new List<Item>();

            for (int i = 1; i <= 20; i++)
            {
                items.Add(new Item { ItemName = $"Item {i}", Price = 200 * i });
            }

            context.Items.AddRange(items);
            context.SaveChanges();
        }
    }
}
