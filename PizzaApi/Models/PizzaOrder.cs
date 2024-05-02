namespace PizzaApi.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Newtonsoft.Json;
    using PizzaApi.Constants;

    public class PizzaOrder
    {
        public PizzaOrder()
        {
        }

        public PizzaOrder(string name, DateTime orderDate, PizzaOrderStatus status, List<Pizza> pizzas)
        {
            Name = name;
            Status = status;
            OrderDate = orderDate;
            Pizzas = pizzas;
        }

        public PizzaOrder(string name, DateTime orderDate, PizzaOrderStatus status)
        {
            Name = name;
            Status = status;
            OrderDate = orderDate;
        }

        public PizzaOrder(DateTime orderDate, PizzaOrderStatus status, List<Pizza> pizzas)
        {
            Name = string.Empty;
            Status = status;
            OrderDate = orderDate;
            Pizzas = pizzas;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime OrderDate { get; set; }

        public PizzaOrderStatus Status { get; set; }

        public virtual List<Pizza> Pizzas { get; set; } = new List<Pizza>();
    }
}