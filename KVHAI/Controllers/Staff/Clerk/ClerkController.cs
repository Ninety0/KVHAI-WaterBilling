﻿using KVHAI.CustomClass;
using Microsoft.AspNetCore.Mvc;

namespace KVHAI.Controllers.Staff.Clerk
{
    public class ClerkController : Controller
    {
        //private readonly WaterReadingRepository _waterReadingRepository;
        private readonly WaterBilling _waterBilling;

        public ClerkController(WaterBilling waterBilling)//WaterReadingRepository waterReadingRepository
        {
            _waterBilling = waterBilling;
        }

        public async Task<IActionResult> Index()
        {
            //var prevReading = await _waterReadingRepository.GetPreviousReading();
            //var currentReading = await _waterReadingRepository.GetCurrentReading();

            //var model = new ModelBinding
            //{
            //    PreviousReading = prevReading.PreviousReading,
            //    CurrentReading = currentReading.CurrentReading,
            //    ResidentAddress = prevReading.ResidentAddress

            //};

            await _waterBilling.UseWaterBilling();

            return View("~/Views/Staff/Clerk/Index.cshtml", _waterBilling);
        }
    }
}
