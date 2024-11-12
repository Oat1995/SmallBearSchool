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

namespace SmallBearSchool.Pages
{
    public class ReportModel : PageModel
    {
        private readonly ILogger<ReportModel> _logger;
        private readonly AccessDbContext _context;
        public List<SubjectAnswer> _Score { get; set; } = new List<SubjectAnswer>();
        public List<SubjectAnswerData> _ScoreData { get; set; } = new List<SubjectAnswerData>();
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;

        public ReportModel(ILogger<ReportModel> logger, AccessDbContext context)
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
                //ถ้าไม่ใช่ผู้ดูแลระบบให้กลับไปที่หน้า login
                if (isAdminValue.ToLower() == "false")
                {
                    //ดึงข้อมูลทั้งหมดจาก DB มาแสดง
                    var _ScoreDatas = await (from a in _context.SubjectAnswer
                                             join u in _context.UserAccount on a.RefUser equals u.ID
                                             join s in _context.Subject on a.RefSubject equals s.ID
                                             where u.ID.ToString() == idValue
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
                }
                else
                {
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
                }
                return Page();
            }
            else return RedirectToPage("/Index");
        }
    }
}
