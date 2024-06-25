﻿using KVHAI.Models;
using KVHAI.Repository;
using Microsoft.AspNetCore.Mvc;
using Ganss.Xss;
using KVHAI.CustomClass;

namespace KVHAI.Controllers
{
    public class LoginController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;
        private readonly InputSanitize _sanitize;

        public LoginController(EmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _sanitize = new InputSanitize();
        }


        public async Task<IActionResult> Index()
        {
            var employees = await _employeeRepository.GetAllEmployeesAsync();
            return View(employees);
            //return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitForm(string user, string pass)
        {
            if (ModelState.IsValid)
            {
                var username = await _sanitize.HTMLSanitizerAsync(user);
                var password = await _sanitize.HTMLSanitizerAsync(pass);

                bool isExist = await _employeeRepository.ValidateCredentials(username, password);
            }
            return RedirectToAction("Index");
        }
    }
}
