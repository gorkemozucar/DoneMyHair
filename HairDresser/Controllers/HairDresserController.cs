using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HairDresser1.Data;
using HairDresser1.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace HairDresser1.Controllers
{
    public class HairDresserController : Controller
    {
        private readonly HairDresserDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;


        public HairDresserController(HairDresserDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {
            _context = context;
            _httpContext = httpContext;
            _userManager = userManager;
        }

        //// GET: HairDresser
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.HairDresser.ToListAsync());
        //}
        public async Task<HairDresser> getHairdresser(string id)
        {
            return await _context.HairDresser.FirstOrDefaultAsync(x => x.ID == id);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hairDresserModel = await _context.HairDresser
                .FirstOrDefaultAsync(m => m.ID == id);
            var salon = _context.Saloon.Where(x => x.ID == hairDresserModel.SaloonID).FirstOrDefaultAsync().Result;
            TempData["owneridd"] = salon.SaloonOwnerID;
            var commmentmodels = await _context.CommentModels.Where(x => x.HairDresserID == id).ToListAsync();
            var commentModel1 = new CommentModel();
            var appointment = new Appointment();
            var model = new MultiHairdresser()
            {
                appointment = appointment,
                commentModel = commentModel1,
                commentsModel = commmentmodels,
                hairDresserModel = hairDresserModel
            };
            if (hairDresserModel == null)
            {
                return NotFound();
            }

            return View(model);
        }

        [Authorize(Roles = "saloon,user")]
        [Route("HairDresser/GetAppointments/{userid}")]
        public async Task<IActionResult> GetAppointments(string userid)
        {
            if (userid != GetCurrentUser().Result.Id)
            {
                return NotFound();
            }
            var model = await _context.Appointments.Where(x => x.UserID == userid).ToListAsync();
            var newmodel = new List<AppointmentModel>();
            foreach (var item in model)
            {
                newmodel.Add(new AppointmentModel()
                {
                    AppointmentDate = item.AppointmentDate,
                    HairDresser = await _context.HairDresser.FirstOrDefaultAsync(x=>x.ID == item.HairDresserID),
                    ID = item.ID,
                    Saloon = await _context.Saloon.FirstOrDefaultAsync(x=>x.ID == item.SaloonID),
                    User = await _userManager.FindByIdAsync(item.UserID)
                });
            }
            return View(newmodel);
        }

        [Authorize(Roles = "user")]
        [Route("HairDresser/DeleteAppointment/{appoid}")]
        public async Task<IActionResult> DeleteAppointment(string appoid)
        {

            var appo = await _context.Appointments.FirstOrDefaultAsync(x => x.ID == appoid);
            if (appo.UserID != GetCurrentUser().Result.Id)
            {
                return NotFound();
            }
            _context.Appointments.Remove(appo);
            await _context.SaveChangesAsync();
            var model = await _context.Appointments.Where(x => x.UserID == appo.UserID).ToListAsync();
            return RedirectToAction("GetAppointments", new { userid = appo.UserID });
        }

        [Authorize(Roles="saloon,user")]
        [HttpPost]
        public async Task<IActionResult> MakeAppointment(MultiHairdresser model)
        {
            string h = TempData["hairid"].ToString();
            string s = TempData["salonid"].ToString();
            var dene = model.appointment;
            dene.ID = Guid.NewGuid().ToString();
            dene.SaloonID = s;
            dene.HairDresserID = h;
            dene.UserID = dene.UserID = GetCurrentUser().Result.Id;           
            _context.Appointments.Add(dene);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details",new { id = h});
        }
        public async Task<ApplicationUser> GetCurrentUser()
        {
            return await _userManager.FindByIdAsync(_httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier));
        }


        [Authorize(Roles = "saloon,user")]

       
      

        [HttpPost]
        public async Task<IActionResult> MakeComment(MultiHairdresser model)
        {
            string h = TempData["hairid"].ToString();
            string s = TempData["salonid"].ToString();
            var dene = model.commentModel;
            dene.ID = Guid.NewGuid().ToString();
            dene.UserID = GetCurrentUser().Result.Id;
            dene.UserName = GetCurrentUser().Result.FirstName + " " + GetCurrentUser().Result.Surname;
            dene.Time = DateTime.Now;
            dene.HairDresserID = h;
            _context.CommentModels.Add(dene);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = h });
        }

        [Route("HairDresser/DeleteComment/{commid}")]
        public async Task<IActionResult> DeleteComment(string commid)
        {
            string h = TempData["hairid"].ToString();
            _context.CommentModels.Remove(await _context.CommentModels.FirstOrDefaultAsync(x => x.ID == commid));
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = h });
        }

        [Authorize(Roles = "saloon")]
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HairDresserModel model)
        {
            if (ModelState.IsValid)
            {
                var hairDresserModel = new HairDresser()
                {
                    
                    HairdresserDescription = model.HairdresserDescription,
                    HairdresserEmail = model.HairdresserEmail,
                    HairdresserName = model.HairdresserName,
                    HairdresserPhoneNumber = model.HairdresserPhoneNumber,
                    HairdresserSurname = model.HairdresserSurname
                };
                hairDresserModel.ID = Guid.NewGuid().ToString();
                hairDresserModel.SaloonID = TempData["sid"].ToString();
                string filePath = System.IO.Path.GetTempFileName();
                if (model.Image != null)
                {
                    using var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                    await model.Image.CopyToAsync(stream, System.Threading.CancellationToken.None);
                }
                hairDresserModel.Image = System.IO.File.ReadAllBytes(filePath);
                _context.HairDresser.Add(hairDresserModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details","Saloon", new { id = hairDresserModel.SaloonID });
            }
            return View(model);
        }

        [Authorize(Roles = "saloon")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hairDresserModel = await _context.HairDresser.FindAsync(id);
            if (hairDresserModel == null)
            {
                return NotFound();
            }
            return View(hairDresserModel);
        }

        // POST: HairDresser/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,SaloonID,HairdresserName,HairdresserSurname,HairdresserPhoneNumber,HairdresserEmail,HairdresserDescription")] HairDresserModel hairDresserModel)
        {
            if (id != hairDresserModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hairDresserModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HairDresserModelExists(hairDresserModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hairDresserModel);
        }

        [Authorize(Roles = "saloon")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hairDresserModel = await _context.HairDresser
                .FirstOrDefaultAsync(m => m.ID == id);
            var s = _context.Saloon.FirstOrDefaultAsync(x => x.ID == hairDresserModel.SaloonID).Result.SaloonOwnerID;
            var u = GetCurrentUser().Result.Id;
            if ( s != u )
            {
                return NotFound();
            }
            if (hairDresserModel == null)
            {
                return NotFound();
            }

            return View(hairDresserModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hairDresserModel = await _context.HairDresser.FindAsync(id);
            _context.HairDresser.Remove(hairDresserModel);
            
            foreach (var item in await _context.Appointments.Where(x => x.HairDresserID == id).ToListAsync())
            {
                _context.Appointments.Remove(item);
            }
            foreach(var item in await _context.CommentModels.Where(x => x.HairDresserID == id).ToListAsync())
            {
                _context.CommentModels.Remove(item);
            }            
            await _context.SaveChangesAsync();
            return RedirectToAction("Details","Saloon",new {id = hairDresserModel.SaloonID });
        }

        private bool HairDresserModelExists(string id)
        {
            return _context.HairDresser.Any(e => e.ID == id);
        }
    }
}
