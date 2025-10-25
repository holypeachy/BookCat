using BookCat.Site.Models;
using Microsoft.AspNetCore.Identity;

namespace BookCat.Site.Data;

public static class DataSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        if (!await roleManager.RoleExistsAsync(Roles.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(nameof(Roles.Admin)));
        }
        if (!await roleManager.RoleExistsAsync(Roles.User))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.User));
        }
    }

    public static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        UserManager<AppUser> userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

        await CreateUserWithRole(userManager, "testadmin@gmail.com", "Admin123!", "holypeach", Roles.Admin);
        await CreateUserWithRole(userManager, "testuser@gmail.com", "User123!", "TestUser", Roles.User);
    }

    private static async Task CreateUserWithRole(UserManager<AppUser> userManager, string email, string password, string username, string role)
    {
        if (await userManager.FindByEmailAsync(email) is null)
        {
            AppUser user = new()
            {
                Email = email,
                EmailConfirmed = true,
                UserName = username,
                JoinedDateTime = new DateTime(2025, 5, 23, 4, 6, 10)
            };

            IdentityResult result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
            else
            {
                throw new Exception($"Failed creating user with email \"{user.Email}\". Errors: \n{string.Join(' ', result.Errors)}");
            }
        }
    }
}