#nullable disable
using Microsoft.EntityFrameworkCore;

namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class ThreeMonthConfirmedOrders
    {
        public Int32 MonthNo { get; set; }
        public string  designcategory { get; set; }
        public Int32 currency_id { get; set; }
        public double TotalValue { get; set; }

    }
}
