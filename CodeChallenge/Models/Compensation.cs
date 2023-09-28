using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeChallenge.Models
{
    public class Compensation
    {
        [Key]
        public String Employee { get; set; }
        public int Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}
