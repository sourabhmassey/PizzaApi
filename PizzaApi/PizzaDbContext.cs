namespace PizzaApi
{
    using Microsoft.EntityFrameworkCore;
    using PizzaApi.Models;

    public class PizzaDbContext : DbContext
    {
        public PizzaDbContext(DbContextOptions<PizzaDbContext> options)
            : base(options)
        {
        }

        public DbSet<PizzaOrder> PizzaOrders { get; set; }

        public DbSet<Pizza> Pizza { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<PizzaOrder>()
                .HasMany(p => p.Pizzas);
        }
    }
}