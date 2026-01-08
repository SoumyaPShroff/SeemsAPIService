namespace SeemsAPIService.Application.DTOs.Reports
{
    public class TentativeQuotedOrdersResult
    {
        public List<Dictionary<string, object?>> TentativeOrders { get; set; } = new();
        public List<Dictionary<string, object?>> QuotedOrders { get; set; } = new();

    }
}
