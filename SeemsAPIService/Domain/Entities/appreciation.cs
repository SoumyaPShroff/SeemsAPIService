using System;
using System.Collections.Generic;

#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    public class Appreciation
    {
        public uint slno { get; set; }
        public string jobno { get; set; }
        public string jobname { get; set; }
        public string customer { get; set; }
        public string month { get; set; }
        public string AppreciationType { get; set; }
        public string ProjectManager { get; set; }
        public string ProjectDesignedBy { get; set; }
        public string QCdoneBy { get; set; }
        public string AppreciationRemarks { get; set; }
        public string status { get; set; }
        public string updatedbyname { get; set; }
        public string updatedbyid { get; set; }
        public string appreciationid { get; set; }
        public DateTime? date { get; set; }
        public string updateddate { get; set; }
        public string approveddate { get; set; }
        public string projectmanagerid { get; set; }
        public string projectdesignedbyid { get; set; }
        public string qcdonebyid { get; set; }
        public string approvedbyname { get; set; }
        public string approvedbyid { get; set; }
        public string resp_name { get; set; }
        public string resp_id { get; set; }
        public string originators { get; set; }
        public string originatorsid { get; set; }
        public string vendors { get; set; }
        public string appreciationaddeddate { get; set; }
        public DateTime? projectcompletedon { get; set; }
        public string appreciationacceptedby { get; set; }
        public string appreciationacceptedbyid { get; set; }
        public string appreciation_Accepted_date { get; set; }
        public string Appreciationlevel { get; set; }
        public string Approved { get; set; }
        public string approvedbydate { get; set; }
        public string app_remarksapp { get; set; }
        public string customer_feedback { get; set; }
        public string appreciation_rejectedby { get; set; }
        public string appreciation_rejectedbyid { get; set; }
        public string appreciation_rejecteddate { get; set; }
        public string appreciation_rejected_remarks { get; set; }
        public string appreciation_revertedby { get; set; }
        public string appreciation_revertedbyid { get; set; }
        public string appreciation_reverteddate { get; set; }
        public string revertedcustomer_remarks { get; set; }
        public string Filename { get; set; }
    }
}
