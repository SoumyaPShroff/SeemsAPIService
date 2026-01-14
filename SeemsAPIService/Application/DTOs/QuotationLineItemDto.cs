namespace SeemsAPIService.Application.DTOs
{
    public class QuotationLineItemDto
    {
        public long slNo { get; set; }
        public string layout { get; set; }
        public string quantity { get; set; }
        public string unit_rate { get; set; }
        public string currency_id { get; set; }
        public string durationtype { get; set; }
        public string updatedbyid { get; set; }
        public long location_id { get; set; }
    }
}
