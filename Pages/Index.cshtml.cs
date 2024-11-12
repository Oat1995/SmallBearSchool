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
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AccessDbContext _context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณาใส่ Username")]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "กรุณาใส่ Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string ErrorMessage { get; set; } = string.Empty;

        public IndexModel(ILogger<IndexModel> logger, AccessDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        public IActionResult OnGet()
        {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("IsAdmin");
            HttpContext.Session.Remove("UserName");
            HttpContext.Session.Remove("ID");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // ตรวจสอบ Username และ Password ที่กรอกเข้ามา
            if (ModelState.IsValid)
            {
                // ใช้ LINQ เพื่อดึงข้อมูลจากฐานข้อมูล Access
                var UserAccount = await _context.UserAccount.Where(w => w.UserId == Username && w.Password == Password).FirstOrDefaultAsync();
                if (UserAccount == null)
                {
                    // กรณี Username หรือ Password ไม่ถูกต้อง
                    ErrorMessage = "ผู้ใช้งาน หรือ รหัสผ่านไม่ถูกต้อง";
                    // ถ้าการตรวจสอบไม่ผ่านให้กลับไปที่หน้า Login
                    return Page();
                }
                else
                {
                    if (UserAccount.IsActive)
                    {
                        HttpContext.Session.SetString("IsAdmin", UserAccount.UserType.ToString());
                        HttpContext.Session.SetString("UserName", UserAccount.UserName.ToString());
                        HttpContext.Session.SetString("ID", UserAccount.ID.ToString());
                        // ถ้าสำเร็จ สามารถเปลี่ยนเส้นทางไปหน้าอื่นได้ เช่นหน้า Index
                        return RedirectToPage("/Home");
                    }
                    else ErrorMessage = "ผู้ใช้งานถูกระงับการใช้งาน";
                }                
            }

            return Page();
        }
    }
}
