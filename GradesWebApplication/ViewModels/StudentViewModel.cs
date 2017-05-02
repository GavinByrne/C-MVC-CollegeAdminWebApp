using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GradesWebApplication.ViewModels
{
    public class StudentViewModel
    {
        public List<string> ListofSubjects { get; set; }

        //Student
        public string StudentID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> DateEntered { get; set; }

        //Grade
        public string SubjectID { get; set; }
        public Nullable<decimal> Grade1 { get; set; }

    }
}