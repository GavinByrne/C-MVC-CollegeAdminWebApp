using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace GradesWebApplication.Models
{
    public class Reporting
    {
        StudentManagementSystemEntities db = new StudentManagementSystemEntities();

        int _min;
        int _max;
        double _avg;

        string _minGrade = "This is the lowest grade achieved in this report criteria:";
        string _maxGrade = "This is the highest grade achieved in this report criteria:";
        string _avgGrade = "The average grade achieved in this report criteria is:";

        public string ErrMessage { get; set; }

        public DataTable SubjectDetails(string subjectid)
        {
            DataTable dt = new DataTable();

            try
            {

                var details2 = db.Grades.Join(db.Students, g => g.StudentID, s => s.StudentID, (g, s) => new { g = g, s = s })
                                    .Join(db.Subjects, x => x.g.SubjectID, su => su.SubjectID, (x, su) => new { x = x, su = su })
                                    .Join(db.Lecturers, y => y.su.LecturerID, l => l.LecturerID, (y, l) => new { y = y, l = l })
                                    .Where(z => z.y.su.SubjectID.Equals(subjectid))
                                    .Select(z => new
                                    {
                                        SubjectID = z.y.x.g.SubjectID,
                                        SubjectName = z.y.su.SubjectName,
                                        StudentID = z.y.x.s.StudentID,
                                        FirstName = z.y.x.s.FirstName,
                                        LastName = z.y.x.s.LastName,
                                        Grade = z.y.x.g.Grade1,
                                        LecturerName = ((z.l.FirstName + " ") + z.l.LastName)
                                    });

                dt = LINQtoDataTable(details2);

            }
            catch (SqlException sqlException)
            {
                this.ErrMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrMessage = "Error: " + e.Message;
                return null;
            }
            return dt;
        }


        public DataTable StudentDetails(string studentId)
        {
            DataTable dt = new DataTable();

            try
            {
                var details2 = db.Grades.Join(db.Students, g => g.StudentID, s => s.StudentID, (g, s) => new { g = g, s = s })
                                .Join(db.Subjects, x => x.g.SubjectID, su => su.SubjectID, (x, su) => new { x = x, su = su })
                                .Join(db.Lecturers, y => y.su.LecturerID, l => l.LecturerID, (y, l) => new { y = y, l = l })
                                .Where(z => z.y.x.s.StudentID.Equals(studentId))
                                .Select(z => new
                                {
                                    StudentID = z.y.x.s.StudentID,
                                    FirstName = z.y.x.s.FirstName,
                                    LastName = z.y.x.s.LastName,
                                    DateOfBirth = z.y.x.s.DateOfBirth,
                                    Address = z.y.x.s.Address,
                                    SubjectID = z.y.x.g.SubjectID,
                                    SubjectName = z.y.su.SubjectName,
                                    Grade = z.y.x.g.Grade1,
                                    LecturerName = ((z.l.FirstName + " ") + z.l.LastName),
                                    DateEntered = z.y.x.g.DateEntered
                                });

                dt = LINQtoDataTable(details2);

            }
            catch (SqlException sqlException)
            {
                this.ErrMessage = "SQL Error: " + sqlException.Message;
                return null;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ErrMessage = "SQL Operation Error: " + invalidOperationException.Message;
                return null;
            }
            catch (Exception e)
            {
                this.ErrMessage = "Error: " + e.Message;
                return null;
            }
            return dt;
        }

        /// <summary>
        /// Retrieves a datatable from a sproc and passes to TOCSV method
        /// </summary>
        public string StudentReport(string studentId)
        {
            DataTable dt = StudentDetails(studentId);

            if (dt == null)
            {
                return null;
            }
            else
            {
                DataTable dtCopy = AddGradeRating(dt);
                string tempReport = ToCsv(dtCopy);

                return tempReport;
            }
        }

        /// <summary>
        /// Retrieves a datatable from a sproc and passes to TOCSV method
        /// </summary>
        public string SubjectReport(string subjectName)
        {
            DataTable dt = SubjectDetails(subjectName);

            if (dt == null)
            {
                return null;
            }
            else
            {
                DataTable dtCopy = AddGradeRating(dt);
                string tempReport = ToCsv(dtCopy);

                return tempReport;
            }
        }

        /// <summary>
        /// Adds a column to the data table and fills each row based on the Grade column
        /// </summary>
        public DataTable AddGradeRating(DataTable originalTable)
        {
            originalTable.Columns.Add(new DataColumn("GradeRating", typeof(string)));

            foreach (DataRow row in originalTable.Rows)
            {
                string temp = "";
                string temp2 = row["Grade"].ToString();

                int grade;

                if (int.TryParse(row["Grade"].ToString(), out grade))
                {
                    if (string.IsNullOrWhiteSpace(temp2))
                    {
                        temp = "";
                    }
                    else if (grade < 40)
                    {
                        temp = "Fail";
                    }
                    else if (grade < 55)
                    {
                        temp = "D";
                    }
                    else if (grade < 70)
                    {
                        temp = "C";
                    }
                    else if (grade < 85)
                    {
                        temp = "B";
                    }
                    else if (grade <= 100)
                    {
                        temp = "A";
                    }
                    else
                    {
                        temp = "";
                    }
                    row["GradeRating"] = temp;
                }

            }

            return originalTable;
        }

        /// <summary>
        /// Preps summary string data and sends datatable to stringbuilder
        /// </summary>
        public string ToCsv(DataTable dtcopy)
        {

            string success;

            List<decimal> gradeListDecimals = dtcopy.AsEnumerable().Where(al => al["Grade"] != DBNull.Value).Select(al => al.Field<decimal>("Grade")).ToList();

            List<int> levels = gradeListDecimals.Select(g => Convert.ToInt32(g)).ToList();


            try
            {
                if (levels.Count >= 1)
                {
                    _min = levels.Min();
                    _max = levels.Max();
                    _avg = levels.Average();

                    success = BuildString(dtcopy);
                }
                else
                {
                    success = BuildString(dtcopy);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return success;
        }

        /// <summary>
        /// Sub Method to build string for report, removes the need for duplicate code
        /// </summary>
        private string BuildString(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                                      Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field =>
                  string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }
            sb.AppendLine();
            sb.AppendLine("," + _maxGrade + "," + _max);
            sb.AppendLine("," + _minGrade + "," + _min);
            sb.AppendLine("," + _avgGrade + "," + Math.Round(_avg, 2, MidpointRounding.AwayFromZero));

            return sb.ToString();
        }

        public DataTable LINQtoDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others 
                //will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition()
                        == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }
    }
}