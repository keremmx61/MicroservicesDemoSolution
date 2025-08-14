using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;

namespace OrderService.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public OrderService(OrderDbContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<Order> Add(int userId, List<OrderItemDto> items)
        {
            var httpClient = _httpClientFactory.CreateClient();
            var productServiceUrl = _configuration["ServiceUrls:ProductService"] + "/api/products/";

            var newOrder = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalPrice = 0,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in items)
            {
                var response = await httpClient.GetAsync(productServiceUrl + item.ProductId);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"ProductService'ten ürün bilgisi alınamadı. ProductId: {item.ProductId}, StatusCode: {response.StatusCode}");

                var productJson = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(productJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (product != null)
                {
                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        Price = product.Price
                    };
                    newOrder.OrderItems.Add(orderItem);
                    newOrder.TotalPrice += orderItem.Price * orderItem.Quantity;
                }
            }

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return newOrder;
        }

        public IEnumerable<Order> GetAll()
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .ToList();
        }

        public Order? GetById(int id)
        {
            return _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefault(o => o.Id == id);
        }

        public void Update(Order order)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        private class Product
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public decimal Price { get; set; }
        }
    }
}
