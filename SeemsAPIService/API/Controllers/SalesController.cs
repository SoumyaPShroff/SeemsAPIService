using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;

namespace SeemsAPIService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SalesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ThreeMonthConfirmedOrders/{startdate}/{enddate}")]
        public async Task<List<ThreeMonthConfirmedOrders>> ThreeMonthConfirmedOrders(string startdate, string enddate)
        {
            List<ThreeMonthConfirmedOrders> list = new();

            // Parse input strings to DateTime safely
            if (!DateTime.TryParse(startdate, out DateTime startDateValue))
                throw new ArgumentException("Invalid start date format.");

            if (!DateTime.TryParse(enddate, out DateTime endDateValue))
                throw new ArgumentException("Invalid end date format.");

            // Add 3 months to the end date -- enddate curent month and next two months
            DateTime endDatePlus3Months = endDateValue.AddMonths(2);

            // Format dates for SQL (yyyy-MM-dd for MySQL)
            string formattedStart = startDateValue.ToString("yyyy-MM-dd");
            string formattedEnd = endDatePlus3Months.ToString("yyyy-MM-dd");

            // Call your stored procedure
            string sql = $"CALL sp_ThreeMonthConfirmedOrderData('{formattedStart}', '{formattedEnd}')";

            list = await _context.ThreeMonthConfirmedOrders
                .FromSqlRaw(sql)
                .ToListAsync();

            return list;
        }


        [HttpGet("TentativeQuotedOrders")]
        public async Task<IActionResult> GetTentativeQuotedOrders()
        {
            var data = await _context.GetTentativeQuotedOrders();
            return Ok(data);
        }

        [HttpGet("OpenConfirmedOrders")]
        public async Task<IActionResult> OpenConfirmedOrders()
        {
            var data = await _context.GetOpenConfirmedOrders();
            return Ok(data);
        }

        [HttpGet("PendingInvoices/{costcenter}")]
        public async Task<List<PendingInvoices>> PendingInvoices(string costcenter)
        {
            string sql = $"CALL sp_PendingInvoices('{costcenter}')";
            return await _context.PendingInvoices.FromSqlRaw(sql).ToListAsync();
        }

        [HttpGet("AllEnquiries")]
        public async Task<List<ViewAllEnquiries>> AllEnquiries(string? salesResponsibilityId = null, string? status = null)
        {
            // Handle nulls properly before passing into SQL
            string srId = string.IsNullOrEmpty(salesResponsibilityId) ? "" : salesResponsibilityId;
            string stat = string.IsNullOrEmpty(status) ? "" : status;

            string sql = $"CALL sp_ViewAllEnquiries('{srId}', '{stat}')";

            return await _context.ViewAllEnquiries.FromSqlRaw(sql).ToListAsync();
        }

        [HttpGet("Customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _context.customer.OrderBy(c => c.Customer).Select(c => new { c.itemno, c.Customer }).ToListAsync();

                if (customers == null || !customers.Any())
                    return NotFound("No customers found.");

                return Ok(customers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching customers.", error = ex.Message });
            }
        }

        [HttpGet("customerlocations")]
        public async Task<IActionResult> customerlocations([FromQuery] int? customerId)
        {
            try
            {
                IQueryable<se_customer_locations> query = _context.se_customer_locations;

                // If customerId is provided, filter by it
                if (customerId.HasValue)
                {
                    query = query.Where(l => l.customer_id == customerId.Value);
                }

                var custlocs = await query
                    .Select(l => new
                    {
                        l.location_id,
                        l.location,
                        l.address,
                        l.phoneno1,
                        l.phoneno2,
                    })
                    .ToListAsync();

                if (custlocs == null || !custlocs.Any())
                    return NotFound("No customer locations found.");

                return Ok(custlocs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while fetching customer locations.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("customercontacts")]
        public async Task<IActionResult> customercontacts([FromQuery] int? customer_id, int? location_id)
        {
            try
            {
                IQueryable<se_customer_contacts> query = _context.se_customer_contacts;

                // If contact_id is provided, filter by it
                if (customer_id.HasValue && location_id.HasValue)
                {
                    query = query.Where(c => c.customer_id == customer_id.Value && c.location_id == location_id.Value);
                }
                else
                {
                    // Otherwise, filter individually if provided
                    if (customer_id.HasValue)
                        query = query.Where(c => c.customer_id == customer_id.Value);

                    if (location_id.HasValue)
                        query = query.Where(c => c.location_id == location_id.Value);
                }

                var custcon = await query
                    .Select(c => new
                    {
                        c.contact_id,
                        c.location_id,
                        c.customer_id,
                        c.ContactTitle,
                        c.ContactName,
                        c.email11,
                        c.mobile1,
                        c.mobile2,
                    })
                    .ToListAsync();

                if (custcon == null || !custcon.Any())
                    return NotFound("No customer contacts found.");

                return Ok(custcon);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "An error occurred while fetching customer contacts.",
                    error = ex.Message
                });
            }
        }
        
        private string GenerateEnquiryNumber()
        {
            var currentYear = DateTime.Now.Month <= 3
                ? DateTime.Now.AddYears(-1).ToString("yy")
                : DateTime.Now.ToString("yy");

            var latest = _context.se_enquiry
                         .OrderByDescending(x => x.enquiryno)
                         .Select(x => x.enquiryno)
                         .FirstOrDefault();

            if (string.IsNullOrEmpty(latest) || !latest.Contains("ENQ" + currentYear))
                return $"ENQ{currentYear}0001";

            var lastNumber = int.Parse(latest.Replace("ENQ", ""));
            return $"ENQ{lastNumber + 1}";
        }

        [HttpPost("AddEnquiryData")]
        public async Task<IActionResult> AddEnquiryData([FromForm] EnquiryDto enquiry, IFormFile? file)
        {
            try
            {
                // Validate required fields
                if (enquiry.customer_id == 0 ||
                    enquiry.contact_id == 0 ||
                    string.IsNullOrWhiteSpace(enquiry.type) ||
                    enquiry.currency_id == 0 ||
                    string.IsNullOrWhiteSpace(enquiry.inputreceivedthru) ||
                    string.IsNullOrWhiteSpace(enquiry.salesresponsibilityid) ||
                    string.IsNullOrWhiteSpace(enquiry.completeresponsibilityid) ||
                    string.IsNullOrWhiteSpace(enquiry.govt_tender) ||
                    string.IsNullOrWhiteSpace(enquiry.quotation_request_lastdate) ||
                    string.IsNullOrWhiteSpace(enquiry.createdBy))
                {
                    return BadRequest(new { message = "Missing required fields" });
                }

                string? savedFilePath = null;

                // 🔹 File upload (optional)
                if (file != null && file.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using var stream = new FileStream(filePath, FileMode.Create);
                    await file.CopyToAsync(stream);

                    savedFilePath = Path.Combine("UploadedFiles", uniqueFileName);
                }

                // 🔹 Create enquiry object
                var newEnquiry = new se_enquiry
                {
                    enquiryno = GenerateEnquiryNumber(),
                    customer_id = enquiry.customer_id,
                    contact_id = enquiry.contact_id,
                    type = enquiry.type,
                    statename = enquiry.statename,
                    currency_id = enquiry.currency_id,
                    inputreceivedthru = enquiry.inputreceivedthru,
                    salesresponsibilityid = enquiry.salesresponsibilityid,
                    completeresponsibilityid = enquiry.completeresponsibilityid,
                    quotation_request_lastdate = enquiry.quotation_request_lastdate,
                    createdBy = enquiry.createdBy,
                    createdOn = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    location_id = enquiry.location_id,

                    // 🔹 Optional enum/string fields
                    //layout
                    design = DefaultNo(enquiry.design),
                    library = DefaultNo(enquiry.library),
                    qacam = DefaultNo(enquiry.qacam),
                    dfm = enquiry.dfm ?? "",
                    layout_fab = DefaultNo(enquiry.layout_fab),
                    layout_testing = DefaultNo(enquiry.layout_testing),
                    layout_others = enquiry.layout_others ?? "",
                    layoutbyid = enquiry.layoutbyid ?? "",
                    dfa = DefaultNo(enquiry.dfa),

                    si = DefaultNo(enquiry.si),
                    pi = DefaultNo(enquiry.pi),
                    emi_net_level = DefaultNo(enquiry.emi_net_level),
                    emi_system_level = DefaultNo(enquiry.emi_system_level),
                    thermal_board_level = DefaultNo(enquiry.thermal_board_level),
                    thermal_system_level = DefaultNo(enquiry.thermal_system_level),
                    analysis_others = enquiry.analysis_others ?? "",
                    analysisbyid = enquiry.analysisbyid ?? "",

                    npi_fab = DefaultNo(enquiry.npi_fab),
                    asmb = DefaultNo(enquiry.asmb),
                    npi_testing = DefaultNo(enquiry.npi_testing),
                    npi_others = DefaultNo(enquiry.npi_others),
                    hardware = DefaultNo(enquiry.hardware),
                    software = DefaultNo(enquiry.software),
                    fpg = DefaultNo(enquiry.fpg),
                    VA_Assembly = DefaultNo(enquiry.VA_Assembly),
                    DesignOutSource = DefaultNo(enquiry.DesignOutSource),
                    npibyid = enquiry.npibyid ?? "",

                    NPINew_BOMProc = DefaultNo(enquiry.NPINew_BOMProc),
                    NPINew_Fab = DefaultNo(enquiry.NPINew_Fab),
                    NPINew_Assbly = DefaultNo(enquiry.NPINew_Assbly),
                    NPINew_Testing = DefaultNo(enquiry.NPINew_Testing),
                    NPINewbyid = enquiry.NPINewbyid ?? "",
                    npinew_jobwork = DefaultNo(enquiry.npinew_jobwork),

                    Remarks = enquiry.Remarks ?? "",
                    status = "Open",
                    uploadedfilename = savedFilePath ?? "",
                    enquirytype = enquiry.enquirytype ?? "",
                    tool = enquiry.tool ?? "",
                    govt_tender = DefaultNo(enquiry.govt_tender),
                    jobnames = enquiry.jobnames ?? "",
                    appendreq = enquiry.appendreq ?? "",
                    ReferenceBy = enquiry.ReferenceBy ?? ""
                };

                _context.se_enquiry.Add(newEnquiry);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Enquiry saved successfully", filePath = savedFilePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving enquiry", details = ex.Message });
            }
        }

        // Helper method
        private string DefaultNo(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "NO";

            return value.ToUpper() == "YES" ? "YES" : "NO";
        }


        //public async Task<IActionResult> AddEnquiryData([FromForm] EnquiryDto enquiry, IFormFile? file)
        //{
        //    try
        //    {
        //        string? savedFilePath = null;

        //        if (file != null && file.Length > 0)
        //        {
        //            // Folder path — you can make this configurable
        //            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        //            if (!Directory.Exists(uploadsFolder))
        //                Directory.CreateDirectory(uploadsFolder);

        //            // Create unique filename
        //            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        //            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //            using (var stream = new FileStream(filePath, FileMode.Create))
        //            {
        //                await file.CopyToAsync(stream);
        //            }

        //            savedFilePath = Path.Combine("UploadedFiles", uniqueFileName);
        //        }
        //        // Example: assuming you're using Entity Framework Core
        //        var newEnquiry = new se_enquiry
        //        {
        //            enquiryno = GenerateEnquiryNumber(),
        //            customer_id = enquiry.customer_id,
        //            location_id = enquiry.location_id,
        //            contact_id = enquiry.contact_id,
        //            currency_id = enquiry.currency_id,
        //            inputreceivedthru = enquiry.inputreceivedthru,
        //            design = enquiry.design,
        //            library = enquiry.library,
        //            qacam = enquiry.qacam,
        //            dfm = enquiry.dfm,
        //            layout_fab = enquiry.layout_fab,
        //            layout_testing = enquiry.layout_testing,
        //            layout_others = enquiry.layout_others,
        //            layoutbyid = enquiry.layoutbyid,
        //            si = enquiry.si,
        //            pi = enquiry.pi,
        //            emi_net_level = enquiry.emi_net_level,
        //            emi_system_level = enquiry.emi_system_level,
        //            thermal_board_level = enquiry.thermal_board_level,
        //            thermal_system_level = enquiry.thermal_system_level,
        //            analysis_others = enquiry.analysis_others,
        //            analysisbyid = enquiry.analysisbyid,
        //            procurement = enquiry.procurement,
        //            npi_fab = enquiry.npi_fab,
        //            asmb = enquiry.asmb,
        //            npi_testing = enquiry.npi_testing,
        //            npi_others = enquiry.npi_others,
        //            npibyid = enquiry.npibyid,
        //            hardware = enquiry.hardware,
        //            software = enquiry.software,
        //            fpg = enquiry.fpg,
        //            hardware_testing = enquiry.hardware_testing,
        //            hardware_others = enquiry.hardware_others,
        //            quotation_request_lastdate = enquiry.quotation_request_lastdate,
        //            govt_tender = enquiry.govt_tender,
        //            completeresponsibilityid = enquiry.completeresponsibilityid,
        //            salesresponsibilityid = enquiry.salesresponsibilityid,
        //            status = "Open",
        //            Remarks = enquiry.Remarks,
        //            uploadedfilename = savedFilePath,
        //            createdBy = enquiry.createdBy,
        //            enquirytype = enquiry.enquirytype,
        //            tool = enquiry.tool,
        //            jobnames = enquiry.jobnames,
        //            appendreq = enquiry.appendreq,
        //            esti = enquiry.esti,
        //            type = enquiry.type,
        //            dfa = enquiry.dfa,
        //            Rfxno = enquiry.Rfxno,
        //            VA_Assembly = enquiry.VA_Assembly,
        //            VA_Outsourcing = enquiry.VA_Outsourcing,
        //            NPINew_BOMProc = enquiry.NPINew_BOMProc,
        //            NPINew_Fab = enquiry.NPINew_Fab,
        //            NPINew_Assbly = enquiry.NPINew_Assbly,
        //            npinew_jobwork = enquiry.npinew_jobwork,
        //            NPINew_Testing = enquiry.NPINew_Testing,
        //            NPINewbyid = enquiry.NPINewbyid,
        //            tm = enquiry.tm,
        //            DesignOutSource = enquiry.DesignOutSource,
        //            ReferenceBy = enquiry.ReferenceBy
        //  };

        //        _context.se_enquiry.Add(newEnquiry);
        //        await _context.SaveChangesAsync();

        //        return Ok(new { message = "Enquiry saved successfully.", filePath = savedFilePath });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error saving enquiry", details = ex.Message });
        //    }
        //}


    }
}
