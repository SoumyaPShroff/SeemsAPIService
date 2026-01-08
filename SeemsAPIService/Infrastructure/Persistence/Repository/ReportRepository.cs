using SeemsAPIService.Application.DTOs.Reports;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Infrastructure.Persistence;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
namespace SeemsAPIService.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TentativeQuotedOrdersResult> GetTentativeQuotedOrdersAsync()
        {
            using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "sp_TentativeQuotedOrders";
            command.CommandType = CommandType.StoredProcedure;

            var result = new TentativeQuotedOrdersResult();

            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                result.TentativeOrders.Add(ReadRow(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                result.QuotedOrders.Add(ReadRow(reader));

            return result;
        }

        public async Task<OpenConfirmedOrdersResult> GetOpenConfirmedOrdersAsync()
        {
            await using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "CALL sp_OpenConfirmedOrders()";
            command.CommandType = CommandType.Text;

            var result = new OpenConfirmedOrdersResult();

            await using var reader = await command.ExecuteReaderAsync();

            int resultSetIndex = 0;
            do
            {
                while (await reader.ReadAsync())
                {
                    var row = ReadRow(reader);

                    if (resultSetIndex == 0)
                        result.OpenOrders.Add(row);
                    else if (resultSetIndex == 1)
                        result.ConfirmedOrders.Add(row);
                }
                resultSetIndex++;
            }
            while (await reader.NextResultAsync());

            return result;
        }

        private Dictionary<string, object?> ReadRow(DbDataReader reader)
        {
            var row = new Dictionary<string, object?>();

            for (int i = 0; i < reader.FieldCount; i++)
                row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);

            return row;
        }
    }

}