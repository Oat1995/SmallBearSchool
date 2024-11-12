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
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AccessDbContext _context;
        public List<UserAccount> _Student { get; set; } = new List<UserAccount>();
        public string ErrorMessage { get; set; } = string.Empty;

        public IndexModel(ILogger<IndexModel> logger, AccessDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            //เช็คเงื่อนไขการ login
            var isAdminValue = HttpContext.Session.GetString("IsAdmin");
            var usernameValue = HttpContext.Session.GetString("UserName");
            if (isAdminValue != null && usernameValue != null)
            {
                //ถ้าไม่ใช่ผู้ดูแลระบบให้กลับไปที่หน้า login
                if (isAdminValue.ToLower() == "false")
                    return RedirectToPage("/Index");

                //ดึงข้อมูลทั้งหมดจาก DB มาแสดง
                _Student = await _context.UserAccount.Where(w => w.UserType == false).ToListAsync();
                return Page();
            }
            else return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostEdit(int? id)
        {
            if (id == null)
            {
                //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                ErrorMessage = "ไม่พบข้อมูล";
                return Page();
            }

            //เรียกดูข้อมูลด้วย id
            var Student = await _context.UserAccount.Where(w => w.UserType == false).FirstOrDefaultAsync(f => f.ID == id);
            if (Student == null)
            {
                //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                ErrorMessage = "ไม่พบข้อมูล";
            }
            else
            {
                //ไปที่หน้าแก้ไขข้อมูล
                return RedirectToPage("/Students/StudentEdit", new { Id = Student.ID });
            }
            return Page();
        }

        public IActionResult OnPostAdd()
        {
            //ไปที่หน้าเพิ่มข้อมูล
            return RedirectToPage("/Students/StudentAdd");
        }

        public async Task<IActionResult> OnPostDelete(int? id)
        {
            if (id == null)
            {
                //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                ErrorMessage = "ไม่พบข้อมูล";
                return Page();
            }

            //เรียกดูข้อมูลด้วย id
            var Student = await _context.UserAccount.Where(w => w.UserType == false).FirstOrDefaultAsync(f => f.ID == id);
            if (Student == null)
            {
                //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                ErrorMessage = "ไม่พบข้อมูล";
            }
            else
            {
                //ลบข้อมูล
                _context.Remove(Student);
                await _context.SaveChangesAsync();

                //ลบข้อมูลคะแนนทั้งหมด
                var Students = await _context.SubjectAnswer.Where(w => w.RefUser == Student.ID).ToListAsync();                
                _context.RemoveRange(Students);
                await _context.SaveChangesAsync();
            }

            //ดึงข้อมูลทั้งหมดจาก DB มาแสดง
            _Student = await _context.UserAccount.Where(w => w.UserType == false).ToListAsync();
            return Page();
        }
    }
}
