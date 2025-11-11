using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Domain.Entities
{
    public class customer
    {
        [Key]
        public Int64  itemno { get; set; }
        public string Customer { get; set; }
        public string Customer_abb { get; set; }
        public string Addedby { get; set; }
        public string Addeddate { get; set; }
        public string sales_resp { get; set; }
        public string sales_resp_id { get; set; }
        public string Customer_Type { get; set; }
        public string Gst_no { get; set; }
        public string sapcustcode { get; set; }

    }
}
