using CSharpApiLab.DB;
using CSharpApiLab.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CSharpApiLab.Controllers
{
    [ApiController, Route("users")]
    public class UsersController : Controller
    {
        private readonly GenericDataContext _context;

        public UsersController(GenericDataContext context)
        {
            _context = context;
        }

        [Authorize(Roles = DB.User.RoleAdministrator)]
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _context.Users.ToList();
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string redirect = null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (!string.IsNullOrEmpty(redirect))
            {
                return Redirect(redirect);
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpGet("login")]
        public async Task<IActionResult> Login(string email, string password, string redirectOK = null, string redirectError = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entry = _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefault();

            // Check user

            if (entry == null)
            {
                if (!string.IsNullOrEmpty(redirectError))
                {
                    return Redirect(redirectError);
                }

                return NotFound("user not found");
            }

            // Check password

            if (entry != null && entry.Password != DB.User.GetHashedPassword(password))
            {
                if (!string.IsNullOrEmpty(redirectError))
                {
                    return Redirect(redirectError);
                }

                return NotFound("Wrong password");
            }

            var claims = new List<Claim>
            {
                new Claim("id", entry.ID.ToString()),
                new Claim(ClaimTypes.Name, entry.Name),
                new Claim(ClaimTypes.Role, entry.Role)
            };

            var claimsIdentity = new ClaimsIdentity(
              claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
              CookieAuthenticationDefaults.AuthenticationScheme,
              new ClaimsPrincipal(claimsIdentity),
              authProperties);

            if (!string.IsNullOrEmpty(redirectOK))
            {
                return Redirect(redirectOK);
            }

            return Ok(entry.ID);
        }

        [AllowAnonymous]
        [HttpPost("new")]
        public IActionResult New([FromForm] UserContentRequest user, string redirectOK = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (_context.Users.Count(u => u.Email == user.email) > 0)
            {
                // Don't allow same email account

                throw new Exception();
            }

            // TODO: We must copy this user to our backup server at 172.22.22.1

            var id = _context.Users.Count() == 0 ? 1 :
                _context.Users.Max(u => u.ID) + 1;

            var entry = new User
            {
                ID = id,
                Name = user.name,
                Email = user.email,
                Role = user.role
            };
            entry.UpdatePassword(user.password);

            _context.Users.Add(entry);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(redirectOK))
            {
                return Redirect(redirectOK);
            }

            return Ok(entry);
        }

        [HttpGet("avatar")]
        public IActionResult Avatar(string avatarPath = null)
        {
            if (!string.IsNullOrEmpty(avatarPath) && System.IO.File.Exists(avatarPath))
            {
                var image = System.IO.File.OpenRead(avatarPath);
                return File(image, "image/jpeg");
            }
            else
            {
                var image = System.IO.File.OpenRead($"wwwroot/avatar.png");
                return File(image, "image/jpeg");
            }
        }

        [Authorize]
        [HttpPost("updateAvatar")]
        public async Task<IActionResult> UpdateAvatar([FromForm] IFormFile file, string redirectOK = null, string redirectError = null)
        {
            var userId = User.Claims.Where(u => u.Type == "id").Select(u => u.Value).FirstOrDefault();
            var user = string.IsNullOrEmpty(userId) ? null : _context.Users.Where(u => u.ID == Convert.ToInt32(userId)).FirstOrDefault();

            if (user == null)
            {
                if (!string.IsNullOrEmpty(redirectError))
                {
                    return Redirect(redirectError);
                }

                return NotFound();
            }

            user.Avatar = $"wwwroot/avatars/{userId}/";

            if (!Directory.Exists(user.Avatar))
            {
                Directory.CreateDirectory(user.Avatar);
            }

            user.Avatar += file.FileName;

            using (var stream = System.IO.File.Create(user.Avatar))
            {
                await file.CopyToAsync(stream);
                _context.SaveChanges();
            }

            if (!string.IsNullOrEmpty(redirectOK))
            {
                return Redirect(redirectOK);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("update")]
        public IActionResult Update([FromForm] UserContentRequest user, string redirectOK = null, string redirectError = null)
        {
            if (!ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(redirectError))
                {
                    return Redirect(redirectError);
                }

                return BadRequest(ModelState);
            }

            var userId = User.Claims.Where(u => u.Type == "id").Select(u => u.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(userId))
            {
                if (!string.IsNullOrEmpty(redirectError))
                {
                    return Redirect(redirectError);
                }

                return NotFound();
            }

            var entry = _context.Users.SingleOrDefault(m => m.ID == Convert.ToInt32(userId));
            if (entry == null)
            {
                if (!string.IsNullOrEmpty(redirectError))
                {
                    return Redirect(redirectError);
                }

                return NotFound();
            }

            entry.Name = user.name;
            entry.Email = user.email;
            entry.Role = user.role;
            entry.UpdatePassword(user.password);

            _context.Users.Update(entry);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(redirectOK))
            {
                return Redirect(redirectOK);
            }

            return Ok(entry);
        }

        [Authorize]
        [HttpDelete("delete")]
        public IActionResult Delete(int id)
        {
            var entry = _context.Users.SingleOrDefault(m => m.ID == id);

            if (entry == null)
            {
                return NotFound();
            }

            _context.Users.Remove(entry);
            _context.SaveChanges();

            return Ok(entry);
        }
    }
}
