using Demo.DAL.Models;
using System.ComponentModel.DataAnnotations;
using System;
using Microsoft.AspNetCore.Http;

namespace Demo.PL.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name Is Required")]
        [MaxLength(50, ErrorMessage = "Maxlength for name is 50")]
        [MinLength(4, ErrorMessage = "Minlength for name is 4")]
        public string Name { get; set; }
        [Range(21, 60)]
        public int? Age { get; set; }
        [RegularExpression(@"^[0-9]{1,3}-[a-zA-Z]{5,10}-[a-zA-Z]{4,10}-[a-zA-Z]{5,10}$", ErrorMessage = "Address must be like = 123-Street-City-Country ")]
        public string Address { get; set; }
        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Hiring Date")]

        public DateTime HireDate { get; set; }

        public IFormFile Image { get; set; }
        public string ImageName { get; set; }

        //[ForeignKey("Department")]
        //[InverseProperty(nameof(Models.Department.Employees))]
        public int? DepartmentId { get; set; }
        public Department Department { get; set; } // Navigation property of [One]
    }
}
