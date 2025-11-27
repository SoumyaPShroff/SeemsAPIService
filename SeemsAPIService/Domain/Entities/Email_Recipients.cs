using System.ComponentModel.DataAnnotations;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    public class Email_Recipients
    {
        [Key]
        public string LoginId { get; set; }
        public string EnqCreated_PositionInEmail { get; set; }
        public string EnqUpdated_PositionInEmail { get; set; }
        public string EstCreated_PositionInEmail { get; set; }
        public string EstUpdated_PositionInEmail { get; set; }
        public string Design { get; set; }
        public string NPI { get; set; }
        public string EnqStatusChange_PositionInEmail { get; set; }

    }
}
