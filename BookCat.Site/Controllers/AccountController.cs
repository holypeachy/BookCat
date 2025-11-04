using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using BookCat.Site.Data;
using BookCat.Site.Repos;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BookCat.Site.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IRepo<Book> _books;
    private readonly IRepo<Review> _reviews;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IRepo<Book> booksRepo, IRepo<Review> reviewRepo)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _books = booksRepo;
        _reviews = reviewRepo;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var model = new DashViewModel
        {
            BooksAdded = (await _books.GetByUserIdAsync(user.Id)).ToList().Count,
            TotalReviews = (await _reviews.GetByUserIdAsync(user.Id)).ToList().Count,
            User = user
        };
        return View(model);
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePfp(IFormFile pfp)
    {
        if (pfp is null || pfp.Length == 0) return View("Error", "Error when uploading file.");
        var user = await _userManager.GetUserAsync(User);

        if (!string.IsNullOrEmpty(user.UserImageUrl))
        {
            string oldPath = Path.Combine("wwwroot", user.UserImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
        }


        using var image = await Image.LoadAsync(pfp.OpenReadStream());

        image.Mutate(x => x.Resize(500, 0));

        string newFileName = $"{Guid.NewGuid()}.jpg";
        var filePath = Path.Combine("wwwroot", "user-images", newFileName);

        await image.SaveAsJpegAsync(filePath, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
        {
            Quality = 80
        });

        filePath = $"/user-images/{newFileName}";
        user.UserImageUrl = filePath;
        await _userManager.UpdateAsync(user);

        return RedirectToAction("Index");
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeBio(string? bio)
    {
        var user = await _userManager.GetUserAsync(User);
        user.Bio = bio ?? string.Empty;
        await _userManager.UpdateAsync(user);

        return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<IActionResult> ChangeEmail()
    {
        var user = await _userManager.GetUserAsync(User);
        ChangeEmailViewModel model = new()
        {
            Email = user.Email
        };
        return View(model);
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);
        user.Email = model.NewEmail;

        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded) return RedirectToAction("Index");

        foreach (var error in result.Errors) ModelState.AddModelError("NewEmail", error.Description);

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> ChangePassword()
    {
        return View(new ChangePasswordViewModel());
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);

        var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

        if (result.Succeeded) return RedirectToAction("Index");

        foreach (var error in result.Errors) ModelState.AddModelError("OldPassword", error.Description);

        return View(model);
    }

    [Authorize]
    public async Task<IActionResult> DeleteAccount()
    {
        return View(new DeleteAccountViewModel());
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.GetUserAsync(User);

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(user.UserImageUrl))
            {
                string oldPath = Path.Combine("wwwroot", user.UserImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors) ModelState.AddModelError("ConfirmDelete", error.Description);

        return View(model);
    }

    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Books");
        return View(new LoginViewModel { ReturnUrl = returnUrl ?? string.Empty });
    }

    [HttpPost]
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
            if (string.IsNullOrEmpty(model.ReturnUrl)) return RedirectToAction("Index", "Books");
            return LocalRedirect(model.ReturnUrl);
        }

        ModelState.AddModelError("Email", "Invalid Login Attempt");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Register(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Books");
        return View(new RegisterViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new AppUser
        {
            Email = model.Email,
            UserName = model.Username,
            JoinedDateTime = DateTime.Now
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: true);

            var userEntity = await _userManager.FindByEmailAsync(model.Email);
            await _userManager.AddToRoleAsync(userEntity, Roles.User);
            if (string.IsNullOrEmpty(model.ReturnUrl)) return RedirectToAction("Index", "Books");
            return LocalRedirect(model.ReturnUrl);
        }

        foreach (var error in result.Errors) ModelState.AddModelError("Email", error.Description);

        return View(model);
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

        [Required, Length(5, 15, ErrorMessage = "Username must be between 5-20 characters")]
        public string Username { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
    }

    public class DashViewModel
    {
        public AppUser User { get; set; }
        public int BooksAdded { get; set; }
        public int TotalReviews { get; set; }
    }

    public class ChangeEmailViewModel
    {
        public string? Email { get; set; }

        [Required(ErrorMessage = "Enter your new email"), EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string NewEmail { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Entered password doesn't match your old password"), DataType(DataType.Password)]
        public string OldPassword { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password), Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class DeleteAccountViewModel
    {
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Type \"Delete\""), ConfirmText("Delete")]
        public string ConfirmDelete { get; set; }
    }
}
