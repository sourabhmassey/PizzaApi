namespace PizzaApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using PizzaApi;
    using PizzaApi.Constants;
    using PizzaApi.Models;

    [Route("[controller]")]
    [ApiController]
    internal sealed class PizzaOrdersController : ControllerBase
    {
        private readonly PizzaDbContext context;

        public PizzaOrdersController(PizzaDbContext context)
        {
            this.context = context;
        }

        [HttpGet("ping")]
        public ActionResult<bool> Ping()
        {
            return Ok();
        }

        // GET: PizzaOrders?status={status}?date={orderDate}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PizzaOrder>>> GetPizzaOrder([FromQuery]PizzaOrderStatus?[] status, [FromQuery]string? date)
        {
            if (!status.Any() && date == null) // Not parameters
            {
                return context.PizzaOrders.ToList();
            }

            DateTime result = default;
            if (date != null && !DateTime.TryParse(date, out result))
            {
                return BadRequest("Invalid date value.");
            }

            if (date == null && status.Any()) // Only statuses
            {
                return await GetOrders(order => status.Contains(order.Status));
            }

            if (date != null && !status.Any()) // Only date
            {
                return await GetOrders(order => order.OrderDate.Date.Equals(result.Date));
            }

            return await GetOrders(order => status.Contains(order.Status) && order.OrderDate.Date.Equals(result.Date));
        }

        // PUT: PizzaOrders
        [HttpPut()]
        public async Task<ActionResult<PizzaOrder>> UpdatePizzaOrder(PizzaOrder updatedPizzaOrder)
        {
            if (!context.PizzaOrders.Any(order => order.Id == updatedPizzaOrder.Id))
            {
                return NotFound();
            }

            if (context.PizzaOrders.Any(order => order.Id.Equals(updatedPizzaOrder.Id) && order.Status.Equals(PizzaOrderStatus.Completed)))
            {
                return BadRequest("A completed pizza order cannot be updated.");
            }

            if (!updatedPizzaOrder.Pizzas.Any())
            {
                return BadRequest("A pizza order should have at least one pizza.");
            }

            var entity = context.PizzaOrders.Update(updatedPizzaOrder);
            await context.SaveChangesAsync();

            return entity.Entity;
        }


        // POST PizzaOrders
        [HttpPost]
        public async Task<ActionResult<long>> PostPizzaOrder(PizzaOrder pizzaOrder)
        {
            if (pizzaOrder.OrderDate < DateTime.Now.AddSeconds(-2))
                return BadRequest($"The order date {pizzaOrder.OrderDate} cannot be in the past.");

            if (!pizzaOrder.Pizzas.Any())
                return BadRequest("A pizza order must have at least one pizza.");

            var entity = context.PizzaOrders.Add(pizzaOrder);

            await context.SaveChangesAsync();

            return StatusCode((int)HttpStatusCode.Created, entity.Entity.Id);
        }


        // DELETE PizzaOrders/id
        [HttpDelete("{id}")]
        public async Task<ActionResult<PizzaOrder>> DeletePizzaOrder(long id)
        {
            var pizzaOrder = await context.PizzaOrders.FindAsync(id);

            if (pizzaOrder == null)
                return NotFound();

            if (pizzaOrder.Status.Equals(PizzaOrderStatus.Deleted) || pizzaOrder.Status.Equals(PizzaOrderStatus.Completed))
            {
                return BadRequest($"Cannot delete a {pizzaOrder.Status} order.");
            }

            pizzaOrder.Status = PizzaOrderStatus.Deleted;

            var entity = context.PizzaOrders.Update(pizzaOrder);
            await context.SaveChangesAsync();

            return entity.Entity;
        }

        // GET: PizzaOrders
        private async Task<List<PizzaOrder>> GetOrders(Expression<Func<PizzaOrder, bool>> predicate)
        {
            return await context.PizzaOrders.Where(predicate).Include(o => o.Pizzas).ToListAsync();
        }
    }
}
