using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Single_Page_Application.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        
        [Required, StringLength(50), Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = default!;
        [Required, StringLength(20), Display(Name = "Customer Email")]
        public string Email { get; set; } = default!;

        [Required, StringLength(50), Display(Name = "Customer Address")]
        public string CustomerAddress { get; set; } = default!;
        [Required, StringLength(50), Display(Name = "Work Description")]
        public string WorkDescription { get; set; } = default!;
        
        public virtual ICollection<WorkArea> WorkAreas { get; set; } = new List<WorkArea>();
    }
    public class WorkArea
    {
        public int WorkAreaId { get; set; }
        [Required, ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [Required, StringLength(60), Display(Name = "WorkArea Name")]
        public string WorkAreaName { get; set; } = default!;
        [Required, Display(Name = "Work Skill")]
        public String Skill { get; set; } = default!;

        [Display(Name = "Available?")]
        public bool IsRunning { get; set; }
        
        public Customer? Customer { get; set; } = default!;
        public virtual ICollection<Work> Works { get; set; } = new List<Work>();
    }
    public class Work
    {
        [ForeignKey("WorkArea")]
        public int WorkAreaId { get; set; }
        [ForeignKey("Worker")]
        public int WorkerId { get; set; }
        public int TotalWorker { get; set; }
        public virtual Worker? Worker { get; set; } = default!;
        public virtual WorkArea? WorkArea { get; set; } = default!;
    }
    public class Worker
    {
  
        public int WorkerId { get; set; }
        [Required, StringLength(50), Display(Name = "Worker Name")]
        public string WorkerName { get; set; } = default!;
        [Required, StringLength(20), Display(Name = "Worker Phone")]
        public String Phone { get; set; } = default!;
        [Required, StringLength(150), Display(Name = "Worker")]
        public string Picture { get; set; } = default!;
        [Required, Column(TypeName = "date"), Display(Name = "Start Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "End Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        [Required, Display(Name = "Payrate")]
        public decimal Payrate { get; set; }

        [Required, Display(Name = "TotalWorkHour")]
        public float TotalWorkHour { get; set; }
        [Required, Display(Name = "TotalPayment")]
        public decimal TotalPayment { get; set; }
        public virtual ICollection<Work> Works { get; set; } = new List<Work>();
    }
    public class HUSDbContext : DbContext
    {
        public HUSDbContext(DbContextOptions<HUSDbContext> options) : base(options) { }
        public DbSet<Worker> Workers { get; set; } = default!;
        public DbSet<Work> Works { get; set; } = default!;
        public DbSet<Customer> Customers { get; set; } = default!;
        public DbSet<WorkArea> WorkAreas { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Work>().HasKey(o => new { o.WorkerId, o.WorkAreaId });
        }

    }
}
