using Demo.BLL.Interfacies;
using Demo.BLL.Repositories;
using Demo.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // private readonly IDepartmentRepository departmentRepository;

        //1- inheritance : DepartmentController is a Controller
        //2- Association (Composition (Required)) : DepartmentController has a DepartmentReposatory
        public DepartmentController(IUnitOfWork unitOfWork /*IDepartmentRepository departmentRepository*/) // Ask CLR for creating object form class implementing IDepartmentRepsitory
        {
            _unitOfWork = unitOfWork;
            // this.departmentRepository = departmentRepository;
        }
        public IActionResult Index()
        {
            var department = _unitOfWork.DepartmentRepository.GetAll();
            return View(department);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Department department)
        {
            if(ModelState.IsValid) // server side validation
            {
                _unitOfWork.DepartmentRepository.Add(department);
                _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                

            }
            return View(department);
        }

        // Department/Details/10
        // Department/Details
        public IActionResult Details(int? id , string viewName="Details")
        {
            if(!id.HasValue)
            {
                return BadRequest();
            }
            var department = _unitOfWork.DepartmentRepository.Get(id.Value);
            if(department == null)
            {
                return NotFound();
            }
            return View(viewName,department);
        }

        //Department/Edit/10
        //Department/Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if(!id.HasValue)
            {
                return BadRequest(); //400
            }
            var department = _unitOfWork.DepartmentRepository.Get(id.Value);
            if(department==null)
            {
                return NotFound();
            }
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id ,Department department)
        {
            if (id!=department.Id)
            {
                return BadRequest();
            }
            if(ModelState.IsValid)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        _unitOfWork.DepartmentRepository.Update(department);
                        _unitOfWork.Complete();
                       
                        return RedirectToAction(nameof(Index));
                        
                    }
                }
                catch (Exception Ex)
                {
                    ModelState.AddModelError(string.Empty, Ex.Message);
                }

            }
            return View(department);
        }




        public IActionResult Delete(int? id)
        {
            return Details(id , "Delete");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete([FromRoute] int id , Department department)
        {
            if (id != department.Id)
            {
                return BadRequest();
            }
            try
            {
                _unitOfWork.DepartmentRepository.Delete(department);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            catch(Exception Ex)
            {
                ModelState.AddModelError(string.Empty, Ex.Message);
            }
            return View(department);
        }
    }
}
