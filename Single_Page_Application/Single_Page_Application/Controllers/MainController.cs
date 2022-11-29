using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Single_Page_Application.Models;
using Single_Page_Application.ViewModels;

namespace Single_Page_Application.Controllers
{
    public class MainController : Controller
    {
        HUSDbContext db;
        IWebHostEnvironment env;
        public MainController(HUSDbContext db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }

        public async Task<IActionResult> Index()
        {
            var id = 0;
            if (db.WorkAreas.Any())
            {
                id = db.WorkAreas.ToList()[0].WorkAreaId;
            }

            DataViewModel data = new DataViewModel();
            data.SelectedWorkAreaId = id;
            data.Customers = await db.Customers.ToListAsync();
            data.Workers = await db.Workers.ToListAsync();
            data.WorkAreas = await db.WorkAreas.ToListAsync();
            data.Works = await db.Works.Where(oi => oi.WorkAreaId == id).ToListAsync();


            return View(data);
        }
        #region child actions
        public async Task<IActionResult> GetSelectedWorks(int id)
        {

            var Works = await db.Works.Include(x => x.Worker).Where(oi => oi.WorkAreaId == id).ToListAsync();
            return PartialView("_WorkTable", Works);
        }
        public IActionResult CreateCustomer()
        {
            return PartialView("_CreateCustomer");
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer c)
        {
            if (ModelState.IsValid)
            {
                await db.Customers.AddAsync(c);
                await db.SaveChangesAsync();
                return Json(c);
            }
            return BadRequest("Unexpected error");
        }
        public async Task<IActionResult> EditCustomer(int id)
        {
            var data = await db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            return PartialView("_EditCustomer", data);
        }
        [HttpPost]
        public async Task<IActionResult> EditCustomer(Customer c)
        {
            if (ModelState.IsValid)
            {
                db.Entry(c).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Json(c);
            }
            return BadRequest("Unexpected error");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            if (!await db.WorkAreas.AnyAsync(o => o.CustomerId == id))
            {
                var o = new Customer { CustomerId = id };
                db.Entry(o).State = EntityState.Deleted;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return Json(new { success = true, message = "Data deleted" });
            }
            return Json(new { success = false, message = "Cannot delete, item has related child." });
        }

        public IActionResult CreateWorker()
        {
            return PartialView("_CreateWorker");
        }
        [HttpPost]
        public async Task<IActionResult> CreateWorker(WorkerInputModel p)
        {
            if (ModelState.IsValid)
            {
                var worker = new Worker { WorkerName = p.WorkerName, Phone = p.Phone,StartDate=p.StartDate,EndDate=p.EndDate,Payrate=p.Payrate, TotalWorkHour=p.TotalWorkHour, TotalPayment=p.TotalPayment };
                string fileName = Guid.NewGuid() + Path.GetExtension(p.Picture.FileName);
                string savePath = Path.Combine(this.env.WebRootPath, "Pictures", fileName);
                var fs = new FileStream(savePath, FileMode.Create);
                p.Picture.CopyTo(fs);
                fs.Close();
                worker.Picture = fileName;
                await db.Workers.AddAsync(worker);
                await db.SaveChangesAsync();
                return Json(worker); ;


            }
            return BadRequest("Falied to insert worker");
        }
        public async Task<IActionResult> EditWorker(int id)
        {
            var data = await db.Workers.FirstAsync(x => x.WorkerId == id);
            ViewData["CurrentPic"] = data.Picture;
            return PartialView("_EditWorker", new WorkerEditModel { WorkerId = data.WorkerId, WorkerName = data.WorkerName, Phone = data.Phone, StartDate = data.StartDate,EndDate=data.EndDate,Payrate=data.Payrate,TotalWorkHour=data.TotalWorkHour,TotalPayment=data.TotalPayment });
        }
        [HttpPost]
        public async Task<IActionResult> EditWorker(WorkerEditModel p)
        {
            if (ModelState.IsValid)
            {
                var worker = await db.Workers.FirstAsync(x => x.WorkerId == p.WorkerId);
                worker.WorkerName = p.WorkerName;
                worker.Phone = p.Phone;
                worker.StartDate = p.StartDate;
                worker.EndDate = p.EndDate;
                worker.Payrate = p.Payrate;
                worker.TotalWorkHour = p.TotalWorkHour;
                worker.TotalPayment = p.TotalPayment;
                if (p.Picture != null)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(p.Picture.FileName);
                    string savePath = Path.Combine(this.env.WebRootPath, "Pictures", fileName);
                    var fs = new FileStream(savePath, FileMode.Create);
                    p.Picture.CopyTo(fs);
                    fs.Close();
                    worker.Picture = fileName;
                }


                await db.SaveChangesAsync();
                return Json(worker);


            }
            return BadRequest();
        }
        public async Task<IActionResult> DeleteWorker(int id)
        {
            if (!await db.Works.AnyAsync(o => o.WorkerId == id))
            {
                var o = new Worker { WorkerId = id };
                db.Entry(o).State = EntityState.Deleted;
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                return Json(new { success = true, message = "Data deleted" });
            }
            return Json(new { success = false, message = "Cannot delete, item has related child." });
        }




        public async Task<IActionResult> CreateWorkArea()
        {
            ViewData["Workers"] = await db.Workers.ToListAsync();
            ViewData["Customers"] = await db.Customers.ToListAsync();
            return PartialView("_CreateWorkArea");
        }
        [HttpPost]
        public async Task<IActionResult> CreateWorkArea(WorkArea o, int[] WorkerId, int[] TotalWorker)
        {
            if (ModelState.IsValid)
            {
                for (var i = 0; i < WorkerId.Length; i++)
                {
                    o.Works.Add(new Work { WorkerId = WorkerId[i], TotalWorker = TotalWorker[i] });
                }
                await db.WorkAreas.AddAsync(o);

                await db.SaveChangesAsync();


                var ord = await GetWorkArea(o.WorkAreaId);
                return Json(ord);
            }
            return BadRequest();
        }
        public async Task<IActionResult> EditWorkArea(int id)
        {
            ViewData["Workers"] = await db.Workers.ToListAsync();
            ViewData["Customers"] = await db.Customers.ToListAsync();
            var data = await db.WorkAreas
                .Include(x => x.Works).ThenInclude(x => x.Worker)
                .FirstOrDefaultAsync(x => x.WorkAreaId == id);
            return PartialView("_EditWorkArea", data);

        }

        [HttpPost]
        public async Task<IActionResult> EditWorkArea(WorkArea o)
        {
            if (ModelState.IsValid)
            {
                var existing = await db.WorkAreas.Include(x => x.Customer).FirstAsync(x => x.WorkAreaId == o.WorkAreaId);
                existing.WorkAreaName = o.WorkAreaName;
                existing.Skill = o.Skill;
                existing.IsRunning = o.IsRunning;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                return Json(existing);
            }

            return BadRequest();
        }
        private async Task<WorkArea?> GetWorkArea(int id)
        {
            var o = await db.WorkAreas.Include(x => x.Customer).FirstOrDefaultAsync(x => x.WorkAreaId == id);
            return o;
        }
        [HttpPost]
        public async Task<IActionResult> DeleteWorkArea(int id)
        {
            var o = new WorkArea { WorkAreaId = id };
            db.Entry(o).State = EntityState.Deleted;
            await db.SaveChangesAsync();
            return Json(new { success = true, message = "Data deleted" });
        }
        public async Task<IActionResult> CreateWorkItem()
        {
            ViewData["Workers"] = await db.Workers.ToListAsync();
            return PartialView("_CreateWorkItem");
        }
        public async Task<IActionResult> CreateWork(int id)
        {
            ViewData["WorkAreaId"] = id;
            ViewData["Workers"] = await db.Workers.ToListAsync();
            return PartialView("_CreateWork");
        }
        [HttpPost]
        public async Task<IActionResult> CreateWork(Work oi)
        {
            if (ModelState.IsValid)
            {
                await db.Works.AddAsync(oi);
                await db.SaveChangesAsync();
                var o = await GetWork(oi.WorkAreaId, oi.WorkerId);
                return Json(o);
            }
            return BadRequest();
        }
        public async Task<IActionResult> EditWork(int oid, int pid)
        {
            ViewData["Workers"] = await db.Workers.ToListAsync();
            var oi = await db.Works.FirstAsync(x => x.WorkAreaId == oid && x.WorkerId == pid);
            return PartialView("_EditWork", oi);
        }
        [HttpPost]
        public async Task<IActionResult> EditWork(Work oi)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oi).State = EntityState.Modified;
                await db.SaveChangesAsync();
                var o = await GetWork(oi.WorkAreaId, oi.WorkerId);
                return Json(o);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteWork([FromQuery] int oid, [FromQuery] int pid)
        {

            var o = new Work { WorkerId = pid, WorkAreaId = oid };
            db.Entry(o).State = EntityState.Deleted;

            await db.SaveChangesAsync();

            return Json(new { success = true, message = "Data deleted" });

        }
        private async Task<Work> GetWork(int oid, int pid)
        {
            var oi = await db.Works
                .Include(o => o.WorkArea)
                .Include(o => o.Worker)
                .FirstAsync(x => x.WorkAreaId == oid && x.WorkerId == pid);
            return oi;
        }
        #endregion 
    }
}