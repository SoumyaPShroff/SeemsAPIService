using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
namespace SeemsAPIService.Infrastructure.Repositories
{
    public class UserQueryRepository : IUserQueryRepository
    {
        private readonly AppDbContext _context;

        public UserQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---------------- HOPCManagerList ----------------
        public async Task<List<HOPCManagerList>> GetHOPCManagerListAsync()
        {
            string sql = "CALL sp_HOPCManagers()";
            return await _context.HOPCManagerList
                .FromSqlRaw(sql)
                .ToListAsync();
        }

        // ---------------- ManagerCostcenterInfo ----------------
        public async Task<List<ManagerCostCenterDto>> GetManagerCostcenterInfoAsync(string loginId)
        {
            return await _context.setting_employee
                .Where(l => l.HOPC1ID == loginId)
                .Select(l => new ManagerCostCenterDto
                {
                    Hopc1Id = l.HOPC1ID,
                    Hopc1Name = l.HOPC1NAME,
                    CostCenter = l.costcenter
                })
                .ToListAsync();
        }

        // ---------------- GetEmailIDs ----------------
        public async Task<List<string?>> GetEmailIDsAsync(List<string> loginIds)
        {
            return await (
                from login in _context.Login
                join emp in _context.general_employee
                    on login.LoginID equals emp.IDno
                where loginIds.Contains(login.LoginID)
                      && emp.EmpStatus == "Active"
                select login.EmailID
            )
            .Distinct()
            .ToListAsync();
        }
    }
}