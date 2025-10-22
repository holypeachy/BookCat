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
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return View("Error", $"User with id \"{id}\" not found.");
        var model = new UserViewModel();
        model.User = user;
        model.BooksAdded = (await _books.GetByUserIdAsync(user.Id)).ToList().Count;
        model.Reviews = (await _reviews.GetByUserIdAsync(user.Id)).Where(r => r.AdminDeleted == false).OrderByDescending(r => r.PostedAt).ToList();
        model.TotalReviews = model.Reviews.Count;
        model.Reviews = model.Reviews.Take(10).ToList();

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public class UserViewModel
    {
        public AppUser User { get; set; }
        public int TotalReviews { get; set; }
        public int BooksAdded { get; set; }
        public List<Review> Reviews{ get; set; }
    }
}
