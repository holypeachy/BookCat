using BookCat.Site.Models;
using Microsoft.AspNetCore.Identity;

namespace BookCat.Site.Middleware;

public class ValidateUserMiddleware
{
    private readonly RequestDelegate _next;

    public ValidateUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var user = await userManager.GetUserAsync(context.User);
            if (user == null)
            {
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Home/Index");
                return;
            }
        }

        await _next(context);
    }
}
