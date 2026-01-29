using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.DTOs.Reports;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task EditEnquiryAsync(se_enquiry enquiry)
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
        public async Task<List<RptViewEnquiryData>> GetRptViewEnquiryDataAsync(string start, string end)
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

        public async Task<List<states_ind>> GetStatesAsync()
        {
            return await _context.states_ind
                .OrderBy(s => s.State)
                .Select(s => new states_ind { State = s.State })
                .ToListAsync();
        }

        public async Task<List<poenquiries>> GetPoEnquiriesAsync()
        {
            return await _context.poenquiries
                .Where(p => p.pbalanceamt != "0")
                .ToListAsync();
        }
        //  public async Task<object?> GetEnqCustLocContDataAsync(string enquiryNo)
        public async Task<EnquiryCustomerDto?> GetEnqCustLocContDataAsync(string enquiryNo)
        {
            return await (
                from e in _context.se_enquiry
                join c in _context.customer on e.customer_id equals c.itemno
                join sl in _context.se_customer_locations on e.location_id equals sl.location_id
                join sc in _context.se_customer_contacts on e.contact_id equals sc.contact_id
                where e.enquiryno == enquiryNo
                select new EnquiryCustomerDto
                {
                    Customer = c.Customer,
                    Location = sl.location,
                    ContactName = sc.ContactName,
                    Address = sl.address,
                    enquirytype = e.enquirytype,
                    locationid = e.location_id,
                    boardref = e.jobnames,
                    RFXNo = e.Rfxno
                }
            ).FirstOrDefaultAsync();
        }
        public async Task<List<se_quotlayout>> GetQuoteBoardDescriptionsAsync()
        {
            var excludedLayouts = new[]
            {
                "PCB Layout",
                "PCBA",
                "Timing Analysis",
                "PCB Layout at Sienna ECAD"
            };

            return await _context.se_quotlayout
                .Where(q => !excludedLayouts.Contains(q.layout))
                .ToListAsync();
        }

        public async Task AddQuotationAsync(se_quotation entity)
        {
            _context.se_quotation.Add(entity);
            await _context.SaveChangesAsync();
        }
        public async Task EditQuotationAsync(se_quotation entity)
        {
            _context.se_quotation.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task<object?> GetQuoteDetailsByEnqQuoteNoAsync(string enquiryNo, string? quoteNo)
        {
            var query =
                from q in _context.se_quotation
                where q.enquiryno == enquiryNo
                      && (quoteNo == null || q.quoteNo == quoteNo) // ✅ optional filter

                join e in _context.se_enquiry
                    on q.enquiryno equals e.enquiryno into eq
                from e in eq.DefaultIfEmpty()   // LEFT JOIN

                join c in _context.customer
                    on e.customer_id equals c.itemno into ec
                from c in ec.DefaultIfEmpty()   // LEFT JOIN

                join i in _context.se_quotation_items
                    on q.quoteNo equals i.quoteNo into items

                select new
                {
                    q.quoteNo,
                    q.board_ref,
                    q.createdBy,
                    q.versionNo,
                    q.tandc,

                    items = items.Select(it => new
                    {
                        it.slNo,
                        it.layout,
                        it.quantity,
                        it.unit_rate,
                        it.currency_id,
                        it.durationtype
                    }).ToList(),

                    createdOn = e != null ? e.createdOn : (DateTime?)null,
                    status = e != null ? e.status : null,
                    enquiryno = e != null ? e.enquiryno : q.enquiryno,
                    Customer = c != null ? c.Customer : null
                };

            if (quoteNo != null)
                return await query.FirstOrDefaultAsync(); // single quote for editing
            else
                return await query.ToListAsync();        // all quotes for listing
        }

        public Task DeleteQuotationAsync(se_quotation detail)
        {
            _context.se_quotation.Remove(detail); //header 
            _context.se_quotation_items.RemoveRange(detail.Items); // details line items
            return Task.CompletedTask;
        }

        public async Task<int> GetMaxQuoteNumberAsync()
        {
            var maxQuoteStr = await _context.se_quotation
            .Where(q => q.quoteNo != null && q.quoteNo != "" && q.quoteNo.CompareTo("2032") > 0)
                .Select(q => q.quoteNo)
                .MaxAsync();
            if (int.TryParse(maxQuoteStr, out int maxQuote))
                return maxQuote;

            return 0;
        }

        public async Task<se_quotation?> GetQuotationDetailsAsync(string quoteNo)
        {
            return await _context.se_quotation
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.quoteNo == quoteNo);
        }


        public async Task<se_quotation?> GetQuotationByQuoteVerAsync(string quoteNo, int versionNo)
        {
            return await _context.se_quotation
                .Include(q => q.Items)
                .FirstOrDefaultAsync(q => q.quoteNo == quoteNo && q.versionNo == versionNo);
        }

        public async Task<List<ViewQuoteDetails>> ViewQuoteDetailsAsync(string? start, string? end, string? quoteno)
        {
            string sql;

            if (string.IsNullOrEmpty(start) &&
                string.IsNullOrEmpty(end) &&
                string.IsNullOrEmpty(quoteno))
            {
                sql = "CALL sp_ViewQuoteDetails(NULL, NULL, NULL)";
            }
            else if (!string.IsNullOrEmpty(start) &&
                     !string.IsNullOrEmpty(end) &&
                     string.IsNullOrEmpty(quoteno))
            {
                sql = $"CALL sp_ViewQuoteDetails('{start}', '{end}', NULL)";
            }
            else if (string.IsNullOrEmpty(start) &&
                     string.IsNullOrEmpty(end) &&
                     !string.IsNullOrEmpty(quoteno))
            {
                sql = $"CALL sp_ViewQuoteDetails(NULL, NULL, '{quoteno}')";
            }
            else
            {
                sql = $"CALL sp_ViewQuoteDetails('{start}', '{end}', '{quoteno}')";
            }

            return await _context.ViewQuoteDetails
                .FromSqlRaw(sql)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<QuotationReportDto?> GetQuotationReportAsync(string quoteNo, int versionNo,string enquiryNo)
        {
            return await _context.GetQuotationReport.FirstOrDefaultAsync();
        }
    }
}
