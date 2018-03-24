using System;

namespace AgileLabs.ContentManager.Entities
{
    public abstract class EntityBase
    {
        public EntityBase()
        {
            this.Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ModificationTime { get; set; }
    }
}