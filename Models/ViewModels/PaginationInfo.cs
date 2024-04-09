namespace LegoMastersPlus.Models.ViewModels
{
    public class PaginationInfo
    {
        public int TotalItems{ get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)(Math.Ceiling((decimal)TotalItems / ItemsPerPage));

        public PaginationInfo(int totalProducts, int itemsPerPage, int currentPage)
        {
            TotalItems = totalProducts;
            ItemsPerPage = itemsPerPage;
            CurrentPage = currentPage;
        }

        public PaginationInfo()
        {
            TotalItems = 0;
            ItemsPerPage = 10;
            CurrentPage = 1;
        }
    }
}
