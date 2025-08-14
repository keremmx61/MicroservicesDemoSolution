using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using ProductService.Data;
using ProductService.Models;

namespace ProductService.Services
{
    public class ProductService : IProductService
    {
        private readonly ProductDbContext _context;
        private readonly IDistributedCache _cache;

        public ProductService(ProductDbContext context, IDistributedCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public IEnumerable<Product> GetAll()
        {
            const string cacheKey = "allProducts";
            string? cachedProductsJson = _cache.GetString(cacheKey);

            if (!string.IsNullOrEmpty(cachedProductsJson))
            {
                return JsonSerializer.Deserialize<List<Product>>(cachedProductsJson) ?? new List<Product>();
            }
            else
            {
                // Eğer veri cache'de yoksa, veritabanından al
                var products = _context.Products.ToList();

                // Veritabanından aldığın veriyi JSON'a çevir
                var serializedProducts = JsonSerializer.Serialize(products);

                // Cache seçeneklerini ayarla
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };

                // Veriyi Redis'e kaydet
                _cache.SetString(cacheKey, serializedProducts, cacheOptions);

                return products;
            }
        }

        // YENİ METODUN UYGULAMASI
        public IEnumerable<Product> GetAllFromDb()
        {
            // Bu metot, cache'i tamamen atlayıp doğrudan veritabanına gider.
            return _context.Products.ToList();
        }

        public Product? GetById(int id)
        {
            return _context.Products.Find(id);
        }

        public Product Add(Product product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();

            // Veri değiştiği için, cache'deki eski listeyi silmeliyiz.
            _cache.Remove("allProducts");

            return product;
        }

        public void Update(Product product)
        {
            var existingProduct = _context.Products.Find(product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.StockQuantity = product.StockQuantity;
                _context.SaveChanges();

                // Veri değiştiği için, cache'deki eski listeyi silmeliyiz.
                _cache.Remove("allProducts");
            }
        }

        public void Delete(int id)
        {
            var productToDelete = _context.Products.Find(id);
            if (productToDelete != null)
            {
                _context.Products.Remove(productToDelete);
                _context.SaveChanges();

                // Veri değiştiği için, cache'deki eski listeyi silmeliyiz.
                _cache.Remove("allProducts");
            }
        }
    }
}