using System.Security.Claims;
using dotnet_store.Models;
using dotnet_store.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet_store.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly DataContext _context;

    public CartController(ICartService cartService, DataContext context)
    {
        _cartService = cartService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCart(userId);

            var cartDto = new CartDto
            {
                Id = cart.CartId,
                UserId = userId,
                Items = cart.CartItems.Select(item => new CartItemDto
                {
                    Id = item.CartItemId,
                    ProductId = item.UrunId,
                    Product = new ProductDto
                    {
                        Id = item.Urun.Id,
                        Name = item.Urun.UrunAdi,
                        Description = item.Urun.Aciklama ?? "",
                        Price = item.Urun.Fiyat,
                        OriginalPrice = item.Urun.Fiyat,
                        CategoryId = item.Urun.KategoriId,
                        Category = new CategoryDto
                        {
                            Id = item.Urun.Kategori.Id,
                            Name = item.Urun.Kategori.KategoriAdi,
                            Description = "",
                            ImageUrl = ""
                        },
                        ImageUrl = !string.IsNullOrEmpty(item.Urun.Resim) ? $"/img/{item.Urun.Resim}" : "",
                        Stock = 100,
                        IsActive = item.Urun.Aktif,
                        CreatedAt = item.Urun.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                        UpdatedAt = item.Urun.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                    },
                    Quantity = item.Miktar,
                    UserId = userId
                }).ToList(),
                TotalAmount = cart.Toplam(),
                ItemCount = cart.CartItems.Count
            };

            return Ok(new ApiResponse<CartDto>
            {
                Success = true,
                Data = cartDto
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Sepet yüklenirken hata oluştu"
            });
        }
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Geçersiz veri"
                });
            }

            await _cartService.AddToCart(request.ProductId, request.Quantity);

            var cart = await _cartService.GetCart(GetCurrentUserId());
            var cartItem = cart.CartItems.FirstOrDefault(i => i.UrunId == request.ProductId);

            if (cartItem != null)
            {
                var cartItemDto = new CartItemDto
                {
                    Id = cartItem.CartItemId,
                    ProductId = cartItem.UrunId,
                    Product = new ProductDto
                    {
                        Id = cartItem.Urun.Id,
                        Name = cartItem.Urun.UrunAdi,
                        Description = cartItem.Urun.Aciklama ?? "",
                        Price = cartItem.Urun.Fiyat,
                        OriginalPrice = cartItem.Urun.Fiyat,
                        CategoryId = cartItem.Urun.KategoriId,
                        Category = new CategoryDto
                        {
                            Id = cartItem.Urun.Kategori.Id,
                            Name = cartItem.Urun.Kategori.KategoriAdi,
                            Description = "",
                            ImageUrl = ""
                        },
                        ImageUrl = !string.IsNullOrEmpty(cartItem.Urun.Resim) ? $"/img/{cartItem.Urun.Resim}" : "",
                        Stock = 100,
                        IsActive = cartItem.Urun.Aktif,
                        CreatedAt = cartItem.Urun.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                        UpdatedAt = cartItem.Urun.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                    },
                    Quantity = cartItem.Miktar,
                    UserId = GetCurrentUserId()
                };

                return Ok(new ApiResponse<CartItemDto>
                {
                    Success = true,
                    Data = cartItemDto
                });
            }

            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Ürün sepete eklenemedi"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Ürün sepete eklenirken hata oluştu"
            });
        }
    }

    [HttpPut("items/{itemId}")]
    public async Task<IActionResult> UpdateCartItem(int itemId, [FromBody] UpdateCartItemRequest request)
    {
        try
        {
            if (request.Quantity <= 0)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Miktar 0'dan büyük olmalıdır"
                });
            }

            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCart(userId);
            var cartItem = cart.CartItems.FirstOrDefault(i => i.CartItemId == itemId);

            if (cartItem == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Sepet öğesi bulunamadı"
                });
            }

            // Önce mevcut ürünü çıkar, sonra yeni miktarla ekle
            await _cartService.RemoveItem(cartItem.UrunId, cartItem.Miktar);
            await _cartService.AddToCart(cartItem.UrunId, request.Quantity);

            var updatedCart = await _cartService.GetCart(userId);
            var updatedItem = updatedCart.CartItems.FirstOrDefault(i => i.UrunId == cartItem.UrunId);

            if (updatedItem != null)
            {
                var cartItemDto = new CartItemDto
                {
                    Id = updatedItem.CartItemId,
                    ProductId = updatedItem.UrunId,
                    Product = new ProductDto
                    {
                        Id = updatedItem.Urun.Id,
                        Name = updatedItem.Urun.UrunAdi,
                        Description = updatedItem.Urun.Aciklama ?? "",
                        Price = updatedItem.Urun.Fiyat,
                        OriginalPrice = updatedItem.Urun.Fiyat,
                        CategoryId = updatedItem.Urun.KategoriId,
                        Category = new CategoryDto
                        {
                            Id = updatedItem.Urun.Kategori.Id,
                            Name = updatedItem.Urun.Kategori.KategoriAdi,
                            Description = "",
                            ImageUrl = ""
                        },
                        ImageUrl = !string.IsNullOrEmpty(updatedItem.Urun.Resim) ? $"/img/{updatedItem.Urun.Resim}" : "",
                        Stock = 100,
                        IsActive = updatedItem.Urun.Aktif,
                        CreatedAt = updatedItem.Urun.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                        UpdatedAt = updatedItem.Urun.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
                    },
                    Quantity = updatedItem.Miktar,
                    UserId = userId
                };

                return Ok(new ApiResponse<CartItemDto>
                {
                    Success = true,
                    Data = cartItemDto
                });
            }

            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Sepet güncellenemedi"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Sepet güncellenirken hata oluştu"
            });
        }
    }

    [HttpDelete("items/{itemId}")]
    public async Task<IActionResult> RemoveFromCart(int itemId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCart(userId);
            var cartItem = cart.CartItems.FirstOrDefault(i => i.CartItemId == itemId);

            if (cartItem == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Sepet öğesi bulunamadı"
                });
            }

            await _cartService.RemoveItem(cartItem.UrunId, cartItem.Miktar);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Ürün sepetten çıkarıldı"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Ürün sepetten çıkarılırken hata oluştu"
            });
        }
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCart(userId);

            foreach (var item in cart.CartItems.ToList())
            {
                await _cartService.RemoveItem(item.UrunId, item.Miktar);
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Sepet temizlendi"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Sepet temizlenirken hata oluştu"
            });
        }
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout()
    {
        try
        {
            var userId = GetCurrentUserId();
            var cart = await _cartService.GetCart(userId);

            if (!cart.CartItems.Any())
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Sepet boş"
                });
            }

            // Burada sipariş oluşturma işlemi yapılacak
            // Şimdilik basit bir sipariş ID döndürüyoruz
            var orderId = new Random().Next(1000, 9999);

            // Sepeti temizle
            foreach (var item in cart.CartItems.ToList())
            {
                await _cartService.RemoveItem(item.UrunId, item.Miktar);
            }

            return Ok(new ApiResponse<CheckoutResponse>
            {
                Success = true,
                Data = new CheckoutResponse
                {
                    OrderId = orderId
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Sipariş oluşturulurken hata oluştu"
            });
        }
    }

    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
    }
}

// DTOs
public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class UpdateCartItemRequest
{
    public int Quantity { get; set; }
}

public class CartDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public List<CartItemDto> Items { get; set; } = new();
    public double TotalAmount { get; set; }
    public int ItemCount { get; set; }
}

public class CartItemDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public ProductDto Product { get; set; } = null!;
    public int Quantity { get; set; }
    public string UserId { get; set; } = null!;
}

public class CheckoutResponse
{
    public int OrderId { get; set; }
} 