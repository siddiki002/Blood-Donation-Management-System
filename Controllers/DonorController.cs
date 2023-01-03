using BDMS.Data;
using BDMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BDMS.Controllers
{
    public class DonorController : Controller
    {
        public readonly ApplicationDbContext _db;

        public DonorController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET
        public IActionResult Index(Donor obj)
        {
            //int id = JsonConvert.DeserializeObject<int>(_session.GetString("Id"));
            if (!TempData.ContainsKey("successDonor"))
            {
                return RedirectToAction("DonorLogin", "Login");
            }

            if (TempData.ContainsKey("Date"))
            {
                TempData.Remove("Date");
            }

            if (TempData.ContainsKey("CampId"))
            {
                TempData.Remove("CampId");
            }

            if (TempData.ContainsKey("Id"))
            {
                obj = _db.Donors.Where(s => s.Id == Convert.ToInt32(TempData["Id"])).Include(s=> s.Slots).FirstOrDefault();
            }
            else
            {
                obj = _db.Donors.Where(s => s.Id == obj.Id).Include(s => s.Slots).FirstOrDefault();
            }

            if (obj == null)
            {
                return NotFound();
            }

            obj.Area = _db.Areas.Where(s => obj.AreaCode == s.Id).FirstOrDefault();

            if(obj.Slots.Where(s => s.Date.Date >= DateTime.Now.Date) != null)
            {
                obj.Slots = obj.Slots.Where(s => s.Date.Date >= DateTime.Now.Date && s.CanDonate == "No" && s.Reject == "No").ToList();

                foreach (Slot s in obj.Slots)
                {
                    s.BloodCamp = _db.BloodCamps.Find(s.CampId);
                    s.BloodCamp.Organization = _db.Organizations.Find(s.BloodCamp.OrgCode);
                    s.BloodCamp.Area = _db.Areas.Find(s.BloodCamp.AreaCode);
                }
            }
            else
            {
                obj.Slots.Clear();
            }

            if (obj.Area == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        public IActionResult Logout()
        {
            if (TempData.ContainsKey("Id"))
            {
                TempData.Remove("Id");
            }

            if (TempData.ContainsKey("Date"))
            {
                TempData.Remove("Date");
            }

            if (TempData.ContainsKey("CampId"))
            {
                TempData.Remove("CampId");
            }

            if (TempData.ContainsKey("successDonor"))
            {
                TempData.Remove("successDonor");
            }

            return RedirectToAction("DonorLogin", "Login");
        }

        // GET
        public IActionResult EditInfo(int Id)
        {
            if (!TempData.ContainsKey("successDonor"))
            {
                return RedirectToAction("DonorLogin", "Login");
            }

            if (Id == null)
            {
                return NotFound();
            }

            var obj = _db.Donors.Find(Id);
            obj.Area = _db.Areas.Find(obj.AreaCode);

            if (obj == null)
            {
                return NotFound();
            }

            return View(obj);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditInfo(Donor obj)
        {
            if (!TempData.ContainsKey("successDonor"))
            {
                return RedirectToAction("DonorLogin", "Login");
            }

            if (obj == null)
            {
                return NotFound();
            }

            obj.Area = _db.Areas.Where(s => s.Name == obj.Area.Name && s.City == obj.Area.City && s.Province == obj.Area.Province).FirstOrDefault();

            if (obj.Area == null)
            {
                ModelState.AddModelError("Area.Name", "Wrong Area !!");
                return View(obj);
            }

            obj.AreaCode = obj.Area.Id;
            _db.Donors.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index", obj);
        }

        // GET
        public IActionResult DonateOrg()
        {
            if (!TempData.ContainsKey("successDonor"))
            {
                return RedirectToAction("DonorLogin", "Login");
            }

            TempData["Id"] = TempData["Id"];

            if (TempData.ContainsKey("Date"))
            {
                TempData.Remove("Date");
            }

            if (TempData.ContainsKey("CampId"))
            {
                TempData.Remove("CampId");
            }

            //IEnumerable<BloodCamp> camps = _db.BloodCamps.FromSql($"SELECT * FROM [BDMS].[dbo].[BloodCamps]");
            //var camps = _db.BloodCamps.FromSql($"SELECT * FROM [BDMS].[dbo].[BloodCamps] b JOIN [BDMS].[dbo].[Areas] a on b.AreaCode=a.Id JOIN [BDMS].[dbo].[Organizations] o on b.OrgCode=o.Id");
            var camps = _db.BloodCamps.Include(b => b.Area).Include(b => b.Organization);

            //foreach(BloodCamp c in camps)
            //{
            //    c.Area = _db.Areas.Find(c.AreaCode);
            //    c.Organization = _db.Organizations.Find(c.OrgCode);
            //}

            return View(camps);
        }

        // GET
        public IActionResult DonateSlot(int id)
        {
            if (!TempData.ContainsKey("successDonor"))
            {
                return RedirectToAction("DonorLogin", "Login");
            }

            TempData["Id"] = TempData["Id"];
            DateTime date = Convert.ToDateTime(TempData["Date"]);

            BloodCamp camp = _db.BloodCamps.FromSql($"SELECT * FROM [BDMS_again].[dbo].[BloodCamps] WHERE Id={id}").FirstOrDefault();

            if(camp == null)
            {
                return NotFound();
            }

            DateTime time = camp.StartTime;

            IEnumerable<Slot> SlotBooked = _db.Slots.FromSql($"SELECT * FROM [BDMS_again].[dbo].[Slots] WHERE CAST( Date AS Date )={date.ToString("yyyy-MM-dd")} and CampId={id}");

            List<Slot> SlotAvailable = new List<Slot>();

            while (time.TimeOfDay != camp.EndTime.TimeOfDay)
            {
                var count = SlotBooked.Where(s => s.Time.TimeOfDay == time.TimeOfDay);

                if (count.Count() < camp.beds)
                {
                    Slot obj = new Slot();
                    obj.Date = Convert.ToDateTime(TempData["Date"]);
                    obj.CanDonate = "No";
                    obj.Reject = "No";
                    obj.CampId = id;
                    obj.bedno = count.Count() + 1;
                    obj.Time = Convert.ToDateTime(time.ToString());
                    SlotAvailable.Add(obj);
                }

                time = time.AddMinutes(30);
            }

            TempData["Date"] = TempData["Date"];
            TempData["CampId"] = id;

            return View(SlotAvailable);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DonateSlot(DateTime date, int Campid, int bed, DateTime time)
        {
            if(date==null || Campid==0 || bed==0 || time == null || !TempData.ContainsKey("Id"))
            {
                return NotFound();
            }
            if (_db.Slots.Where(s => s.Date.Date >= DateTime.Now.Date && s.DonorId == Convert.ToInt32(TempData["Id"]) && s.CanDonate == "No" && s.Reject == "No").Count() > 0)
            {
                TempData["booked"] = "You have already booked an appointment";
                TempData["Date"] = date;
                return RedirectToAction("DonateSlot", new { id = Campid });
            }

            Slot obj = new Slot();
            obj.Date = date;
            obj.CanDonate = "No";
            obj.Reject = "No";
            obj.CampId = Campid;
            obj.bedno = bed;
            obj.Time = time;
            obj.DonorId = Convert.ToInt32(TempData["Id"]);
            TempData["Id"] = TempData["Id"];

            _db.Slots.Add(obj);
            _db.SaveChanges();

            if (TempData.ContainsKey("Date"))
            {
                TempData.Remove("Date");
            }

            if (TempData.ContainsKey("CampId"))
            {
                TempData.Remove("CampId");
            }

            return RedirectToAction("Index");
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SlotDate(DateTime date)
        {
            if(date < DateTime.Now)
            {
                TempData["Date"] = DateTime.Now.Date;
            }
            else
            {
                TempData["Date"] = date;
            }

            return RedirectToAction("DonateSlot", new { id = Convert.ToInt32(TempData["CampId"]) });
        }

        // GET
        public IActionResult DeleteSlot(int id)
        {
            if (!TempData.ContainsKey("successDonor"))
            {
                return RedirectToAction("DonorLogin", "Login");
            }

            Slot obj = _db.Slots.Find(id);
            if(obj == null)
            {
                return NotFound();
            }

            _db.Slots.Remove(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET
        public IActionResult History()
        {
            if (!TempData.ContainsKey("successDonor"))
            {
                return RedirectToAction("DonorLogin", "Login");
            }

            int id = Convert.ToInt32(TempData["Id"]);
            var hist = _db.Slots.Where(s => s.DonorId == id && s.CanDonate == "Yes")
                .Include(b=> b.BloodCamp)
                .Include(o=> o.BloodCamp.Organization)
                .Include(o => o.BloodCamp.Area);

            TempData["Id"] = TempData["Id"];

            return View(hist);
        }

        // GET
        public IActionResult Result(int id) 
        {
            if (!TempData.ContainsKey("successDonor"))
            {
                return RedirectToAction("DonorLogin", "Login");
            }

            BloodBag obj = _db.BloodBags.Where(s => s.History == id).Include(t => t.TestedBags).FirstOrDefault();

            if(obj == null)
            {
                return NotFound();
            }

            foreach(var test in obj.TestedBags)
            {
                test.Disease = _db.Diseases.Find(test.DiseaseId);
            }

            return View(obj);
        }
    }
}
