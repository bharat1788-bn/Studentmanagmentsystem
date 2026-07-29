using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Studentmanagmentsystem.Models;
using Oracle.ManagedDataAccess.Client;
using Studentmanagmentsystem.Data;

namespace Studentmanagmentsystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly OracleDb db = new OracleDb();

        // 1. INDEX (View Students) - Sirf BCA ke liye set kiya gaya hai
        public IActionResult Index()
        {
            List<Student> bcaStudents = new List<Student>();

            using (OracleConnection con = db.GetConnection())
            {
                con.Open();

                string query = @"SELECT STUDENT_ID, STUDENT_NAME, ENROLLMENT_NO,
                         EMAIL, MOBILE_NO, DATE_OF_BIRTH, GENDER,
                         COURSE, ADDRESS, ADMISSION_DATE
                         FROM STUDENT";

                OracleCommand cmd = new OracleCommand(query, con);
                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Student student = new Student();

                    student.StudentId = Convert.ToInt32(dr["STUDENT_ID"]);
                    student.StudentName = dr["STUDENT_NAME"].ToString();
                    student.EnrollmentNumber = dr["ENROLLMENT_NO"].ToString();
                    student.Email = dr["EMAIL"].ToString();
                    student.MobileNumber = dr["MOBILE_NO"].ToString();
                    student.DateOfBirth = Convert.ToDateTime(dr["DATE_OF_BIRTH"]);
                    student.Gender = dr["GENDER"].ToString();
                    student.Course = dr["COURSE"].ToString();
                    student.Address = dr["ADDRESS"].ToString();
                    student.AdmissionDate = Convert.ToDateTime(dr["ADMISSION_DATE"]);

                    // Ab sirf BCA students hi list mein jayenge
                    if (student.Course != null && student.Course.ToUpper() == "BCA")
                    {
                        bcaStudents.Add(student);
                    }
                }
            }

            ViewBag.BcaStudents = bcaStudents;
            return View();
        }

        // 2. ADD STUDENT (GET - Yahan Auto ID Generate Hogi)
        [HttpGet]
        public IActionResult Add()
        {
            int nextId = 1;

            try
            {
                using (OracleConnection con = db.GetConnection())
                {
                    con.Open();
                    // Database se sabse bada ID nikal kar 1 jodenge
                    string query = "SELECT NVL(MAX(STUDENT_ID), 0) + 1 FROM STUDENT";

                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        nextId = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "ID Generation Error: " + ex.Message;
            }

            // Naya ID pehle se model mein daal kar View ko bhej rahe hain
            Student newStudent = new Student();
            newStudent.StudentId = nextId;

            return View(newStudent);
        }

        // ADD STUDENT (POST)
        [HttpPost]
        public IActionResult Add(Student student)
        {
            if (ModelState.IsValid)
            {
                using (OracleConnection con = db.GetConnection())
                {
                    con.Open();

                    string query = @"INSERT INTO STUDENT
                    (STUDENT_ID, STUDENT_NAME, ENROLLMENT_NO, EMAIL,
                     MOBILE_NO, DATE_OF_BIRTH, GENDER, COURSE,
                     ADDRESS, ADMISSION_DATE)
                    VALUES
                    (:StudentId, :StudentName, :EnrollmentNumber, :Email,
                     :MobileNumber, :DateOfBirth, :Gender, :Course,
                     :Address, :AdmissionDate)";

                    OracleCommand cmd = new OracleCommand(query, con);
                    cmd.BindByName = true;

                    cmd.Parameters.Add(":StudentId", student.StudentId);
                    cmd.Parameters.Add(":StudentName", student.StudentName);
                    cmd.Parameters.Add(":EnrollmentNumber", student.EnrollmentNumber);
                    cmd.Parameters.Add(":Email", student.Email);
                    cmd.Parameters.Add(":MobileNumber", student.MobileNumber);
                    cmd.Parameters.Add(":DateOfBirth", student.DateOfBirth);
                    cmd.Parameters.Add(":Gender", student.Gender);
                    cmd.Parameters.Add(":Course", student.Course);
                    cmd.Parameters.Add(":Address", student.Address);
                    cmd.Parameters.Add(":AdmissionDate", student.AdmissionDate);

                    cmd.ExecuteNonQuery();
                }

                return RedirectToAction("Index");
            }

            return View(student);
        }

        // 3. UPDATE STUDENT (GET & POST)
        [HttpGet]
        public IActionResult Update(int id)
        {
            Student student = new Student();

            using (OracleConnection con = db.GetConnection())
            {
                con.Open();
                string query = "SELECT * FROM STUDENT WHERE STUDENT_ID = :id";
                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add(":id", id);
                OracleDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    student.StudentId = Convert.ToInt32(dr["STUDENT_ID"]);
                    student.StudentName = dr["STUDENT_NAME"].ToString();
                    student.EnrollmentNumber = dr["ENROLLMENT_NO"].ToString();
                    student.Email = dr["EMAIL"].ToString();
                    student.MobileNumber = dr["MOBILE_NO"].ToString();
                    student.DateOfBirth = Convert.ToDateTime(dr["DATE_OF_BIRTH"]);
                    student.Gender = dr["GENDER"].ToString();
                    student.Course = dr["COURSE"].ToString();
                    student.Address = dr["ADDRESS"].ToString();
                    student.AdmissionDate = Convert.ToDateTime(dr["ADMISSION_DATE"]);
                }
            }
            return View(student);
        }

        [HttpPost]
        public IActionResult Update(Student student)
        {
            using (OracleConnection con = db.GetConnection())
            {
                con.Open();
                string query = @"UPDATE STUDENT SET
                        STUDENT_NAME = :StudentName,
                        ENROLLMENT_NO = :EnrollmentNumber,
                        EMAIL = :Email,
                        MOBILE_NO = :MobileNumber,
                        DATE_OF_BIRTH = :DateOfBirth,
                        GENDER = :Gender,
                        COURSE = :Course,
                        ADDRESS = :Address,
                        ADMISSION_DATE = :AdmissionDate
                        WHERE STUDENT_ID = :StudentId";

                OracleCommand cmd = new OracleCommand(query, con);
                cmd.BindByName = true;

                cmd.Parameters.Add(":StudentName", student.StudentName);
                cmd.Parameters.Add(":EnrollmentNumber", student.EnrollmentNumber);
                cmd.Parameters.Add(":Email", student.Email);
                cmd.Parameters.Add(":MobileNumber", student.MobileNumber);
                cmd.Parameters.Add(":DateOfBirth", student.DateOfBirth);
                cmd.Parameters.Add(":Gender", student.Gender);
                cmd.Parameters.Add(":Course", student.Course);
                cmd.Parameters.Add(":Address", student.Address);
                cmd.Parameters.Add(":AdmissionDate", student.AdmissionDate);
                cmd.Parameters.Add(":StudentId", student.StudentId);

                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 4. DELETE STUDENT
        [HttpGet]
        public IActionResult Delete(int id)
        {
            using (OracleConnection con = db.GetConnection())
            {
                con.Open();
                string query = "DELETE FROM STUDENT WHERE STUDENT_ID = :id";
                OracleCommand cmd = new OracleCommand(query, con);
                cmd.Parameters.Add(":id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        // 5. SEARCH STUDENT
        public IActionResult Search(string query, string course)
        {
            List<Student> students = new List<Student>();

            if (string.IsNullOrEmpty(query))
            {
                return View(students);
            }

            using (OracleConnection con = db.GetConnection())
            {
                con.Open();
                string sql = @"SELECT STUDENT_ID, STUDENT_NAME, ENROLLMENT_NO, EMAIL, COURSE
                               FROM STUDENT 
                               WHERE 1=1";

                if (!string.IsNullOrEmpty(query))
                {
                    sql += " AND LOWER(STUDENT_NAME) = LOWER(:search)";
                }

                if (!string.IsNullOrEmpty(course) && course != "All")
                {
                    sql += " AND COURSE = :course";
                }

                OracleCommand cmd = new OracleCommand(sql, con);
                cmd.BindByName = true;

                if (!string.IsNullOrEmpty(query))
                {
                    cmd.Parameters.Add(":search", query.Trim());
                }

                if (!string.IsNullOrEmpty(course) && course != "All")
                {
                    cmd.Parameters.Add(":course", course);
                }

                OracleDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Student student = new Student();
                    student.StudentId = Convert.ToInt32(dr["STUDENT_ID"]);
                    student.StudentName = dr["STUDENT_NAME"].ToString();
                    student.EnrollmentNumber = dr["ENROLLMENT_NO"].ToString();
                    student.Email = dr["EMAIL"].ToString();
                    student.Course = dr["COURSE"].ToString();
                    students.Add(student);
                }
            }

            ViewBag.SearchQuery = query;
            ViewBag.SearchCourse = course;
            return View(students);
        }

        // 6. REPORT (MCA Hataya Gaya Hai)
        public IActionResult Report()
        {
            int totalStudents = 0;
            int bcaCount = 0;
            List<Student> reportList = new List<Student>();

            using (OracleConnection con = db.GetConnection())
            {
                con.Open();
                string query = "SELECT STUDENT_ID, STUDENT_NAME, ENROLLMENT_NO, COURSE FROM STUDENT";

                using (OracleCommand cmd = new OracleCommand(query, con))
                {
                    using (OracleDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Student s = new Student();
                            s.StudentId = Convert.ToInt32(dr["STUDENT_ID"]);
                            s.StudentName = dr["STUDENT_NAME"].ToString();
                            s.EnrollmentNumber = dr["ENROLLMENT_NO"].ToString();
                            s.Course = dr["COURSE"].ToString();

                            reportList.Add(s);

                            totalStudents++;

                            if (s.Course != null && s.Course.ToUpper() == "BCA")
                            {
                                bcaCount++;
                            }
                        }
                    }
                }
            }

            ViewBag.TotalStudents = totalStudents;
            ViewBag.BcaCount = bcaCount;

            return View(reportList);
        }
    }
}