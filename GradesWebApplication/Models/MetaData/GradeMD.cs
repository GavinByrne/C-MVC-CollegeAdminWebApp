using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradesWebApplication.Models
{
    [MetadataType(typeof(GradeMDMetaData))]
    public partial class Grade
    {
    }

    public class GradeMDMetaData
    {
        
        public int ID { get; set; }

        [Required]
        [Display(Name = "Subject ID")]
        public string SubjectID { get; set; }

        [Required]
        [Display(Name = "Grade")]
        [Range(0, 100.00)]
        public Nullable<decimal> Grade1 { get; set; }

        
        [Display(Name = "Date Entered")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateEntered { get; set; }

        [Required]
        [Display(Name = "Student ID")]
        public string StudentID { get; set; }
    }
}