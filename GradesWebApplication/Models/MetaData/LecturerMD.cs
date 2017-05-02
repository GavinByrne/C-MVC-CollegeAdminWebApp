using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GradesWebApplication.Models
{
    [MetadataType(typeof(LecturerMDMetaData))]
    public partial class Lecturer
    {
        public string Name { get { return FirstName + " " + LastName; } }
    }

    public class LecturerMDMetaData
    {
        
        [Display(Name = "Lecturer ID")]
        public string LecturerID { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        
        [Display(Name = "Date Entered")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DateEntered { get; set; }

        
    }
}