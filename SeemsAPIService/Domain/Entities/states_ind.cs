using Microsoft.EntityFrameworkCore;
#nullable disable
using System.ComponentModel.DataAnnotations;

namespace SeemsAPIService.Domain.Entities
{
    public class states_ind
    {
        [Key]
        public long ID { get; set; }
        public string State { get; set; }
    }
}
