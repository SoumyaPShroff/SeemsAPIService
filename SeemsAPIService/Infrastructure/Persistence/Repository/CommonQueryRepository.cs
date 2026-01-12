using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using SeemsAPIService.Application.DTOs;
namespace SeemsAPIService.Infrastructure.Repositories
{

    public class CommonQueryRepository : ICommonQueryRepository
    {
        private readonly AppDbContext _context;

        public CommonQueryRepository(AppDbContext context)
        {
            _context = context;
        }

        // -------- SalesManagers --------
        public async Task<List<SalesManagerDto>> GetSalesManagersAsync()
        {
            var settingList = await (
                from s in _context.setting_employee
                join l in _context.Login on s.HOPC1ID equals l.LoginID
                where s.costcenter_sales == "YES"
                   && s.costcenter_status == "Active"
                select new SalesManagerDto
                {
                    id = s.HOPC1ID,
                    name = s.HOPC1NAME,
                    emailID = l.EmailID
                }).ToListAsync();

            var generalList = await (
                from g in _context.general_employee
                join l in _context.Login on g.IDno equals l.LoginID
                where g.Functional == "Selling"
                   && g.JobTitle.Contains("Sales")
                   && g.EmpStatus == "Active"
                select new SalesManagerDto
                {
                    id = g.IDno,
                    name = g.Name,
                    emailID = l.EmailID
                }).ToListAsync();

            return settingList
                .Concat(generalList)
                .GroupBy(x => x.id)
                .Select(g => g.First())
                .OrderBy(x => x.name)
                .ToList();
        }

        // -------- salesnpiusers --------
        public async Task<List<SalesNpiUserDto>> GetSalesNpiUsersAsync()
        {
            return await (
                from s in _context.view_salesnpiusers
                join l in _context.Login on s.IDno equals l.LoginID
                select new SalesNpiUserDto
                {
                    iDno = s.IDno,
                    name = s.Name,
                    EmailId = l.EmailID
                }).ToListAsync();
        }

        // -------- SalesEnq_Email_Recipients --------
        public async Task<List<SalesEnqRecipientDto>> GetSalesEnqRecipientsAsync()
        {
            return await (
                from a in _context.Email_Recipients
                join b in _context.Login on a.LoginId equals b.LoginID into g
                from b in g.DefaultIfEmpty()
                where a.Design == "1"
                select new SalesEnqRecipientDto
                {
                    LoginId = a.LoginId,
                    EnqCreatedPositionInEmail = a.EnqCreated_PositionInEmail,
                    Email = b != null ? b.EmailID : null
                }).ToListAsync();
        }

        // -------- SideBarAccessMenus --------
        public async Task<List<SidebarAccessMenus>> GetSideBarAccessMenusAsync(int designationId)
        {
            return await _context.SidebarAccessMenus
                .FromSqlRaw("CALL sp_GetSidebarAccess(@p0)", designationId)
                .ToListAsync();
        }

        // -------- RoleDesignID --------
        public async Task<long> GetRoleDesignIdAsync(string designationName)
        {
            return await _context.roledesignations
                .Where(r => r.designation == designationName)
                .Select(r => r.designationid)
                .FirstOrDefaultAsync();
        }
    }
}