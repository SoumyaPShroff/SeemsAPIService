using SeemsAPIService.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Application.DTOs
{
    public class QuotationDto
    {
        // ===== Header =====
        public string? quoteNo { get; set; }   // ✅ nullable, no [Required]

        [Required]
        public string enquiryno { get; set; }
        public string? board_ref { get; set; }
        public string createdBy { get; set; }
        public int versionNo { get; set; }
        public string tandc { get; set; }

        // ===== Line Items =====
        [Required]
        public List<QuotationLineItemDto> Items { get; set; } = new();
        // <-- NEW: track deleted line item IDs
        public List<int> deletedSlNos { get; set; } = new List<int>();
    }

}
