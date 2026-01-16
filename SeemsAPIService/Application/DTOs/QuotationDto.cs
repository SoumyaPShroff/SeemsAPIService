using SeemsAPIService.Domain.Entities;

namespace SeemsAPIService.Application.DTOs
{
    public class QuotationDto
    {
        // ===== Header =====
        public string enquiryno { get; set; }
        public string board_ref { get; set; }
        public string? quoteNo { get; set; } 
        public string createdBy { get; set; }
        public int versionNo { get; set; }
        public string tandc { get; set; }

        // ===== Line Items =====
        public List<QuotationLineItemDto> Items { get; set; } = new();
    }

}
