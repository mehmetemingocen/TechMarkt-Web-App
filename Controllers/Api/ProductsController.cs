using dotnet_store.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly DataContext _context;

    public ProductsController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] int? categoryId, [FromQuery] string? search)
    {
        try
        {
            var query = _context.Urunler
                .Include(u => u.Kategori)
                .Where(u => u.Aktif);

            if (categoryId.HasValue)
            {
                query = query.Where(u => u.KategoriId == categoryId.Value);
            }

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(u => u.UrunAdi.ToLower().Contains(search.ToLower()) ||
                                       u.Aciklama.ToLower().Contains(search.ToLower()));
            }

            var products = await query
                .Select(u => new ProductDto
                {
                    Id = u.Id,
                    Name = u.UrunAdi,
                    Description = u.Aciklama ?? "",
                    Price = u.Fiyat,
                    OriginalPrice = u.Fiyat, // Şimdilik aynı fiyat
                    CategoryId = u.KategoriId,
                    Category = new CategoryDto
                    {
                        Id = u.Kategori.Id,
                        Name = u.Kategori.KategoriAdi,
                        Description = "",
                        ImageUrl = ""
                    },
                    ImageUrl = !string.IsNullOrEmpty(u.Resim) ? $"/img/{u.Resim}" : "",
                    Stock = 100, // Şimdilik sabit stok
                    IsActive = u.Aktif,
                    CreatedAt = u.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    UpdatedAt = u.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<ProductDto>>
            {
                Success = true,
                Data = products
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Ürünler yüklenirken hata oluştu"
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        try
        {
            var product = await _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefaultAsync(u => u.Id == id && u.Aktif);

            if (product == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Ürün bulunamadı"
                });
            }

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.UrunAdi,
                Description = product.Aciklama ?? "",
                Price = product.Fiyat,
                OriginalPrice = product.Fiyat,
                CategoryId = product.KategoriId,
                Category = new CategoryDto
                {
                    Id = product.Kategori.Id,
                    Name = product.Kategori.KategoriAdi,
                    Description = "",
                    ImageUrl = ""
                },
                ImageUrl = !string.IsNullOrEmpty(product.Resim) ? $"/img/{product.Resim}" : "",
                Stock = 100,
                IsActive = product.Aktif,
                CreatedAt = product.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                UpdatedAt = product.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
            };

            return Ok(new ApiResponse<ProductDto>
            {
                Success = true,
                Data = productDto
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Ürün detayı yüklenirken hata oluştu"
            });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Arama terimi gerekli"
                });
            }

            var products = await _context.Urunler
                .Include(u => u.Kategori)
                .Where(u => u.Aktif && 
                           (u.UrunAdi.ToLower().Contains(q.ToLower()) ||
                            u.Aciklama.ToLower().Contains(q.ToLower())))
                .Select(u => new ProductDto
                {
                    Id = u.Id,
                    Name = u.UrunAdi,
                    Description = u.Aciklama ?? "",
                    Price = u.Fiyat,
                    OriginalPrice = u.Fiyat,
                    CategoryId = u.KategoriId,
                    Category = new CategoryDto
                    {
                        Id = u.Kategori.Id,
                        Name = u.Kategori.KategoriAdi,
                        Description = "",
                        ImageUrl = ""
                    },
                    ImageUrl = !string.IsNullOrEmpty(u.Resim) ? $"/img/{u.Resim}" : "",
                    Stock = 100,
                    IsActive = u.Aktif,
                    CreatedAt = u.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    UpdatedAt = u.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<ProductDto>>
            {
                Success = true,
                Data = products
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Arama yapılırken hata oluştu"
            });
        }
    }
}

// DTOs
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public double Price { get; set; }
    public double? OriginalPrice { get; set; }
    public int CategoryId { get; set; }
    public CategoryDto Category { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public int Stock { get; set; }
    public bool IsActive { get; set; }
    public string CreatedAt { get; set; } = null!;
    public string UpdatedAt { get; set; } = null!;
}

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
} 