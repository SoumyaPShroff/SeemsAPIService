using Microsoft.EntityFrameworkCore;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class BillingPlannerRpt
    {
        public string JobNumber { get; set; }
        public string Customer { get; set; }
        public string StartDate { get; set; }
        public string PlannedEndDate { get; set; }
        public double? TotalHrs { get; set; }
        public double? PlannedHrs { get; set; }
        public double? BilHrs_CurrentMonth { get; set; }
        public double? BillPerctg_CurMonth { get; set; }
        public double? ProjectComp_Perc { get; set; }
        public string UpdatedByPrevDay { get; set; }
        public double? BillableECOHrs { get; set; }
        public double? ECO { get; set; }
        public double? BilHrsPrevDay { get; set; }
        public double? WIPAmount { get; set; }
        public string EnqType { get; set; }
        public string Enquiryno { get; set; }
        public string govt_tender { get; set; }
        public double? EstimatedHours { get; set; }
        public string PONumber { get; set; }
        public double? HourlyRate { get; set; }
        public string PoRcvd { get; set; }
        public double? PoAmount { get; set; }
        public string BillingType { get; set; }
        public string ExpectedDeliveryDate { get; set; }
        public string ActualEndDate { get; set; }
        public double? NonBillableHrs { get; set; }
        public double? TotalBillableHrs { get; set; }
        public string FlagRaisedOn { get; set; }
        public double? TotalInvoicedHrs { get; set; }
        public double? TotalInvoicedAmt { get; set; }
        public string type { get; set; }
        public string CostCenter { get; set; }
        public string ProjectManager { get; set; }
        public string SalesManager { get; set; }
        public string jobtitle { get; set; }
        public double? RejectedHrs { get; set; }
        public string projectmanagerid { get; set; }
        public string PODate { get; set; }
        public string RealisedDate { get; set; }
    }
}
