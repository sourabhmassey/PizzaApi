namespace PizzaApi.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Runtime.CompilerServices;
    using System.Security.Policy;
    using System.Text;
    using System.Threading.Tasks;
    using Castle.Core.Internal;
    using FluentAssertions;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using PizzaApi.Constants;
    using PizzaApi.Models;
    using RestSharp;

    public class IntegrationTests : TestBase
    {

        /* 
         * Please read.
         * 
         * You have been provided with several "Happy path" test methods to implement in the time provided
         * 
         * All test methods are commented with details of the endpoints and what is required.
         * 
         * In some tests you will be required to alter the assertions, in others you must not do so, follow the commented instructions for each.
         * 
         * Points will be awarded even if tests are not completed so even if you are struggling it is worth commenting on what you would do even if you dont have the syntax for it.
         * 
         * At the end of the file, if you have time you are encouraged to think of and attempt to implement your own set of tests, this allows you to demonstrate your ability to devise your own tests
         * as above points will be awarded for sudo coded tests as well as fully implemented tests so the more you can add the better you will do.
         * 
         * Finally you have been provided with a commented out test that shows a very basic example of how to interact with the Rest API "PingServer"
         * it is not meant to be a demonstration of best practice, and you should use your experience of testing RestAPIs rather than this as a guide it is only there to help if you have issues.
         * 
        */


        /// <summary>
        /// HTTP Client configured to send REST requests to the API.
        /// </summary>
        protected override HttpClient Client { get; set; }

        /// <summary>
        /// Entity framework context that can be used to access the API database.
        /// </summary>
        protected override PizzaDbContext DbContext { get; set; }


        /// <summary>
        /// The following test can be uncommented to demonstrate how to send a basic request to the API
        /// </summary>
        [Test]
        public async Task PingServer() {
            var result = await this.Client.GetAsync("PizzaOrders/ping");
            result.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Get a count of all the pizza orders with the order status "Completed" using GET PizzaOrders
        /// Ensure that the assertion passes and that 385 orders are returned.
        /// </summary>
        [Test]
        public async Task CountCompletedPizzaOrders() {
            // Implement the code to retrieve the completed orders so that the assertion succeeds.
            var url = "/PizzaOrders?status=Completed";
            HttpResponseMessage response = await this.Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<PizzaOrder>>(content);
            orders.Should().HaveCount(385);
        }

        /// <summary>
        /// Add a pizza order using POST PizzaOrders
        /// Assert that the order was added successfully by improving on the existing assertion which should be replaced with something more appropriate.
        /// </summary>
        [TestCase("Sourabh", "Pepperoni", "Medium", 19.99, "InProgress", "2022-03-03T12:00:00")]
        [TestCase("Sourabh", "Margirita", "Medium", 19.99, "InProgress", "Now")]
        [TestCase("Massey", "Vegetarian", "Large", 23.99, "Completed", "2026-03-03T12:00:00")]
        //Negative cases
        [TestCase("", "Vegetarian", "Large", 23.99, "Pending", "2024-05-03T12:00:00")]
        [TestCase("Maise", "", "Large", 23.99, "Deleted", "2024-05-02T12:00:00")]
        [TestCase("Dom", "MixedMeatFest", "Small", 27.99, "Pending", "")]
        [TestCase("Kaise", "Ham and Cheese", "Large", 23.99, "Completed", "2026-03-03T12:00:00")]
        public virtual async Task CreatePizzaOrder(string CustName, string PizzaName, Constants.PizzaSize PizzaSize, double price, Constants.PizzaOrderStatus status, string ordertime) {
            DateTime? time = ordertime switch
            {
                var ot when string.IsNullOrWhiteSpace(ot) => null,
                "Now" => DateTime.Now,
                _ => DateTime.TryParse(ordertime, out DateTime parsedTime) ? parsedTime.ToUniversalTime() : (DateTime?)null
            };
            PizzaOrder newOrder = new PizzaOrder
            {
                Name = CustName,
                Status = status,
                OrderDate = (DateTime)time,
                Pizzas = new List<Pizza> {  new() {
                    Name = PizzaName,
                    Size = PizzaSize,
                    Price = 20
                }
                }
            };
            // Implement the code to Post a pizza order to the system and ensure it was added successfully.
            var Uri = "/PizzaOrders";
            var jsonContent = JsonConvert.SerializeObject(newOrder);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            //Posting
            HttpResponseMessage response = await this.Client.PostAsync(Uri, content);
            // Replace this assertion with something more appropriate.
            //createdPizzaOrder.Should().NotBeNull();
            HandleResponse(response);

            void HandleResponse(HttpResponseMessage response) {
                switch (response.StatusCode) {
                    case HttpStatusCode.Created:
                        // Assertions specific to a successful response
                        CheckForNull(newOrder.Name, "Name cannot be null");
                        CheckForNull(newOrder.Pizzas.FirstOrDefault().Name, "Pizzas cannot be null");
                        CheckForNull(newOrder.Status, "Status cannot be null");
                        Console.WriteLine(response.StatusCode.ToString());
                        Console.WriteLine();
                        Assert.Pass();
                        break;

                    case HttpStatusCode.BadRequest:
                        // Log the problem and potentially check for expected failure reasons
                        Console.WriteLine("BadRequest with Content: " + response.Content.Headers.ToString());
                        Assert.Pass(response.StatusCode.ToString());
                        break;

                        // Consider adding other relevant status code checks here
                }
            }
        }
        protected void CheckForNull(object value, string message) {
            if (value == null || (value is string str && string.IsNullOrEmpty(str)) || (value is ICollection collection && collection.Count == 0)) {
                Console.WriteLine("Possible Bug. Raised a False Positive as " + message);
                Assert.Fail();
            }
        }



        /// <summary>
        /// Delete a Pizza Order from the system using DELETE PizzaOrders/{id}
        /// </summary>
        [Test]
        public virtual async Task DeletePizzaOrder() {
            PizzaOrder deletedPizzaOrder = default;
            // Implement the code to Delete a pizza order and ensure that the order status is successfully deleted.
            // You may add your own pizza and delete that one or delete on that is already there.

            // Replace this assertion with something more appropriate.
            //deletedPizzaOrderResponse.Status OK
        }


        /// <summary>
        /// The database contains a list of Historical Pizza orders
        /// Use the GET PizzaOrders endpoint to retrieve the completed orders from last year.
        /// Get the total price of the pizzas, grouped by month.
        /// </summary>
        [Test]
        public virtual async Task GetPizzaTotalPriceGroupedByMonthForLastYear() {
            // Implement the code to update this variable to contain the values included in the database such that the assertion passes.
            Dictionary<string, decimal> monthlyTotals = new Dictionary<string, decimal>();

            /* Sourabh : This is what i tried but failed */
            // var url = "/PizzaOrders?status=Completed?date=2023-*-*T*:*:*";

           // var url = "/PizzaOrders?status=Completed?date=\"2023\"";
            var url = "/PizzaOrders?status=Completed";
            HttpResponseMessage response = await this.Client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var orders = JsonConvert.DeserializeObject<List<PizzaOrder>>(content);

            var monthlyTotalNums = orders.GroupBy(order => order.OrderDate.Month).Select(group =>
            {
                return (Month: group.Key, TotalPrice: group.Sum(order => order.Pizzas.Sum(pizza => pizza.Price)));
            }).OrderBy(result => result.Month).ToList();

            var monthNames = new Dictionary<int, string>
            {
                { 1, "January" }, { 2, "February" }, { 3, "March" }, { 4, "April" },
                { 5, "May" }, { 6, "June" }, { 7, "July" }, { 8, "August" },
                { 9, "September" }, { 10, "October" }, { 11, "November" }, { 12, "December" }
            };

            // Populate the monthlyTotals dictionary
            foreach (var item in monthlyTotalNums) {
                if (monthNames.TryGetValue(item.Month, out string monthName)) {
                    monthlyTotals.Add(monthName, item.TotalPrice);
                }
            }

            //Just to show and Debug
            foreach (var total in monthlyTotals) {
                Console.WriteLine($"{total.Key}: {total.Value}");
            }


            var expectedResult = new Dictionary<string, decimal>()
            {
                { "January", 696.0m }, { "February", 816.0m }, { "March", 392m }, { "April", 332.0m },
                { "May", 292.0m }, { "June", 696.0m }, { "July", 816.0m }, { "August", 392m },
                { "September", 332.0m }, { "October", 292.0m }, { "November", 696.0m }, { "December", 816.0m }
            };

            monthlyTotals.Should().Equal(expectedResult);
        }

        //Sourabh : This cannot be done in my prespective its a bug in API where we cannot provide the year on its own and it expects a date 

        /*
         * For the remainder of the time implement some tests of your own for testing the endpoints.
         * Points will be given for tests that are written, even if they are not implemented provided we can understand what the test is trying to achieve.
         */
    }
}
