using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Studentmanagmentsystem.Controllers
{
    public class AccountController : Controller
    {
        // DEMO KE LIYE FAKE DATABASE (List)
        private static int demoTotalStudents = 5;
        private static int demoBcaStudents = 3;
        private static int demoNextId = 101;

        // 1. LOGIN - GET
        public IActionResult Login()
        {
            return View();
        }

        // 2. LOGIN - POST (Bypass for Demo)
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "123")
            {
                return RedirectToAction("Dashboard");
            }
            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

        // 3. DASHBOARD (Demo Data)
        public IActionResult Dashboard()
        {
            ViewBag.TotalStudents = demoTotalStudents;
            ViewBag.BcaStudents = demoBcaStudents;
            return View();
        }

        // 4. CHANGE PASSWORD - GET
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // 5. CHANGE PASSWORD - POST (Fake Success for Demo)
        [HttpPost]
        public IActionResult ChangePassword(string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "New Password and Confirm Password do not match!";
                return View();
            }
            ViewBag.Success = "Password changed successfully! (Demo Mode)";
            return View();
        }

        // 6. ADD STUDENT - GET (Demo ID logic)
        [HttpGet]
        public IActionResult AddStudent()
        {
            ViewBag.NextStudentId = demoNextId;
            return View();
        }

        // 7. ADD STUDENT - POST (Fake Save for Demo)
        [HttpPost]
        public IActionResult AddStudent(int studentId, string studentName, string course)
        {
            // Demo data update kar rahe hain
            demoTotalStudents++;
            if (course != null && course.ToUpper() == "BCA")
            {
                demoBcaStudents++;
            }
            demoNextId++;

            ViewBag.Success = "Student " + studentName + " Added Successfully! (Demo)";
            ViewBag.NextStudentId = demoNextId;
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
