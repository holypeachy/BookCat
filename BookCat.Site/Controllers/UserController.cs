using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Repos;
using Microsoft.AspNetCore.Identity;

namespace BookCat.Site.Controllers;

public class UserController : Controller
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IRepo<Book> _books;
    private readonly IRepo<Review> _reviews;

    public UserController(IRepo<Book> bookRepo, IRepo<Review> reviewRepo, UserManager<AppUser> userManager)
    {
        _books = bookRepo;
        _reviews = reviewRepo;
        _userManager = userManager;
    }

    [HttpGet("User/{id}")]
    public async Task<IActionResult> Index(string id)
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
