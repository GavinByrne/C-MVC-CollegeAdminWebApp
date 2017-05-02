using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradesWebApplication.Models
{
    [MetadataType(typeof(SubjectMDMetaData))]
    public partial class Subject
    {
    }

    public class SubjectMDMetaData
    {
        
        [Display(Name = "Subject ID")]
        public string SubjectID { get; set; }

        [Required]
        [Display(Name = "Subject Name")]
        public string SubjectName { get; set; }

        [Required]
        [Display(Name = "Lecturer ID")]
        public string LecturerID { get; set; }

    }
}