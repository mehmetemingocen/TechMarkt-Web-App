using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using dotnet_store.Models;

public static class SeedDatabase
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Veritabanını oluştur (yoksa) ve migration'ları uygula
        await context.Database.MigrateAsync();

        // Admin rolünü ekle
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new AppRole { Name = "Admin" });
        }

        // Admin kullanıcısını oluştur
        var adminUser = await userManager.FindByEmailAsync("info@sadikturan.com");
        if (adminUser == null)
        {
            adminUser = new AppUser
            {
                AdSoyad = "Sadık Turan",
                UserName = "sadikturan",
                Email = "info@sadikturan.com"
            };
            
            await userManager.CreateAsync(adminUser, "12345678");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Müşteri kullanıcısını oluştur
        var customerUser = await userManager.FindByEmailAsync("info@cinarturan.com");
        if (customerUser == null)
        {
            customerUser = new AppUser
            {
                AdSoyad = "Çınar Turan",
                UserName = "cinarturan",
                Email = "info@cinarturan.com"
            };
            
            await userManager.CreateAsync(customerUser, "12345678");
        }
    }
}