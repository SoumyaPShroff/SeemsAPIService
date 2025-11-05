#nullable disable
using Microsoft.EntityFrameworkCore;

namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class general_employee
    {
        public string TeamDescription { get; set; }
        public string Age { get; set; }
        public string CostCenter_old { get; set; }
        public string costcenter { get; set; }
        public string Functional { get; set; }
        public string JobFamily { get; set; }
        public string JobTitle { get; set; }
        public string Gender { get; set; }
        public string BloodGroup { get; set; }
        public string ExtNumber { get; set; }
        public string Sysno { get; set; }
        public string IDno { get; set; }
        public string Name { get; set; }
        public string DOB { get; set; }
        public string CAddr { get; set; }
        public string PAddr { get; set; }
        public string Phone { get; set; }
        public string Cellnumber { get; set; }
        public string EmailId { get; set; }
        public string Pasportno { get; set; }
        public string Pancardno { get; set; }
        public string ReportTo { get; set; }
        public string ReportToPerson { get; set; }
        public byte[] Image { get; set; }
        public string imagename { get; set; }
        public string addedby { get; set; }
        public string date { get; set; }
        public string priority { get; set; }
        public string ReportToPersonID { get; set; }
        public string pafstatus { get; set; }
        public string Photo { get; set; }
        public string rating { get; set; }
        public string imagepath { get; set; }
        public string location { get; set; }
        public string Designation { get; set; }
        public string EmpStatus { get; set; }
    }
}
