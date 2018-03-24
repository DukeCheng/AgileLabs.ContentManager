using System.Collections.Generic;
using System.Linq;
using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using AgileLabs.ContentManager.Entities;

namespace AgileLabs.ContentManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ResourceDirectoryController : Controller
    {
        private readonly MongoDbBaseRepository<ResourceDirectory> _resourceDirectoryRepository;

        public ResourceDirectoryController(MongoDbBaseRepository<ResourceDirectory> resourceDirectoryRepository)
        {
            _resourceDirectoryRepository = resourceDirectoryRepository;
        }

        [HttpGet]
        public IActionResult BulidFolderTree(string parent = null)
        {
            var records = _resourceDirectoryRepository.SearchAsync(Builders<ResourceDirectory>.Filter.Empty).Result;

            var tempateTreeRoot = new TreeNode { id = "1", text =@"root", state = true };


            records.Where(item => item.Parent == null)
                .ToList()
                .ForEach(item =>
                {
                    tempateTreeRoot.children.Add(RecursionBuildTempalteTreeNode(records, item));
                });

            //IMemoryCache cache = HttpContext.RequestServices.GetRequiredService<IMemoryCache>();

            //var tempateTreeRoot = new TreeNode();
            //if (string.IsNullOrEmpty(parent))
            //{
            //    var list = new List<ResourceDirectory>();
            //    tempateTreeRoot.id = "1";
            //    tempateTreeRoot.text = records[0].FolderName;
            //    tempateTreeRoot.state = true;
            //    list.Add(records[0]);
            //    cache.Set("root", list);
            //}
            //else
            //{
            //    var value = (List<ResourceDirectory>)cache.Get("root");
            //    records.ToList().ForEach(item =>
            //    {
            //        value.Add(item);

            //        cache.Set("root", value);
            //    });

            //    value.ForEach(item =>
            //    {
            //        if (item.Parent == null)
            //        {
            //            tempateTreeRoot.id = "1";
            //            tempateTreeRoot.text = item.FolderName;
            //            tempateTreeRoot.state = true;
            //        }
            //        else
            //        {
            //            tempateTreeRoot.children.Add(RecursionBuildTempalteTreeNode(value, item));

            //        }
            //    });
            //}

            return Ok(new[] { tempateTreeRoot });
        }
        private TreeNode RecursionBuildTempalteTreeNode(IList<ResourceDirectory> entities, ResourceDirectory entity)

        {
            var currentNode = new TreeNode { id = entity.Id.ToString(), text = entity.FolderName };
            //currentNode.attributes.url = Url.Action(actionName, new { templateId = entity.Id });
            if (entities.Any(x => x.Parent == entity.FolderName))
            {
                entities.Where(item => item.Parent == entity.FolderName).ToList().ForEach(item =>
                {
                    currentNode.children.Add(RecursionBuildTempalteTreeNode(entities, item));
                });
            }
            return currentNode;
        }

        [HttpPost]
        public IActionResult Create(ResourceDirectory model)
        {
            //await _resourceDirectoryRepository.InsertManyAsync(new List<ResourceDirectory>()
            //{
            //    new ResourceDirectory(){FolderName = @"MongoDB",Parent = "Databases"},
            //    new ResourceDirectory(){FolderName = @"dbm",Parent = "Databases"},
            //    new ResourceDirectory(){FolderName = @"Databases",Parent = "Programming"},
            //    new ResourceDirectory(){FolderName = @"Languages",Parent = "Programming"},
            //    new ResourceDirectory(){FolderName = @"Programming",Parent = "Books"},
            //    new ResourceDirectory(){FolderName = @"Books",Parent = null}
            //});

            _resourceDirectoryRepository.Insert(model);


            return Ok(new WebResponseModel());
        }

        public IActionResult Remove(ResourceDirectory model)
        {
            return Ok();
        }
    }
}
