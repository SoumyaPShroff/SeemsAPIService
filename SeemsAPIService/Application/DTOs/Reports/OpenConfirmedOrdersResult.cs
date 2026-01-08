namespace SeemsAPIService.Application.DTOs.Reports
{
    public class OpenConfirmedOrdersResult
    {
        public List<Dictionary<string, object?>> OpenOrders { get; set; } = new();
        public List<Dictionary<string, object?>> ConfirmedOrders { get; set; } = new();

    }
}
