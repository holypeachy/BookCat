using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Services;
using BookCat.Site.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using BookCat.Site.Data;
using Microsoft.AspNetCore.Authentication;

namespace BookCat.Site.Controllers;

public class BooksController : Controller
{
    public class CatalogIndexModel
    {
        public List<Book>? Books { get; set; }
        public List<Review>? Reviews { get; set; }
    }

    public class ResultsModel
    {
        public List<Book>? Books { get; set; }
        public string Query { get; set; } = string.Empty;
    }

    public class ReviewModel
    {
        [Required]
        public string BookId { get; set; }
        [Required]
        public int Rating { get; set; }
        [MaxLength(50)]
        [Required]
        public string Title { get; set; }
        [Required]
        public string Comment { get; set; }
    }

    private readonly ILogger<HomeController> _logger;
    private readonly GoogleBooksService _googleAPI;
    private readonly IRepo<Book> _books;
    private readonly IRepo<Review> _reviews;
    private readonly IRepo<BookIdentifier> _identifiers;
    private readonly UserManager<AppUser> _userManager;

    public BooksController(ILogger<HomeController> logger,
        GoogleBooksService googleBooksService,
        IRepo<Book> bookRepo, IRepo<Review> reviewRepo,
        IRepo<BookIdentifier> identifierRepo,
        UserManager<AppUser> userManager)
    {
        _logger = logger;
        _googleAPI = googleBooksService;
        _books = bookRepo;
        _reviews = reviewRepo;
        _identifiers = identifierRepo;
        _userManager = userManager;
    }

    [HttpGet("Books/")]
    public async Task<IActionResult> Index()
    {
        CatalogIndexModel model = new();

        List<Book> books = (await _books.GetAllAsync()).ToList();
        books = books.OrderByDescending(b => b.AddedOn).ToList();
        if (books.Count > 10) books = books.GetRange(0, 10);
        model.Books = books;

        List<Review> reviews = (await _reviews.GetAllAsync()).ToList();
        reviews = reviews.OrderByDescending(b => b.PostedAt).ToList();
        if (reviews.Count > 5) reviews = reviews.GetRange(0, 5);
        model.Reviews = reviews;

        return View(model);
    }

    [HttpGet("Books/Details/{id}")]
    public async Task<IActionResult> Details(string id)
    {
        try
        {
            var book = await _books.GetByIdAsync(new Guid(id));
            book.Reviews = (await _reviews.GetAllAsync()).Where( r => r.Book.Id == book.Id).ToList();
            book.Reviews = book.Reviews.OrderByDescending(r => r.PostedAt).ToList();
            // if (book.Reviews.Count > 5) book.Reviews = book.Reviews.GetRange(0, 5);
            return View("Details", book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, string.Empty);
            return View("Error", $"Book with id \"{id}\" not found.");
        }
    }

    [HttpGet("Books/Details/{id}/{page}")]
    public async Task<IActionResult> Details(string id, int page)
    {
        var book = await _books.GetByIdAsync(new Guid(id));
        book.Reviews = (await _reviews.GetAllAsync()).Where( r => r.Book.Id == book.Id).ToList();
        // try
        // {
        //     book.Reviews = book.Reviews.GetRange(5 * page - 5, 5);
        // }
        // catch
        // {
        //     return BadRequest($"Number of Products: {book.Reviews.Count}");
        // }
        book.Reviews = book.Reviews.OrderByDescending(r => r.PostedAt).ToList();
        return View("Details", book);
    }

    public async Task<IActionResult> Test()
    {
        return View();
    }

    public async Task<IActionResult> Search(string query)
    {
        string cleanQuery = query.Trim().ToLower();
        List<Book> books;

        if (BookHelpers.IsISBN(cleanQuery))
        {
            var identifiers = (await _identifiers.GetAllAsync()).Where(i => i.Value.ToLower() == cleanQuery).ToList();
            if(identifiers.Count < 1)
            {
                var bookDtos = await _googleAPI.BookSearchIdentifier(cleanQuery);
                return View("AddResults", bookDtos);
            }

            books = new();
            foreach (var item in identifiers)
            {
                books.Add(item.Book);
            }
            return View("Results", new ResultsModel{ Books = books, Query = query});
        }
        else
        {
            books = new();
            books = (await _books.GetAllAsync()).Where(b => b.Title.ToLower().Contains(cleanQuery)).ToList();
            if (books.Count < 1)
            {
                var bookDtos = await _googleAPI.BookSearchName(cleanQuery);
                return View("AddResults", bookDtos);
            }

            return View("Results", new ResultsModel{ Books = books, Query = query});
        }
    }

    public async Task<IActionResult> ExSearch(string query)
    {
        Console.WriteLine(query);
        string cleanQuery = query.Trim().ToLower();

        List<BookDto> bookDtos;
        if (BookHelpers.IsISBN(cleanQuery))
        {
            bookDtos = await _googleAPI.BookSearchIdentifier(cleanQuery);
        }
        else
        {
            bookDtos = await _googleAPI.BookSearchName(cleanQuery);
        }

        return View("AddResults", bookDtos);
    }

    [Authorize]
    public async Task<IActionResult> Add(string id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Login", "Account");
        }
        // Check if it exists in Db
        var dbBooks = (await _books.GetAllAsync()).ToList();
        var results = dbBooks.Where(b => b.GoogleId == id).ToList();

        if (results.Count > 0) return RedirectToAction("Details", "Books", new { id = results[0].Id.ToString() });

        BookDto dto = await _googleAPI.GetBookById(id);
        Book book = new()
        {
            Id = Guid.NewGuid(),
            GoogleId = dto.GoogleId,
            Title = dto.Title,
            Subtitle = dto.Subtitle,
            Author = dto.Author,
            Description = dto.Description,
            Publisher = dto.Publisher,
            PublishedDate = dto.PublishedDate,
            CoverUrl = dto.CoverUrl,
            AddedOn = DateOnly.FromDateTime(DateTime.Now),
            AddedById = (await _userManager.GetUserAsync(User))?.Id
        };

        await _books.AddAsync(book);

        if (dto.Identifiers is not null)
        {
            foreach (var item in dto.Identifiers)
            {
                var idf = new BookIdentifier
                {
                    Id = Guid.NewGuid(),
                    BookId = book.Id,
                    Type = item.Type,
                    Value = item.Identifier ?? "Not Found"
                };
                await _identifiers.AddAsync(idf);
            }
        }

        return RedirectToAction($"Details", "Books", new { id = book.Id.ToString("D") });
    }

    [Authorize]
    public async Task<IActionResult> WriteReview(string id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Login", "Account");
        }
        Book book = await _books.GetByIdAsync(new Guid(id));
        return View("Review", book);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddReview(ReviewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Login", "Account");
        }

        Review review = new()
        {
            Id = Guid.NewGuid(),
            BookId = new Guid(model.BookId),
            UserId = (await _userManager.GetUserAsync(User)).Id,
            Rating = model.Rating,
            Title = model.Title,
            Comment = model.Comment,
            PostedAt = DateTime.Now
        };
        await _reviews.AddAsync(review);

        return RedirectToAction("Details", "Books", new {id = model.BookId});
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
