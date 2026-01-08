using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
namespace SeemsAPIService.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<Login?> GetUserAsync(string loginId) =>
            _context.Login.FirstOrDefaultAsync(x => x.LoginID == loginId);

        public async Task UpdatePasswordAsync(string loginId, string hash)
        {
            var user = await _context.Login.FirstOrDefaultAsync(x => x.LoginID == loginId);
            if (user == null) throw new Exception("User not found");

            user.Password = hash;
            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetEmailByLoginIdAsync(string loginId)
        {
            return await _context.Login
                .Where(x => x.LoginID == loginId)
                .Select(x => x.EmailID)
                .FirstOrDefaultAsync();
        }
    }
}