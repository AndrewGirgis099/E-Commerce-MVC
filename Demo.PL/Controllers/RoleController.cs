using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Demo.PL.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public RoleController(RoleManager<IdentityRole> roleManager , IMapper mapper)
        {
            _roleManager = roleManager;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
                var Roles = await _roleManager.Roles.Select(U => new RoleViewModel
                {
                    Id = U.Id,
                    RoleName = U.Name,
                  
                }).ToListAsync();
                return View(Roles);
            }
            else
            {
                var Role = await _roleManager.FindByNameAsync(Name);
                if (Role is not null)
                {
                    var mappedRole = new RoleViewModel
                    {
                        Id= Role.Id,
                        RoleName = Role.Name,
                    };
                    return View(new list<RoleViewModel>() { });
                }
            }
            return View(Enumerable.Empty<RoleViewModel>());

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel RoleVm)
        {
            if(ModelState.IsValid)
            {
                var mappedRole = _mapper.Map<RoleViewModel, IdentityRole>(RoleVm);
                await _roleManager.CreateAsync(mappedRole);
                return RedirectToAction(nameof(Index));
            }
            return View(RoleVm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id, string ViewName = "Details")
        {
            if (id is null)
            {
                return BadRequest();
            }
            var Role = await _roleManager.FindByIdAsync(id);
            if (Role is null)
            {
                return NotFound();
            }

            var mappedRole = _mapper.Map<IdentityRole, RoleViewModel>(Role);

            return View(ViewName, mappedRole);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id, "Edit");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromRoute] string id, RoleViewModel RoleVm)
        {
            if (id != RoleVm.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var Role = await _roleManager.FindByIdAsync(id);
                    if (Role != null)
                    {

                        Role.Name = RoleVm.RoleName;
                        await _roleManager.UpdateAsync(Role);
                        return RedirectToAction(nameof(Index));


                    }
                }
                catch (Exception Ex)
                {
                    ModelState.AddModelError(string.Empty, Ex.Message);
                }
            }
            return View(RoleVm);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(string id)
        {

            try
            {
                var Role = await _roleManager.FindByIdAsync(id);
                await _roleManager.DeleteAsync(Role);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, Ex.Message);
                return View("Error", "Home");
            }

        }


    }
}
    
