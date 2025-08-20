using AzureHomework2t1.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureHomework2t1.Controllers
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ImageRecord> ImageRecords { get; set; }
 
    }
}