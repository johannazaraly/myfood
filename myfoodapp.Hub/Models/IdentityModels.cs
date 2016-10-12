using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace myfoodapp.Hub.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.Message> Messages { get; set; }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.MessageType> MessageTypes { get; set; }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.Measure> Measures { get; set; }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.SensorType> SensorTypes { get; set; }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.ProductionUnit> ProductionUnits { get; set; }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.ProductionUnitType> ProductionUnitTypes { get; set; }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.Option> Options { get; set; }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.OptionList> OptionList { get; set; }

        public System.Data.Entity.DbSet<myfoodapp.Hub.Models.ProductionUnitOwner> ProductionUnitOwner { get; set; }

    }
}