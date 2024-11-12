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

namespace SmallBearSchool.Pages.ScoreBoard
{
    public class ScoreEditModel : PageModel
    {
        private readonly ILogger<ScoreEditModel> _logger;
        private readonly AccessDbContext _context;

        [BindProperty]
        [Required(ErrorMessage = "กรุณากรอกข้อมูลให้ครบถ้วน")]
        public SubjectAnswer _Score { get; set; } = new SubjectAnswer();
        public string _UserAccount { get; set; } = string.Empty;
        public string _Subject { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; } = 0;

        public string ErrorMessage { get; set; } = string.Empty;

        public ScoreEditModel(ILogger<ScoreEditModel> logger, AccessDbContext context)
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
                var _Scores = await _context.SubjectAnswer.FirstOrDefaultAsync(f => f.ID == Id);
                if (_Scores == null)
                {
                    //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                    ErrorMessage = "ไม่พบข้อมูล";
                }
                else
                {
                    _Score = _Scores;
                    var _UserAccounts = await _context.UserAccount.Where(w => w.ID == _Scores.RefUser).Select(s => s.UserName).FirstOrDefaultAsync();
                    _UserAccount = _UserAccounts == null ? string.Empty : _UserAccounts;
                    var _Subjects = await _context.Subject.Where(w => w.ID == _Scores.RefSubject).Select(s => s.Name).FirstOrDefaultAsync();
                    _Subject = _Subjects == null ? string.Empty : _Subjects;
                }
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
                var Scores = await _context.SubjectAnswer.ToListAsync();
                //เช็คการกรอกชื่อซ้ำ
                if (Scores.Where(w => w.RefUser == _Score.RefUser && w.RefSubject == _Score.RefSubject && w.ID != id).Any())
                {
                    ErrorMessage = "ชื่อนักเรียน มีข้อมูลการผูกรายวิชานี้อยู่แล้วอยู่แล้วในระบบ";
                }
                else
                {
                    //เรียกดูข้อมูลด้วย id
                    var Score = Scores.FirstOrDefault(f => f.ID == id);
                    if (Score == null)
                    {
                        //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                        ErrorMessage = "ไม่พบข้อมูล";
                    }
                    else
                    {
                        //แก้ไขข้อมูล
                        Score.Score = _Score.Score;
                        _context.Update(Score);
                        await _context.SaveChangesAsync();

                        //ไปที่หน้ารายการข้อมูล
                        return RedirectToPage("/ScoreBoard/Index");
                    }
                }
            }
            return Page();
        }
    }
}
