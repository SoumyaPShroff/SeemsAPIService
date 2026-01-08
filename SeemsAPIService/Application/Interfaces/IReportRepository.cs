using SeemsAPIService.Application.DTOs.Reports;

namespace SeemsAPIService.Application.Interfaces
{
    public interface IReportRepository
    {
        Task<TentativeQuotedOrdersResult> GetTentativeQuotedOrdersAsync();
        Task<OpenConfirmedOrdersResult> GetOpenConfirmedOrdersAsync();
    }
}
