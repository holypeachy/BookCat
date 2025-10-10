using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Services;
using BookCat.Site.Repos;

namespace BookCat.Site.Controllers;

public class BooksController : Controller
{
    public class CatalogIndexModel
    {
        public List<Book>? Books { get; set; }
        public List<Review>? Reviews { get; set; }
        public string? Search { get; set; }
    }

    private readonly ILogger<HomeController> _logger;
    private readonly GoogleBooksService _booksService;
    private readonly IRepo<Book> _books;
    private readonly IRepo<Review> _reviews;

    public BooksController(ILogger<HomeController> logger, GoogleBooksService booksService, IRepo<Book> bookRepo, IRepo<Review> reviewRepo)
    {
        _logger = logger;
        _booksService = booksService;
        _books = bookRepo;
        _reviews = reviewRepo;
    }

    public async Task<IActionResult> Index()
    {
        CatalogIndexModel model = new();
        model.Books = (await _books.GetAllAsync()).ToList();
        model.Books.Add(model.Books[0]);
        model.Books.Add(model.Books[0]);
        model.Books.Add(model.Books[0]);
        model.Books.Add(model.Books[0]);
        model.Books.Add(model.Books[0]);
        model.Books.Add(model.Books[0]);
        model.Reviews= (await _reviews.GetAllAsync()).ToList();
        model.Reviews.Add(model.Reviews[0]);
        model.Reviews.Add(model.Reviews[0]);
        model.Reviews.Add(model.Reviews[0]);
        return View(model);
    }

    public async Task<IActionResult> Search(CatalogIndexModel model)
    {
        Console.WriteLine(model.Search);
        return Redirect("/Books");
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
}
