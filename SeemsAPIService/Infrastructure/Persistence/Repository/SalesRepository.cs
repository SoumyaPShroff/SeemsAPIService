using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;

namespace SeemsAPIService.Infrastructure.Repositories
{
    public class SalesRepository : ISalesRepository
    {
        private readonly AppDbContext _context;

        public SalesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ThreeMonthConfirmedOrders>> GetThreeMonthConfirmedOrdersAsync(string startdate, string enddate)
        {
            string sql = $"CALL sp_ThreeMonthConfirmedOrderData('{startdate}', '{enddate}')";
            return await _context.ThreeMonthConfirmedOrders.FromSqlRaw(sql).ToListAsync();
        }


        public async Task AddEnquiryAsync(se_enquiry enquiry)
        {
            await _context.se_enquiry.AddAsync(enquiry);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetCustomerAbbreviationAsync(long customerId)
        {
            return await _context.customer
                .Where(c => c.itemno == customerId)
                .Select(c => c.Customer_abb)
                .FirstOrDefaultAsync();
        }

        public async Task<dynamic?> GetCustomerByIdAsync(long customerId)
        {
            return await _context.customer
                .Where(c => c.itemno == customerId)
                .Select(c => new { c.itemno, c.Customer })
                .FirstOrDefaultAsync();
        }
        public async Task<se_enquiry?> GetEnquiryByNoAsync(string enquiryNo)
        {
            return await _context.se_enquiry
                .FirstOrDefaultAsync(e => e.enquiryno == enquiryNo);
        }

        public async Task UpdateEnquiryAsync(se_enquiry enquiry)
        {
            _context.se_enquiry.Update(enquiry);
            await Task.CompletedTask;
        }

        public async Task<List<ViewAllEnquiries>> GetAllEnquiriesAsync(string srId, string status)
        {
            string sql = $"CALL sp_ViewAllEnquiries('{srId}', '{status}')";
            return await _context.ViewAllEnquiries.FromSqlRaw(sql).ToListAsync();
        }

 
        public async Task<List<PendingInvoices>> PendingInvoicesAsync(string costcenter)
        {
            string sql = $"CALL sp_PendingInvoices('{costcenter}')";
            return await _context.PendingInvoices.FromSqlRaw(sql).ToListAsync();
        }

 
        public async Task<string> GetCustomerAbbreviation(long pItemNo)
        {
            var customerAbbrev = await _context.customer
                .Where(c => c.itemno == pItemNo)
                .Select(c => c.Customer_abb)
                .FirstOrDefaultAsync();

            return customerAbbrev ?? "";
        }
        //public async Task<List<se_quotlayout>> GetQuoteBoardDescriptionsAsync()
        //{
        //    var excludedLayouts = new[]
        //    {
        //        "PCB Layout",
        //        "PCBA",
        //        "Timing Analysis",
        //        "PCB Layout at Sienna ECAD"
        //    };

        //    return await _context.se_quotlayout
        //        .Where(q => !excludedLayouts.Contains(q.layout))
        //        .ToListAsync();
        //}
        public async Task<List<object>> GetCustomersAsync()
        {
            return await _context.customer
                .OrderBy(c => c.Customer)
                .Select(c => new { c.itemno, c.Customer })
                .ToListAsync<object>();
        }
        public async Task<List<object>> GetCustomerLocationsAsync(int? customerId)
        {
            IQueryable<se_customer_locations> query = _context.se_customer_locations;

            if (customerId.HasValue)
                query = query.Where(l => l.customer_id == customerId.Value);

            return await query
                .Select(l => new
                {
                    l.location_id,
                    l.location,
                    l.address,
                    l.phoneno1,
                    l.phoneno2
                })
                .ToListAsync<object>();
        }

        public async Task<List<object>> GetCustomerContactsAsync(int? customerId, int? locationId)
        {
            IQueryable<se_customer_contacts> query = _context.se_customer_contacts;

            if (customerId.HasValue && locationId.HasValue)
                query = query.Where(c => c.customer_id == customerId && c.location_id == locationId);
            else
            {
                if (customerId.HasValue)
                    query = query.Where(c => c.customer_id == customerId);

                if (locationId.HasValue)
                    query = query.Where(c => c.location_id == locationId);
            }

            return await query
                .Select(c => new
                {
                    c.contact_id,
                    c.location_id,
                    c.customer_id,
                    c.ContactTitle,
                    c.ContactName,
                    c.email11,
                    c.mobile1,
                    c.mobile2
                })
                .ToListAsync<object>();
        }
        public async Task<List<RptViewEnquiryData>> GetRptViewEnquiryDataAsync(string? start, string? end)
        {
            string sql;

            if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
                sql = "CALL sp_ViewEnqData(NULL, NULL)";
            else
                sql = $"CALL sp_ViewEnqData('{start}', '{end}')";

            return await _context.RptViewEnquiryData
                .FromSqlRaw(sql)
                .ToListAsync();
        }
        public async Task<List<string>> GetStatesAsync()
        {
            return await _context.states_ind
                .OrderBy(s => s.State)
                .Select(s => s.State)
                .ToListAsync();
        }
        public async Task<List<poenquiries>> GetPoEnquiriesAsync()
        {
            return await _context.poenquiries
                .Where(p => p.pbalanceamt != "0")
                .ToListAsync();
        }
        public async Task<object?> GetEnqCustLocContDataAsync(string enquiryNo)
        {
            return await (
                from e in _context.se_enquiry
                join c in _context.customer on e.customer_id equals c.itemno
                join sl in _context.se_customer_locations on e.location_id equals sl.location_id
                join sc in _context.se_customer_contacts on e.contact_id equals sc.contact_id
                where e.enquiryno == enquiryNo
                select new
                {
                    Customer = c.Customer,
                    Location = sl.location,
                    ContactName = sc.ContactName,
                    Address = sl.address,
                    enquirytype = e.enquirytype
                }
            ).FirstOrDefaultAsync();
        }
        public async Task AddQuotationAsync(se_quotation quotation)
        {
            await _context.se_quotation.AddAsync(quotation);
        }
 

        // HEADER info
        //public async Task<object?> GetQuoteDetailsByQuoteNoAsync(string quoteNo)
        //{
        //    return await (
        //        from q in _context.se_quotation
        //        join e in _context.se_enquiry on q.enquiryno equals e.enquiryno
        //        join c in _context.customer on e.customer_id equals c.itemno
        //        where q.quoteNo == quoteNo
        //        select new
        //        {
        //            q.quoteNo,
        //            e.createdOn,
        //            e.status,
        //            e.enquiryno,
        //            c.Customer
        //        }
        //    ).FirstOrDefaultAsync();
        //}

        // ONE detail by ID
        //public async Task<se_quotation> GetQuotationDetailByQuoteAsync(string quoteno)
        //{
        //    return await _context.se_quotation.FirstOrDefaultAsync(d => d.quoteNo == quoteno);
        //}

        //public Task DeleteQuotationDetailAsync(se_quotation detail)
        //{
        //    _context.se_quotation.Remove(detail);
        //    return Task.CompletedTask;
        //}

        //public Task DeleteQuotationDetailsAsync(List<se_quotation> details)
        //{
        //    _context.se_quotation.RemoveRange(details);
        //    return Task.CompletedTask;
        //} 

        //public async Task<int> GetMaxQuoteNumberAsync(int financialYear)
        //{
        //    return await _context.se_quotation
        //        .Where(q => q.quoteNo != null && q.quoteNo.CompareTo("2032") > 0)
        //        .Select(q => Convert.ToInt32(q.quoteNo))
        //        .DefaultIfEmpty(0)
        //        .MaxAsync();
        //}

    }
}
