using Microsoft.EntityFrameworkCore;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class Complaint
    {
        public uint slno { get; set; }
        public string jobno { get; set; }
        public string jobname { get; set; }
        public string customer { get; set; }
        public string month { get; set; }
        public string ComplaintType { get; set; }
        public string ProjectManager { get; set; }
        public string ProjectDesignedBy { get; set; }
        public string QCdoneBy { get; set; }
        public string ComplaintRemarks { get; set; }
        public string status { get; set; }
        public string updatedbyname { get; set; }
        public string updatedbyid { get; set; }
        public string Complaintid { get; set; }
        public DateTime? date { get; set; }
        public string updatedbydate { get; set; }
        public string root_cause { get; set; }
        public string corrective_acction { get; set; }
        public string preventive_acction { get; set; }
        public string accupdatedbyname { get; set; }
        public string accupdatedbyid { get; set; }
        public string accupdatedbydate { get; set; }
        public string Complaintaddeddate { get; set; }
        public string projectmanagerid { get; set; }
        public string projectdesignedbyid { get; set; }
        public string qcdonebyid { get; set; }
        public string approvedbyname { get; set; }
        public string approvedbydate { get; set; }
        public string approvedbyid { get; set; }
        public string PEMonth { get; set; }
        public string Severity { get; set; }
        public string Reworkcost { get; set; }
        public string Approved { get; set; }
        public string Resp_id { get; set; }
        public string Resp_name { get; set; }
        public string Originators { get; set; }
        public string originatorsid { get; set; }
        public DateTime? CAR_Responsedate { get; set; }
        public string Defecttype { get; set; }
        public string vendorcategory1 { get; set; }
        public string vendorcategory2 { get; set; }
        public string vendorcategory3 { get; set; }
        public string vendorcategory4 { get; set; }
        public string vendorcategory5 { get; set; }
        public string vendorcategory6 { get; set; }
        public string vendorcategory7 { get; set; }
        public string Fabrication_vendor { get; set; }
        public string complaintacceptedby { get; set; }
        public string complaintacceptedbyid { get; set; }
        public string Internal_Members { get; set; }
        public string ExtEm_name1 { get; set; }
        public string ExtEm_email1 { get; set; }
        public string ExtEm_phoneno1 { get; set; }
        public string ExtEm_cmpny1 { get; set; }
        public string ExtEm_designation1 { get; set; }
        public string ExtEm_name2 { get; set; }
        public string ExtEm_email2 { get; set; }
        public string ExtEm_phoneno2 { get; set; }
        public string ExtEm_cmpny2 { get; set; }
        public string ExtEm_designation2 { get; set; }
        public string ExtEm_name3 { get; set; }
        public string ExtEm_email3 { get; set; }
        public string ExtEm_phoneno3 { get; set; }
        public string ExtEm_cmpny3 { get; set; }
        public string ExtEm_designation3 { get; set; }
        public string problem_description { get; set; }
        public string Correction { get; set; }
        public string Effectivenessofcorrectiveaction { get; set; }
        public string Congragulate_team { get; set; }
        public string Internal_Members_Names { get; set; }
        public DateTime? projectcompletedon { get; set; }
        public string vendors { get; set; }
        public string complaint_Accepted_date { get; set; }
        public string comp_remarksbyapprovedperson { get; set; }
        public string complaint_rejectedby { get; set; }
        public string complaint_rejectedbyid { get; set; }
        public string complaintrejecteddate { get; set; }
        public string complaint_rejected_remarks { get; set; }
        public string monitoring_time { get; set; }
        public string severityaddedby { get; set; }
        public string severityaddedbyid { get; set; }
        public string severityaddeddate { get; set; }
        public string severityaddedremarks { get; set; }
        public string _8dreport_updatedby { get; set; }
        public string _8dreport_updatedbyid { get; set; }
        public string _8dreport_updateddate { get; set; }
        public string FilePath { get; set; }
    }
}
