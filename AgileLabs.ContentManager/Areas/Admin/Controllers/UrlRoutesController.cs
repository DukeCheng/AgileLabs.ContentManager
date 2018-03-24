using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AgileLabs.ContentManager.Areas.Admin.AdminModel;
using AgileLabs.ContentManager.Filters;
using AgileLabs.ContentManager.Entities;

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UrlRoutesController : Controller
    {
        private readonly MongoDbBaseRepository<UrlRecord> _urlRecordRepository;

        public UrlRoutesController(MongoDbBaseRepository<UrlRecord> urlRecordRepository)
        {
            this._urlRecordRepository = urlRecordRepository;
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
        public IActionResult Create(UrlRecord record)
        {
            _urlRecordRepository.Insert(record);
            return RedirectToAction(nameof(Edit), new { id = record.Id });
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var record = _urlRecordRepository.GetByIdAsync(id).Result;
            return View(record);
        }

        [HttpPost]
        public IActionResult Edit(UrlRecord record)
        {
            _urlRecordRepository.Update(record);
            return Ok(new WebResponseModel
            {
                methodName = nameof(Edit)
            });
        }

        public async Task<IActionResult> GetUrlRoutesList(int page = 1, int rows = 10)
        {
            var pagedRecords = await _urlRecordRepository.PaginationSearchAsync(Builders<UrlRecord>.Filter.Empty,
                Builders<UrlRecord>.Sort.Descending(x => x.CreationTime), page, rows);

            var list = new List<UrlRoutesModel>();

            pagedRecords.Records.ForEach(item =>
            {
                list.Add(new UrlRoutesModel
                {
                    Id = item.Id,
                    CreationTime = item.CreationTime,
                    ModificationTime = item.ModificationTime,
                    TypeStr = item.Type.ToString(),
                    RefValue = item.RefValue,
                    Slug=item.Slug
                });
            });

            return Ok(new EasyUIPage<UrlRoutesModel>
            {
                Total = pagedRecords.Paging.Total,
                Rows = list
            });
        }
    }
}
