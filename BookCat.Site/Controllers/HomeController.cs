using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Repos;

namespace BookCat.Site.Controllers;

public class HomeController : Controller
{
    private readonly IRepo<Book> _books;
    private readonly IRepo<Review> _reviews;

    public HomeController(IRepo<Book> bookRepo, IRepo<Review> reviewRepo)
    {
        _books = bookRepo;
        _reviews = reviewRepo;
    }

    public async Task<IActionResult> Index()
    {
        List<Book> books = (await _books.GetAllAsync()).OrderByDescending(b => b.AddedOn).ToList();
        int bookCount = books.Count;

        if (bookCount > 7) books = books.GetRange(0, 7);

        IndexViewModel model = new()
        {
            Books = books,
            BookCount = bookCount,
            ReviewCount = await _reviews.GetCount()
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
