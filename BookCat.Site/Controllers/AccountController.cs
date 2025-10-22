using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using BookCat.Site.Data;

namespace BookCat.Site.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Books");
        return View(new LoginViewModel{ReturnUrl = returnUrl ?? string.Empty});
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            ModelState.AddModelError("Email", "Invalid Login Attempt");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

        if (result.Succeeded)
        {
            if(string.IsNullOrEmpty(model.ReturnUrl)) return RedirectToAction("Index", "Books");
            return LocalRedirect(model.ReturnUrl);
        } 

        ModelState.AddModelError("Email", "Invalid Login Attempt");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Books");
        return View(new RegisterViewModel{ ReturnUrl = returnUrl});
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (User.Identity?.IsAuthenticated == true) await _signInManager.SignOutAsync();

        if (!ModelState.IsValid) return View(model);

        var user = new AppUser
        {
            Email = model.Email,
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);

            var entityUser = await _userManager.FindByEmailAsync(model.Email);
            await _userManager.AddToRoleAsync(entityUser, Roles.User);
            if(string.IsNullOrEmpty(model.ReturnUrl)) return RedirectToAction("Index", "Books");
            return LocalRedirect(model.ReturnUrl);
        }

        foreach (var error in result.Errors) ModelState.AddModelError("Email", error.Description);

        return View(model);
    }


    public IActionResult Privacy()
    {
        return Redirect("/Home/Privacy");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }
}
