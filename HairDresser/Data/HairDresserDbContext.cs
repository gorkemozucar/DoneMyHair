using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HairDresser1.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HairDresser1.Data
{
    public class HairDresserDbContext : IdentityDbContext<ApplicationUser>
    {
        public HairDresserDbContext (DbContextOptions<HairDresserDbContext> options)
            : base(options)
        {
        }

        public DbSet<Saloon> Saloon { get; set; }    
        public DbSet<HairDresser> HairDresser { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<CommentModel> CommentModels { get; set; }

    }
}
