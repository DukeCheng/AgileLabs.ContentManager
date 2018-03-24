using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AgileLabs.ContentManager.Entities;
using AgileLabs.ContentManager.Models;

namespace AgileLabs.ContentManager.WebFramework
{
    public class ControllerBase : Controller
    {
        protected readonly ConcurrentDictionary<string, List<ResourceDirectory>> Directories = new ConcurrentDictionary<string, List<ResourceDirectory>>();

        protected TreeNode BuildTreeJson<TEntity>(IList<TEntity> entities, string actionName)
            where TEntity : EntityBase, IParentId
        {
            var tempateTreeRoot = new TreeNode { id = "1", text = "root", state = true };

            entities.Where(item => item.ParentId == null)
                .ToList()
                .ForEach(item =>
                {
                    tempateTreeRoot.children.Add(RecursionBuildTempalteTreeNode(entities, item, actionName, item.FileName));
                });

            return tempateTreeRoot;
        }

        protected TreeNode RecursionBuildTempalteTreeNode<TEntity>(IList<TEntity> entities, TEntity entity, string actionName, string textName)
            where TEntity : EntityBase, IParentId
        {
            var currentNode = new TreeNode { id = entity.Id.ToString(), text = textName };
            currentNode.attributes.url = Url.Action(actionName, new { templateId = entity.Id });
            if (entities.Any(x => x.ParentId == entity.Id))
            {
                entities.Where(item => item.ParentId == entity.Id).ToList().ForEach(item =>
                   {
                       currentNode.children.Add(RecursionBuildTempalteTreeNode(entities, item, actionName, textName));
                   });
            }
            return currentNode;
        }
    }
}
