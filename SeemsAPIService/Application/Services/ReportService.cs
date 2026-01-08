using SeemsAPIService.Application.DTOs.Reports;
using SeemsAPIService.Application.Interfaces;

namespace SeemsAPIService.Application.Services
{
    public class ReportService : IReportService   // ✅ implements SERVICE interface
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository)
        {
            _repository = repository;
        }

        public async Task<TentativeQuotedOrdersResult> GetTentativeQuotedOrdersAsync()
        {
            var result = await _repository.GetTentativeQuotedOrdersAsync();

            if (!result.TentativeOrders.Any() && !result.QuotedOrders.Any())
                throw new Exception("No tentative or quoted orders found");

            return result;
        }

        public async Task<OpenConfirmedOrdersResult> GetOpenConfirmedOrdersAsync()
        {
            var result = await _repository.GetOpenConfirmedOrdersAsync();

            if (!result.OpenOrders.Any() && !result.ConfirmedOrders.Any())
                throw new Exception("No open or confirmed orders found");

            return result;
        }
    }
}
