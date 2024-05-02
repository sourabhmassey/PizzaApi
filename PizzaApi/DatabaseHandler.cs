namespace PizzaApi
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using PizzaApi.Models;
    using static Constants.PizzaOrderStatus;

    public static class DatabaseHandler
    {
        private static int lastYear = DateTime.Now.Year - 1;

        public async static Task InitDataBaseAsync(IWebHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<PizzaDbContext>();
                    dbContext.RemoveRange(dbContext.PizzaOrders);                  
                    await dbContext.SaveChangesAsync();
                    await AddTestDataAsync(dbContext);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Startup>>();
                    logger.LogError(ex, "An error occurred adding test data to the DB.");
                }
            }
        }

        public async static Task AddTestDataAsync(PizzaDbContext dbContext)
        {
            var pizzaOrders = new List<PizzaOrder>();

            for(int year = lastYear; year >= lastYear - 3; year--)
            {
                pizzaOrders.AddRange(AddPizzaOrders(year, 1, GetPizzas1));
                pizzaOrders.AddRange(AddPizzaOrders(year, 2, GetPizzas2));
                pizzaOrders.AddRange(AddPizzaOrders(year, 3, GetPizzas3));
                pizzaOrders.AddRange(AddPizzaOrders(year, 4, GetPizzas4));
                pizzaOrders.AddRange(AddPizzaOrders(year, 5, GetPizzas5));
                pizzaOrders.AddRange(AddPizzaOrders(year, 6, GetPizzas1));
                pizzaOrders.AddRange(AddPizzaOrders(year, 7, GetPizzas2));
                pizzaOrders.AddRange(AddPizzaOrders(year, 8, GetPizzas3));
                pizzaOrders.AddRange(AddPizzaOrders(year, 9, GetPizzas4));
                pizzaOrders.AddRange(AddPizzaOrders(year, 10, GetPizzas5));
                pizzaOrders.AddRange(AddPizzaOrders(year, 11, GetPizzas1));
                pizzaOrders.AddRange(AddPizzaOrders(year, 12, GetPizzas2));
            }

            pizzaOrders.AddRange(new PizzaOrder[] {
                //Some pizza orders from yesterday
                new PizzaOrder(DateTime.Today.AddDays(-1).AddMinutes(100), Pending, GetPizzas1()),
                new PizzaOrder(DateTime.Today.AddDays(-1).AddMinutes(200), Deleted, GetPizzas2()),
                new PizzaOrder(DateTime.Today.AddDays(-1).AddMinutes(250), Pending, GetPizzas3()),
                new PizzaOrder(DateTime.Today.AddDays(-1).AddMinutes(300), Deleted, GetPizzas4()),
                new PizzaOrder(DateTime.Today.AddDays(-1).AddMinutes(390), Pending, GetPizzas5()),
                new PizzaOrder(DateTime.Today.AddDays(-1).AddMinutes(400), Completed, GetPizzas1()),
                new PizzaOrder(DateTime.Today.AddDays(-1).AddMinutes(500), InProgress, GetPizzas2()),
            });

            dbContext.AddRange(pizzaOrders);
            await dbContext.SaveChangesAsync();

            var pepe = dbContext.PizzaOrders;
        }

        private static List<Pizza> GetPizzas1()
            => new List<Pizza>
            {
                new Pizza("Margherita", Constants.PizzaSize.Large, 19.5m),
                new Pizza("Meat", Constants.PizzaSize.Small, 12),
                new Pizza("Veggie", Constants.PizzaSize.Medium, 15),
                new Pizza("BBQ", Constants.PizzaSize.Large, 20.5m),
                new Pizza("Pepperoni", Constants.PizzaSize.Large, 20),
            };

        private static List<Pizza> GetPizzas2()
            => new List<Pizza>
            {
                new Pizza("Meat", Constants.PizzaSize.Small, 12),
                new Pizza("Veggie", Constants.PizzaSize.Large, 19),
                new Pizza("BBQ", Constants.PizzaSize.Large, 20.5m),
                new Pizza("Pepperoni", Constants.PizzaSize.Medium, 15),
                new Pizza("BBQ", Constants.PizzaSize.Large, 20.5m),
                new Pizza("Pepperoni", Constants.PizzaSize.Medium, 15),
            };

        private static List<Pizza> GetPizzas3()
            => new List<Pizza>
            {
                new Pizza("Ortolana", Constants.PizzaSize.Large, 19),
                new Pizza("Pesto", Constants.PizzaSize.Medium, 15),
                new Pizza("Rustica", Constants.PizzaSize.Medium, 15),
            };

        private static List<Pizza> GetPizzas4()
            => new List<Pizza>
            {
                new Pizza("Prosciutto e funghi", Constants.PizzaSize.Small, 11.5m),
                new Pizza("Pesto", Constants.PizzaSize.Medium, 15),
                new Pizza("Rustica", Constants.PizzaSize.Medium, 15),
            };

        private static List<Pizza> GetPizzas5()
            => new List<Pizza>
            {
                new Pizza("Garlic", Constants.PizzaSize.Large, 21.5m),
                new Pizza("Rustica", Constants.PizzaSize.Medium, 15),
            };

        private static List<PizzaOrder> AddPizzaOrders(int year, int month, Func<List<Pizza>> getPizzasFunc)
        => new List<PizzaOrder>() {
                new PizzaOrder("Stella", new DateTime(year, month, 11, 10, 00, 00), Pending, getPizzasFunc()),
                new PizzaOrder("Paul", new DateTime(year, month, 11, 12, 30, 00), Completed, getPizzasFunc()),
                new PizzaOrder("Steve", new DateTime(year, month, 11, 13, 00, 00), InProgress, getPizzasFunc()),
                new PizzaOrder("Peter", new DateTime(year, month, 11, 12, 45, 00), Completed, getPizzasFunc()),
                new PizzaOrder("Peter", new DateTime(year, month, 12, 17, 00, 00), Completed, getPizzasFunc()),
                new PizzaOrder("Edward", new DateTime(year, month, 12, 18, 00, 00), Completed, getPizzasFunc()),
                new PizzaOrder("Dave", new DateTime(year, month, 13, 17, 00, 00), Deleted, getPizzasFunc()),
                new PizzaOrder("Antonio", new DateTime(year, month, 14, 17, 00, 00), Completed, getPizzasFunc()),
                new PizzaOrder("Loretta", new DateTime(year, month, 15, 17, 00, 00), Completed, getPizzasFunc()),
                new PizzaOrder("Stella", new DateTime(year, month, 19, 17, 00, 00), Completed, getPizzasFunc()),
                new PizzaOrder("Lucy", new DateTime(year, month, 21, 17, 00, 00), Deleted, getPizzasFunc()),
                new PizzaOrder("Debra", new DateTime(year, month, 22, 17, 00, 00), Completed, getPizzasFunc()),
                new PizzaOrder("Stella", new DateTime(year, month, 25, 17, 00, 00), Pending, getPizzasFunc()),
        };
    }
}
