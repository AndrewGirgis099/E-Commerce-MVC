using System.ComponentModel.DataAnnotations;

namespace Demo.PL.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "New Password Is Required")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [Compare(nameof(NewPassword), ErrorMessage = "Confirm Password Doesn't Match Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

    }
}
