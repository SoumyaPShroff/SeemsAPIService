using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using System.Data;
using System.Reflection.Metadata.Ecma335;

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

        //[HttpPost("AddEnquiryData")]
        //public async Task<IActionResult> AddEnquiryData([FromForm] AddEnquiryData dto, IFormFile file)
        //{
        //    if (file != null && file.Length > 0)
        //    {
        //        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");
        //        if (!Directory.Exists(uploadPath))
        //            Directory.CreateDirectory(uploadPath);

        //        var filePath = Path.Combine(uploadPath, Path.GetFileName(file.FileName));
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        dto.AttachmentPath = filePath; // store path in DB
        //    }

        //    // Save enquiry to database...
        //    // await _context.Enquiries.AddAsync(dto);
        //    // await _context.SaveChangesAsync();

        //    return Ok(new { message = "Enquiry added successfully." });
        //}
        
  [HttpGet("Customers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var customers = await _context.customer.OrderBy(c=>c.Customer).Select(c => new {c.itemno,c.Customer}).ToListAsync();

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

    }
}
