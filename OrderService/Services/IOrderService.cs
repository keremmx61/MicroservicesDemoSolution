using OrderService.Models;

namespace OrderService.Services
{
    public interface IOrderService
    {
        IEnumerable<Order> GetAll();
        Order? GetById(int id);
        Task<Order> Add(int userId, List<OrderItemDto> items);
        void Update(Order order);
        void Delete(int id);
    }
}
