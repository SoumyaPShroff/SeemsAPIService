using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeemsAPIService.Infrastructure.Repositories
{
    public class EmailRecipientService : IEmailRecipientService
    {
        private readonly AppDbContext _context;

        public EmailRecipientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<string> To, List<string> Cc)> GetRecipientsAsync(string type)
        {
            // If later you add multiple email types, you can filter by `type` here

            var recipients = await (
                from r in _context.Email_Recipients.AsNoTracking()
                join l in _context.Login.AsNoTracking()
                    on r.LoginId equals l.LoginID
                select new
                {
                    r.EnqCreated_PositionInEmail,
                    l.EmailID
                }
            ).ToListAsync();

            var to = recipients
                .Where(r => r.EnqCreated_PositionInEmail == "TO")
                .Select(r => r.EmailID)
                .Distinct()
                .ToList();

            var cc = recipients
                .Where(r => r.EnqCreated_PositionInEmail == "CC")
                .Select(r => r.EmailID)
                .Distinct()
                .ToList();

            return (to, cc);
        }
    }
}
