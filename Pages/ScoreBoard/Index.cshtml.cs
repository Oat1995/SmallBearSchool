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
using Microsoft.Extensions.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SmallBearSchool.Pages.ScoreBoard
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly AccessDbContext _context;
        public List<SubjectAnswer> _Score { get; set; } = new List<SubjectAnswer>();
        public List<SubjectAnswerData> _ScoreData { get; set; } = new List<SubjectAnswerData>();
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
                var _ScoreDatas = await (from a in _context.SubjectAnswer
                                         join u in _context.UserAccount on a.RefUser equals u.ID
                                         join s in _context.Subject on a.RefSubject equals s.ID
                                         select new { ID = a.ID, Score = a.Score, UserName = u.UserName, Address = u.Address, SubjectName = s.Name }
                ).ToListAsync();

                List<SubjectAnswerData> subjectAnswerDataList = _ScoreDatas
    .Select(x => new SubjectAnswerData
    {
        ID = x.ID,
        Score = x.Score,
        UserName = x.UserName,
        Address = x.Address,
        SubjectName = x.SubjectName
    })
    .ToList();
                _ScoreData = subjectAnswerDataList;
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
            var Score = await _context.SubjectAnswer.FirstOrDefaultAsync(f => f.ID == id);
            if (Score == null)
            {
                //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                ErrorMessage = "ไม่พบข้อมูล";
            }
            else
            {
                //ไปที่หน้าแก้ไขข้อมูล
                return RedirectToPage("/ScoreBoard/ScoreEdit", new { Id = Score.ID });
            }
            return Page();
        }

        public IActionResult OnPostAdd()
        {
            //ไปที่หน้าเพิ่มข้อมูล
            return RedirectToPage("/ScoreBoard/ScoreAdd");
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
            var Score = await _context.SubjectAnswer.FirstOrDefaultAsync(f => f.ID == id);
            if (Score == null)
            {
                //ถ้า id ไม่มีข้อมูลให้ขึ้นหน้า 404
                ErrorMessage = "ไม่พบข้อมูล";
            }
            else
            {
                //ลบข้อมูล
                _context.Remove(Score);
                await _context.SaveChangesAsync();
            }

            //ดึงข้อมูลทั้งหมดจาก DB มาแสดง
            var _ScoreDatas = await (from a in _context.SubjectAnswer
                                     join u in _context.UserAccount on a.RefUser equals u.ID
                                     join s in _context.Subject on a.RefSubject equals s.ID
                                     select new { ID = a.ID, Score = a.Score, UserName = u.UserName, Address = u.Address, SubjectName = s.Name }
                ).ToListAsync();

            List<SubjectAnswerData> subjectAnswerDataList = _ScoreDatas
.Select(x => new SubjectAnswerData
{
    ID = x.ID,
    Score = x.Score,
    UserName = x.UserName,
    Address = x.Address,
    SubjectName = x.SubjectName
})
.ToList();
            _ScoreData = subjectAnswerDataList;
            return Page();
        }
    }
}
