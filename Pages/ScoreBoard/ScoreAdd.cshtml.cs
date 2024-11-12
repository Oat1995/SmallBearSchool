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
using static System.Formats.Asn1.AsnWriter;

namespace SmallBearSchool.Pages.ScoreBoard
{
    public class ScoreAddModel : PageModel
    {
        private readonly ILogger<ScoreAddModel> _logger;
        private readonly AccessDbContext _context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public SubjectAnswer _Score { get; set; } = new SubjectAnswer();
        public List<UserAccount> _UserAccount { get; set; } = new List<UserAccount>();
        public List<Subject> _Subject { get; set; } = new List<Subject>();

        public string ErrorMessage { get; set; } = string.Empty;

        public ScoreAddModel(ILogger<ScoreAddModel> logger, AccessDbContext context)
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

                _UserAccount = await _context.UserAccount.Where(w => w.IsActive && w.UserType == false).ToListAsync();
                _Subject = await _context.Subject.Where(w => w.IsActive).ToListAsync();
                return Page();
            }
            else return RedirectToPage("/Index");
        }


        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                //ดึงข้อมูลทั้งหมดจาก DB มาแสดง
                var SubjectAnswer = await _context.SubjectAnswer.ToListAsync();
                //เช็คการกรอกชื่อซ้ำ
                if (SubjectAnswer.Where(w => w.RefSubject == _Score.RefSubject && w.RefUser == _Score.RefUser).Any())
                {
                    ErrorMessage = "ชื่อนักเรียน มีข้อมูลการผูกรายวิชานี้อยู่แล้วอยู่แล้วในระบบ";
                    _UserAccount = await _context.UserAccount.Where(w => w.IsActive && w.UserType == false).ToListAsync();
                    _Subject = await _context.Subject.Where(w => w.IsActive).ToListAsync();
                }
                else
                {
                    _context.Add(_Score);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/ScoreBoard/Index");
                }
            }
            return Page();
        }
    }
}
