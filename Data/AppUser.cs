using Microsoft.AspNetCore.Identity;

namespace dotnet_store.Models;

public class AppUser : IdentityUser<int>
{
    public string AdSoyad { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}