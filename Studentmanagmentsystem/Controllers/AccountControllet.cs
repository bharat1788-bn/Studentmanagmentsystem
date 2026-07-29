using System;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using Studentmanagmentsystem.Data;

namespace Studentmanagmentsystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly OracleDb db = new OracleDb();

        // 1. LOGIN - GET
        public IActionResult Login()
        {
            return View();
        }

        // 2. LOGIN - POST 
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            try
            {
                using (OracleConnection con = db.GetConnection())
                {
                    con.Open();
                    string query = @"SELECT COUNT(*) FROM ADMIN 
                                     WHERE LOWER(TRIM(USERNAME)) = LOWER(TRIM(:p_user)) 
                                     AND TRIM(PASSWORD) = TRIM(:p_pass)";

                    OracleCommand cmd = new OracleCommand(query, con);
                    cmd.BindByName = true;
                    cmd.Parameters.Add("p_user", username);
                    cmd.Parameters.Add("p_pass", password);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    if (count > 0)
                    {
                        return RedirectToAction("Dashboard");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database Error: " + ex.Message;
                return View();
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

        // 3. DASHBOARD (MCA Hataya Gaya Hai)
        public IActionResult Dashboard()
        {
            int totalStudents = 0;
            int bcaStudents = 0;

            try
            {
                using (OracleConnection con = db.GetConnection())
                {
                    con.Open();
                    using (OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM STUDENT", con))
                    {
                        totalStudents = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    using (OracleCommand cmd = new OracleCommand("SELECT COUNT(*) FROM STUDENT WHERE COURSE = 'BCA'", con))
                    {
                        bcaStudents = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            ViewBag.TotalStudents = totalStudents;
            ViewBag.BcaStudents = bcaStudents;

            return View();
        }

        // 4. CHANGE PASSWORD - GET
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // 5. CHANGE PASSWORD - POST
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New Password and Confirm Password do not match!";
                return View();
            }

            try
            {
                using (OracleConnection con = db.GetConnection())
                {
                    con.Open();
                    string checkQuery = @"SELECT COUNT(*) FROM ADMIN 
                                          WHERE LOWER(TRIM(USERNAME)) = 'admin' 
                                          AND TRIM(PASSWORD) = TRIM(:p_oldPass)";

                    OracleCommand checkCmd = new OracleCommand(checkQuery, con);
                    checkCmd.BindByName = true;
                    checkCmd.Parameters.Add("p_oldPass", oldPassword);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0)
                    {
                        string updateQuery = "UPDATE ADMIN SET PASSWORD = :p_newPass WHERE LOWER(TRIM(USERNAME)) = 'admin'";
                        OracleCommand updateCmd = new OracleCommand(updateQuery, con);
                        updateCmd.BindByName = true;
                        updateCmd.Parameters.Add("p_newPass", newPassword);
                        updateCmd.ExecuteNonQuery();

                        ViewBag.Success = "Password changed successfully! You can use your new password next time.";
                    }
                    else
                    {
                        ViewBag.Error = "Incorrect Old Password!";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database Error: " + ex.Message;
            }

            return View();
        }

        // 6. ADD STUDENT - GET (Auto Increment ID Logic)
        [HttpGet]
        public IActionResult AddStudent()
        {
            int nextId = 1;

            try
            {
                using (OracleConnection con = db.GetConnection())
                {
                    con.Open();
                    // Database se check karega sabse bada ID aur 1 plus karega
                    string query = "SELECT NVL(MAX(ID), 0) + 1 FROM STUDENT";

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

            // HTML page ko agla ID bhej raha hai
            ViewBag.NextStudentId = nextId;
            return View();
        }

        // 7. ADD STUDENT - POST (Save to Database)
        [HttpPost]
        public IActionResult AddStudent(int studentId, string studentName, string course)
        {
            try
            {
                using (OracleConnection con = db.GetConnection())
                {
                    con.Open();
                    string query = "INSERT INTO STUDENT (ID, NAME, COURSE) VALUES (:p_id, :p_name, :p_course)";

                    OracleCommand cmd = new OracleCommand(query, con);
                    cmd.BindByName = true;
                    cmd.Parameters.Add("p_id", studentId);
                    cmd.Parameters.Add("p_name", studentName);
                    cmd.Parameters.Add("p_course", course);

                    cmd.ExecuteNonQuery();
                    ViewBag.Success = "Student Added Successfully!";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database Error: " + ex.Message;
            }

            // Save hone ke baad agla number form par bhejega
            ViewBag.NextStudentId = studentId + 1;
            return View();
        }

        public IActionResult ViewStudent()
        {
            return View();
        }

        public IActionResult UpdateStudent()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}