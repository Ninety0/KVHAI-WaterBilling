﻿using KVHAI.CustomClass;
using KVHAI.Models;
using KVHAI.Repository;
using Microsoft.AspNetCore.Mvc;

namespace KVHAI.Controllers.Staff.Admin
{
    public class StreetController : Controller
    {
        private readonly StreetRepository _streetRepository;
        private readonly Pagination<Streets> _pagination;

        public StreetController(StreetRepository streetRepository, Pagination<Streets> pagination)
        {
            _streetRepository = streetRepository;
            _pagination = pagination;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var pagination = new Pagination<Streets>
                {
                    ModelList = await _streetRepository.GetAllStreets(offset: 0, limit: 10),
                    NumberOfData = await _streetRepository.CountStreetsData(),
                    ScriptName = "pagination"
                };
                pagination.set(10, 5, 1);

                return View("~/Views/Staff/Admin/Streets.cshtml", pagination);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Pagination(string search, int page_index)
        {
            try
            {
                var _search = search == null || string.IsNullOrEmpty(search) ? "" : search;

                var pagination = new Pagination<Streets>
                {
                    NumberOfData = await _streetRepository.CountStreetsData(_search),
                    ModelList = await _streetRepository.GetAllStreets(offset: 0, limit: 10),
                    ScriptName = "pagination"
                };
                pagination.set(10, 5, page_index);
                pagination.ModelList = await _streetRepository.GetAllStreets(_search, pagination.Offset, 10);

                return View("~/Views/Staff/Admin/Streets.cshtml", pagination);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterStreet(Streets formData)
        {
            try
            {
                if (await _streetRepository.StreetExists(formData))
                {
                    return Ok(new { message = "exist" });
                }

                int result = await _streetRepository.CreateStreets(formData);

                if (result == 0)
                    return BadRequest(new { message = "There was an error saving the street." });

                //return Ok(new { message = "Registration Successful." });
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An error occurred while processing your request." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStreet(Streets formData)
        {
            try
            {

                int result = await _streetRepository.UpdateStreets(formData);

                if (result == 0)
                    return BadRequest(new { message = "There was an error updating the street" });

                //return Ok(new { message = "Registration Successful." });
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An error occurred while processing your request." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetStreet(string id)
        {
            var st = await _streetRepository.GetSingleStreet(id);
            if (st == null)
            {
                return NotFound();
            }
            return Ok(st);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteStreet(int id)
        {
            var st = await _streetRepository.DeleteStreets(id);
            if (st == 0)
            {
                return BadRequest(new { message = "error" });
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
