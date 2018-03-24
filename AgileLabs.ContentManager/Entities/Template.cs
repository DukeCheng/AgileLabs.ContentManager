using System;

namespace AgileLabs.ContentManager.Entities
{
    public class Template : EntityBase
    {
        public string Name { get; set; }
        public string HeadContent { get; set; }
        public string BodyContent { get; set; }
        public string FootContent { get; set; }
        public Guid? ParentTemplateId { get; set; }
    }
}