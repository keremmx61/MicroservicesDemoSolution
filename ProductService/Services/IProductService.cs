using ProductService.Models;

namespace ProductService.Services
{
    public interface IProductService
    {
        IEnumerable<Product> GetAll();

        // Redis cache'i kullanmadan doğrudan veritabanından alacak metodumuz.
        IEnumerable<Product> GetAllFromDb();

        Product? GetById(int id);
        Product Add(Product product);
        void Update(Product product);
        void Delete(int id);
    }
}