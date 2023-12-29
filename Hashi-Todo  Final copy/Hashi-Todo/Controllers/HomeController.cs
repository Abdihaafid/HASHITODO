using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Hashi_Todo.Models;

namespace Hashi_Todo.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }
}

