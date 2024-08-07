﻿using KVHAI.CustomClass;
using KVHAI.Models;
using KVHAI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace KVHAI.Controllers.Staff.Admin
{
    public class AdminAccountController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly ResidentRepository _residentRepository;
        private readonly IWebHostEnvironment _environment;

        public AdminAccountController(EmployeeRepository employeeRepository, ResidentRepository residentRepository, IWebHostEnvironment environment)
        {
            _employeeRepository = employeeRepository;
            _residentRepository = residentRepository;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            //EMPLOYEE
            var pagination1 = new Pagination<Employee>
            {
                ModelList = await _employeeRepository.GetAllEmployeesAsync(0, 10),
                NumberOfData = await _employeeRepository.CountEmployeeData(),
                ScriptName = "emppagination"
            };
            pagination1.set(pagination1.ModelList.Count, 5, 1);

            //RESIDENT
            var pagination2 = new Pagination<Resident>
            {
                ModelList = await _residentRepository.GetAllResidentAsync(0, 10, "false"),
                NumberOfData = await _residentRepository.CountResidentData("false"),
                ScriptName = "respagination"
            };
            pagination2.set(10, 5, 1);

            var viewmodel = new ModelBinding
            {
                EmployeePagination = pagination1,
                ResidentPagination = pagination2
            };

            return View("~/Views/Staff/Admin/Account.cshtml", viewmodel);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeePagination(string search, string category, int page_index)
        {
            try
            {
                var empSearch = search == null || string.IsNullOrEmpty(search) ? "" : search;
                //EMPLOYEE
                var pagination1 = new Pagination<Employee>
                {
                    NumberOfData = await _employeeRepository.CountEmployeeData(category, empSearch),
                    ScriptName = "emppagination"
                };
                pagination1.set(10, 5, page_index);
                pagination1.ModelList = await _employeeRepository.GetAllEmployeesAsync(empSearch, category, pagination1.Offset, 10);

                //RESIDENT
                var pagination2 = new Pagination<Resident>
                {
                    ModelList = await _residentRepository.GetAllResidentAsync(0, 10, "false"),
                    NumberOfData = await _residentRepository.CountResidentData("false"),
                    ScriptName = "respagination"
                };
                pagination2.set(10, 5, 1);

                var viewmodel = new ModelBinding
                {
                    EmployeePagination = pagination1,
                    ResidentPagination = pagination2
                };

                return View("~/Views/Staff/Admin/Account.cshtml", viewmodel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ResidentPagination(string search, string category, string active, int page_index)
        {
            try
            {
                var resSearch = search == null || string.IsNullOrEmpty(search) ? "" : search;

                //RESIDENT
                var pagination2 = new Pagination<Resident>
                {
                    NumberOfData = await _residentRepository.CountResidentData(active, category, resSearch),
                    ScriptName = "respagination"
                };
                pagination2.set(10, 5, page_index);
                pagination2.ModelList = await _residentRepository.GetAllResidentAsync(pagination2.Offset, 10, active, category, resSearch);

                //EMPLOYEE
                var pagination1 = new Pagination<Employee>
                {
                    ModelList = await _employeeRepository.GetAllEmployeesAsync(0, 10),
                    NumberOfData = await _employeeRepository.CountEmployeeData(),
                    ScriptName = "emppagination"
                };
                pagination1.set(pagination1.ModelList.Count, 5, 1);

                var viewmodel = new ModelBinding
                {
                    EmployeePagination = pagination1,
                    ResidentPagination = pagination2
                };


                return View("~/Views/Staff/Admin/Account.cshtml", viewmodel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterEmployee(Employee formData)
        {
            try
            {
                if (await _employeeRepository.UserExists(formData))
                {
                    return Ok(new { message = "exist" });
                }

                int result = await _employeeRepository.CreateEmployee(formData);

                if (result == 0)
                    return BadRequest(new { message = "There was an error saving the resident and the image." });

                //return Ok(new { message = "Registration Successful." });
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An error occurred while processing your request." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(Employee formData)
        {
            try
            {

                int result = await _employeeRepository.UpdateEmployee(formData);

                if (result == 0)
                    return BadRequest(new { message = "There was an error updating credentials" });

                //return Ok(new { message = "Registration Successful." });
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An error occurred while processing your request." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int res_id, string status)
        {
            try
            {
                int result = await _residentRepository.UpdateStatus(res_id, status);
                if (result < 1)
                {
                    return BadRequest("There was an error updating the status");
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees(string id)
        {
            var employee = await _employeeRepository.GetSingleEmployee(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpGet]
        public async Task<IActionResult> GetImageBase64(string id)
        {
            var imagePath = await _residentRepository.GetImagePathAsync(id);
            if (string.IsNullOrEmpty(imagePath))
                return Json(new { success = false });

            var fullPath = Path.Combine(_environment.WebRootPath, imagePath);
            if (!System.IO.File.Exists(fullPath))
                return Json(new { success = false });

            byte[] imageArray = await System.IO.File.ReadAllBytesAsync(fullPath);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);

            return Json(new { success = true, imageBase64 = base64ImageRepresentation });
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _employeeRepository.DeleteEmployee(id);
            if (employee == 0)
            {
                return BadRequest(new { message = "error" });
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
