namespace AgileLabs.ContentManager.Entities
{
    public class ResourceDirectory : EntityBase
    {
        public string FolderName { get; set; } = @"root";
        public string Parent { get; set; }
    }
}