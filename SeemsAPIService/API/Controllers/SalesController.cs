using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using System.Data;

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

       // [HttpGet("GetThreeMonthConfirmedOrders/{startdate}/{enddate}")]
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
            List<PendingInvoices> list = new();

            // Call your stored procedure
            string sql = $"CALL sp_PendingInvoices('{costcenter}')";

            list = await _context.PendingInvoices.FromSqlRaw(sql).ToListAsync();

            return list;
        }

    }
}
