using Microsoft.EntityFrameworkCore;
#nullable disable

namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class purchase_order
    {
            public uint Sno { get; set; }
            public string Jobnumber { get; set; }
            public string Ponumber { get; set; }
            public string POamount { get; set; }
            public DateTime? POdate { get; set; }
            public float? Currencyrate { get; set; }
            public string Jobid { get; set; }
            public string Currency { get; set; }
            public string File_attached { get; set; }
            public string porcvd { get; set; }
            public DateTime? deliverydate { get; set; }
            public DateTime? postartdate { get; set; }
            public decimal? Rateperhour { get; set; }
            public decimal? Estimatedhours { get; set; }
            public decimal? POhours { get; set; }
            public decimal? Contingencyhours { get; set; }
            public string Comments { get; set; }
    } 
}
 
