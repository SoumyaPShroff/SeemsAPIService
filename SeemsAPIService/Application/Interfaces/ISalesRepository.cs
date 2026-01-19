using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Interfaces
{
    public interface ISalesRepository
    {
        Task<List<ThreeMonthConfirmedOrders>> GetThreeMonthConfirmedOrdersAsync(string startdate, string enddate);
        Task AddEnquiryAsync(se_enquiry enquiry);
        Task SaveAsync();
        Task<string?> GetCustomerAbbreviationAsync(long customerId);
        Task<dynamic?> GetCustomerByIdAsync(long customerId);
        Task<se_enquiry?> GetEnquiryByNoAsync(string enquiryNo);
        Task EditEnquiryAsync(se_enquiry enquiry);
        Task<List<ViewAllEnquiries>> GetAllEnquiriesAsync(string srId, string status);
        Task<List<object>> GetCustomersAsync();
        Task<List<object>> GetCustomerLocationsAsync(int? customerId);
        Task<List<object>> GetCustomerContactsAsync(int? customerId, int? locationId);
        Task<List<RptViewEnquiryData>> GetRptViewEnquiryDataAsync(string start, string end);
        Task<List<states_ind>> GetStatesAsync();
        Task<List<PendingInvoices>> PendingInvoicesAsync(string costcenter);
        Task<List<poenquiries>> GetPoEnquiriesAsync();

        // Quotation Related
        Task<object?> GetEnqCustLocContDataAsync(string enquiryNo);
        // Task<QuotationDto?> GetQuotationDetailsAsync(string quoteNo);
        Task<se_quotation> GetQuotationDetailsAsync(string quoteNo);
        Task AddQuotationAsync(se_quotation entity);
        Task EditQuotationAsync(se_quotation entity);
        Task DeleteQuotationAsync(se_quotation detail);
        Task<int> GetMaxQuoteNumberAsync();
        Task<List<se_quotlayout>> GetQuoteBoardDescriptionsAsync();
        Task<object?> GetQuoteDetailsByEnqQuoteNoAsync(string enquiryNo,string quoteNo);
       
    }
}
