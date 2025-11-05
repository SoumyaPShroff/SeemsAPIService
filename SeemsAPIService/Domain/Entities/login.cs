using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

#nullable disable

namespace SeemsAPIService.Domain.Entities
{
    public class Login
    {
        [Key]
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string EmailID { get; set; }
        public string Design { get; set; }
        public string Library { get; set; }
        public string QA { get; set; }
        public string CAM { get; set; }
        public string Finance { get; set; }
        public string HR { get; set; }
        public string ProjectAdmin { get; set; }
        public string BillingPlanner   { get; set; }
        public string ActIctjobs { get; set; }
        public string saleslogin { get; set; }
        public string IT { get; set; }
        public string JobRegister { get; set; }
        public string CapacityUtilizationReport { get; set; }
        public string TimeSheet  { get; set; }

}
}
