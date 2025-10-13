using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Services;
using BookCat.Site.Repos;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace BookCat.Site.Controllers;

public class BooksController : Controller
{
    public class CatalogIndexModel
    {
        public List<Book>? Books { get; set; }
        public List<Review>? Reviews { get; set; }
        [Required]
        public string Search { get; set; } = string.Empty;
    }

    private readonly ILogger<HomeController> _logger;
    private readonly GoogleBooksService _googleAPI;
    private readonly IRepo<Book> _books;
    private readonly IRepo<Review> _reviews;
    private readonly IRepo<BookIdentifier> _identifiers;

    public BooksController(ILogger<HomeController> logger, GoogleBooksService googleBooksService, IRepo<Book> bookRepo, IRepo<Review> reviewRepo, IRepo<BookIdentifier> identifierRepo)
    {
        _logger = logger;
        _googleAPI = googleBooksService;
        _books = bookRepo;
        _reviews = reviewRepo;
        _identifiers = identifierRepo;
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
        model.Reviews = (await _reviews.GetAllAsync()).ToList();
        model.Reviews.Add(model.Reviews[0]);
        model.Reviews.Add(model.Reviews[0]);
        model.Reviews.Add(model.Reviews[0]);
        return View(model);
    }

    public async Task<IActionResult> Test()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Search(CatalogIndexModel model)
    {
        string query = model.Search.Trim();
        List<Book> books;

        if (BookHelpers.IsISBN(query))
        {
            var identifiers = (await _identifiers.GetAllAsync()).Where(i => i.Value == query).ToList();
            if(identifiers.Count < 1)
            {
                var bookDtos = await _googleAPI.BookSearchIdentifier(query);
                return View("AddResults", bookDtos);
            }

            books = new();
            foreach (var item in identifiers)
            {
                books.Add(item.Book);
            }
            return View("Results", books);
        }
        else
        {
            books = new();
            books = (await _books.GetAllAsync()).Where(b => b.Title.Contains(query)).ToList();
            if (books.Count < 1)
            {
                var bookDtos = await _googleAPI.BookSearchName(query);
                return View("AddResults", bookDtos);
            }

            return View("Results", books);
        }
    }

    [Authorize]
    public async Task<IActionResult> Add(BookDto bookDto)
    {
        return View();
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
