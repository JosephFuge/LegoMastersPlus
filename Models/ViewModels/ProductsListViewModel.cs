namespace LegoMastersPlus.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public bool NoFilters { get; set; }
        public List<Product> Products { get; set; }
        public List<Category> Categories { get; set; }

        public List<string> PrimaryColors { get; set; }
        public List<string> SecondaryColors { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public int? PageSize { get; set; }
        public int? PageNum { get; set; }

        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();

        public ProductsListViewModel(List<Product> tempProducts, PaginationInfo tempPaginationInfo, int? tempPageSize, int? tempPageNum, List<Category> tempCategories, string? tempPrimaryColor, string? tempSecondaryColor)
        {
            this.Products = tempProducts;
            this.PaginationInfo = tempPaginationInfo;
            this.Categories = tempCategories;
            this.PrimaryColor = tempPrimaryColor;
            this.SecondaryColor = tempSecondaryColor;
            this.PageSize = tempPageSize;
            this.PageNum = tempPageNum;
            this.NoFilters = false;
        }

        public ProductsListViewModel()
        {
            this.Products = new List<Product>();
            this.PaginationInfo = new PaginationInfo();
            this.NoFilters = true;
        }
    }
}
