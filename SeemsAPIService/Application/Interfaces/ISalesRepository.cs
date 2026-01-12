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
        Task<object?> GetEnqCustLocContDataAsync(string enquiryNo);
        Task<List<poenquiries>> GetPoEnquiriesAsync();

        //        Task<List<se_quotlayout>> GetQuoteBoardDescriptionsAsync();
        // Task AddQuotationAsync(se_quotation quotation);
        // Task<object?> GetQuoteDetailsByQuoteNoAsync(string quoteNo);
        //Task DeleteQuotationDetailAsync(se_quotation detail);

        // for deleting ONE detail row
        //   Task<se_quotation> GetQuotationDetailByQuoteAsync(string quoteNo);

        // for deleting ALL details
        // Task<List<se_quotation>> GetQuotationDetailsAsync(string quoteNo);
        //   Task DeleteQuotationDetailsAsync(List<se_quotation> details);
        //  Task<int> GetMaxQuoteNumberAsync(int year);
    }
}
