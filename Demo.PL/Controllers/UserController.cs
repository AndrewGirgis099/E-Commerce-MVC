using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        
        public UserController(UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager , IMapper mapper)
        {
           _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                var users = await _userManager.Users.ToListAsync();
                var userViewModels = new List<UserViewModel>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var userViewModel = new UserViewModel
                    {
                        Id = user.Id,
                        FName = user.FName,
                        LName = user.LName,
                        PhoneNumber = user.PhoneNumber,
                        Roles = roles
                    };
                    userViewModels.Add(userViewModel);
                }

                return View(userViewModels);
            }
            else
            {
             var user =  await _userManager.FindByEmailAsync(email);
                if(user is not null)
                {
                    var mappedUser = new UserViewModel
                    {
                        Id = user.Id,
                        FName = user.FName,
                        LName = user.LName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Roles = _userManager.GetRolesAsync(user).Result
                    };
                    return View(new list<UserViewModel>() {  });
                }
            }
            return View(Enumerable.Empty<UserViewModel>());

        }

        [HttpGet]
        public async Task<IActionResult> Details(string id , string ViewName = "Details")
        {
            if(id is null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(id);
            if(user is null)
            {
                return NotFound();
            }

            var mappedUser = _mapper.Map<ApplicationUser, UserViewModel>(user);

            return View(ViewName , mappedUser);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id , UserViewModel UserVm)
        {
            if(id != UserVm.Id)
            {
                return BadRequest();
            }

            if(ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    if(user != null)
                    {
                        user.FName = UserVm.FName;
                        user.LName = UserVm.LName;
                        user.PhoneNumber = UserVm.PhoneNumber;
                       await _userManager.UpdateAsync(user);
                        return RedirectToAction(nameof(Index));
                        

                    }
                }
                catch(Exception Ex)
                {
                    ModelState.AddModelError(string.Empty, Ex.Message);
                }
            }
            return View(UserVm);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id )
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string id)
        {
           
                try
                {
                    var user = await _userManager.FindByIdAsync(id);
                    await _userManager.DeleteAsync(user);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception Ex)
                {
                    ModelState.AddModelError(string.Empty, Ex.Message);
                    return View("Error" , "Home");
                }
            
        }


    }
}
