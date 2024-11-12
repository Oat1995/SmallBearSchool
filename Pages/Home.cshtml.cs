using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmallBearSchool.Models;
using System.Reflection.PortableExecutable;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SmallBearSchool.Pages
{
    public class HomeModel : PageModel
    {
        private readonly ILogger<HomeModel> _logger;
        private readonly AccessDbContext _context;

        public bool IsAdmin { get; set; } = false;
        public string Username { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public HomeModel(ILogger<HomeModel> logger, AccessDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult OnGet()
        {
            var isAdminValue = HttpContext.Session.GetString("IsAdmin");
            var usernameValue = HttpContext.Session.GetString("UserName");
            if (isAdminValue != null && usernameValue != null)
            {
                IsAdmin = isAdminValue == "True"; // แปลงค่าเป็น bool
                Username = usernameValue == null ? "" : usernameValue;

                return Page();
            }
            else return RedirectToPage("/Index");
        }
    }
}
