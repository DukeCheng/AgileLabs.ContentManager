using AgileLabs.ContentManager.Entities;
using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PagesController : Controller
    {
        private MongoDbBaseRepository<Page> _pageRepository;
        private readonly MongoDbBaseRepository<Template> _templateRepository;

        public PagesController(MongoDbBaseRepository<Page> pageRepository, MongoDbBaseRepository<Template> templateRepository)
        {
            this._pageRepository = pageRepository;
            this._templateRepository = templateRepository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TreeJson()
        {
            var pagedRecords = await _pageRepository.SearchAsync(Builders<Page>.Filter.Empty);

            var tempateTreeRoot = new TreeNode { id = "1", text = "root", state = true };

            foreach (var item in pagedRecords.Where(x => x.ParentPagesId == null))
            {
                tempateTreeRoot.children.Add(RecursionBuildTempalteTreeNode(item, pagedRecords));
            }

            return Json(new[] { tempateTreeRoot });
        }

        private TreeNode RecursionBuildTempalteTreeNode(Page currentPage, IList<Page> pages)
        {
            var currentNode = new TreeNode { id = currentPage.Id.ToString(), text = currentPage.Title };
            currentNode.attributes.url = Url.Action(nameof(ManagePage), new { pageId = currentPage.Id });

            if (pages.Any(x => x.ParentPagesId == currentPage.Id))
            {
                foreach (var childTemplate in pages.Where(x => x.ParentPagesId == currentPage.Id))
                {
                    currentNode.children.Add(RecursionBuildTempalteTreeNode(childTemplate, pages));
                }
            }
            return currentNode;
        }

        public async Task<IActionResult> ManagePage(Guid pageId)
        {
            var pageRecord = await _pageRepository.GetByIdAsync(pageId);
            if (pageRecord.TemplateId.HasValue)
            {
                PupulateAviliableTemplate(pageRecord.TemplateId.Value);

                var template = await _templateRepository.GetByIdAsync((Guid)pageRecord.TemplateId);
                pageRecord.BodyContent = template.BodyContent;
                pageRecord.FootContent = template.FootContent;
                pageRecord.HeadContent = template.HeadContent;
            }
            return View(pageRecord);
        }

        private void PupulateAviliableTemplates(Guid? selectedTemplateId = null)
        {
            var templates = _templateRepository.GetAll();
            var aviliableTempates = templates.Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString(), Selected = selectedTemplateId == x.Id }).ToList();
            aviliableTempates.Insert(0, new SelectListItem() { Text = "Select Template" });
            ViewBag.AviliableTemplates = aviliableTempates;
        }

        private async void PupulateAviliableTemplate(Guid selectedTemplateId)
        {
            var template = await _templateRepository.GetByIdAsync(selectedTemplateId);

            ViewBag.AviliableTemplateName = template.Name;
            ViewBag.TemplateId = template.Id;
        }

        [HttpGet]
        public IActionResult Create()
        {
            PupulateAviliableTemplates();
            return View(nameof(Edit));
        }

        [HttpPost]
        public IActionResult Create(Page pageRecord, [FromServices] MongoDbBaseRepository<UrlRecord> urlRecordRepository)
        {
            //需要判断一下url
            if (!string.IsNullOrEmpty(pageRecord.Url))
                pageRecord.Url = pageRecord.Url.ToLower();


            var filters = Builders<UrlRecord>.Filter.Where(x => x.Type == UrlRecordType.Page && x.Slug == pageRecord.Url);
            var urlRecord = urlRecordRepository.SigleOrDefault(filters).Result;
            if (urlRecord != null)
            {
                throw new Exception($"URL:{pageRecord.Url} already exists");
            }
            //新的UrlRecord不存在，插入新的UrlRecord
            urlRecordRepository.Insert(new UrlRecord()
            {
                Slug = pageRecord.Url,
                Type = UrlRecordType.Page,
                RefValue = pageRecord.Id.ToString()
            });
            _pageRepository.Insert(pageRecord);

            return Ok(new WebResponseModel());
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var record = _pageRepository.GetByIdAsync(id).Result;
            PupulateAviliableTemplates(record.TemplateId);
            return View(record);
        }

        [HttpPost]
        public IActionResult Edit(Page page, [FromServices] MongoDbBaseRepository<UrlRecord> urlRecordRepository)
        {
            if (page.Url != null)
            {
                page.Url = page.Url.ToLower();
            }

            var oldPage = _pageRepository.GetByIdAsync(page.Id).Result;
            var filters = Builders<UrlRecord>.Filter.Where(x => x.Type == UrlRecordType.Page && x.RefValue == oldPage.Id.ToString());
            var existsUrlRecord = urlRecordRepository.SigleOrDefault(filters).Result;

            if (existsUrlRecord == null)
            {
                //之前不存在，补充进去
                //for fix some page not set urlrecords issue
                var newUrlRecord = new UrlRecord()
                {
                    Slug = page.Url,
                    Type = UrlRecordType.Page,
                    RefValue = page.Id.ToString()
                };
                urlRecordRepository.Insert(newUrlRecord);
            }
            else if (page.Url != existsUrlRecord.Slug)
            {
                //旧页面的Url已存在，但是URL不相同，意味着在新页面编辑的时候更新了URL

                //旧的URL做301处理
                //keep old url, set old url redirect path
                existsUrlRecord.Type = UrlRecordType.RedirectPermanent;
                existsUrlRecord.RefValue = page.Url;
                urlRecordRepository.Update(existsUrlRecord);

                //并创建新的页面
                //create new url for the new page
                var newUrlRecord = new UrlRecord()
                {
                    Slug = page.Url,
                    Type = UrlRecordType.Page,
                    RefValue = page.Id.ToString()
                };
                urlRecordRepository.Insert(newUrlRecord);
            }
            else
            {
                //other no change for url record
            }

            page.ModificationTime = DateTime.UtcNow;
            page.Title = oldPage.Title;
            _pageRepository.Update(page);
            return Ok(new WebResponseModel());
        }

        [HttpPost]
        public IActionResult SelectedTemplate()
        {
            return Ok(_templateRepository.GetAll());
        }



        [HttpPost]
        public IActionResult Delete(Page page)
        {
            if (_pageRepository.Delete(page))
                return Ok(new WebResponseModel());
            return Ok(new WebResponseModel
            {
                error = true,
                errorMsg = $"system error!"
            });

        }
    }
}
