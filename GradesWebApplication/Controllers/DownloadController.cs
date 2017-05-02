using GradesWebApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GradesWebApplication.Controllers
{
    [HandleError(ExceptionType = typeof(MetadataException), View = "MetaError", Order = 4)]
    [HandleError(ExceptionType = typeof(SqlException), View = "SqlError", Order = 3)]
    [HandleError(ExceptionType = typeof(EntityException), View = "EntityError", Order = 2)]
    [HandleError(ExceptionType = typeof(NullReferenceException), View = "Error", Order = 1)]
    [HandleError(ExceptionType = typeof(Exception), View = "Error", Order = 0)]
    public class DownloadController : Controller
    {
        Reporting _report = new Reporting();

        // GET: Download
        public ActionResult Index()
        {
            return View();
        }

        public FileContentResult StudentCSV(string studentid)
        {
            string csv = _report.StudentReport(studentid);

            if (String.IsNullOrEmpty(csv))
            {
                ViewBag.Error = _report.ErrMessage;
                RedirectToRoute("ReportError");
            }
            
            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", studentid + " " + "Student Report.csv");            
        }

        public FileContentResult SubjectCSV(string subjectid)
        {
            string csv = _report.SubjectReport(subjectid);

            if (String.IsNullOrEmpty(csv))
            {
                ViewBag.Error = _report.ErrMessage;
                RedirectToRoute("ReportError");
            }

            return File(new System.Text.UTF8Encoding().GetBytes(csv), "text/csv", subjectid + " " + "Subject Report.csv");
        }
    }
}