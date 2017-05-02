using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GradesWebApplication.Models;
using GradesWebApplication.ViewModels;
using PagedList;
using System.Data.Entity.Core;
using System.Data.SqlClient;

namespace GradesWebApplication.Controllers
{
    [HandleError(ExceptionType = typeof(MetadataException), View = "MetaError", Order = 4)]
    [HandleError(ExceptionType = typeof(SqlException), View = "SqlError", Order = 3)]
    [HandleError(ExceptionType = typeof(EntityException), View = "EntityError", Order = 2)]
    [HandleError(ExceptionType = typeof(NullReferenceException), View = "Error", Order = 1)]
    [HandleError(ExceptionType = typeof(Exception), View = "Error", Order = 0)]
    public class StudentsController : Controller
    {
        private StudentManagementSystemEntities db = new StudentManagementSystemEntities();

        // GET: Students
        public ActionResult Index(string sortOrder, string option, string searchString, string currentFilter, int? page)
        {
            //return View(db.Students.ToList());

            ViewBag.CurrentSort = sortOrder;
            ViewBag.studentidSortParm = String.IsNullOrEmpty(sortOrder) ? "studentid_desc" : "";
            ViewBag.firstnameSortParm = sortOrder == "firstname" ? "firstname_desc" : "firstname";
            ViewBag.lastnameSortParm = sortOrder == "lastname" ? "lastname_desc" : "lastname";
            ViewBag.dobSortParm = sortOrder == "dob" ? "dob_desc" : "dob";
            ViewBag.addressSortParm = sortOrder == "address" ? "address_desc" : "address";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var details = from d in db.Students select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                if (option == "StudentID")
                {
                    details = details.Where(d => d.StudentID.StartsWith(searchString));
                }
                else if (option == "FirstName")
                {
                    details = details.Where(d => d.FirstName.StartsWith(searchString));
                }
                else
                {
                    details = details.Where(d => d.LastName.StartsWith(searchString));
                }
            }

            switch (sortOrder)
            {
                case "studentid_desc":
                    details = details.OrderByDescending(d => d.StudentID);
                    break;
                case "firstname":
                    details = details.OrderBy(d => d.FirstName);
                    break;
                case "firstname_desc":
                    details = details.OrderByDescending(d => d.FirstName);
                    break;
                case "lastname":
                    details = details.OrderBy(d => d.LastName);
                    break;
                case "lastname_desc":
                    details = details.OrderByDescending(d => d.LastName);
                    break;
                case "dob":
                    details = details.OrderBy(d => d.DateOfBirth);
                    break;
                case "dob_desc":
                    details = details.OrderByDescending(d => d.DateOfBirth);
                    break;
                case "address":
                    details = details.OrderBy(d => d.Address);
                    break;
                case "address_desc":
                    details = details.OrderByDescending(d => d.Address);
                    break;
                default:
                    details = details.OrderBy(d => d.StudentID);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(details.ToPagedList(pageNumber, pageSize));

            ///VIEWMODEL CODE

            //List<StudentViewModel> StudentVMlist = new List<StudentViewModel>();

            //var studentlist = (from s in db.Students
            //                   join g in db.Grades on s.StudentID equals g.StudentID
            //                   select new { s.StudentID, s.FirstName, s.LastName, s.Address, g.Grade1 }).ToList();

            //foreach (var item in studentlist)
            //{
            //    StudentViewModel stvm = new StudentViewModel();

            //    stvm.StudentID = item.StudentID;
            //    stvm.FirstName = item.FirstName;
            //    stvm.LastName = item.LastName;
            //    stvm.Address = item.Address;
            //    stvm.Grade1 = item.Grade1;
            //    StudentVMlist.Add(stvm);
            //}
            //return View(StudentVMlist);



        }

        // GET: Students/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StudentID,FirstName,LastName,DateOfBirth,Address,DateEntered")] Student student)
        {
            student.DateEntered = DateTime.Now;
            student.StudentID = CreateStudentId();

            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StudentID,FirstName,LastName,DateOfBirth,Address,DateEntered")] Student student)
        {
            student.DateEntered = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private string CreateStudentId()
        {
            Random r = new Random();
            int idNumber = r.Next(0, 9999999);

            string wholeId = "ST" + idNumber;

            return wholeId;
        }
    }
}
