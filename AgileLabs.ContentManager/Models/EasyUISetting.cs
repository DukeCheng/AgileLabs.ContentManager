using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AgileLabs.ContentManager.Models
{
    public class EasyUIPage<T>
    {
        public long Total { get; set; }
        public List<T> Rows { get; set; }
    }
}
