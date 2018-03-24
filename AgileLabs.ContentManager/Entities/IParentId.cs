using System;

namespace AgileLabs.ContentManager.Entities
{
    public interface IParentId
    {
        Guid? ParentId { get; set; }
        string FileName { get; set; }
    }
}