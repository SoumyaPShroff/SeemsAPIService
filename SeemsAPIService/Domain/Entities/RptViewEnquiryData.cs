using Microsoft.EntityFrameworkCore;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class RptViewEnquiryData
    {
        public string enquiryno { get; set; }
        public string customer { get; set; }    
        public string enquiry_createdon { get; set; }   
        public string quote_generatedon { get; set; }
        public string Job_Createdon {  get; set; }
        public string salesperson { get; set; }
        public string  completeresponsibility { get; set; }
        public string quoteCreatedby { get; set; }  
        public string status { get; set; }
        public string remarks { get; set; }
        public string cancelledremarks { get; set; }
}
}
