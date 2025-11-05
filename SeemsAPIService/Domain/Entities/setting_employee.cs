using Microsoft.EntityFrameworkCore;
#nullable disable
namespace SeemsAPIService.Domain.Entities
{
    [Keyless]
    public class setting_employee
    {
        public uint itemnumber { get; set; }
        public string costcenter { get; set; }
        public string costcenter_status { get; set; }
        public string costcenter_analysis { get; set; }
        public string costcenter_npi { get; set; }
        public string costcenter_qa { get; set; }
        public string costcenter_sales { get; set; }
        public string functional { get; set; }
        public string jobfamily { get; set; }
        public string jobtitle { get; set; }
        public string gender { get; set; }
        public string bloodgroup { get; set; }
        public string extnumber { get; set; }
        public string systemnumber { get; set; }
        public int? priority { get; set; }
        public string jobtitle1 { get; set; }
        public string Projectlocation { get; set; }
        public string Projecttype { get; set; }
        public string Projectbillability { get; set; }
        public DateTime? rangedatefortimesheet { get; set; }
        public string tasktype { get; set; }
        public string description { get; set; }
        public string levels { get; set; }
        public string HOPC1ID { get; set; }
        public string HOPC1NAME { get; set; }
        public string HOPC2ID { get; set; }
        public string HOPC2NAME { get; set; }
        public string HOPC3ID { get; set; }
        public string HOPC3NAME { get; set; }
        public string HOPC4ID { get; set; }
        public string HOPC4NAME { get; set; }
        public string design { get; set; }
        public string DesignationClass { get; set; }
        public string currency { get; set; }
        public string Library_standard { get; set; }
        public string library_standard_text { get; set; }
        public string design1 { get; set; }
        public byte condition_task { get; set; }
    }
}
