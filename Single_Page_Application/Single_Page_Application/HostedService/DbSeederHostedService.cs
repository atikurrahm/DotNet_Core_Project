using Single_Page_Application.Models;

namespace Single_Page_Application.HostedService
{
    public class DbSeederHostedService : IHostedService
    {
       
        IServiceProvider serviceProvider;
        public DbSeederHostedService(
            IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
           
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
               
                  var db = scope.ServiceProvider.GetRequiredService<HUSDbContext>();

                  await SeedDbAsync(db);
               
            }
        }
       public async Task SeedDbAsync(HUSDbContext db)
        {
            await db.Database.EnsureCreatedAsync();
            if (!db.Customers.Any())
            {
                var c1 = new Customer {CustomerName = "Mahidul Mullah",Email="mahidul@gmail.com", CustomerAddress = "Armanitola, Armenia", WorkDescription= "Garage electric wiring" };
                await db.Customers.AddAsync(c1);
                var p1 = new Worker {WorkerName="W1" , Phone="01698546",Picture = "1.jpg", StartDate = DateTime.Today.AddDays(-8), EndDate = DateTime.Today.AddDays(-1), Payrate = 650,TotalWorkHour=900,TotalPayment=20000 };
                await db.Workers.AddAsync(p1);
                var o1 = new WorkArea { WorkAreaName = "Mirpur", Customer = c1, Skill = "Plumbbing",IsRunning=true };
                o1.Works.Add(new Work { WorkArea = o1, Worker = p1, TotalWorker = 5 });
                await db.WorkAreas.AddAsync(o1);
                await db.SaveChangesAsync();
            }

        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        
    }
}
