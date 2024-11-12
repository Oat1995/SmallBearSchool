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

namespace SmallBearSchool.Pages
{
    public class ManageUserModel : PageModel
    {
        private readonly ILogger<ManageUserModel> _logger;
        private readonly AccessDbContext _context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public UserAccount _Student { get; set; } = new UserAccount();

        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;

        public ManageUserModel(ILogger<ManageUserModel> logger, AccessDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            //เช็คเงื่อนไขการ login
            var isAdminValue = HttpContext.Session.GetString("IsAdmin");
            var usernameValue = HttpContext.Session.GetString("UserName");
            var idValue = HttpContext.Session.GetString("ID");
            if (isAdminValue != null && usernameValue != null && idValue != null)
            {
                IsAdmin = isAdminValue == "True";

                //เรียกดูข้อมูลด้วย id
                var _Students = await _context.UserAccount.FirstOrDefaultAsync(f => f.ID.ToString() == idValue);
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


        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var idValue = HttpContext.Session.GetString("ID");
                //ดึงข้อมูลทั้งหมดจาก DB มาแสดง
                var Students = await _context.UserAccount.ToListAsync();
                var isAdminValue = HttpContext.Session.GetString("IsAdmin");
                if (isAdminValue != null)
                {        
                    //เรียกดูข้อมูลด้วย id
                    var Student = Students.FirstOrDefault(f => f.ID.ToString() == idValue);
                    if (Student == null)
                    {
                        //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                        ErrorMessage = "ไม่พบข้อมูล";
                    }
                    else
                    {
                        //แก้ไขข้อมูล
                        Student.Password = _Student.Password;
                        Student.Address = _Student.Address;
                        Student.IsActive = true;
                        if (isAdminValue.ToLower() == "true")
                            Student.UserType = true;
                        else
                            Student.UserType = false;
                        _context.Update(Student);
                        await _context.SaveChangesAsync();

                        //ไปที่หน้ารายการข้อมูล
                        return RedirectToPage("/Home");
                    }
                }
            }
            return Page();
        }
    }
}
