namespace LegoMastersPlus.Models.ViewModels
{
    public class ProductsListViewModel
    {
        public IQueryable<Product> Products { get; set; }

        public ProductPaginationInfo PaginationInfo { get; set; } = new ProductPaginationInfo();

        public ProductsListViewModel(IQueryable<Product> tempProducts, ProductPaginationInfo tempPaginationInfo)
        {
            this.Products = tempProducts;
            this.PaginationInfo = tempPaginationInfo;
        }
    }
}
