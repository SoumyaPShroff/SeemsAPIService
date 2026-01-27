using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.DTOs.Reports;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Application.Mapper;
using SeemsAPIService.Application.Mapper.SeemsAPIService.Application.Mapper;
using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.Services
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IReusableService _reusableService;
        private readonly IEmailService _emailService;
        private readonly IEmailRecipientService _recipientService;
        private readonly IEntityMapper<EnquiryDto, se_enquiry, string?> _enquiryMapper;
        private readonly IEntityMapper<QuotationDto, se_quotation, string?> _quotationMapper;

        public SalesService(ISalesRepository salesRepository, IReusableService reusableService,
            IEmailService emailService, IEmailRecipientService recipientService,
            IEntityMapper<EnquiryDto, se_enquiry, string?> enquiryMapper,
            IEntityMapper<QuotationDto, se_quotation, string?> quotationMapper
            )
        {
            _salesRepository = salesRepository;
            _reusableService = reusableService;
            _emailService = emailService;
            _recipientService = recipientService;
            _enquiryMapper = enquiryMapper;
            _quotationMapper = quotationMapper;
        }


        private void ValidateEnquiry(EnquiryDto dto)
        {
            if (dto.customer_id == 0)
                throw new ArgumentException("Customer is required");

            if (dto.contact_id == 0)
                throw new ArgumentException("Contact is required");

            if (string.IsNullOrWhiteSpace(dto.type))
                throw new ArgumentException("Type is required");

            if (string.IsNullOrWhiteSpace(dto.salesresponsibilityid))
                throw new ArgumentException("Sales responsibility is required");

            if (string.IsNullOrWhiteSpace(dto.completeresponsibilityid))
                throw new ArgumentException("Complete responsibility is required");

            if (string.IsNullOrWhiteSpace(dto.createdBy))
                throw new ArgumentException("CreatedBy is required");
        }

        public async Task<object> AddEnquiryAsync(EnquiryDto enquiry, IFormFile? file)
        {
            ValidateEnquiry(enquiry);

            // 2️⃣ Save file (if any)
            string? savedFilePath = await SaveFileAsync(file);

            enquiry.enquiryno = GenerateEnquiryNumber();

            enquiry.design = DefaultNo(enquiry.design);
            enquiry.library = DefaultNo(enquiry.library);
            enquiry.qacam = DefaultNo(enquiry.qacam);
            enquiry.layout_fab = DefaultNo(enquiry.layout_fab);
            enquiry.layout_testing = DefaultNo(enquiry.layout_testing);
            enquiry.dfa = DefaultNo(enquiry.dfa);

            enquiry.si = DefaultNo(enquiry.si);
            enquiry.pi = DefaultNo(enquiry.pi);
            enquiry.emi_net_level = DefaultNo(enquiry.emi_net_level);
            enquiry.emi_system_level = DefaultNo(enquiry.emi_system_level);
            enquiry.thermal_board_level = DefaultNo(enquiry.thermal_board_level);
            enquiry.thermal_system_level = DefaultNo(enquiry.thermal_system_level);

            enquiry.npi_fab = DefaultNo(enquiry.npi_fab);
            enquiry.asmb = DefaultNo(enquiry.asmb);
            enquiry.npi_testing = DefaultNo(enquiry.npi_testing);
            enquiry.hardware = DefaultNo(enquiry.hardware);
            enquiry.software = DefaultNo(enquiry.software);
            enquiry.fpg = DefaultNo(enquiry.fpg);

            enquiry.NPINew_BOMProc = DefaultNo(enquiry.NPINew_BOMProc);
            enquiry.NPINew_Fab = DefaultNo(enquiry.NPINew_Fab);
            enquiry.NPINew_Assbly = DefaultNo(enquiry.NPINew_Assbly);
            enquiry.NPINew_Testing = DefaultNo(enquiry.NPINew_Testing);
            enquiry.npinew_jobwork = DefaultNo(enquiry.npinew_jobwork);

            enquiry.govt_tender = DefaultNo(enquiry.govt_tender);
            enquiry.vaMech = DefaultNo(enquiry.vaMech);

            // 3️⃣ Map DTO → Entity  ✅ FIXED
            var newEnquiry = _enquiryMapper.MapForAdd(enquiry,savedFilePath);

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
     
        public async Task<object> EditEnquiryAsync(EnquiryDto dto, IFormFile? file)
        {
            ValidateEnquiry(dto);

            // 1️⃣ Get existing enquiry
            var existing = await _salesRepository.GetEnquiryByNoAsync(dto.enquiryno);
            if (existing == null)
                throw new Exception("Enquiry not found");

            // 2️⃣ Save file (if any)
            string? savedFilePath = await SaveFileAsync(file);

            // 3️⃣ Map DTO → EXISTING entity
           _enquiryMapper.MapForEdit(dto,existing, savedFilePath);

            // 4️⃣ Save changes
            await _salesRepository.SaveAsync(); // <-- This is correct, do not assign to var

            return new
            {
                message = "Enquiry updated successfully",
                enquiryNo = existing.enquiryno,
                filePath = savedFilePath
            };
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

            return await _salesRepository.GetThreeMonthConfirmedOrdersAsync(startDate.ToString("yyyy-MM-dd"), endPlus2);
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
        public async Task<object> GetRptViewEnquiryDataAsync(string start, string end)
        {
            return await _salesRepository.GetRptViewEnquiryDataAsync(start, end);
        }
        public async Task<List<states_ind>> GetStatesAsync()
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
        public async Task<object?> GetQuoteDetailsByEnqQuoteNoAsync(string enquiryNo,string quoteNo)
        {
           var result =  await _salesRepository.GetQuoteDetailsByEnqQuoteNoAsync(enquiryNo,quoteNo);
            return result;
        }

        public async Task<List<se_quotlayout>> GetQuoteBoardDescriptionsAsync()
        {
            var result = await _salesRepository.GetQuoteBoardDescriptionsAsync();
            if (result == null)
                throw new Exception("No data found");

            return result;
        }

        public async Task<QuotationDto> AddQuotationAsync(QuotationDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.quoteNo))
            {
                int maxQuote = await _salesRepository.GetMaxQuoteNumberAsync();
                dto.quoteNo = (maxQuote + 1).ToString();
                dto.versionNo = dto.versionNo == 0 ? 1 : dto.versionNo;
            }

            var entity = _quotationMapper.MapForAdd(dto, null);
            await _salesRepository.AddQuotationAsync(entity);
            await _salesRepository.SaveAsync();

            // ✅ dto now contains generated quoteNo
            return dto;
        }

        public async Task<QuotationDto> EditQuotationAsync(QuotationDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.quoteNo))
                throw new ArgumentException("QuoteNo is required for edit");

            var existingQuote = await _salesRepository
                .GetQuotationDetailsAsync(dto.quoteNo);

            if (existingQuote == null)
                throw new InvalidOperationException($"Quotation {dto.quoteNo} not found");

            // Mapper handles update + delete + add items
            _quotationMapper.MapForEdit(dto, existingQuote, null);

            await _salesRepository.EditQuotationAsync(existingQuote);
            await _salesRepository.SaveAsync();

            return dto; // or map from existingQuote
        }


        public async Task<object> DeleteQuotationAsync(string quoteno)
        {
            // get quotation WITH items
            var quotation = await _salesRepository.GetQuotationDetailsAsync(quoteno);
            if (quotation == null)
                throw new Exception("Quotation not found");

            // map dto → entity
            var entity = new se_quotation
            {
                quoteNo = quotation.quoteNo,
                Items = quotation.Items.Select(i => new se_quotation_items
                {
                    slNo = i.slNo
                }).ToList()
            };

            await _salesRepository.DeleteQuotationAsync(entity);
            await _salesRepository.SaveAsync();

            return new { message = "Quotation deleted successfully" };
        }
        public async Task<se_quotation?> GetQuotationDetailsAsync(string quoteNo)
        {
            if (string.IsNullOrWhiteSpace(quoteNo))
                throw new ArgumentException("Quote number is required");

            var result = await _salesRepository.GetQuotationDetailsAsync(quoteNo);

            if (result == null)
                throw new Exception("Quotation not found");

            return result;
        }

        public async Task<int> GetMaxQuoteNumberAsync()
        {
            return await _salesRepository.GetMaxQuoteNumberAsync();
        }


        public async Task<object?> GetCustomerAbbreviationAsync(long customerId)
        {
            if (customerId <= 0)
                throw new ArgumentException("Invalid customer id");

            var result = await _salesRepository.GetCustomerAbbreviationAsync(customerId);

            if (string.IsNullOrWhiteSpace(result))
                return null;

            return result;
        }
        private string GenerateEnquiryNumber()
        {
            var year = DateTime.Now.Month <= 3
                ? DateTime.Now.AddYears(-1).ToString("yy")
                : DateTime.Now.ToString("yy");

            return $"ENQ{year}{DateTime.Now.Ticks % 10000:0000}";
        }

        private string DefaultNo(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "NO";

            return value.ToUpper() == "YES" ? "YES" : "NO";
        }
        public async Task<List<RptQuoteDetails>>  RptQuoteDetailsAsync(string? start, string? end, string? quoteno)
        {
            return await _salesRepository.RptQuoteDetailsAsync(start, end,quoteno);
        }
        public async Task<QuotationReportDto> GetQuotationReportAsync(string enquiryNo,string quoteNo)
        {
            var raw = await _salesRepository
                .GetQuoteDetailsByEnqQuoteNoAsync(enquiryNo, quoteNo);

            if (raw == null)
                throw new Exception("Quotation not found");

            dynamic q = raw;

            var report = new QuotationReportDto
            {
                QuoteNo = q.quoteNo,
                EnquiryNo = q.enquiryno,
                Customer = q.Customer,
                BoardRef = q.board_ref,
                VersionNo = q.versionNo,
                CreatedBy = q.createdBy,
                TermsAndConditions = q.tandc
            };

            int sl = 1;
            decimal grandTotal = 0;

            foreach (var item in q.items)
            {
                var lineTotal = item.quantity * item.unit_rate;

                report.Items.Add(new RptQuotationLineDto
                {
                    SlNo = sl++,
                    Layout = item.layout,
                    Quantity = item.quantity,
                    UnitRate = item.unit_rate,
                    DurationType = item.durationtype,
                    CurrencySymbol = GetCurrencySymbol(item.currency_id)
                });

                grandTotal += lineTotal;
            }

            report.GrandTotal = grandTotal;

            return report;
        }

        private string GetCurrencySymbol(int currencyId)
        {
            return currencyId switch
            {
                1 => "₹",
                2 => "$",
                3 => "€",
                _ => ""
            };
        }
}
}
