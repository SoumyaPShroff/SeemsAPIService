using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Application.Services;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _service;
    private readonly IReportRepository _reportservice;

    public SalesController(ISalesService service, IReportRepository repservice)
    {
        _service = service;
        _reportservice = repservice;
    }


    [HttpGet("ThreeMonthConfirmedOrders/{startdate}/{enddate}")]
    public async Task<IActionResult> ThreeMonthConfirmedOrders(string startdate, string enddate)
    {

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

        var result = await _service.GetThreeMonthConfirmedOrdersAsync(formattedStart, formattedEnd);
        return Ok(result);
    }

    [HttpGet("TentativeQuotedOrders")]
    public async Task<IActionResult> GetTentativeQuotedOrders()
    {
        var data = await _reportservice.GetTentativeQuotedOrdersAsync();
        return Ok(data);
    }

    [HttpGet("OpenConfirmedOrders")]
    public async Task<IActionResult> OpenConfirmedOrders()
    {
        var data = await _reportservice.GetOpenConfirmedOrdersAsync();
        return Ok(data);
    }


    [HttpPost("AddEnquiryData")]
    public async Task<IActionResult> AddEnquiry([FromForm] EnquiryDto dto, IFormFile? file)
    {
        var result = await _service.CreateEnquiryAsync(dto, file);
        return Ok(result);
    }

    [HttpPut("EditEnquiryData")]
    public async Task<IActionResult> EditEnquiry([FromForm] EnquiryDto dto, IFormFile? file)
    {
        await _service.UpdateEnquiryAsync(dto, file);
        return Ok(new { message = "Enquiry updated successfully" });
    }

    [HttpGet("EnquiryDetailsByEnquiryno/{enquiryno}")]
    public async Task<IActionResult> GetByEnquiryNo(string enquiryno)
    {
        var data = await _service.GetEnquiryByNumberAsync(enquiryno);
        return Ok(data);

    }
        [HttpGet("AllEnquiries")]
        public async Task<IActionResult> AllEnquiries([FromQuery] string? salesResponsibilityId, [FromQuery] string? status)
        {
            var result = await _service.GetAllEnquiriesAsync(salesResponsibilityId, status);
            return Ok(result);
        }

        [HttpGet("Customers")]
        public async Task<IActionResult> Customers()
        {
            var result = await _service.GetCustomersAsync();
            return Ok(result);
        }

        [HttpGet("customerlocations")]
        public async Task<IActionResult> CustomerLocations([FromQuery] int? customerId)
        {
            var result = await _service.GetCustomerLocationsAsync(customerId);
            return Ok(result);
        }

        [HttpGet("customercontacts")]
        public async Task<IActionResult> CustomerContacts([FromQuery] int? customerId, [FromQuery] int? locationId)
        {
            var result = await _service.GetCustomerContactsAsync(customerId, locationId);
            return Ok(result);
        }

        [HttpGet("customerabbreviation")]
        public async Task<IActionResult> GetCustomerAbbreviation([FromQuery] long? itemno)
        {
            var result = await _service.GetCustomerAbbreviationAsync(itemno.Value);
            if (result == null)
                return NotFound("No customer abbreviation found.");

            return Ok(result);
        }


        [HttpGet("RptViewEnquiryData/{startdate}/{enddate}")]
        public async Task<IActionResult> RptViewEnquiryData(string startdate, string enddate)
        {
            var result = await _service.GetRptViewEnquiryDataAsync(startdate, enddate);
            return Ok(result);
        }

        [HttpGet("States")]
        public async Task<IActionResult> States()
        {
            var result = await _service.GetStatesAsync();
            return Ok(result);
        }

        [HttpGet("poenquiries")]
        public async Task<IActionResult> PoEnquiries()
        {
            var result = await _service.GetPoEnquiriesAsync();
            return Ok(result);
        }

        [HttpGet("CustomerById")]
        public async Task<IActionResult> CustomerById([FromQuery] long itemno)
        {
            var result = await _service.GetCustomerByIdAsync(itemno);
            return Ok(result);
        }

        [HttpGet("EnqCustLocContData")]
        public async Task<IActionResult> EnqCustLocContData([FromQuery] string penquiryno)
        {
            var result = await _service.GetEnqCustLocContDataAsync(penquiryno);
            return Ok(result);
        }
   
        [HttpGet("PendingInvoices/{costcenter}")]
        public async Task<IActionResult> PendingInvoices(string costcenter)
        {
            var result = await _service.PendingInvoicesAsync(costcenter);
            return Ok(result);
        }

    //[HttpPost("AddQuotation")]
    //public async Task<IActionResult> AddQuotation([FromBody] QuotationDto dto)
    //{
    //    var result = await _service.AddQuotationAsync(dto);
    //    return Ok(result);
    //}
    //}

    //[HttpGet("QuoteBoardDescriptions")]
    //public async Task<IActionResult> QuoteBoardDescriptions()
    //{
    //    var result = await _service.GetQuoteBoardDescriptionsAsync();
    //    return Ok(result);
    //}

    //[HttpGet("QuoteDetailsByQuoteNo")]
    //public async Task<IActionResult> QuoteDetailsByQuoteNo([FromQuery] string quoteno)
    //{
    //    var result = await _service.GetQuoteDetailsByQuoteNoAsync(quoteno);
    //    return Ok(result);
    //}

    //[HttpDelete("DeleteAllQuotationDetails")]
    //public async Task<IActionResult> DeleteAllQuotationDetails([FromQuery] string quoteno)
    //{
    //    var result = await _service.DeleteAllQuotationDetailsAsync(quoteno);
    //    return Ok(result);
    //}
}