using BDMS.Data;
using BDMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDMS.Controllers
{
    public class EmpController : Controller
    {

        public readonly ApplicationDbContext _db;

        public EmpController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET
        public IActionResult Index(Employee obj)
        {
            int id;

            if (!TempData.ContainsKey("successEmp"))
            {
                return RedirectToAction("EmpLogin", "Login");
            }

            if (TempData.ContainsKey("Slot"))
            {
                TempData.Remove("Slot");
            }

            if (TempData.ContainsKey("Id"))
            {
                id = Convert.ToInt32(TempData["Id"]);
            }

            else if (obj.Id != 0)
            {
                TempData["Id"] = _db.BloodCamps.Where(x => x.OrgCode == obj.OrgCode && x.AreaCode == obj.AreaCode).First().Id; ;
                id = _db.BloodCamps.Where(x => x.OrgCode == obj.OrgCode && x.AreaCode == obj.AreaCode).First().Id; ;
            }

            else
            {
                return NotFound();
            }

            id = Convert.ToInt32(TempData["Id"]);
            var Slots = _db.Slots.Where(x => x.CampId == id && x.Date.Date == DateTime.Now.Date && x.CanDonate == "No" && x.Reject == "No").Include(d=> d.Donor);
            

            return View(Slots);
        }

        // GET
        public IActionResult Logout()
        {
            if (TempData.ContainsKey("Id"))
            {
                TempData.Remove("Id");
            }

            if (TempData.ContainsKey("Slot"))
            {
                TempData.Remove("Slot");
            }

            if (TempData.ContainsKey("successEmp"))
            {
                TempData.Remove("successEmp");
            }

            return RedirectToAction("EmpLogin", "Login");
        }

        // GET
        public IActionResult CheckUp(int id)
        {

            if (!TempData.ContainsKey("successEmp"))
            {
                return RedirectToAction("EmpLogin", "Login");
            }

            TempData["Id"] = TempData["Id"];

            Slot s = _db.Slots.Where(x=> x.Id==id).Include(d=> d.Donor).FirstOrDefault();

            if(s == null)
            {
                return NotFound();
            }


            s.Donor.Slots = _db.Slots.Where(x => x.Date.Date <= DateTime.Now.Date && x.DonorId == s.DonorId && x.CanDonate == "Yes").ToList();
            s.Donor.Slots.OrderByDescending(x => x.Date.Date);

            return View(s);
        }

        // GET
        public IActionResult DonorHistory(int id)
        {

            if (!TempData.ContainsKey("successEmp"))
            {
                return RedirectToAction("EmpLogin", "Login");
            }

            TempData["Slot"] = TempData["Slot"];

            Donor s = _db.Donors.Find(id);

            if(s == null)
            {
                return NotFound();
            }

            s.Slots = _db.Slots.Where(x => x.Date.Date <= DateTime.Now.Date && x.DonorId == s.Id && x.CanDonate == "Yes").Include(b=> b.BloodBags).ToList();
            s.Slots.OrderBy(x => x.Date.Date);

            List<BloodBag> bags = new List<BloodBag>();

            foreach(var item in s.Slots)
            {
                bags.Add(item.BloodBags.First());
            }

            foreach (var item in bags)
            {
                item.Slot = _db.Slots.Find(item.History);
                item.TestedBags = _db.TestedBags.Where(x => x.BagId == item.Id).Include(d => d.Disease).ToList();
            }

            return View(bags);
        }

        // GET
        public IActionResult AcceptDonor(int id)
        {
            if (!TempData.ContainsKey("successEmp"))
            {
                return RedirectToAction("EmpLogin", "Login");
            }

            if (TempData.ContainsKey("Slot"))
            {
                TempData.Remove("Slot");
            }

            Slot s = _db.Slots.Find(id);

            if (s == null)
            {
                return NotFound();
            }

            s.CanDonate = "Yes";
            _db.Slots.Update(s);
            _db.SaveChanges();

            BloodBag bag = new BloodBag();
            bag.BloodGrp = "";
            bag.History = id;

            _db.BloodBags.Add(bag);
            _db.SaveChanges();

            Employee obj = _db.Employees.Find(Convert.ToInt32(TempData["Id"]));

            return RedirectToAction("Index", obj);
        }

        // GET
        public IActionResult RejectDonor(int id)
        {
            if (!TempData.ContainsKey("successEmp"))
            {
                return RedirectToAction("EmpLogin", "Login");
            }

            if (TempData.ContainsKey("Slot"))
            {
                TempData.Remove("Slot");
            }

            Slot s = _db.Slots.Find(id);

            if (s == null)
            {
                return NotFound();
            }

            s.Reject = "Yes";
            _db.Slots.Update(s);
            _db.SaveChanges();

            Employee obj = _db.Employees.Find(Convert.ToInt32(TempData["Id"]));

            return RedirectToAction("Index", obj);
        }

        // GET
        public IActionResult Reports()
        {
            if (!TempData.ContainsKey("successEmp"))
            {
                return RedirectToAction("EmpLogin", "Login");
            }

            if (TempData.ContainsKey("Slot"))
            {
                TempData.Remove("Slot");
            }

            int id = Convert.ToInt32(TempData["Id"]);
            var Slots = _db.Slots.Where(x => x.CampId == id && x.CanDonate == "Yes" && x.Reject == "No").Include(d => d.BloodBags);

            List<BloodBag> bags = new List<BloodBag>();

            foreach (var item in Slots)
            {
                BloodBag b = item.BloodBags.First();

                if (_db.TestedBags.Where(x => x.BagId == b.Id).Count() == 0)
                {
                    b.Slot = _db.Slots.Find(b.History);
                    bags.Add(b);
                }
            }

            return View(bags);
        }

        // GET
        public IActionResult AddReport(int id)
        {
            if (!TempData.ContainsKey("successEmp"))
            {
                return RedirectToAction("EmpLogin", "Login");
            }

            if (TempData.ContainsKey("Slot"))
            {
                TempData.Remove("Slot");
            }

            BloodBag bag = _db.BloodBags.Find(id);

            if(bag == null)
            {
                return NotFound();
            }

            return View(bag);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddReport(int Id, String BloodGrp, int[] Diseases)
        {
            if (!TempData.ContainsKey("successEmp"))
            {
                return RedirectToAction("EmpLogin", "Login");
            }

            if (TempData.ContainsKey("Slot"))
            {
                TempData.Remove("Slot");
            }

            BloodBag bag = _db.BloodBags.Find(Id);
            bag.BloodGrp = BloodGrp;
            _db.BloodBags.Update(bag);
            _db.SaveChanges();

            for(int i=0; i<Diseases.Count(); i++)
            {
                TestedBags tb = new TestedBags();
                tb.BagId = bag.Id;
                tb.DiseaseId = Diseases[i];
                _db.TestedBags.Add(tb);
            }
            _db.SaveChanges();

            return RedirectToAction("Reports");
        }
    }
}
