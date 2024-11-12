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
    public class StudentEditModel : PageModel
    {
        private readonly ILogger<StudentEditModel> _logger;
        private readonly AccessDbContext _context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public UserAccount _Student { get; set; } = new UserAccount();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } = 0;

        public string ErrorMessage { get; set; } = string.Empty;

        public StudentEditModel(ILogger<StudentEditModel> logger, AccessDbContext context)
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

                //เรียกดูข้อมูลด้วย id
                var _Students = await _context.UserAccount.FirstOrDefaultAsync(f => f.ID == Id);
                if (_Students == null)
                {
                    //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                    ErrorMessage = "ไม่พบข้อมูล";
                }
                else _Student = _Students;
                return Page();
            }
            else return RedirectToPage("/Index");
        }


        public async Task<IActionResult> OnPost(int? id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                    ErrorMessage = "ไม่พบข้อมูล";
                    return Page();
                }
                //ดึงข้อมูลทั้งหมดจาก DB มาแสดง
                var Students = await _context.UserAccount.ToListAsync();
                //เช็คการกรอกชื่อซ้ำ
                if (Students.Where(w => (w.UserName == _Student.UserName || w.UserId == _Student.UserId) && w.ID != id).Any())
                {
                    ErrorMessage = "ชื่อนักเรียน หรือชื่อผู้ใช้งานมีข้อมูลอยู่แล้วในระบบ";
                }
                else
                {
                    //เรียกดูข้อมูลด้วย id
                    var Student = Students.FirstOrDefault(f => f.ID == id);
                    if (Student == null)
                    {
                        //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                        ErrorMessage = "ไม่พบข้อมูล";
                    }
                    else
                    {
                        //แก้ไขข้อมูล
                        Student.UserName = _Student.UserName;
                        Student.UserId = _Student.UserId;
                        Student.Password = _Student.Password;
                        Student.Address = _Student.Address;
                        Student.IsActive = _Student.IsActive;
                        Student.UserType = false;
                        _context.Update(Student);
                        await _context.SaveChangesAsync();
                        //ไปที่หน้ารายการข้อมูล
                        return RedirectToPage("/Students/Index");
                    }
                }
            }
            return Page();
        }
    }
}
