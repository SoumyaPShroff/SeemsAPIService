using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.DTOs;
using SeemsAPIService.Application.Interfaces;

namespace SeemsAPIService.Infrastructure.Persistence.Repository
{
    public class UserAccessRepository : IUserAccessRepository
    {
        private readonly AppDbContext _context;

        public UserAccessRepository(AppDbContext context)
        {
            _context = context;
        }

        // ---------- UserRoleInternalRights ----------
        public async Task<bool> HasRoleAccessAsync(string role, string pageName)
        {
            var roleRecord = await _context.employeeroles
                .FirstOrDefaultAsync(e => e.Roles == role.Trim());

            if (roleRecord == null) return false;

            var property = roleRecord.GetType().GetProperty(
                pageName,
                System.Reflection.BindingFlags.IgnoreCase |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance);

            if (property == null) return false;

            var value = property.GetValue(roleRecord);
            return Convert.ToInt32(value ?? 0) == 1;
        }

        // ---------- UserDesignation ----------
        public async Task<string?> GetUserDesignationAsync(string loginId)
        {
            return await _context.general_employee
                .Where(g => g.IDno == loginId && g.EmpStatus == "Active")
                .Select(g => g.JobTitle)
                .FirstOrDefaultAsync();
        }

        // ---------- AllActiveEmployees ----------
        public async Task<List<EmployeeBasicDto>> GetAllActiveEmployeesAsync()
        {
            return await _context.general_employee
                .Where(e => e.EmpStatus == "Active")
                .Select(e => new EmployeeBasicDto
                {
                    iDno = e.IDno,
                    name = e.Name,
                    emailId = e.EmailId
                })
                .ToListAsync();
        }

        // ---------- AnalysisManagers ----------
        public async Task<List<ManagerDto>> GetAnalysisManagersAsync()
        {
            return await (
                from s in _context.setting_employee
                join l in _context.Login
                    on s.HOPC1ID equals l.LoginID
                where s.costcenter_analysis == "YES"
                   && s.costcenter_status == "Active"
                select new ManagerDto
                {
                    HOPC1ID = s.HOPC1ID,
                    HOPC1NAME = s.HOPC1NAME,
                    emailID = l.EmailID
                }
            ).ToListAsync();
        }

        // ---------- DesignManagers ----------
        public async Task<List<ManagerDto>> GetDesignManagersAsync()
        {
            return await (
                from s in _context.setting_employee
                join l in _context.Login
                    on s.HOPC1ID equals l.LoginID
                where s.design == "YES"
                   && s.costcenter_status == "Active"
                select new ManagerDto
                {
                    HOPC1ID = s.HOPC1ID,
                    HOPC1NAME = s.HOPC1NAME,
                    emailID = l.EmailID
                }
            ).ToListAsync();
        }
    }
}
