using Microsoft.EntityFrameworkCore;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class PendingInvoices
    {
        public string JobNumber { get; set; }
        public string StartDate { get; set; }
        public string Enddate { get; set; }
        public string CostCenter { get; set; }
        public string ProjectManager { get; set; }
        public string Status { get; set; }
        public string Flag_Raisedon { get; set; }
        public double? TotTimesheetHrs { get; set; }
        public double? ApprovedHrs { get; set; }
        public double? Rateperhour { get; set; }
        public string PoDate { get; set; }
        public string PONumber { get; set; }
        public double? UnBilledAmount { get; set; }
        public string EnquiryNo { get; set; }
        public string EnquiryType { get; set; }
        public string Type { get; set; }
        public string govt_tender { get; set; }
        public double? POAmount { get; set; }
    }
}
