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
    public List<string> Messages = new()
    {
        "Meow!", "Book worms!!!", "How about some tea?", "Get your bookshelf ready!", "📖😺 Lots of text here!", "Writer's Block?",
        "90% bug free!", "Tell your friends!", "20 GOTO 10!", "Forced to use JavaScript", "I love matcha!", "Kangaroo Court 🦘",
        "Instant Crush 🤖", "Akane Kurokawa"
    };
    public readonly Random _rand = new();

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
            ReviewCount = await _reviews.GetCountAsync(),
            Message = Messages[_rand.Next(0, Messages.Count)]
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
        public string Message { get; set; }
    }
}
