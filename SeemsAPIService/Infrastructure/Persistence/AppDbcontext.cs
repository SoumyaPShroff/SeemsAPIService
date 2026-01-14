using Microsoft.EntityFrameworkCore;
using SeemsAPIService.Application.Services;
using SeemsAPIService.Domain.Entities;
using System.Data;

namespace SeemsAPIService.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;
        public AppDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to mysql with connection string from app settings
            var connectionString = Configuration.GetConnectionString("Conn");
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        public DbSet<Login> Login { get; set; }
        public DbSet<general_employee> general_employee { get; set; }
        public DbSet<setting_employee> setting_employee { get; set; }
        public DbSet<Job> job { get; set; }
        public DbSet<Appreciation> appreciation { get; set; }
        public DbSet<Complaint > complaint { get; set; }
        public DbSet<invoice_details > invoice_details { get; set; }
        public DbSet<purchase_order> purchase_order { get; set; }
        public DbSet<BillingPlannerRpt> BillingPlannerRpt { get; set; }
        public DbSet<HOPCManagerList> HOPCManagerList { get; set; }      
        public DbSet<Invoicedictionary> Invoicedictionary { get; set; }
        public DbSet<ThreeMonthConfirmedOrders> ThreeMonthConfirmedOrders { get; set; }
        public DbSet<PendingInvoices> PendingInvoices { get; set; }
        public DbSet<ViewAllEnquiries> ViewAllEnquiries { get; set; }
        public DbSet<employeeroles> employeeroles { get; set; }
        public DbSet<customer> customer { get; set; }
        public DbSet<view_salesnpiusers> view_salesnpiusers { get; set; }
        public DbSet<tool> tool { get; set; }
        public DbSet<se_customer_locations> se_customer_locations { get; set; }
        public DbSet<se_customer_contacts>  se_customer_contacts { get; set; }
        public DbSet<se_enquiry> se_enquiry { get; set; }
        public DbSet<RptViewEnquiryData> RptViewEnquiryData { get; set; }
        public DbSet<states_ind> states_ind { get; set; }
        public DbSet<poenquiries> poenquiries { get; set; }
        public DbSet<Email_Recipients> Email_Recipients { get; set; }   
        public DbSet<SidebarAccessMenus> SidebarAccessMenus { get; set; }
        public DbSet<roledesignations> roledesignations { get; set; }
        public DbSet<se_stages_tools> se_stages_tools { get; set; }
        public DbSet<se_quotlayout> se_quotlayout {  get; set; }
        public DbSet<se_quotation> se_quotation { get; set; }
       public DbSet<se_quotation_items> se_quotation_items { get; set; }

        public async Task<object> GetTentativeQuotedOrders()
        {
            using var connection = Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "sp_TentativeQuotedOrders";
            command.CommandType = CommandType.StoredProcedure;

            var result = new
            {
                TentativeOrders = new List<Dictionary<string, object>>(),
                QuotedOrders = new List<Dictionary<string, object>>(),
            };

            using var reader = await command.ExecuteReaderAsync();

            // 1️⃣ Tentative Orders
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                result.TentativeOrders.Add(row);
            }

            // 2️⃣ Quoted Orders
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                    row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                result.QuotedOrders.Add(row);
            }
            return result;
        }
        public async Task<object> GetOpenConfirmedOrders()
        {
            await using var connection = Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "CALL sp_OpenConfirmedOrders()"; // ✅ MySQL syntax
            command.CommandType = CommandType.Text; // ✅ MySQL uses text when using CALL

            var openOrders = new List<Dictionary<string, object>>();
            var confirmedOrders = new List<Dictionary<string, object>>();

            await using var reader = await command.ExecuteReaderAsync();

            int resultSetIndex = 0;
            do
            {
                var rows = new List<Dictionary<string, object>>();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    rows.Add(row);
                }

                if (resultSetIndex == 0)
                    openOrders.AddRange(rows);
                else if (resultSetIndex == 1)
                    confirmedOrders.AddRange(rows);

                resultSetIndex++;
            }
            while (await reader.NextResultAsync());

            return new
            {
                OpenOrders = openOrders,
                ConfirmedOrders = confirmedOrders
            };
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<se_quotation>()
                .HasMany(q => q.Items)
                .WithOne(i => i.Quotation)
                .HasForeignKey(i => i.quoteNo)
                .HasPrincipalKey(q => q.quoteNo);

            modelBuilder.Entity<se_quotation_items>()
            .HasKey(x => new { x.quoteNo, x.slNo });
        }

    }
}
