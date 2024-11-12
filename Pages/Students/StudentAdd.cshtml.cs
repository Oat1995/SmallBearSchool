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
using System.Runtime.InteropServices.JavaScript;

namespace SmallBearSchool.Pages.Students
{
    public class StudentAddModel : PageModel
    {
        private readonly ILogger<StudentAddModel> _logger;
        private readonly AccessDbContext _context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public UserAccount _Student { get; set; } = new UserAccount();

        public string ErrorMessage { get; set; } = string.Empty;

        public StudentAddModel(ILogger<StudentAddModel> logger, AccessDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult OnGet()
        {
            //เช็คเงื่อนไขการ login
            var isAdminValue = HttpContext.Session.GetString("IsAdmin");
            var usernameValue = HttpContext.Session.GetString("UserName");
            if (isAdminValue != null && usernameValue != null)
            {
                //ถ้าไม่ใช่ผู้ดูแลระบบให้กลับไปที่หน้า login
                if (isAdminValue.ToLower() == "false")
                    return RedirectToPage("/Index");

                return Page();
            }
            else return RedirectToPage("/Index");
        }


        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                //ดึงข้อมูลทั้งหมดจาก DB มาแสดง
                var Student = await _context.UserAccount.ToListAsync();
                //เช็คการกรอกชื่อซ้ำ
                if (Student.Where(w => w.UserName == _Student.UserName || w.UserId == _Student.UserId).Any())
                {
                    ErrorMessage = "ชื่อนักเรียน หรือชื่อผู้ใช้งานมีข้อมูลอยู่แล้วในระบบ";
                }
                else
                {
                    _Student.UserType = false;
                    _context.Add(_Student);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/Students/Index");
                }
            }
            return Page();
        }
    }
}
