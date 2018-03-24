using AgileLabs.ContentManager.Entities;
using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WidgetsController : Controller
    {
        private readonly MongoDbBaseRepository<Widget> _widgetRepository;

        public WidgetsController(MongoDbBaseRepository<Widget> templateRepository)
        {
            this._widgetRepository = templateRepository;
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
        public IActionResult Create(Widget record)
        {
            _widgetRepository.Insert(record);
            return Ok(new WebResponseModel());
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var record = _widgetRepository.GetByIdAsync(id).Result;
            return View(record);
        }

        [HttpPost]
        public IActionResult Edit(Widget record)
        {
            _widgetRepository.Update(record);
            return Json(new WebResponseModel() { methodName = nameof(Edit) });
        }

        [HttpGet]
        public async Task<IActionResult> GetWidgetsList(int page = 1, int rows = 10)
        {
            var pagedRecords = await _widgetRepository.PaginationSearchAsync(Builders<Widget>.Filter.Empty,
                Builders<Widget>.Sort.Descending(x => x.CreationTime), page, rows);

            return Ok(new EasyUIPage<Widget>
            {
                Total = pagedRecords.Paging.Total,
                Rows = pagedRecords.Records
            });
        }
    }
}
