using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.DTOs.Reports;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Interfaces
{
    public interface ISalesService
    {
        Task<object> AddEnquiryAsync(EnquiryDto enquiry, IFormFile? file);
        Task<object> EditEnquiryAsync(EnquiryDto enquiry, IFormFile? file);
        Task<object> GetEnquiryByNumberAsync(string enquiryNo);
        Task<List<ThreeMonthConfirmedOrders>> GetThreeMonthConfirmedOrdersAsync(string start,string end);
        Task<object> GetAllEnquiriesAsync(string? salesId, string? status);
        Task<List<PendingInvoices>> PendingInvoicesAsync(string  costcenter);
        Task<object> GetCustomersAsync();
        Task<object> GetCustomerLocationsAsync(int? customerId);
        Task<object> GetCustomerContactsAsync(int? customerId, int? locationId);
        Task<object> GetRptViewEnquiryDataAsync(string start, string end);
        Task<List<states_ind>> GetStatesAsync();
        Task<List<poenquiries>> GetPoEnquiriesAsync();
        Task<object> GetCustomerByIdAsync(long customerId);
        Task<object> GetEnqCustLocContDataAsync(string enquiryNo);
        Task<object?> GetCustomerAbbreviationAsync(long itemno);
        Task<object?> GetQuoteDetailsByEnqQuoteNoAsync(string enquiryNo,string quoteNo);
        Task<QuotationDto> AddQuotationAsync(QuotationDto dto);

        Task<QuotationDto> EditQuotationAsync(QuotationDto dto);
        Task<object> DeleteQuotationAsync(string quoteNo);
        Task<int> GetMaxQuoteNumberAsync();
        Task<se_quotation> GetQuotationDetailsAsync(string quoteNo);
        Task<List<se_quotlayout>> GetQuoteBoardDescriptionsAsync();
        Task<List<RptQuoteDetails>> RptQuoteDetailsAsync(string? start, string? end, string? quoteno);

        Task<QuotationReportDto> GetQuotationReportAsync(string enquiryNo, string quoteNo);
    }

}
