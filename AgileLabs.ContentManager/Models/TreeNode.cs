using System.Collections.Generic;

namespace AgileLabs.ContentManager.Models
{
    public sealed class TreeNode
    {
        public TreeNode()
        {
            this.attributes = new TreeNodeAttribute();
            this.children = new List<TreeNode>();
        }
        public string id { get; set; }
        public string text { get; set; }
        public bool state { get; set; }
        public TreeNodeAttribute attributes { get; set; }
        public List<TreeNode> children { get; set; }
    }
}