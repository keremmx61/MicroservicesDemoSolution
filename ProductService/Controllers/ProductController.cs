using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Services;
using Microsoft.Extensions.Localization;
using ProductService.Localization;

namespace ProductService.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase // Sınıf adı standartlara uygun olarak güncellendi
    {
        private readonly IProductService _productService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ProductsController(IProductService productService, IStringLocalizer<SharedResources> localizer)
        {
            _productService = productService;
            _localizer = localizer;
        }

        // === GÜNCELLENEN BÖLÜM ===

        // Bu, Redis cache'ini kullanan ana ve varsayılan metot olacak.
        // URL: GET /api/products
        [HttpGet]
        public IActionResult GetAllFromCache()
        {
            var products = _productService.GetAll();
            return Ok(products);
        }

        // YENİ EKLENDİ: Bu metot, cache'i atlayıp her zaman doğrudan veritabanına gider.
        // URL: GET /api/products/from-db
        [HttpGet("from-db")]
        public IActionResult GetAllFromDb()
        {
            var products = _productService.GetAllFromDb();
            return Ok(products);
        }

        // === MEVCUT METOTLARIN GERİ KALANI ===

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            var newProduct = _productService.Add(product);
            return CreatedAtAction(nameof(GetById), new { id = newProduct.Id }, newProduct);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            var existingProduct = _productService.GetById(id);
            if (existingProduct == null)
                return NotFound();

            _productService.Update(product);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _productService.GetById(id);
            if (product == null)
                return NotFound();

            _productService.Delete(id);
            return NoContent();
        }

        // Bu test metodu kalabilir, bir zararı yok.
        [HttpGet("test-localization")]
        public IActionResult TestLocalization()
        {
            return Ok(new
            {
                ProductNameRequired = _localizer["ProductNameRequired"],
                PriceGreaterThanZero = _localizer["PriceGreaterThanZero"]
            });
        }
    }
}