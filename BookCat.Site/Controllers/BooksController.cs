using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Services;
using BookCat.Site.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;

namespace BookCat.Site.Controllers;

public class BooksController : Controller
{

    private readonly GoogleBooksService _googleAPI;
    private readonly IRepo<Book> _books;
    private readonly IRepo<Review> _reviews;
    private readonly IRepo<BookIdentifier> _identifiers;
    private readonly UserManager<AppUser> _userManager;

    public BooksController(GoogleBooksService googleBooksService,
        IRepo<Book> bookRepo, IRepo<Review> reviewRepo,
        IRepo<BookIdentifier> identifierRepo,
        UserManager<AppUser> userManager)
    {
        _googleAPI = googleBooksService;
        _books = bookRepo;
        _reviews = reviewRepo;
        _identifiers = identifierRepo;
        _userManager = userManager;
    }

    [HttpGet("Books/")]
    public async Task<IActionResult> Index()
    {
        CatalogViewModel model = new();

        model.Books = (await _books.GetAllAsync()).OrderByDescending(b => b.AddedOn).Take(7).ToList();
        model.Reviews = (await _reviews.GetAllAsync()).OrderByDescending(b => b.PostedAt).Where(b => b.AdminDeleted == false).Take(7).ToList();

        return View(model);
    }

    [HttpGet("Books/Details/{id}/{page?}")]
    public async Task<IActionResult> Details(string id, int page = 1)
    {
        int pageSize = 5;

        var book = await _books.GetByIdAsync(new Guid(id));
        if (book is null) return View("Error", "Book not found");

        book.Reviews = (await _reviews.GetByBookIdAsync(new Guid(id)))
                        .Where(r => r.AdminDeleted == false).OrderByDescending(r => r.PostedAt).ToList();

        var currentUser = await _userManager.GetUserAsync(User);
        Guid? userReviewId = null;
        if(currentUser is not null)
        {
            var reviews = book.Reviews.Where(r => r.UserId == currentUser.Id).Take(1).ToList();
            if (reviews.Count > 0) userReviewId = reviews[0].Id;
        }
        int reviewCount = book.Reviews.Count;

        int maxPages = book.Reviews.Count / pageSize;
        if (book.Reviews.Count % pageSize != 0) maxPages++;

        if (page >= maxPages)
        {
            book.Reviews = book.Reviews.Skip((maxPages - 1) * pageSize).Take(pageSize).ToList();
            return View("Details", new BookDetailsViewModel{ Book = book, CurrentPage = page > maxPages ? maxPages : page, TotalPages = maxPages, UserReviewId = userReviewId});
        }

        book.Reviews = book.Reviews.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        return View("Details", new BookDetailsViewModel { Book = book, CurrentPage = page < 1 ? 1 : page, TotalPages = maxPages, TotalReviews = reviewCount, UserReviewId = userReviewId});
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
            return View("Results", new ResultsViewModel{ Books = books, Query = query});
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

            return View("Results", new ResultsViewModel{ Books = books, Query = query});
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
        Book? book = await _books.GetByIdAsync(new Guid(id));
        if (book is null) return View("Error", $"Book not found {id}");
        return View("Review", new ReviewViewModel{ Book = book});
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddReview(ReviewViewModel model)
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public class BookDetailsViewModel
    {
        public Book Book { get; set; }
        public int TotalReviews { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public Guid? UserReviewId { get; set; }
    }

    public class CatalogViewModel
    {
        public List<Book> Books { get; set; } = [];
        public List<Review> Reviews { get; set; } = [];
    }

    public class ResultsViewModel
    {
        public List<Book>? Books { get; set; }
        public string Query { get; set; } = string.Empty;
    }

    public class ReviewViewModel
    {
        public Book? Book { get; set; }

        [Required]
        public string BookId { get; set; }

        [Required, Range(1, 5, ErrorMessage = "Please select a rating")]
        public int Rating { get; set; }

        [Required, MaxLength(70)]
        public string Title { get; set; }

        [Required]
        public string Comment { get; set; }
    }

}
