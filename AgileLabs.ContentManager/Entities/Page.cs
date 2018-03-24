using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.IO;
using AgileLabs.ContentManager.Common.ShareModels;

namespace AgileLabs.ContentManager.Entities
{
    public class Page : EntityBase
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string HeadContent { get; set; }
        public string BodyContent { get; set; }
        public string FootContent { get; set; }
        public Guid? TemplateId { get; set; }
        public Guid? ParentPagesId { get; set; }
        public PageSeoInfo SeoInfo { get; set; }
    }
}