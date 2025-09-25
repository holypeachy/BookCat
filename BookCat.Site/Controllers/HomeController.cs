using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookCat.Site.Models;
using BookCat.Site.Services;

namespace BookCat.Site.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly GoogleBooksService _booksService;

    public HomeController(ILogger<HomeController> logger, GoogleBooksService booksService)
    {
        _logger = logger;
        _booksService = booksService;
    }

    public async Task<IActionResult> Index()
    {
        // await _booksService.BookSearchIdentifier("978-1-266-79685-2");
        await _booksService.BookSearchName("The Maze Runner");
        return View();
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
