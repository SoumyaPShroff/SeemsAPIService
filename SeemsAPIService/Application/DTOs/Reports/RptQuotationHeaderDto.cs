using Microsoft.EntityFrameworkCore;

namespace SeemsAPIService.Application.DTOs.Reports
{
    [Keyless]
    public class RptQuotationHeaderDto
    {
        public string QuoteNo { get; set; }
        public string EnquiryNo { get; set; }  
        public string CustomerName { get; set; }  
        public string CustomerAddress { get; set; }
        public string CustomerContactPerson { get; set; }
        public string RFXNo { get; set;}
        public string CreatedById { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedByEmail { get; set; }
        public int VersionNo { get; set; }
        public string BoardRef { get; set; }

    }

}
