using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Interfaces;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
namespace SeemsAPIService.Infrastructure.Repositories
{
    public class ReusableRepository : IReusableRepository
    {
        private readonly AppDbContext _context;

        public ReusableRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetUserNameAsync(string loginId)
        {
            var name = await (
                from login in _context.Login
                join emp in _context.general_employee
                    on login.LoginID equals emp.IDno
                where login.LoginID == loginId
                      && emp.EmpStatus == "Active"
                select emp.Name
            ).FirstOrDefaultAsync();

            return name?.Trim() ?? string.Empty;
        }

        public async Task<List<se_stages_tools>> GetStageToolsAsync(long? toolId)
        {
            return await _context.se_stages_tools
                .Where(s => s.condition_tool == 1 &&
                       (!toolId.HasValue || s.Idno == toolId.Value))
                .OrderBy(s => s.Tools)
                .ToListAsync();
        }

        public async Task<List<setting_employee>> GetHOPCTasksAsync(long? taskId)
        {
            return await _context.setting_employee
                .Where(s => s.condition_task == 1 &&
                            s.tasktype != null &&
                            (!taskId.HasValue || s.itemnumber == taskId.Value))
                .Select(s => new setting_employee
                {
                    itemnumber = s.itemnumber,
                    tasktype = s.tasktype
                })
                .OrderBy(s => s.tasktype)
                .ToListAsync();
        }
    }
}