using System;

namespace AgileLabs.ContentManager.Entities
{
    //数据绑定

    public class UrlRecord : EntityBase
    {
        public UrlRecord()
        {
            CreationTime = DateTime.UtcNow;
        }

        public string Slug { get; set; }
        public UrlRecordType Type { get; set; }
        public string RefValue { get; set; }
    }
}