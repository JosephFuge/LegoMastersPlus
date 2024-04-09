namespace LegoMastersPlus.Models.ViewModels
{
    public class ProductPaginationInfo
    {
        public int TotalItems{ get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages => (int)(Math.Ceiling((decimal)TotalItems / ItemsPerPage));

        public ProductPaginationInfo(int totalBooks, int itemsPerPage, int currentPage)
        {
            TotalItems = totalBooks;
            ItemsPerPage = itemsPerPage;
            CurrentPage = currentPage;
        }

        public ProductPaginationInfo()
        {
            TotalItems = 0;
            ItemsPerPage = 10;
            CurrentPage = 1;
        }
    }
}
