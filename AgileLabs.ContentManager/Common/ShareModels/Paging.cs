namespace AgileLabs.ContentManager.Common.ShareModels
{
    public class Paging
    {
        public Paging()
        {
            PageIndex = 1;
            PageSize = 10;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public long TotalPage { get { return Total / PageSize + (Total % PageSize > 0 ? 1 : 0); } }
        public long Total { get; set; }
    }
}