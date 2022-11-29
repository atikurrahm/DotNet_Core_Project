using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Single_Page_Application.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Single_Page_Application.ViewModels
{
    public class WorkerEditModel
    {

        public int WorkerId { get; set; }
        [Required, StringLength(50), Display(Name = "Worker Name")]
        public string WorkerName { get; set; } = default!;
        [Required, StringLength(20), Display(Name = "Worker Phone")]
        public String Phone { get; set; } = default!;
        [Required]
        public IFormFile Picture { get; set; } = default!;
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
    }
}
