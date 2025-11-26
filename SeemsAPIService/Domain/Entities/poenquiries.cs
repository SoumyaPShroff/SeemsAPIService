using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Domain.Entities
{
    public class poenquiries
    {
        [Key]
        public long id { get; set; }
        public string? penquiryno { get; set; }

        public string? pponumber { get; set; }

        public string podate { get; set; }

        public string ppoamount { get; set; }

        public string pbalanceamt { get; set; }
        public string layQty { get; set; }

        public string layRateperhr { get; set; }

        public string playout { get; set; }

        public string analyQty { get; set; }
        public string analyRateperhr { get; set; }


        public string panalysis { get; set; }

        public string vaQty { get; set; }

        public string vaRateperhr { get; set; }

        public string pva { get; set; }

        public string npiQty { get; set; }

        public string npiRateperhr { get; set; }

        public string pnpi { get; set; }

        public string libQty { get; set; }

        public string libRateperhr { get; set; }

        public string plibrary { get; set; }


        public string dfmQty { get; set; }

        public string dfmRateperhr { get; set; }

        public string ondfm { get; set; }

        public string onsiteQty { get; set; }

        public string onsiteRateperhr { get; set; }

        public string onsite { get; set; }

        public string ppaymentterm { get; set; }

       public string pcurrency_id { get; set; }

      public string  pconvrate { get; set; }

     public string?  pcomments { get; set; }

         public string?    pquoteno { get; set; }
        public string? pcreatedon { get; set; }

        public string? pcreatedby { get; set; }

        public string? pupdatedby { get; set; }

        public string? pupdatedon { get; set; }

    }
}
