using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Domain.Entities;
using SeemsAPIService.Infrastructure.Persistence;
using System.Xml.Linq;

namespace SeemsAPIService.Application.Services
{
    public interface IReusableServices
    {
        string GetUserName(string loginId);
        Task<List<se_stages_tools>> GetStageTools(long? pToolId);
        Task<List<setting_employee>> GetHOPCTasks(long? pTaskId);
    }
    public class ReusableServices : IReusableServices
    {
        private readonly AppDbContext _context;

        public ReusableServices(AppDbContext context)
        {
            _context = context;
        }

        public string GetUserName(string loginId)
        {
            if (string.IsNullOrWhiteSpace(loginId))
                return string.Empty;

            var name = (
               from login in _context.Login
               join emp in _context.general_employee
                  on login.LoginID equals emp.IDno
               where login.LoginID == loginId && emp.EmpStatus == "Active"
               select emp.Name
            ).FirstOrDefault();
            return name?.Trim() ?? string.Empty;

        }
        public async Task<List<se_stages_tools>> GetStageTools(long? pToolId)
        {
            var stageTools = await _context.se_stages_tools
                .Where(s => s.condition_tool == 1 &&
                       (!pToolId.HasValue || s.Idno == pToolId.Value))
                .OrderBy(s => s.Tools)
                .ToListAsync();

            return stageTools;
        }
        public async Task<List<setting_employee>> GetHOPCTasks(long? pTaskId)
        {
            var hopcTasks = await _context.setting_employee
                .Where(s => s.condition_task == 1 &&
                            s.tasktype != null &&
                            (!pTaskId.HasValue || s.itemnumber == pTaskId.Value))
                .Select(s => new setting_employee
                {
                    itemnumber = s.itemnumber,
                    tasktype = s.tasktype
                })
                .OrderBy(s => s.tasktype)
                .ToListAsync();

            return hopcTasks;
        }
    }
}
 