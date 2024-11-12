using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SmallBearSchool.Models
{
    public class SubjectAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int Score { get; set; } = 0;
        public int RefSubject { get; set; } = 0;
        public int RefUser { get; set; } = 0;
    }
    public class SubjectAnswerData
    {
        public int ID { get; set; }
        public int Score { get; set; } = 0;
        public string SubjectName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }

}
