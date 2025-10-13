using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Services;
using BookCat.Site.Repos;

namespace BookCat.Site.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepo<Book> _books;

    public HomeController(ILogger<HomeController> logger, IRepo<Book> bookRepo)
    {
        _logger = logger;
        _books = bookRepo;
    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Book> books = await _books.GetAllAsync();
        List<Book> list =
        [
            books.ToList()[0],
            books.ToList()[0],
            books.ToList()[0],
            books.ToList()[0],
            books.ToList()[0],
            books.ToList()[0],
            books.ToList()[0],
        ];
        return View(list.GetRange(0, 7));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
