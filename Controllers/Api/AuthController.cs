using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotnet_store.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_store.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Geçersiz veri",
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray()
            });
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Geçersiz email veya şifre"
            });
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
        if (!result.Succeeded)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Geçersiz email veya şifre"
            });
        }

        var token = GenerateJwtToken(user);
        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.AdSoyad.Split(' ').FirstOrDefault() ?? "",
            LastName = user.AdSoyad.Split(' ').Skip(1).FirstOrDefault() ?? "",
            Phone = user.PhoneNumber,
            CreatedAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
            UpdatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
        };

        return Ok(new ApiResponse<LoginResponse>
        {
            Success = true,
            Data = new LoginResponse
            {
                User = userDto,
                Token = token
            }
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Geçersiz veri",
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray()
            });
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Bu email adresi zaten kullanılıyor"
            });
        }

        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            AdSoyad = $"{request.FirstName} {request.LastName}".Trim(),
            PhoneNumber = request.Phone
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Kayıt oluşturulamadı",
                Errors = result.Errors.Select(e => e.Description).ToArray()
            });
        }

        var token = GenerateJwtToken(user);
        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            CreatedAt = user.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
            UpdatedAt = user.UpdatedAt.ToString("yyyy-MM-ddTHH:mm:ss")
        };

        return Ok(new ApiResponse<LoginResponse>
        {
            Success = true,
            Data = new LoginResponse
            {
                User = userDto,
                Token = token
            }
        });
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Başarıyla çıkış yapıldı"
        });
    }

    private string GenerateJwtToken(AppUser user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "your-secret-key-here"));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.AdSoyad)
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"] ?? "hepsisurada",
            audience: _configuration["Jwt:Audience"] ?? "hepsisurada-mobile",
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// DTOs
public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
}

public class LoginResponse
{
    public UserDto User { get; set; } = null!;
    public string Token { get; set; } = null!;
}

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string CreatedAt { get; set; } = null!;
    public string UpdatedAt { get; set; } = null!;
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string[]? Errors { get; set; }
} 