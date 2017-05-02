using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradesWebApplication.Models;
using PagedList;
using System.Text.RegularExpressions;
using System.Data.Entity.Core;
using System.Data.SqlClient;

namespace GradesWebApplication.Controllers
{
    [HandleError(ExceptionType = typeof(MetadataException), View = "MetaError", Order = 4)]
    [HandleError(ExceptionType = typeof(SqlException), View = "SqlError", Order = 3)]
    [HandleError(ExceptionType = typeof(EntityException), View = "EntityError", Order = 2)]
    [HandleError(ExceptionType = typeof(NullReferenceException), View = "Error", Order = 1)]
    [HandleError(ExceptionType = typeof(Exception), View = "Error", Order = 0)]
    public class SubjectsController : Controller
    {
        private StudentManagementSystemEntities db = new StudentManagementSystemEntities();

        // GET: Subjects
        public ActionResult Index(string sortOrder, string option, string searchString, string currentFilter, int? page)
        {
            //var subjects = db.Subjects.Include(s => s.Lecturer);
            //var subjects = db.Subjects;
            //return View(subjects.ToList());

            ViewBag.CurrentSort = sortOrder;
            ViewBag.subjectidSortParm = String.IsNullOrEmpty(sortOrder) ? "subject_desc" : "";
            ViewBag.nameSortParm = sortOrder == "name" ? "name_desc" : "name";
            //ViewBag.lecturerSortParm = sortOrder == "lecturer" ? "lecturer_desc" : "lecturer";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var details = from d in db.Subjects select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                if (option == "SubjectID")
                {
                    details = details.Where(d => d.SubjectID.StartsWith(searchString));
                }
                else
                {
                    details = details.Where(d => d.SubjectName.StartsWith(searchString));
                }
            }

            switch (sortOrder)
            {
                case "subject_desc":
                    details = details.OrderByDescending(d => d.SubjectID);
                    break;
                case "name":
                    details = details.OrderBy(d => d.SubjectName);
                    break;
                case "name_desc":
                    details = details.OrderByDescending(d => d.SubjectName);
                    break;
                //case "lecturer":
                //    details = details.OrderBy(d => d.Lecturer.Caption);
                //    break;
                //case "lecturer_desc":
                //    details = details.OrderByDescending(d => d.Lecturer.Caption);
                //    break;
                default:
                    details = details.OrderBy(d => d.SubjectID);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(details.ToPagedList(pageNumber, pageSize));
        }

        // GET: Subjects/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // GET: Subjects/Create
        public ActionResult Create()
        {
            ViewBag.LecturerID = new SelectList(db.Lecturers, "LecturerID", "Name");
            return View();
        }

        // POST: Subjects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SubjectID,SubjectName,LecturerID")] Subject subject)
        {
            string tempid = GetLatestSubjectId();

            subject.SubjectID = NewIds(tempid);

            if (ModelState.IsValid)
            {
                if (db.Subjects.Any(s => s.SubjectName == subject.SubjectName))
                    ViewBag.Error = "Subject Already Exists";
                else
                {
                    db.Subjects.Add(subject);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            ViewBag.LecturerID = new SelectList(db.Lecturers, "LecturerID", "Name");
            return View(subject);
        }

        // GET: Subjects/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = db.Subjects.Find(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            ViewBag.LecturerID = new SelectList(db.Lecturers, "LecturerID", "Name", subject.LecturerID);
            return View(subject);
        }

        // POST: Subjects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SubjectID,SubjectName,LecturerID")] Subject subject)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subject).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LecturerID = new SelectList(db.Lecturers, "LecturerID", "Name", subject.LecturerID);
            return View(subject);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GetLatestSubjectId()
        {

            string subjectid = db.Subjects
                       .OrderByDescending(s => s.SubjectID)
                       .Select(s => s.SubjectID)
                       .Take(1).FirstOrDefault();

            return subjectid;
        }

        private string NewIds(string idNumber)
        {
            int integer;
            string _string;
            string newId;

            integer = Int32.Parse(Regex.Match(idNumber, @"\d+").Value) + 1;

            _string = Regex.Replace(idNumber, @"[^A-Z]+", String.Empty);

            if (_string == "L")
            {
                if (integer < 10)
                {
                    newId = _string + "0000" + integer;
                }
                else if (integer < 100)
                {
                    newId = _string + "000" + integer;
                }
                else if (integer < 1000)
                {
                    newId = _string + "00" + integer;
                }
                else if (integer < 10000)
                {
                    newId = _string + "0" + integer;
                }
                else
                {
                    newId = _string + integer;
                }
            }
            else
            {
                if (integer < 10)
                {
                    newId = _string + "000" + integer;
                }
                else if (integer < 100)
                {
                    newId = _string + "00" + integer;
                }
                else if (integer < 1000)
                {
                    newId = _string + "0" + integer;
                }
                else
                {
                    newId = _string + integer;
                }
            }
            return newId;
        }
    }
}
