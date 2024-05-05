namespace RazorPages.Controllers.Utils
{
    public class Paging
    {
        private int currentPage = 1;
        private int pageSize = 15;
        
        public void GetParams(out int skip, out int take)
        {
            skip = (currentPage - 1) * pageSize;
            take = pageSize;
        }

        public bool HasNext(int totalNumber) => totalNumber > (currentPage * pageSize);
        public void Next() => ++currentPage;
        public bool HasPrevious() => currentPage > 1;
        public void Previous() => --currentPage;
    }
}
