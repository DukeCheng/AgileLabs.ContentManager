using AgileLabs.ContentManager.Controllers;
using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgileLabs.ContentManager.Filters;
using AgileLabs.ContentManager.Entities;

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TemplatesController : Controller
    {
        private readonly MongoDbBaseRepository<Template> _templateRepository;

        public TemplatesController(MongoDbBaseRepository<Template> templateRepository)
        {
            this._templateRepository = templateRepository;
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        //[ResponseCache(Duration = 3600)]
        public async Task<IActionResult> TreeJson()
        {
            var pagedRecords = await _templateRepository.SearchAsync(Builders<Template>.Filter.Empty);

            var tempateTreeRoot = new TreeNode { id = "1", text = "root", state = true };

            foreach (var item in pagedRecords.Where(x => x.ParentTemplateId == null))
            {
                tempateTreeRoot.children.Add(RecursionBuildTempalteTreeNode(item, pagedRecords));
            }

            return Json(new[] { tempateTreeRoot });
        }

        public TreeNode RecursionBuildTempalteTreeNode(Template currentTemplate, IList<Template> templates)
        {
            var currentNode = new TreeNode { id = currentTemplate.Id.ToString(), text = currentTemplate.Name };
            currentNode.attributes.url = Url.Action(nameof(ManageTemplate), new { templateId = currentTemplate.Id });

            if (templates.Any(x => x.ParentTemplateId == currentTemplate.Id))
            {
                foreach (var childTemplate in templates.Where(x => x.ParentTemplateId == currentTemplate.Id))
                {
                    currentNode.children.Add(RecursionBuildTempalteTreeNode(childTemplate, templates));
                }
            }

            return currentNode;

        }

        public async Task<IActionResult> ManageTemplate(Guid templateId)
        {
            var record = await _templateRepository.GetByIdAsync(templateId);
            return View(record);
        }

        [HttpGet]
        public IActionResult Create()
        {
            PupulateAviliableTemplates();
            return View(nameof(Edit));
        }

        [HttpPost]
        public IActionResult Create(Template template)
        {
            _templateRepository.Insert(template);
            return Ok(new WebResponseModel());
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var record = _templateRepository.GetByIdAsync(id).Result;
            PupulateAviliableTemplates(record.ParentTemplateId, new List<Guid>() { record.Id });
            return View(record);
        }

        [HttpPost]
        public IActionResult Edit(Template template)
        {
            _templateRepository.Update(template);
            return Ok(new WebResponseModel());
        }

        private void PupulateAviliableTemplates(Guid? selectedTemplateId = null, List<Guid> excludeIds = null)
        {
            var templates = _templateRepository.GetAll();
            if (excludeIds == null)
            {
                excludeIds = new List<Guid>(0);
            }
            var aviliableTempates = templates.Where(x => !excludeIds.Contains(x.Id)).Select(x => new SelectListItem()
            {
                Text = x.Name,
                Value = x.Id.ToString(),
                Selected = selectedTemplateId == x.Id
            }).ToList();
            aviliableTempates.Insert(0, new SelectListItem() { Text = "Select Template" });
            ViewBag.AviliableTemplates = aviliableTempates;
        }

        /// <summary>
        /// 删除
        /// </summary>
        [HttpPost]
        public IActionResult Delete(Template template)
        {
            return Ok(_templateRepository.Delete(template) ? new WebResponseModel() : new WebResponseModel { error = true, errorMsg = $"系统异常" });
        }
    }
}
