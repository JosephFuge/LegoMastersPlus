namespace LegoMastersPlus.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public IQueryable<Product> Products { get; set; }

        public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();

        public ProductsListViewModel(IQueryable<Product> tempProducts, PaginationInfo tempPaginationInfo)
        {
            this.Products = tempProducts;
            this.PaginationInfo = tempPaginationInfo;
        }
    }
}
