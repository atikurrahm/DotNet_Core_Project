using Microsoft.EntityFrameworkCore;
using Single_Page_Application.Models;

namespace Single_Page_Application.ViewModels
{
    public class DataViewModel
    {

        public int SelectedWorkAreaId { get; set; }
        public IEnumerable<Worker> Workers { get; set; } = default!;
        public IEnumerable<Work> Works { get; set; } = default!;
        public IEnumerable<Customer> Customers { get; set; } = default!;
        public IEnumerable<WorkArea> WorkAreas { get; set; } = default!;

    }
}
