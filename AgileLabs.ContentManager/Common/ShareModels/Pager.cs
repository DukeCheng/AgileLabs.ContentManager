using System.Collections.Generic;

namespace AgileLabs.ContentManager.Common.ShareModels
{
    public class Pager<T>
    {
        public List<T> Records { get; set; }
        public Paging Paging { get; set; }
    }
}