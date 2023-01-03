using BDMS.Data;
using BDMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BDMS.Controllers
{
    public class OrganizationController : Controller
    {
        private readonly ApplicationDbContext _db;

        public OrganizationController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(Organization obj)
        {
            var orgfromdb = _db.Organizations.Where(s => s.Name == obj.Name).Include(b => b.Employees).Include(x=> x.BloodCamps).FirstOrDefault();
            return View(orgfromdb);
        }
        //GET
        public IActionResult Login()
        {
            return View();
        }

        //POST
        [HttpPost,ActionName("Login")]
        [ValidateAntiForgeryToken]
        public IActionResult LoginPOST(Organization obj)
        {
            if (obj == null)
            {
                return View(obj);
            }
            var orgfromdb = _db.Organizations.Where(s => s.Name == obj.Name).FirstOrDefault();
            

            if (orgfromdb == null)
            {
                ModelState.AddModelError("Name", "The Name is wrong");
            }
            else if (orgfromdb.Password != obj.Password)
            {
                ModelState.AddModelError("Password", "The Password is wrong");
            }
            else
            {
               return RedirectToAction("Index", orgfromdb);
                
            }
            //return RedirectToAction("Login",obj);
            return View(obj);
        }
        //GET
        public IActionResult Addemployee(int id)
        {
            Employee empfromdb = new Employee();
            empfromdb.OrgCode = id;
            //var orgfromdb = _db.Organizations.Where(x => x.Id == id);
            //empfromdb.OrgCode = 
            return View(empfromdb);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]  
       public IActionResult Addemployee(int OrgCode, string Firstname, string Lastname, string cnic, string email, string Password,Area area)
        {
            var countnic = _db.Employees.Where(x=> x.Cnic==cnic).Count();
            if (countnic > 0)
            {
                TempData["Cnic"] = "The Cnic already exists";
                ModelState.AddModelError("Cnic", "The Cnic already exists");
                return RedirectToAction("Addemployee", OrgCode);
            }
            if (Firstname == null || Lastname == null || cnic == null || email == null || Password == null || area==null)
            {
                TempData["Invalid_entry"] = "Invalid input. Kindly input again";
                return RedirectToAction("Addemployee", OrgCode);
            }
            if (Password.Length < 8)
            {
                TempData["Password"] = "Password should be of length greater than 8";
                ModelState.AddModelError("Password", "Password should be of length greater than 8");
                return RedirectToAction("Addemployee", OrgCode);
            }
            var Areaofemp = _db.Areas.Where(x => x.Name == area.Name && x.City == area.City && x.Province == area.Province).FirstOrDefault();
            if(Areaofemp==null)
            {
                TempData["Area"] = "Please enter a valid area";
                ModelState.AddModelError("Area", "Please enter a valid area");
                return RedirectToAction("Addemployee", OrgCode);
            }
            var employee = new Employee();
            employee.OrgCode = OrgCode;
            employee.FirstName= Firstname;
            employee.LastName= Lastname;
            employee.Cnic = cnic;
            employee.Email = email;
            employee.Password = Password;
            employee.AreaCode = Areaofemp.Id;
            _db.Employees.Add(employee);
            _db.SaveChanges();
            var organization = _db.Organizations.Find(OrgCode);
            return RedirectToAction("Index", organization);
        }
        //GET
        public IActionResult EditEmployee(int id)
        {
            var emp = _db.Employees.Find(id);
            return View(emp);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEmployee(Employee emp)
        {
            if (emp == null)
            {
                return NotFound();
            }
            var countnic = _db.Employees.Where(x => x.Cnic == emp.Cnic).Count();
            if(countnic>1)
            {
                TempData["Cnic"] = "The Cnic already exists";
                ModelState.AddModelError("Cnic", "The Cnic already exists");
                return RedirectToAction("EditEmployee", emp.Id);
            }
            var area = _db.Areas.Where(x => x.Name == emp.Area.Name && x.City == emp.Area.City && x.Province == emp.Area.Province).FirstOrDefault();
            if (area == null)
            {
                TempData["Area"] = "Please enter a valid area";
                ModelState.AddModelError("Area", "Please enter a valid area");
                return RedirectToAction("EditEmployee", emp.Id);
            }
            _db.Employees.Update(emp);
            _db.SaveChanges();
            var org = _db.Organizations.Find(emp.OrgCode);
            return RedirectToAction("Index", org);
        }
        //GET
        public IActionResult DeleteEmployee(int id)
        {
            var emp = _db.Employees.Find(id);
            return View(emp);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEmployee(int? id, int OrgCode)
        {
            if(id==0 || id== null)
            {
                return NotFound();
            }
            var emp = _db.Employees.Find(id);
            var org = _db.Organizations.Find(OrgCode);
            if(emp!=null)
            {
                _db.Employees.Remove(emp);
                _db.SaveChanges();
            }
            
            return RedirectToAction("Index", org);
        }
        //GET
        public IActionResult Organizationregister()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult Organizationregister(Organization obj)
        {
            var orgfromdb = _db.Organizations.Where(s => s.Name == obj.Name);
            if (orgfromdb == null) { return NotFound(); }
            if (orgfromdb.Count() > 0)
            {
                ModelState.AddModelError("Name", "Name already exist");
                return RedirectToAction("Organizationregister");
            }
            _db.Organizations.Add(obj);
            _db.SaveChanges();
            return RedirectToAction("Login");
        }
        //GET
        public IActionResult AddCamp(int id)
        {
            var camp = new BloodCamp();
            camp.OrgCode = id;
            return View(camp);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCamp(int OrgCode, DateTime StartTime,DateTime EndTime,int beds,Area area)
        {
            if(area==null)
            {
                return NotFound();
            }
            
            var areafromdb = _db.Areas.Where(x => x.Name == area.Name && x.City == area.City && x.Province == area.Province).FirstOrDefault();

            if(areafromdb == null)
            {
                return NotFound(); 
            }
            var Camp = new BloodCamp();
            Camp.OrgCode = OrgCode;
            Camp.StartTime = StartTime;
            Camp.EndTime = EndTime;
            Camp.beds = beds;
            Camp.AreaCode = areafromdb.Id;
            if (_db.BloodCamps.Where(x => x.AreaCode == areafromdb.Id && x.OrgCode == OrgCode).Count()>0)
            {
                ModelState.AddModelError("exist", "A blood camp in area already exist");
            }
            else
            {
                _db.BloodCamps.Add(Camp);
                _db.SaveChanges();
            }

            var org = _db.Organizations.Find(OrgCode);
            if(org == null) {
                return NotFound();
            }
            return RedirectToAction("Index", org);
        }
        //GET
        public IActionResult EditCamp(int id)
        {
            var camp = _db.BloodCamps.Find(id);
            if(camp==null)
            {
                return NotFound();
            }
            return View(camp);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditCamp(BloodCamp bloodcamp)
        {
            if (bloodcamp == null)
            {
                return NotFound();
            }
            if (bloodcamp.beds < 0)
            {
                return NotFound();
            }
            if(bloodcamp.EndTime < bloodcamp.StartTime)
            {
                return NotFound();
            }
            var areafromdb = _db.Areas.Where(x => x.Name == bloodcamp.Area.Name && x.City == bloodcamp.Area.City && x.Province == bloodcamp.Area.Province).FirstOrDefault();
            if(areafromdb==null)
            {
                return NotFound();
            }
            bloodcamp.AreaCode = areafromdb.Id;
            var org = _db.Organizations.Find(bloodcamp.OrgCode);
            if(org==null)
            {
                return NotFound();
            }
            _db.BloodCamps.Update(bloodcamp);
            _db.SaveChanges();
            return RedirectToAction("Index", org);
        }
        //GET
        public IActionResult DeleteCamp(int id)
        {
            var Camp = _db.BloodCamps.Find(id);
            if(Camp==null)
            {
                return NotFound();
            }
            return View(Camp);
        }
        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCamp(int OrgCode,int id)
        {
            var camp = _db.BloodCamps.Find(id);
            var org = _db.Organizations.Find(OrgCode);
            if(camp==null)
            {
                return RedirectToAction("Index", OrgCode);
            }
            _db.BloodCamps.Remove(camp);
            _db.SaveChanges();
            return RedirectToAction("Index", org);
        }
        public IActionResult seeDetails(int id)
        {
            var camp = _db.BloodCamps.Where(x=> x.Id == id).Include(b=> b.Slots).FirstOrDefault();
            if (camp == null)
            {
                return NotFound();
            }
            foreach (var slots in camp.Slots)
            {
                var blcamp = _db.BloodCamps.Find(slots.CampId);

                if (blcamp != null)
                {    
                    var areaofblcamp = _db.Areas.Find(blcamp.AreaCode);
                    if (areaofblcamp != null)
                    {
                        blcamp.Area = areaofblcamp;
                    }
                
                    slots.BloodCamp = blcamp;
                }
                
                var donor = _db.Donors.Find(slots.DonorId);
                if(donor!=null)
                {
                    slots.Donor = donor;
                }
                
            }
            return View(camp);
            
        }
        public IActionResult viewOrg(int id)
        {
            if(id==0)
            {
                return NotFound();
            }
            var org = _db.Organizations.Find(id);
            return RedirectToAction("Index", org);
        }
    }
}
