using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Services
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IReusableService _reusableService;
        private readonly IEmailService _emailService;
        private readonly IEmailRecipientService _recipientService;

        public SalesService(ISalesRepository salesRepository,IReusableService reusableService,IEmailService emailService,IEmailRecipientService recipientService)
        {
            _salesRepository = salesRepository;
            _reusableService = reusableService;
            _emailService = emailService;
            _recipientService = recipientService;
        }

        private string GenerateEnquiryNumber()
        {
            var year = DateTime.Now.Month <= 3
                ? DateTime.Now.AddYears(-1).ToString("yy")
                : DateTime.Now.ToString("yy");

            return $"ENQ{year}{DateTime.Now.Ticks % 10000:0000}";
        }

        public async Task<object> CreateEnquiryAsync(EnquiryDto enquiry, IFormFile? file)
        {
            // 1️⃣ Generate enquiry number
            string enquiryNo = GenerateEnquiryNumber();

            // 2️⃣ Save file (if any)
            string? savedFilePath = await SaveFileAsync(file);

            // 3️⃣ Map DTO → Entity  ✅ FIXED
            var newEnquiry = MapToEntity(enquiry, savedFilePath, enquiryNo);

            // 4️⃣ Save enquiry
            await _salesRepository.AddEnquiryAsync(newEnquiry);
            await _salesRepository.SaveAsync();

            // 5️⃣ Trigger email
            await SendEnquiryCreatedEmailAsync(newEnquiry, enquiry);

            return new { message = "Enquiry saved successfully", filePath = savedFilePath };
        }

        // ---------------- PRIVATE METHODS ----------------

        private async Task SendEnquiryCreatedEmailAsync(se_enquiry enquiry, EnquiryDto dto)
        {
            // Recipients
            var toUsers = !string.IsNullOrEmpty(dto.ToMailList)
                ? JsonConvert.DeserializeObject<List<string>>(dto.ToMailList) ?? new()
                : new();

            var ccUsers = !string.IsNullOrEmpty(dto.CCMailList)
                ? JsonConvert.DeserializeObject<List<string>>(dto.CCMailList) ?? new()
                : new();

            var (dbTo, dbCc) = await _recipientService.GetRecipientsAsync("EnqCreated");

            toUsers.AddRange(dbTo);
            ccUsers.AddRange(dbCc);

            toUsers = toUsers.Distinct().ToList();
            ccUsers = ccUsers.Distinct().ToList();

            // Data
            var customerAbb =
                await _salesRepository.GetCustomerAbbreviationAsync(enquiry.customer_id ?? 0) ?? "";

            var completeResp =
                await _reusableService.GetUserNameAsync(enquiry.completeresponsibilityid);

            var salesResp =
                await _reusableService.GetUserNameAsync(enquiry.salesresponsibilityid);

            var customer =
                await _salesRepository.GetCustomerByIdAsync(enquiry.customer_id ?? 0);

            var subject =
                $"{enquiry.enquiryno} - {customerAbb} : New {enquiry.enquirytype} Enquiry Added";

            var body = BuildEmailBody(enquiry, dto, customer, completeResp, salesResp);

            await _emailService.SendEmailAsync(
                string.Join(";", toUsers),
                subject,
                body,
                string.Join(";", ccUsers)
            );
        }

        private string BuildEmailBody(
            se_enquiry enquiry,
            EnquiryDto dto,
            dynamic customer,
            string completeResp,
            string salesResp)
        {
            return $@"
                    Hello Team,<br/><br/>
                    {dto.createdBy} has requested a new enquiry.<br/><br/>

                    <table border='1' cellpadding='6'>
                    <tr><td><b>Enquiry No</b></td><td>{enquiry.enquiryno}</td></tr>
                    <tr><td><b>Customer</b></td><td>{customer?.Customer}</td></tr>
                    <tr><td><b>Sales Responsibility</b></td><td>{salesResp}</td></tr>
                    <tr><td><b>Complete Responsibility</b></td><td>{completeResp}</td></tr>
                    </table>

                    <br/>Regards,<br/>SEEMS";
        }

        private async Task<string?> SaveFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(), "UploadedFiles");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return Path.Combine("UploadedFiles", uniqueFileName);
        }

        private se_enquiry MapToEntity(EnquiryDto dto, string? filePath, string enquiryNo)
        {
            return new se_enquiry
            {
                enquiryno = enquiryNo,   // ✅ passed from service
                customer_id = dto.customer_id,
                contact_id = dto.contact_id,
                type = dto.type,
                salesresponsibilityid = dto.salesresponsibilityid,
                completeresponsibilityid = dto.completeresponsibilityid,
                createdBy = dto.createdBy,
                createdOn = DateTime.Now,
                uploadedfilename = filePath ?? "",
                status = "Open"
            };
        }

        public async Task UpdateEnquiryAsync(EnquiryDto dto, IFormFile? file)
        {
            var existing = await _salesRepository.GetEnquiryByNoAsync(dto.enquiryno);
            if (existing == null)
                throw new Exception("Enquiry not found");

            // Update fields (same as old controller – trimmed)
            existing.customer_id = dto.customer_id;
            existing.contact_id = dto.contact_id;
            existing.type = dto.type;
            existing.salesresponsibilityid = dto.salesresponsibilityid;
            existing.completeresponsibilityid = dto.completeresponsibilityid;
            existing.Remarks = dto.Remarks ?? "";
            existing.enquirytype = dto.enquirytype ?? "";

            // File upload
            if (file != null && file.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                existing.uploadedfilename = Path.Combine("UploadedFiles", uniqueFileName);
            }

            await _salesRepository.UpdateEnquiryAsync(existing);
            await _salesRepository.SaveAsync();
        }

        public async Task<object> GetEnquiryByNumberAsync(string enquiryNo)
        {
            var enquiry = await _salesRepository.GetEnquiryByNoAsync(enquiryNo);
            if (enquiry == null)
                throw new Exception("Enquiry not found");

            return enquiry;
        }

        public async Task<List<ThreeMonthConfirmedOrders>> GetThreeMonthConfirmedOrdersAsync(
            string start,
            string end)
        {
            if (!DateTime.TryParse(start, out var startDate))
                throw new Exception("Invalid start date");

            if (!DateTime.TryParse(end, out var endDate))
                throw new Exception("Invalid end date");

            // old logic → end + 2 months
            var endPlus2 = endDate.AddMonths(2).ToString("yyyy-MM-dd");

            return await _salesRepository.GetThreeMonthConfirmedOrdersAsync(startDate.ToString("yyyy-MM-dd"),endPlus2);
        }

        public async Task<object> GetAllEnquiriesAsync(string? salesId, string? status)
        {
            string srId = string.IsNullOrEmpty(salesId) ? "" : salesId;
            string stat = string.IsNullOrEmpty(status) ? "" : status;

            return await _salesRepository.GetAllEnquiriesAsync(srId, stat);
        }
 
        public async Task<object> GetCustomersAsync()
        {
            var customers = await _salesRepository.GetCustomersAsync();

            if (customers == null || !customers.Any())
                throw new Exception("No customers found.");

            return customers;
        }
        public async Task<object> GetCustomerLocationsAsync(int? customerId)
        {
            var data = await _salesRepository.GetCustomerLocationsAsync(customerId);

            if (data == null || !data.Any())
                throw new Exception("No customer locations found.");

            return data;
        }
        public async Task<object> GetCustomerContactsAsync(int? customerId, int? locationId)
        {
            var data = await _salesRepository.GetCustomerContactsAsync(customerId, locationId);

            if (data == null || !data.Any())
                throw new Exception("No customer contacts found.");

            return data;
        }
        public async Task<object> GetRptViewEnquiryDataAsync(string? start, string? end)
        {
            return await _salesRepository.GetRptViewEnquiryDataAsync(start, end);
        }
        public async Task<object> GetStatesAsync()
        {
            var states = await _salesRepository.GetStatesAsync();
            if (!states.Any())
                throw new Exception("No states found");

            return states;
        }
        public async Task<List<poenquiries>> GetPoEnquiriesAsync()
        {
            var data = await _salesRepository.GetPoEnquiriesAsync();

            if (data == null || !data.Any())
                throw new Exception("No PO enquiries found");

            return data;
        }
 
        public async Task<object> GetCustomerByIdAsync(long customerId)
        {
            var customer = await _salesRepository.GetCustomerByIdAsync(customerId);
            if (customer == null)
                throw new Exception("Customer not found");

            return customer;
        }
        public async Task<object> GetEnqCustLocContDataAsync(string enquiryNo)
        {
            var result = await _salesRepository.GetEnqCustLocContDataAsync(enquiryNo);
            if (result == null)
                throw new Exception("No data found");

            return result;
        }
        private int GetCurrentFinancialYear()
        {
            var today = DateTime.Today;

            // Financial year starts in April
            if (today.Month >= 4)
                return today.Year;
            else
                return today.Year - 1;
        }

        public async Task<List<PendingInvoices>> PendingInvoicesAsync(string costcenter)
        {
            var result = await _salesRepository.PendingInvoicesAsync(costcenter);
            if (result == null)
                throw new Exception("No data found");

            return result;
        }
 

        Task<object?> ISalesService.GetCustomerAbbreviationAsync(long itemno)
        {
            throw new NotImplementedException();
        }

        //public async Task<object> GetQuoteBoardDescriptionsAsync()
        //{
        //    return await _salesRepository.GetQuoteBoardDescriptionsAsync();
        //}

        //private async Task<string> GetNewQuoteNumberAsync()
        //{
        //    int year = GetCurrentFinancialYear();

        //    int maxQuoteNo = await _salesRepository.GetMaxQuoteNumberAsync(year);

        //    if (maxQuoteNo < (year * 10000))
        //        return $"{year}0001";

        //    return (maxQuoteNo + 1).ToString();
        //}

        //public async Task<object> AddQuotationAsync(QuotationDto dto)
        //{
        //    // 1️⃣ Generate quotation number
        //    string quoteNo = await GetNewQuoteNumberAsync();

        //    // 2️⃣ Map DTO → Entity
        //    var quotation = new se_quotation
        //    {
        //        quoteno = quoteNo,
        //        enquiryno = dto.enquiryno,
        //        customer_id = dto.customer_id,
        //        createdBy = dto.createdBy,
        //        createdOn = DateTime.Now,
        //        status = "Open"
        //    };

        //    // 3️⃣ Save
        //    await _salesRepository.AddQuotationAsync(quotation);
        //    await _salesRepository.SaveAsync();

        //    return new
        //    {
        //        message = "Quotation created successfully",
        //        quoteno = quoteNo
        //    };
        //}
        //public async Task<object> GetQuoteDetailsByQuoteNoAsync(string quoteNo)
        //{
        //    var result = await _salesRepository.GetQuoteDetailsByQuoteNoAsync(quoteNo);

        //    if (result == null)
        //        throw new Exception("Quotation not found");

        //    return result;
        //}
        //public async Task<object> DeleteQuotationDetailAsync(string quoteno)
        //{
        //    var detail = await _salesRepository.GetQuotationDetailByQuoteAsync(quoteno);

        //    if (detail == null)
        //        throw new Exception("Quotation detail not found");

        //    await _salesRepository.DeleteQuotationDetailAsync(detail);
        //    await _salesRepository.SaveAsync();

        //    return new { message = "Quotation detail deleted successfully" };
        //}
        //public async Task<object> DeleteAllQuotationDetailsAsync(string quoteNo)
        //{
        //    var details = await _salesRepository.GetQuotationDetailsAsync(quoteNo);

        //    if (!details.Any())
        //        throw new Exception("No quotation details found");

        //    await _salesRepository.DeleteQuotationDetailsAsync(details);
        //    await _salesRepository.SaveAsync();

        //    return new { message = "All quotation details deleted successfully" };
        //}


    }
}
