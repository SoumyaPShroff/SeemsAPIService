using Microsoft.AspNetCore.Mvc;
using SeemsAPIService.Infrastructure.Persistence;
using System.Xml.Linq;

namespace SeemsAPIService.Application.Services
{
    public interface IReusableServices
    {
        string GetUserName(string loginId);
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
    }
}