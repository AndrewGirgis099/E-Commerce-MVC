using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(UserManager<ApplicationUser> userManager , SignInManager<ApplicationUser> signInManager)
        {
			_userManager = userManager;
			_signInManager = signInManager;
		}
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> SignUp(SignUpViewModel model)
		{
            if(ModelState.IsValid)  // server side validation
            {
                var User = new ApplicationUser()
                {
                    UserName =  model.Email.Split("@")[0],
                    Email = model.Email,
                    IsAgree = model.IsAgree,
                    FName = model.FName,
                    LName = model.LName,
                };

                var result = await _userManager.CreateAsync(User , model.Password);
                if(result.Succeeded)
                {
                    return RedirectToAction(nameof(SignIn));
                }
                foreach(var Error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, Error.Description);
                }

            }
			return View(model);
		}

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user is not null) 
                {
                    bool flag = await _userManager.CheckPasswordAsync(user, model.Password);
                    if(flag)
                    {
                        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                        if (result.Succeeded)
                        {
                            return RedirectToAction(nameof(HomeController.Index) , "Home");
                        }
                    }
                
                }
                ModelState.AddModelError(string.Empty, "Invalid Login");
            }
            return View(model);
        }


        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user is not null)
                {
                    var token =await _userManager.GeneratePasswordResetTokenAsync(user);
                    var ResetPasswordUrl = Url.Action("ResetPassword" , "Account" , new {email = model.Email , token = token });
                    var email = new Email()
                    {
                        Subject = "Reset Your Password",
                        Recipients = model.Email,
                        Body = ResetPasswordUrl
                    };
                    EmailSetting.SendEmail(email);
                    return RedirectToAction(nameof(CheckYourInbox));
                }
                ModelState.AddModelError(string.Empty, "Inavlid Email");
            }
            return View(model);
        }

        public IActionResult CheckYourInbox()
        {

            return View();
        }

        public IActionResult ResetPassword(string email , string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }

        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = TempData["email"] as string;
                string token = TempData["token"] as string;

                var user = await _userManager.FindByEmailAsync(email);

                var result = await _userManager.ResetPasswordAsync(user, token , model.NewPassword);
                if(result.Succeeded)
                {
                    RedirectToAction(nameof(SignIn));
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

    }
}
