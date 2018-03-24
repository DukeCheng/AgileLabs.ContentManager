using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgileLabs.ContentManager.Models;
using AgileLabs.ContentManager.Entities;

namespace AgileLabs.ContentManager.Areas.Admin.AdminModel
{
    public class UrlRoutesModel : EntityBase
    {
        public string Slug { get; set; }
        public string TypeStr { get; set; }
        public string RefValue { get; set; }
    }
}
