using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Services;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SeemsAPIService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailTriggerService _emailTriggerService;
        private readonly IReusableServices _reusableServices;
        public SalesController(AppDbContext context, IReusableServices reusableServices, EmailTriggerService emailTriggerService)
        {
            _context = context;
            _reusableServices = reusableServices;
            _emailTriggerService = emailTriggerService;
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
        public async Task<IActionResult> Customers()
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

        [HttpGet("GetCustomerAbbreviation")]
        public async Task<string> GetCustomerAbbreviation(long pItemNo)
        {
            var customerAbbrev = await _context.customer
                .Where(c => c.itemno == pItemNo)
                .Select(c => c.Customer_abb)
                .FirstOrDefaultAsync();

            return customerAbbrev ?? "";
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
                    string.IsNullOrWhiteSpace(enquiry.salesresponsibilityid) ||
                    string.IsNullOrWhiteSpace(enquiry.completeresponsibilityid) ||
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
                    statename = enquiry.statename ?? "-",
                    currency_id = enquiry.currency_id,
                    inputreceivedthru = enquiry.inputreceivedthru,
                    salesresponsibilityid = enquiry.salesresponsibilityid,
                    completeresponsibilityid = enquiry.completeresponsibilityid,
                    quotation_request_lastdate = enquiry.quotation_request_lastdate,
                    createdBy = enquiry.createdBy,
                    createdOn = DateTime.Now,
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
                    ReferenceBy = enquiry.ReferenceBy ?? "",
                    tm = enquiry.tm,
                    vaMech = DefaultNo(enquiry.vaMech),

                    toolLicense = enquiry.toolLicense,          //onsite fields
                    toolId = enquiry.toolId,
                    taskId = enquiry.taskId,
                    expFrom = enquiry.expFrom,
                    expTo = enquiry.expTo,
                    noOfResources = enquiry.noOfResources,
                    tentStartDate = enquiry.tentStartDate,
                    logistics = enquiry.logistics,
                    onsiteDurationType = enquiry.onsiteDurationType,
                    hourlyRateType = enquiry.hourlyRateType,
                    hourlyReate = enquiry.hourlyReate,
                    profReqLastDate = enquiry.profReqLastDate,
                    onsiteDuration = enquiry.onsiteDuration,
                };

                _context.se_enquiry.Add(newEnquiry);
                var result = await _context.SaveChangesAsync();

                if (result > 0)   //  Successful adding
                {

                    ///  Email Trigger code portion
                    var toUsers = !string.IsNullOrEmpty(enquiry.ToMailList)
                                ? JsonConvert.DeserializeObject<List<string>>(enquiry.ToMailList)
                                : new List<string>();

                    var ccUsers = !string.IsNullOrEmpty(enquiry.CCMailList)
                                    ? JsonConvert.DeserializeObject<List<string>>(enquiry.CCMailList)
                                    : new List<string>();

                    // 🔥 Get TO & CC recipients from SalesEnq_Email_Recipients table
                    var recipients = (
                        from r in _context.Email_Recipients
                        join l in _context.Login
                            on r.LoginId equals l.LoginID
                        select new
                        {
                            r.EnqCreated_PositionInEmail,
                            l.EmailID
                        }
                    ).ToList();

                    // Merge DB TO recipients into toUsers list
                    var dbToRecipients = recipients?
                        .Where(r => r.EnqCreated_PositionInEmail == "TO")
                        .Select(r => r.EmailID)
                        .ToList() ?? new List<string>();
                    toUsers.AddRange(dbToRecipients);

                    // Merge DB CC recipients into ccUsers list
                    var dbCcRecipients = recipients?
                        .Where(r => r.EnqCreated_PositionInEmail == "CC")
                        .Select(r => r.EmailID)
                        .ToList() ?? new List<string>();
                    ccUsers.AddRange(dbCcRecipients);

                    // Remove duplicates
                    toUsers = toUsers.Distinct().ToList();
                    ccUsers = ccUsers.Distinct().ToList();

                    var customerAbbrev = GetCustomerAbbreviation(newEnquiry.customer_id ?? 0);
                    var completeRespName = _reusableServices.GetUserName(newEnquiry.completeresponsibilityid);
                    var salesRespName = _reusableServices.GetUserName(newEnquiry.salesresponsibilityid);
                    var customerResult = await CustomerById(newEnquiry.customer_id ?? 0) as OkObjectResult;
                    dynamic customer = customerResult.Value;
                    string customername = customer.Customer;

                    var taskname = _reusableServices.GetHOPCTasks(newEnquiry.taskId);
                    var toolname = _reusableServices.GetStageTools(newEnquiry.toolId);
                    var toolLicenseName = "";
                    if (newEnquiry.toolId == 1)
                    {
                        toolLicenseName = "With License";
                    }
                    {
                        toolLicenseName = "Without License";
                    }
                    string body = "";
                    var subject = $"{newEnquiry.enquiryno} - {customerAbbrev.Result} : New {newEnquiry.enquirytype} Enquiry Added into SEEMS";

                    if (enquiry.enquirytype == "OFFSHORE")
                    {

                        body = $@"
                            Hello Team,
                            <br/>
                            <br/>
                            {enquiry.createdBy} has requested a new enquiry with the following details:
                            <br/>
                            <table border='1' cellspacing='0' cellpadding='6' style='font-family: Arial; font-size: 14px;'>
                            <tr><td><b>Enquiry No</b></td><td>{newEnquiry.enquiryno}</td></tr>
                            <tr><td><b>Customer</b></td><td>{customername}</td></tr>
                            <tr><td><b>Job Name</b></td><td>{newEnquiry.jobnames}</td></tr>
                            <tr><td><b>Complete Responsibility</b></td><td>{completeRespName}</td></tr> 
                            <tr><td><b>Sales Responsibility</b></td><td>{salesRespName}</td></tr>
                            <tr><td><b>Reference By</b></td><td>{newEnquiry.ReferenceBy}</td></tr>
                            <tr><td><b>Quotation Request Last Date</b></td><td>{newEnquiry.quotation_request_lastdate:dd-MMM-yyyy}</td></tr>
                            <tr><td><b>Remarks</b></td><td>{newEnquiry.Remarks}</td></tr>
                            </table>

                            <p>Thank you,<br/><br/>

                            Regards,<br/>
                            SEEMS</p>
                            ";
                    }
                    {
                        //onsite
                        body = $@"
                            Hello Team,
                            <br/>
                            <br/>
                            {enquiry.createdBy} has requested a new enquiry with the following details:
                            <br/>
                            <table border='1' cellspacing='0' cellpadding='6' style='font-family: Arial; font-size: 14px;'>
                            <tr><td><b>Enquiry No</b></td><td>{newEnquiry.enquiryno}</td></tr>
                            <tr><td><b>Customer</b></td><td>{customername}</td></tr>
                            <tr><td><b>Task</b></td><td>{taskname.Result[0].tasktype}</td></tr>
                            <tr><td><b>Complete Responsibility</b></td><td>{completeRespName}</td></tr> 
                            <tr><td><b>Sales Responsibility</b></td><td>{salesRespName}</td></tr>
                            <tr><td><b>Reference By</b></td><td>{newEnquiry.ReferenceBy}</td></tr>
                            <tr><td><b>Profile Request Last Date</b></td><td>{newEnquiry.profReqLastDate:dd-MMM-yyyy}</td></tr>
                            <tr><td><b>Remarks</b></td><td>{newEnquiry.Remarks}</td></tr>
                            <tr><td><b>License</b></td><td>{toolLicenseName}</td></tr>
                            <tr><td><b>Tool</b></td><td>{toolname.Result[0].Tools}</td></tr>
                            <tr><td><b>Year Exp From</b></td><td>{newEnquiry.expFrom}</td></tr>
                            <tr><td><b>Year Exp To</b></td><td>{newEnquiry.expTo}</td></tr>
                            <tr><td><b>No Of Resources</b></td><td>{newEnquiry.noOfResources}</td></tr>
                            </table>

                            <p>Thank you,<br/><br/>

                            Regards,<br/>
                            SEEMS</p>
                            ";

                    }
                    // }
                    // 🚀 Send email with final merged To + CC list
                    await _emailTriggerService.SendEmailAsync(
                        toEmail: string.Join(";", toUsers),
                        subject: subject,
                        body: body,
                        ccEmail: string.Join(";", ccUsers)
                    );
                }
                return Ok(new { message = "Enquiry saved successfully", filePath = savedFilePath });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving enquiry", details = ex.Message });
            }
        }


        [HttpPut("EditEnquiryData")]
        public async Task<IActionResult> EditEnquiryData([FromForm] EnquiryDto enquiry, IFormFile? file)
        {
            try
            {
                var existing = await _context.se_enquiry
                    .FirstOrDefaultAsync(e => e.enquiryno == enquiry.enquiryno);

                if (existing == null)
                    return NotFound(new { message = "Enquiry not found" });

                // 🔹 Update only the editable fields
                existing.customer_id = enquiry.customer_id;
                existing.contact_id = enquiry.contact_id;
                existing.type = enquiry.type;
                existing.statename = enquiry.statename ?? "-";
                existing.currency_id = enquiry.currency_id;
                existing.inputreceivedthru = enquiry.inputreceivedthru;
                existing.salesresponsibilityid = enquiry.salesresponsibilityid;
                existing.completeresponsibilityid = enquiry.completeresponsibilityid;
                existing.quotation_request_lastdate = enquiry.quotation_request_lastdate;
                existing.location_id = enquiry.location_id;

                // 🔹 Update optional section fields
                existing.design = DefaultNo(enquiry.design);
                existing.library = DefaultNo(enquiry.library);
                existing.qacam = DefaultNo(enquiry.qacam);
                existing.dfm = enquiry.dfm ?? "";
                existing.layout_fab = DefaultNo(enquiry.layout_fab);
                existing.layout_testing = DefaultNo(enquiry.layout_testing);
                existing.layout_others = enquiry.layout_others ?? "";
                existing.layoutbyid = enquiry.layoutbyid ?? "";
                existing.dfa = DefaultNo(enquiry.dfa);

                existing.si = DefaultNo(enquiry.si);
                existing.pi = DefaultNo(enquiry.pi);
                existing.emi_net_level = DefaultNo(enquiry.emi_net_level);
                existing.emi_system_level = DefaultNo(enquiry.emi_system_level);
                existing.thermal_board_level = DefaultNo(enquiry.thermal_board_level);
                existing.thermal_system_level = DefaultNo(enquiry.thermal_system_level);
                existing.analysis_others = enquiry.analysis_others ?? "";
                existing.analysisbyid = enquiry.analysisbyid ?? "";

                existing.npi_fab = DefaultNo(enquiry.npi_fab);
                existing.asmb = DefaultNo(enquiry.asmb);
                existing.npi_testing = DefaultNo(enquiry.npi_testing);
                existing.npi_others = enquiry.npi_others ?? "";
                existing.hardware = DefaultNo(enquiry.hardware);
                existing.software = DefaultNo(enquiry.software);
                existing.fpg = DefaultNo(enquiry.fpg);
                existing.VA_Assembly = DefaultNo(enquiry.VA_Assembly);
                existing.DesignOutSource = DefaultNo(enquiry.DesignOutSource);
                existing.npibyid = enquiry.npibyid ?? "";

                existing.NPINew_BOMProc = DefaultNo(enquiry.NPINew_BOMProc);
                existing.NPINew_Fab = DefaultNo(enquiry.NPINew_Fab);
                existing.NPINew_Assbly = DefaultNo(enquiry.NPINew_Assbly);
                existing.NPINew_Testing = DefaultNo(enquiry.NPINew_Testing);
                existing.NPINewbyid = enquiry.NPINewbyid ?? "";
                existing.npinew_jobwork = DefaultNo(enquiry.npinew_jobwork);

                existing.Remarks = enquiry.Remarks ?? "";
                existing.enquirytype = enquiry.enquirytype ?? "";
                existing.tool = enquiry.tool ?? "";
                existing.govt_tender = DefaultNo(enquiry.govt_tender);
                existing.jobnames = enquiry.jobnames ?? "";
                existing.appendreq = enquiry.appendreq ?? "";
                existing.ReferenceBy = enquiry.ReferenceBy ?? "";
                existing.tm = enquiry.tm;
                existing.vaMech = enquiry.vaMech;

                existing.toolLicense = enquiry.toolLicense;        //onsite fields
                existing.toolId = enquiry.toolId;
                existing.taskId = enquiry.taskId;
                existing.expFrom = enquiry.expFrom;
                existing.expTo = enquiry.expTo;
                existing.noOfResources = enquiry.noOfResources;
                existing.tentStartDate = enquiry.tentStartDate;
                existing.logistics = enquiry.logistics;
                existing.onsiteDurationType = enquiry.onsiteDurationType;
                existing.hourlyRateType = enquiry.hourlyRateType;
                existing.hourlyReate = enquiry.hourlyReate;
                existing.profReqLastDate = enquiry.profReqLastDate;

                // 🔹 File upload on Edit (optional)
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

                existing.createdOn = DateTime.Now;
                existing.createdBy = enquiry.createdBy;

                await _context.SaveChangesAsync();

                return Ok(new { message = "Enquiry edited successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error editing enquiry", details = ex.Message });
            }
        }

        // Helper method
        private string DefaultNo(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "NO";

            return value.ToUpper() == "YES" ? "YES" : "NO";
        }

        [HttpGet("EnquiryDetailsByEnquiryno/{enquiryno}")]
        public async Task<IActionResult> EnquiryDetailsByEnquiryno(string enquiryno)
        {
            try
            {
                var enquiry = await _context.se_enquiry
                    .Where(e => e.enquiryno == enquiryno)
                    .ToListAsync();

                if (enquiry == null)
                    return NotFound("No enquiry data found for the selected enquiry.");

                return Ok(enquiry);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching enquiries.",
                    error = ex.Message
                });
            }
        }

        [HttpGet("RptViewEnquiryData")]
        public async Task<List<RptViewEnquiryData>> RptEnquiryData(string? startdate = null, string? enddate = null)
        {
            string sql;

            // If start/end are not provided → call SP without parameters (load all)
            if (string.IsNullOrEmpty(startdate) || string.IsNullOrEmpty(enddate))
            {
                sql = "CALL sp_ViewEnqData(NULL, NULL)";
            }
            else
            {
                sql = $"CALL sp_ViewEnqData('{startdate}', '{enddate}')";
            }

            return await _context.RptViewEnquiryData.FromSqlRaw(sql).ToListAsync();
        }

        [HttpGet("States")]
        public async Task<IActionResult> States()
        {
            try
            {
                var states = await _context.states_ind.OrderBy(s => s.State).Select(s => new { s.State }).ToListAsync();

                if (states == null || !states.Any())
                    return NotFound("No customers found.");

                return Ok(states);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching states.", error = ex.Message });
            }
        }

        [HttpGet("poenquiries")]
        public async Task<IActionResult> poenquiries()
        {
            try
            {
                var poenqs = await _context.poenquiries.Where(p => p.pbalanceamt != "0").ToListAsync();

                if (poenqs == null || !poenqs.Any())
                    return NotFound("No poenquiries found.");

                return Ok(poenqs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching poenquiries.", error = ex.Message });
            }
        }
        [HttpGet("CustomerById")]
        public async Task<IActionResult> CustomerById(long itemno)
        {
            var customerName = await _context.customer
                .Where(c => c.itemno == itemno)
                .Select(c => new { c.itemno, c.Customer })
                .FirstOrDefaultAsync();

            if (customerName == null)
                return NotFound("customerName not found.");

            return Ok(customerName);
        }

        [HttpGet("EnqCustLocContData")]
        public async Task<IActionResult> EnqCustLocContData(string penquiryno)
        {
            try
            {
                var result = (
                from e in _context.se_enquiry
                join c in _context.customer on e.customer_id equals c.itemno
                join sl in _context.se_customer_locations on e.location_id equals sl.location_id
                join sc in _context.se_customer_contacts on e.contact_id equals sc.contact_id
                where e.enquiryno == penquiryno
                select new
                {
                    Customer = c.Customer,
                    Location = sl.location,
                    ContactName = sc.ContactName,
                    Address = sl.address,
                    enquirytype = e.enquirytype
                }
            ).FirstOrDefault();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching EnqCustLocContData.", error = ex.Message });
            }
        }

        [HttpGet("QuoteBoardDescriptions")]
        public async Task<IActionResult> QuoteBoardDescriptions()
        {
            try
            {
                var excludedLayouts = new[]
                {
                    "PCB Layout",
                    "PCBA",
                    "Timing Analysis",
                    "PCB Layout at Sienna ECAD"
                };

                var result = await _context.se_quotlayout
                    .Where(q => !excludedLayouts.Contains(q.layout))
                    .ToListAsync();

                return Ok(result);

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "An error occurred while fetching QuoteBoardDescriptions.", error = ex.Message });
            }

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

        private async Task<string> GetNewQuoteNumberAsync()
        {
            int year = GetCurrentFinancialYear();

            // Get max quote number > 2032
            int maxQuoteNo = await _context.se_quotation
                .Where(q => q.quoteNo != null && q.quoteNo.CompareTo("2032") > 0)
                .Select(q => Convert.ToInt32(q.quoteNo))
                .DefaultIfEmpty(0)
                .MaxAsync();

            string newQuoteNumber;

            if (maxQuoteNo < (year * 10000))
            {
                newQuoteNumber = $"{year}0001";
            }
            else
            {
                newQuoteNumber = (maxQuoteNo + 1).ToString();
            }

            return newQuoteNumber;
        }

        [HttpPost("AddQuotation")]
        public async Task<IActionResult> AddQuotation([FromBody] QuotationDto dto)

        {
            //   using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var quoteNo = await GetNewQuoteNumberAsync();

                // 🔹 1. Insert into se_quotation (HEADER)
                var quotation = new se_quotation
                {
                    quoteNo = quoteNo,
                    board_ref = dto.board_ref,
                    enquiryno = dto.enquiryno,
                    createdBy = dto.createdBy,
                    versionNo = dto.versionNo,
                    tandc = dto.tandc,
                };

                _context.se_quotation.Add(quotation);
                await _context.SaveChangesAsync();

                // 🔹 2. Insert into se_quotation_items (LINE ITEMS)
                var items = dto.Items.Select(i => new se_quotation_items
                {
                    quoteNo = quoteNo,
                    layout = i.layout,
                    quantity = i.quantity,
                    unit_rate = i.unit_rate,
                    currency_id = i.currency_id,
                    created_on = DateTime.Now.ToString("yyyy-MM-dd"),
                    updatedbyid =  i.updatedbyid,
                    versionNo = i.versionNo,
                    durationtype = i.durationtype,

                }).ToList();

                _context.se_quotation_items.AddRange(items);

                return Ok(new
                {
                    message = "Quotation saved successfully",
                    quoteNo = dto.quoteNo
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
   
       [HttpGet("QuoteDetailsByQuoteNo/{penquiryno}/{pquoteno}")]
        public async Task<IActionResult> QuoteDetailsByQuoteNo(string penquiryno,string pquoteno)
        {
            try
            {
                var quotation = await _context.se_quotation.Where(s => s.enquiryno == penquiryno && s.quoteNo == pquoteno)
                    .ToListAsync();

                if (quotation == null)
                    return NotFound("No quotation data found for the selected quotation.");

                return Ok(quotation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An error occurred while fetching quotation.",
                    error = ex.Message
                });
            }
        }
    }
}