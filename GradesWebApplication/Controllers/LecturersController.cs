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
    public class LecturersController : Controller
    {

        private StudentManagementSystemEntities db = new StudentManagementSystemEntities();

        // GET: Lecturers
        public ActionResult Index(string sortOrder, string option, string searchString, string currentFilter, int? page)
        {
            //return View(db.Lecturers.ToList());

            ViewBag.CurrentSort = sortOrder;
            ViewBag.lectureridSortParm = String.IsNullOrEmpty(sortOrder) ? "lecturerid_desc" : "";
            ViewBag.firstnameSortParm = sortOrder == "firstname" ? "firstname_desc" : "firstname";
            ViewBag.lastnameSortParm = sortOrder == "lastname" ? "lastname_desc" : "lastname";
            ViewBag.emailSortParm = sortOrder == "email" ? "email_desc" : "email";
            ViewBag.phoneSortParm = sortOrder == "phone" ? "phone_desc" : "phone";
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

            var details = from d in db.Lecturers select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                if (option == "LecturerID")
                {
                    details = details.Where(d => d.LecturerID.StartsWith(searchString));
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
                case "lecturerid_desc":
                    details = details.OrderByDescending(d => d.LecturerID);
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
                case "email":
                    details = details.OrderBy(d => d.Email);
                    break;
                case "email_desc":
                    details = details.OrderByDescending(d => d.Email);
                    break;
                case "phone":
                    details = details.OrderBy(d => d.Phone);
                    break;
                case "phone_desc":
                    details = details.OrderByDescending(d => d.Phone);
                    break;
                case "address":
                    details = details.OrderBy(d => d.Address);
                    break;
                case "address_desc":
                    details = details.OrderByDescending(d => d.Address);
                    break;
                default:
                    details = details.OrderBy(d => d.LecturerID);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(details.ToPagedList(pageNumber, pageSize));
        }

        // GET: Lecturers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecturer lecturer = db.Lecturers.Find(id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            return View(lecturer);
        }

        // GET: Lecturers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Lecturers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LecturerID,FirstName,LastName,Email,Phone,Address,DateEntered")] Lecturer lecturer)
        {
            string tempid = GetLatestLecturerId();

            lecturer.LecturerID = NewIds(tempid);
            lecturer.DateEntered = DateTime.Now;

            if (ModelState.IsValid)
            {
                db.Lecturers.Add(lecturer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(lecturer);
        }

        // GET: Lecturers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Lecturer lecturer = db.Lecturers.Find(id);
            if (lecturer == null)
            {
                return HttpNotFound();
            }
            return View(lecturer);
        }

        // POST: Lecturers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LecturerID,FirstName,LastName,Email,Phone,Address,DateEntered")] Lecturer lecturer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(lecturer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(lecturer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public string GetLatestLecturerId()
        {
            string lecturerid = db.Lecturers
                                .OrderByDescending(l => l.LecturerID)
                                .Select(l => l.LecturerID)
                                .Take(1).FirstOrDefault();

            return lecturerid;
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
