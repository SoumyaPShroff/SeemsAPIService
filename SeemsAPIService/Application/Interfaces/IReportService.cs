using SeemsAPIService.Application.DTOs.Reports;

namespace SeemsAPIService.Application.Interfaces
{
    public interface IReportService
    {
        Task<TentativeQuotedOrdersResult> GetTentativeQuotedOrdersAsync();
        Task<OpenConfirmedOrdersResult> GetOpenConfirmedOrdersAsync();

    }
}
