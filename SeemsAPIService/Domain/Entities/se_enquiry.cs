using Microsoft.AspNetCore.Http.HttpResults;
using SeemsAPIService.Application.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace SeemsAPIService.Domain.Entities
{
    public class se_enquiry
    {
        [Key]
        public string enquiryno { get; set; }
        public Int64 customer_id { get; set; }
        public Int64 location_id { get; set; }
        public Int64 contact_id { get; set; }
        public Int64 currency_id { get; set; }
        public string inputreceivedthru { get; set; }
        public string design { get; set; } = "NO";
        public string library { get; set; } = "NO";
        public string qacam { get; set; } = "NO";
        public string dfm { get; set; } = "NO";
        public string layout_fab { get; set; } = "NO";
        public string layout_testing { get; set; } = "NO";
        public string layout_others { get; set; } = "NO";
        public string layoutbyid { get; set; }  
        public string si { get; set; } = "NO";
        public string pi { get; set; } = "NO";
        public string emi_net_level { get; set; }
        public string emi_system_level { get; set; } = "NO";
        public string thermal_board_level { get; set; } = "NO";
        public string thermal_system_level { get; set; } = "NO";
        public string analysis_others { get; set; } = "NO";
        public string analysisbyid { get; set; }
        public string npi_fab { get; set; } = "NO";
        public string asmb { get; set; } = "NO";
        public string npi_testing { get; set; } = "NO";
        public string npi_others { get; set; } = "NO";
        public string hardware { get; set; } = "NO";
        public string software { get; set; } = "NO";
        public string fpg { get; set; } = "NO";

        public string npibyid { get; set; }
        public string quotation_request_lastdate { get; set; } 
        public string govt_tender { get; set; } = "NO";
        public string completeresponsibilityid { get; set; }
        public string salesresponsibilityid { get; set; }
        public string status { get; set; }
        public string Remarks { get; set; }
        public string uploadedfilename { get; set; }
        public string createdBy { get; set; }
        public string createdOn { get; set; }
        //public string layout { get; set; } = "NO";
        public string enquirytype { get; set; }
        public string tool { get; set; }
        public string jobnames { get; set; }
        public string appendreq { get; set; }
        public string type { get; set; }
        public string statename { get; set; }
        public string dfa { get; set; }
        public string VA_Assembly { get; set; } = "NO";
        public string DesignOutSource { get; set; } = "NO";
        public string NPINew_BOMProc { get; set; } = "NO";
        public string NPINew_Fab { get; set; } = "NO";
        public string NPINew_Assbly { get; set; } = "NO";
        public string NPINew_Testing { get; set; } = "NO";
        public string NPINewbyid { get; set; }
        public string npinew_jobwork { get; set; } = "NO";
        public string ReferenceBy { get; set; }
    }
}
