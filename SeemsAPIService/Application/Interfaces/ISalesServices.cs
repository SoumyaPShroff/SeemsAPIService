using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Interfaces
{
    public interface ISalesService
    {
        Task<object> CreateEnquiryAsync(EnquiryDto enquiry, IFormFile? file);
        Task UpdateEnquiryAsync(EnquiryDto enquiry, IFormFile? file);
        Task<object> GetEnquiryByNumberAsync(string enquiryNo);
        Task<List<ThreeMonthConfirmedOrders>> GetThreeMonthConfirmedOrdersAsync(string start,string end);
        Task<object> GetAllEnquiriesAsync(string? salesId, string? status);
        Task<List<PendingInvoices>> PendingInvoicesAsync(string  costcenter);
        Task<object> GetCustomersAsync();
        Task<object> GetCustomerLocationsAsync(int? customerId);
        Task<object> GetCustomerContactsAsync(int? customerId, int? locationId);
        Task<object> GetRptViewEnquiryDataAsync(string start, string end);
        Task<object> GetStatesAsync();
        Task<List<poenquiries>> GetPoEnquiriesAsync();
        Task<object> GetCustomerByIdAsync(long customerId);
        Task<object> GetEnqCustLocContDataAsync(string enquiryNo);
        Task<object?> GetCustomerAbbreviationAsync(long itemno);

        //Task<object> AddQuotationAsync(QuotationDto dto);
        //Task<object> GetQuoteDetailsByQuoteNoAsync(string quoteNo);
        //Task<object> DeleteAllQuotationDetailsAsync(string quoteNo);
        //Task<int> GetMaxQuoteNumberAsync(int financialYear);
        //Task<object> DeleteQuotationDetailAsync(string quoteno);
        //Task<object> GetQuoteBoardDescriptionsAsync();
    }

}
