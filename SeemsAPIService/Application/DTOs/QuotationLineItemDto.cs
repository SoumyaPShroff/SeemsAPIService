namespace SeemsAPIService.Application.DTOs
{
    public class QuotationLineItemDto
    {
        public int slNo { get; set; }
        public string layout { get; set; }
        public int quantity { get; set; }             //table dfn altered as int
        public decimal unit_rate { get; set; }  //table dfn altered as decimal
        public int currency_id { get; set; }
        public string durationtype { get; set; }
        public string updatedbyid { get; set; }
        public int location_id { get; set; }
        public int versionNo { get; set; }
    }
}
