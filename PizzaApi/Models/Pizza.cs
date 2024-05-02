namespace PizzaApi.Models
{
    using Newtonsoft.Json;
    using PizzaApi.Constants;

    public class Pizza
    {
        public Pizza()
        {
        }
        
        public Pizza(string name, PizzaSize size, decimal price)
        {
            Name = name;
            Size = size;
            Price = price;
        }

        // Unique identifier.
        public long Id { get; set; }

        // Name of the pizza.
        public string Name { get; set; }

        // Size of the pizza.
        public PizzaSize Size { get; set; }

        // Price of the pizza.
        public decimal Price { get; set; }
    }
}
