using System;

namespace AgileLabs.ContentManager.Entities
{
    public class Resource : EntityBase, IParentId
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string FileId { get; set; }
        public string FileName { get; set; }//FileName
        public bool Enabled { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public Guid? ParentId { get; set; }
    }
}