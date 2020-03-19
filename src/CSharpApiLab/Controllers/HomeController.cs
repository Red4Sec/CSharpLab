using CSharpApiLab.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CSharpApiLab.Controllers
{
    [ApiController, Route("home"), AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly GenericDataContext _context;

        public HomeController(GenericDataContext context)
        {
            _context = context;
        }

        [HttpGet("index")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            var userId = User.Claims.Where(u => u.Type == "id").Select(u => u.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var user = _context.Users
                .Where(u => u.ID == Convert.ToInt32(userId))
                .AsNoTracking()
                .FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpGet("login")]
        public IActionResult Login(int errorCode = 0)
        {
            return View(errorCode);
        }

        [HttpGet("newUser")]
        public IActionResult NewUser()
        {
            return View();
        }

        [Authorize]
        [HttpGet("servers")]
        public IActionResult Servers(string filter = null)
        {
            var userId = User.Claims.Where(u => u.Type == "id").Select(u => u.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(userId))
            {
                return NotFound();
            }

            var where = "";
            if (!string.IsNullOrEmpty(filter))
            {
                where += $" AND (ID like '%{filter}%' OR Name like '%{filter}%' OR Ip like '%{filter}%')";
            }

            var entries = _context.Servers
                .FromSqlRaw($"SELECT * FROM Servers WHERE USERID={userId}" + where)
                .AsNoTracking()
                .ToList();

            return View(entries);
        }

        [HttpGet("api")]
        public IActionResult Api()
        {
            return Redirect("/swagger/index.html");
        }
    }
}
