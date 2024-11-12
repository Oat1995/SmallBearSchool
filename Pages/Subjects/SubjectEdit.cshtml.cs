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
using System.Linq;

namespace SmallBearSchool.Pages.Subjects
{
    public class SubjectEditModel : PageModel
    {
        private readonly ILogger<SubjectEditModel> _logger;
        private readonly AccessDbContext _context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public Subject _Subject { get; set; } = new Subject();

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } = 0;

        public string ErrorMessage { get; set; } = string.Empty;

        public SubjectEditModel(ILogger<SubjectEditModel> logger, AccessDbContext context)
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
                var _Subjects = await _context.Subject.FirstOrDefaultAsync(f => f.ID == Id);
                if (_Subjects == null)
                {
                    //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                    ErrorMessage = "ไม่พบข้อมูล";
                }
                else _Subject = _Subjects;
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
                var Subjects = await _context.Subject.ToListAsync();
                //เช็คการกรอกชื่อซ้ำ
                if (Subjects.Where(w => w.Name == _Subject.Name && w.ID != id).Any())
                {
                    ErrorMessage = "มีข้อมูลอยู่แล้วในระบบ";
                }
                else
                {
                    //เรียกดูข้อมูลด้วย id
                    var Subject = Subjects.FirstOrDefault(f => f.ID == id);
                    if (Subject == null)
                    {
                        //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                        ErrorMessage = "ไม่พบข้อมูล";
                    }
                    else
                    {
                        //แก้ไขข้อมูล
                        Subject.Name = _Subject.Name;
                        Subject.IsActive = _Subject.IsActive;
                        _context.Update(Subject);
                        await _context.SaveChangesAsync();

                        //ไปที่หน้ารายการข้อมูล
                        return RedirectToPage("/Subjects/Index");
                    }
                }
            }
            return Page();
        }
    }
}
