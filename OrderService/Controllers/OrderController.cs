using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Services;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var orders = _orderService.GetAll();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var order = _orderService.GetById(id);
            if (order == null)
                return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrderDto createOrderDto)
        {
            if (createOrderDto == null || !createOrderDto.Items.Any())
            {
                return BadRequest("Sipariş bilgileri boş olamaz.");
            }

            var newOrder = await _orderService.Add(createOrderDto.UserId, createOrderDto.Items);

            return CreatedAtAction(nameof(GetById), new { id = newOrder.Id }, newOrder);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Order order)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return NoContent();
        }
    }
}
