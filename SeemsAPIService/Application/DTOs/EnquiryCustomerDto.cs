namespace SeemsAPIService.Application.DTOs
{
    public class EnquiryCustomerDto
    {
        public string Customer { get; set; }
        public string Location { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string RFXNo { get; set; }
        public string enquirytype { get; set; }
        public long? locationid { get; set; }
        public string boardref { get; set; }
    }
}