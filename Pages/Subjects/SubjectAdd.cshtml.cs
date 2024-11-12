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

namespace SmallBearSchool.Pages.Subjects
{
    public class SubjectAddModel : PageModel
    {
        private readonly ILogger<SubjectAddModel> _logger;
        private readonly AccessDbContext _context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public Subject _Subject { get; set; } = new Subject();

        public string ErrorMessage { get; set; } = string.Empty;

        public SubjectAddModel(ILogger<SubjectAddModel> logger, AccessDbContext context)
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
                var Subject = await _context.Subject.ToListAsync();
                //เช็คการกรอกชื่อซ้ำ
                if (Subject.Where(w => w.Name == _Subject.Name).Any())
                {
                    ErrorMessage = "มีข้อมูลอยู่แล้วในระบบ";
                }
                else
                {
                    _context.Add(_Subject);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/Subjects/Index");
                }
            }
            return Page();
        }
    }
}
