using System.ComponentModel.DataAnnotations;

namespace SaloonWebApi.Models
{
    public class Employee
    {
        [Key]
        public int Employee_Id { get; set; }
        public string Employee_Code { get; set; }
        public string Employee_Name { get; set; }
        public string Employee_LastName { get; set; }
        public string Employee_FatherName { get; set; }
        public string? Employee_Email { get; set; }
        public string Employee_Address { get; set; }
        public string? Employee_ZipCode { get; set; }
        public string Phone_No { get; set; }
        public string Employee_CNIC { get; set; }
        public DateTime CNIC_Exp_Date { get; set; }
        public int Emp_Category { get; set; }
        public int YearsOfExperience { get; set; }
        public int Availability { get; set; }
        public int Vaccination_Id { get; set; }
        public string? Certification { get; set; }
        public string? Skills { get; set; }
        public DateTime Joining_Date { get; set; }
        public decimal Employee_Salary { get; set; }
        public int? Benefits { get; set; }
        public decimal? Benefit_Percent { get; set; }
        public int Active { get; set; }
        public int Insert_User { get; set; }
        public DateTime Insert_Date { get; set; }
        public int? Modify_User { get; set; }
        public DateTime? Modify_Date { get; set; }
        public int Branch_Id { get; set; }
        public int? Employee_Type { get; set; }
        public string? Account_No { get; set; }
        public string? db_EmployeeCode { get; set; }
        public string? Password { get; set; }
    }
}