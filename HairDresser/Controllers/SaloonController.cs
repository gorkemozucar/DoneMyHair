using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HairDresser1.Data;
using HairDresser1.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HairDresser1.Controllers
{
    public class SaloonController : Controller
    {
        private readonly HairDresserDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContext;
        public SaloonController(HairDresserDbContext context, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {
            _context = context;
            _userManager = userManager;
            _httpContext = httpContext;
        }
        public async Task<ApplicationUser> GetCurrentUser()
        {
            var userId = _httpContext.HttpContext.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            return user;
        }
        // GET: Saloon
        public async Task<IActionResult> Index()
        {
            return View(await _context.Saloon.ToListAsync());
        }

        public async Task<SaloonModel> GetSaloonbyID(string id)
        {
            return await _context.Saloon.Where(m => m.ID == id).Select(x => new SaloonModel()
            {
                ID = x.ID,
                Description = x.Description,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                SaloonAdress = x.SaloonAdress,
                SaloonName = x.SaloonName,
                SaloonOwnerID = x.SaloonOwnerID
            }).FirstOrDefaultAsync();

        }
        public async Task<ApplicationUser> GetUserbyID(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        // GET: Saloon/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Saloon = await _context.Saloon.Where(m => m.ID == id).FirstOrDefaultAsync();
            ViewBag.dressers = await _context.HairDresser.Where(x => x.SaloonID == Saloon.ID).ToListAsync();
            if (Saloon == null)
            {
                return NotFound();
            }

            return View(Saloon);
        }

        [Authorize(Roles = "saloon")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Saloon/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaloonModel model)
        {
            if (ModelState.IsValid)
            {
                var user = GetCurrentUser().Result;
                var saloon = new Saloon()
                {
                    Description = model.Description,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    SaloonAdress = model.SaloonAdress,
                    SaloonName = model.SaloonName,
                    SaloonOwnerID = user.Id,
                    SaloonOwnerName = user.FirstName + " " + user.Surname
                };
                string filePath = System.IO.Path.GetTempFileName();
                if (model.Image != null)
                {
                    using var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                    await model.Image.CopyToAsync(stream, System.Threading.CancellationToken.None);
                }
                saloon.Images = System.IO.File.ReadAllBytes(filePath);
                saloon.ID = Guid.NewGuid().ToString();
                _context.Add(saloon);
                await _context.SaveChangesAsync();
                ViewBag.dressers = new List<HairDresser>();
                return RedirectToAction("Details", new { id = saloon.ID });
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

            var Saloon = await _context.Saloon.Where(z => z.ID == id).Select(x => new SaloonModel()
            {
                ID = x.ID,
                Description = x.Description,
                Email = x.Email,
                PhoneNumber = x.PhoneNumber,
                SaloonName = x.SaloonName,
                SaloonAdress = x.SaloonAdress,
                SaloonOwnerID = x.SaloonOwnerID,
                SaloonOwnerName = x.SaloonOwnerName
            }).FirstOrDefaultAsync();
            if (Saloon == null)
            {
                return NotFound();
            }
            return View(Saloon);
        }

        // POST: Saloon/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SaloonModel model)
        {
            Saloon saloon;
            if (id != model.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    saloon = new Saloon()
                    {
                        ID = model.ID,
                        Description = model.Description,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        SaloonName = model.SaloonName,
                        SaloonAdress = model.SaloonAdress,
                        SaloonOwnerID = model.SaloonOwnerID,
                        SaloonOwnerName = model.SaloonOwnerName
                    };
                    string filePath = System.IO.Path.GetTempFileName();
                    if (model.Image != null)
                    {
                        using var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create);
                        await model.Image.CopyToAsync(stream, System.Threading.CancellationToken.None);
                    }

                    saloon.Images = System.IO.File.ReadAllBytes(filePath);

                    _context.Saloon.Update(saloon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SaloonExists(model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = saloon.ID });
            }
            return View(model);
        }

        [Authorize(Roles = "saloon")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Saloon = await _context.Saloon
                .FirstOrDefaultAsync(m => m.ID == id);
            if (Saloon.SaloonOwnerID != GetCurrentUser().Result.Id)
            {
                return NotFound();
            }
            if (Saloon == null)
            {
                return NotFound();
            }

            return View(Saloon);
        }

        // POST: Saloon/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var Saloon = await _context.Saloon.FindAsync(id);
            _context.Saloon.Remove(Saloon);
            var appo = _context.Appointments.Where(x => x.SaloonID == Saloon.ID).ToListAsync().Result;
            var hair = _context.HairDresser.Where(x => x.SaloonID == Saloon.ID).ToListAsync().Result;
            List<CommentModel> comm = new();
            foreach (var item in hair)
            {
                comm = _context.CommentModels.Where(x => x.HairDresserID == item.ID).ToListAsync().Result;
            }
            foreach (var item in hair)
            {
                _context.HairDresser.Remove(item);
            }
            foreach (var item in comm)
            {
                _context.CommentModels.Remove(item);
            }
            foreach (var item in appo)
            {
                _context.Appointments.Remove(item);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SaloonExists(string id)
        {
            return _context.Saloon.Any(e => e.ID == id);
        }


        public async Task<IActionResult> GetAllAppointments(string id)
        {
            var model = await _context.Appointments.Where(x => x.SaloonID == id).ToListAsync();
            var apolist = new List<AppointmentModel>();
            foreach (var item in model)
            {
                apolist.Add(new AppointmentModel()
                {
                    ID = item.ID,
                    User = _userManager.FindByIdAsync(item.UserID).Result,
                    AppointmentDate = item.AppointmentDate,
                    HairDresser = _context.HairDresser.FirstOrDefaultAsync(x => x.ID == item.HairDresserID).Result,
                    Saloon = _context.Saloon.FirstOrDefaultAsync(x => x.ID == item.SaloonID).Result
                });
            }
            return View(apolist);
        }

        [Authorize(Roles = "saloon,user")]
        [Route("Saloon/DeleteAppointment/{appoid}")]
        public async Task<IActionResult> DeleteAppointment(string appoid)
        {
            var appo = await _context.Appointments.FirstOrDefaultAsync(x => x.ID == appoid);
            _context.Appointments.Remove(appo);
            await _context.SaveChangesAsync();
            return RedirectToAction("GetAllAppointments", new { id = appo.SaloonID });
        }
    }
}
