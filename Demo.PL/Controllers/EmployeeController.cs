using AutoMapper;
using Demo.BLL.Interfacies;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;

namespace Demo.PL.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        //  private readonly IEmployeeRepository EmployeeRepository;
        //  private readonly IDepartmentRepository departmentRepository;

        //1- inheritance : EmployeeController is a Controller
        //2- Association (Composition (Required)) : EmployeeController has a EmployeeReposatory
        public EmployeeController(IMapper Mapper , IUnitOfWork unitOfWork /*, IEmployeeRepository EmployeeRepository*//*, IDepartmentRepository departmentRepository*/) // Ask CLR for creating object form class implementing IEmployeeRepsitory
        {
            _mapper = Mapper;
            _unitOfWork = unitOfWork;
            // this.EmployeeRepository = EmployeeRepository;
            //this.departmentRepository = departmentRepository;
        }
        public IActionResult Index(string searchInp)
        {
            if(string.IsNullOrEmpty(searchInp))
            {
                var Employee = _unitOfWork.EmployeeRepository.GetAll();
                var mappedEmp = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(Employee);
                var image = mappedEmp.Select(E => E.Image).FirstOrDefault();
                return View(mappedEmp);
            }
            else
            {
                var Employee = _unitOfWork.EmployeeRepository.GetEmployeeByName(searchInp.ToLower());
                var mappedEmp = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(Employee);
                return View(mappedEmp);
            }
        }

        public IActionResult Create()
        {
           // ViewBag.Departments= departmentRepository.GetAll();
            return View();
        }
        [HttpPost]
        public IActionResult Create(EmployeeViewModel EmployeeVm)
        {
            if (ModelState.IsValid) // server side validation
            {
                EmployeeVm.ImageName= DocumentSettings.UploadFile(EmployeeVm.Image, "Images");
                var mappedEmp=_mapper.Map<EmployeeViewModel , Employee>(EmployeeVm);
                _unitOfWork.EmployeeRepository.Add(mappedEmp);
                _unitOfWork.Complete();

                return RedirectToAction(nameof(Index));
              

            }
            return View(EmployeeVm);
        }

        // Employee/Details/10
        // Employee/Details
        public IActionResult Details(int? id, string viewName = "Details")
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }
            var Employee = _unitOfWork.EmployeeRepository.Get(id.Value);
            var mappedEmp = _mapper.Map<Employee, EmployeeViewModel>(Employee); 
            if (Employee == null)
            {
                return NotFound();
            }
            return View(viewName, mappedEmp);
        }

        //Employee/Edit/10
        //Employee/Edit
        [HttpGet]
        public IActionResult Edit(int? id)
        {
           // ViewBag.Departments = departmentRepository.GetAll();

            if (!id.HasValue)
            {
                return BadRequest(); //400
            }
            var Employee = _unitOfWork.EmployeeRepository.Get(id.Value);
            var mappedEmp = _mapper.Map<Employee, EmployeeViewModel>(Employee);

            if (Employee == null)
            {
                return NotFound();
            }
            return View(mappedEmp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, EmployeeViewModel EmployeeVm)
        {
            if (id != EmployeeVm.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    EmployeeVm.ImageName = DocumentSettings.UploadFile(EmployeeVm.Image, "Images");
                    var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(EmployeeVm);


                    _unitOfWork.EmployeeRepository.Update(mappedEmp);
                    _unitOfWork.Complete();
                    
                    return RedirectToAction(nameof(Index));
                    
                    
                }
                catch (Exception Ex)
                {
                    ModelState.AddModelError(string.Empty, Ex.Message);
                }

            }
            return View(EmployeeVm);
        }




        public IActionResult Delete(int? id)
        {
            return Details(id, "Delete");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult Delete([FromRoute] int id, EmployeeViewModel EmployeeVm)
        {
            if (id != EmployeeVm.Id)
            {
                return BadRequest();
            }
            try
            {
                var mappedEmp = _mapper.Map<EmployeeViewModel, Employee>(EmployeeVm);
                DocumentSettings.Delete(EmployeeVm.ImageName, "Images");


                _unitOfWork.EmployeeRepository.Delete(mappedEmp);
                _unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception Ex)
            {
                ModelState.AddModelError(string.Empty, Ex.Message);
            }
            return View(EmployeeVm);
        }
    }
}
