using AgileLabs.ContentManager.Entities;
using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private MongoDbBaseRepository<Settings> _settingsRepository;

        public SettingsController(MongoDbBaseRepository<Settings> settingsRepository)
        {
            this._settingsRepository = settingsRepository;
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(nameof(Edit));
        }

        [HttpPost]
        public IActionResult Create(Settings record)
        {
            _settingsRepository.Insert(record);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var record = _settingsRepository.GetByIdAsync(id).Result;
            return View(record);
        }

        [HttpPost]
        public IActionResult Edit(Settings record)
        {
            _settingsRepository.Update(record);
            //return Ok(new WebResponseModel
            //{
            //    methodName = nameof(Edit)
            //});
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetSettingList(int page = 1, int rows = 10)
        {
            var pagedRecords = _settingsRepository.PaginationSearchAsync(Builders<Settings>.Filter.Empty,
                Builders<Settings>.Sort.Descending(x => x.CreationTime), page, rows).Result;

            return Ok(new EasyUIPage<Settings>
            {
                Total = pagedRecords.Paging.Total,
                Rows = pagedRecords.Records
            });
        }
    }
}
