using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookCat.Site.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AccountController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    public async Task<IActionResult> Login()
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Books");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user is null)
        {
            ModelState.AddModelError("", "Invalid Login Attempt");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);

        if (result.Succeeded) return RedirectToAction("Index", "Books");

        ModelState.AddModelError("", "Invalid Login Attempt");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Register()
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Books");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        if (User.Identity?.IsAuthenticated == true) await _signInManager.SignOutAsync();

        if (!ModelState.IsValid) return View(model);

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("Password", "Passwords must match");
            return View(model);
        }

        var user = new AppUser
        {
            Email = model.Email,
            UserName = model.Username
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);
            Console.Beep();
            return RedirectToAction("Index", "Books");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError("Email", error.Description);

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

    public class LoginModel
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterModel
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
