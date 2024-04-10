namespace LegoMastersPlus.Models.ViewModels;

public class OrdersListViewModel
{
    public IQueryable<Order> Orders { get; set; }

    public PaginationInfo PaginationInfo { get; set; } = new PaginationInfo();

    public OrdersListViewModel(IQueryable<Order> tempOrders, PaginationInfo tempPaginationInfo)
    {
        this.Orders = tempOrders;
        this.PaginationInfo = tempPaginationInfo;
    }
}