using System;
using System.ComponentModel.DataAnnotations;

namespace Studentmanagmentsystem.Models
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string EnrollmentNumber { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string MobileNumber { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Course { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime AdmissionDate { get; set; }
    }
}