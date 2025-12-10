using System;
using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Domain.Entities
{
    public class se_enquiry
    {
        [Key]
        //public string enquiryno { get; set; }
        public string? enquiryno { get; set; }
        public long? customer_id { get; set; }
        public long? location_id { get; set; }
        public long? contact_id { get; set; }
        public string? currency_id { get; set; }
        public string? inputreceivedthru { get; set; }
        public string? design { get; set; } = "NO";
        public string? library { get; set; } = "NO";
        public string? qacam { get; set; } = "NO";
        public string? dfm { get; set; } = "NO";
        public string? layout_fab { get; set; } = "NO";
        public string? layout_testing { get; set; } = "NO";
        public string? layout_others { get; set; } = "NO";
        public string? layoutbyid { get; set; }
        public string? si { get; set; } = "NO";
        public string? pi { get; set; } = "NO";
        public string? emi_net_level { get; set; }
        public string? emi_system_level { get; set; } = "NO";
        public string? thermal_board_level { get; set; } = "NO";
        public string? thermal_system_level { get; set; } = "NO";
        public string? analysis_others { get; set; } = "NO";
        public string? analysisbyid { get; set; }
        public string? npi_fab { get; set; } = "NO";
        public string? asmb { get; set; } = "NO";
        public string? npi_testing { get; set; } = "NO";
        public string? npi_others { get; set; } = "NO";
        public string? hardware { get; set; } = "NO";
        public string? software { get; set; } = "NO";
        public string? fpg { get; set; } = "NO";
        public string? npibyid { get; set; }
        public DateTime   quotation_request_lastdate { get; set; }
        public string? govt_tender { get; set; } = "NO";
        public string completeresponsibilityid { get; set; }
        public string salesresponsibilityid { get; set; }
        public string? status { get; set; }
        public string? Remarks { get; set; }
        public string? uploadedfilename { get; set; }
        public string createdBy { get; set; }
        public DateTime createdOn { get; set; }
        public string enquirytype { get; set; }
        public string tm { get; set; }
        public string tool { get; set; }
        public string? jobnames { get; set; }
        public string? appendreq { get; set; }
        public string type { get; set; }
        public string statename { get; set; }
        public string? dfa { get; set; }
        public string? VA_Assembly { get; set; } = "NO";
        public string? DesignOutSource { get; set; } = "NO";
        public string? NPINew_BOMProc { get; set; } = "NO";
        public string? NPINew_Fab { get; set; } = "NO";
        public string? NPINew_Assbly { get; set; } = "NO";
        public string? NPINew_Testing { get; set; } = "NO";
        public string? NPINewbyid { get; set; }
        public string? npinew_jobwork { get; set; } = "NO";
        public string? ReferenceBy { get; set; }

        // onsite extra fields
        public long? toolLicense { get; set; }
        public long? toolId { get; set; }
        public long? taskId { get; set; }
        public long? expFrom { get; set; }
        public long? expTo { get; set; }
        public long? noOfResources { get; set; }
        public DateTime?  tentStartDate { get; set; }
        public long? logistics { get; set; }
        public long? onsiteDurationType { get; set; }
        public long? hourlyRateType { get; set; }
        public Double? hourlyReate  { get; set; }
        public DateTime? profReqLastDate { get; set; }

        public long? onsiteDuration { get; set; }


    }
}
