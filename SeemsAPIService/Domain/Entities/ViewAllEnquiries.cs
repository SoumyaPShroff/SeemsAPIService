using Microsoft.EntityFrameworkCore;
#nullable disable

namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class ViewAllEnquiries
    {
        public string Enquiryno { get; set; }
        public string Customer { get; set; }
        public string Createdon { get; set; }
        public string EndDate { get; set; }
        public string SalesResponsibility { get; set; }
        public string status { get; set; }
        public string Esti { get; set; }
        public string CompleteResponsibility { get; set; }
        public string EnquiryType { get; set; }
        public string BoardRef { get; set; }
        public string ReferenceBy { get; set; }

    }
}
