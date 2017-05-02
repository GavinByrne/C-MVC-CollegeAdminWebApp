using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradesWebApplication.Models;
using System.Data.Entity.Core;
using System.Data.SqlClient;

namespace GradesWebApplication.Controllers
{
    [HandleError(ExceptionType = typeof(MetadataException), View = "MetaError", Order = 4)]
    [HandleError(ExceptionType = typeof(SqlException), View = "SqlError", Order = 3)]
    [HandleError(ExceptionType = typeof(EntityException), View = "EntityError", Order = 2)]
    [HandleError(ExceptionType = typeof(NullReferenceException), View = "Error", Order = 1)]
    [HandleError(ExceptionType = typeof(Exception), View = "Error", Order = 0)]
    public class GradesController : Controller
    {
        private StudentManagementSystemEntities db = new StudentManagementSystemEntities();


        // GET: Grades/Create
        public ActionResult Create(string studentid)
        {
            ViewBag.Sid = studentid;

            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "StudentID", studentid);
            ViewBag.SubjectID = new SelectList(db.Subjects, "SubjectID", "SubjectName");
            //return View();
            return View();
        }

        // POST: Grades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,SubjectID,Grade1,DateEntered,StudentID")] Grade grade)
        {
            ViewBag.Sid = grade.StudentID;
            grade.DateEntered = DateTime.Now;

            if (ModelState.IsValid)
            {
                if (db.Grades.Any(g => g.StudentID.Equals(grade.StudentID) && g.SubjectID.Equals(grade.SubjectID)))
                    ViewBag.Error = "Student already taking this subject";
                else
                {
                    db.Grades.Add(grade);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Students", new { id = grade.StudentID });
                }
            }

            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "StudentID", grade.StudentID);
            ViewBag.SubjectID = new SelectList(db.Subjects, "SubjectID", "SubjectName", grade.SubjectID);
            return View(grade);
            
        }

        // GET: Grades/Edit/5
        public ActionResult Edit(int? id)
        {
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Grade grade = db.Grades.Find(id);
            ViewBag.Sid = grade.StudentID;
            if (grade == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "StudentID", grade.StudentID);
            ViewBag.SubjectID = new SelectList(db.Subjects, "SubjectID", "SubjectName", grade.SubjectID);
            return View(grade);
        }

        // POST: Grades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,SubjectID,Grade1,DateEntered,StudentID")] Grade grade)
        {
            grade.DateEntered = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(grade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Students", new { id = grade.StudentID });
            }
            ViewBag.StudentID = new SelectList(db.Students, "StudentID", "StudentID", grade.StudentID);
            ViewBag.SubjectID = new SelectList(db.Subjects, "SubjectID", "SubjectName", grade.SubjectID);
            return View(grade);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
