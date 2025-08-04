using dotnet_store.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly DataContext _context;

    public CategoriesController(DataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        try
        {
            var categories = await _context.Kategoriler
                .Select(k => new CategoryDto
                {
                    Id = k.Id,
                    Name = k.KategoriAdi,
                    Description = "",
                    ImageUrl = ""
                })
                .ToListAsync();

            return Ok(new ApiResponse<List<CategoryDto>>
            {
                Success = true,
                Data = categories
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Kategoriler yüklenirken hata oluştu"
            });
        }
    }

    [HttpGet("{id}/products")]
    public async Task<IActionResult> GetProductsByCategory(int id)
    {
        try
        {
            var category = await _context.Kategoriler
                .FirstOrDefaultAsync(k => k.Id == id);

            if (category == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Kategori bulunamadı"
                });
            }

            var products = await _context.Urunler
                .Include(u => u.Kategori)
                .Where(u => u.KategoriId == id && u.Aktif)
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
                Message = "Kategori ürünleri yüklenirken hata oluştu"
            });
        }
    }
} 