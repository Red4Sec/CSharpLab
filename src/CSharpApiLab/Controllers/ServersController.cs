using CSharpApiLab.DB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CSharpApiLab.Controllers
{
    [ApiController, Route("servers")]
    public class ServersController : Controller
    {
        private readonly GenericDataContext _context;

        public ServersController(GenericDataContext context)
        {
            _context = context;
        }

        [HttpGet, Authorize]
        public IEnumerable<Server> Get()
        {
            var userId = User.Claims.Where(u => u.Type == "id").Select(u => u.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            return _context.Servers.Where(u => u.UserId == Convert.ToInt32(userId)).ToList();
        }

        [HttpGet("new"), Authorize]
        public IActionResult New(int id, string name, string ip, string redirectOK = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.Claims.Where(u => u.Type == "id").Select(u => u.Value).FirstOrDefault();

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException();
            }

            var entry = _context.Servers.SingleOrDefault(m => m.ID == id && m.UserId == Convert.ToInt32(userId));
            if (entry != null)
            {
                entry.Name = name;
                entry.Ip = ip;
            }
            else
            {
                entry = new Server()
                {
                    UserId = Convert.ToInt32(userId),
                    ID = id,
                    Name = name,
                    Ip = ip
                };
                _context.Servers.Add(entry);
            }

            _context.SaveChanges();

            if (!string.IsNullOrEmpty(redirectOK))
            {
                return Redirect(redirectOK);
            }

            return Ok(entry);
        }

        [HttpGet("ping")]
        public IActionResult Ping(int userId, int id)
        {
            var entry = _context.Servers.SingleOrDefault(m => m.ID == id && m.UserId == userId);

            if (entry == null)
            {
                return NotFound();
            }

            var proc = "cmd.exe";
            var args = @"/C ping " + entry.Ip;

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                proc = "ping";
                args = "-c 4 " + entry.Ip;
            }

            var info = new ProcessStartInfo(proc, args)
            {
                RedirectStandardOutput = true
            };

            var pr = Process.Start(info);
            pr.WaitForExit();

            return Ok(pr.StandardOutput.ReadToEnd());
        }

        [HttpGet("delete")]
        public IActionResult Delete(int userId, int id, string redirectOK = null)
        {
            var entry = _context.Servers.SingleOrDefault(m => m.ID == id && m.UserId == userId);

            if (entry == null)
            {
                return NotFound();
            }

            _context.Servers.Remove(entry);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(redirectOK))
            {
                return Redirect(redirectOK);
            }

            return Ok(entry);
        }

        [HttpGet("exists"), Authorize]
        public bool Exists(int userId, string ip)
        {
            return _context.Servers
                .FromSqlRaw($"SELECT Id FROM Servers WHERE USERID={userId} AND Ip like '%{ip}%'")
                .AsNoTracking()
                .Count() > 0;
        }
    }
}
