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

    [HttpGet("User/{id}/{page?}")]
    public async Task<IActionResult> Index(string id, int page = 1)
    {
        int pageSize = 10;

        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return View("Error", $"User with id \"{id}\" not found.");
        var model = new UserViewModel
        {
            User = user,
            BooksAdded = (await _books.GetByUserIdAsync(user.Id)).ToList().Count,
            Reviews = (await _reviews.GetByUserIdAsync(user.Id)).Where(r => r.AdminDeleted == false).OrderByDescending(r => r.PostedAt).ToList()
        };
        model.TotalReviews = model.Reviews.Count;

        int maxPages = model.Reviews.Count / pageSize;
        if (model.Reviews.Count % pageSize != 0) maxPages++;

        if (page >= maxPages)
        {
            model.Reviews = model.Reviews.Skip((maxPages - 1) * pageSize).Take(pageSize).ToList();
            model.CurrentPage = maxPages;
            model.TotalPages = maxPages;
            return View(model);
        }

        model.CurrentPage = page < 1 ? 1 : page;
        model.Reviews = model.Reviews.Skip((model.CurrentPage - 1) * pageSize).Take(pageSize).ToList();
        model.TotalPages = maxPages;

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
        public int BooksAdded { get; set; }
        public List<Review> Reviews { get; set; }
        public int TotalReviews { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
