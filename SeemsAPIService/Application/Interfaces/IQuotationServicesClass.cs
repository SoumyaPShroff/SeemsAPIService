using SeemsAPIService.Application.DTOs;

namespace SeemsAPIService.Application.Interfaces
{
    public interface IQuotationService
    {
        Task<string> CreateQuotationAsync(QuotationDto dto);
        Task<object> GetQuotationByNoAsync(string enqNo, string quoteNo);
    }
}
