using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Repos;
using BookCat.Site.Data;

namespace BookCat.Site.Controllers;

public class HomeController : Controller
{
    private readonly IRepo<Book> _books;
    private readonly IRepo<Review> _reviews;
    private readonly AppDbContext _db;

    public HomeController(IRepo<Book> bookRepo, IRepo<Review> reviewRepo, AppDbContext db)
    {
        _books = bookRepo;
        _reviews = reviewRepo;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        List<Book> books = _db.Books.OrderByDescending(b => b.AddedOn).Take(7).ToList();

        IndexViewModel model = new()
        {
            Books = books,
            BookCount = await _books.GetCountAsync(),
            ReviewCount = await _reviews.GetCountAsync()
        };

        return View(model);
    }

    public IActionResult About()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public class IndexViewModel
    {
        public List<Book> Books { get; set; }
        public int BookCount { get; set; }
        public int ReviewCount { get; set; }
    }
}
