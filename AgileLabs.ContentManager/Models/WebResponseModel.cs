using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Models
{
    public class WebResponseModel
    {
        public bool error { get; set; } = false;

        public string errorMsg { get; set; }

        public string methodName { get; set; } = $"Create";
    }
}
