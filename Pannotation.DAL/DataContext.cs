using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Domain.Entities.Logging;
using Pannotation.Domain.Entities.Order;
using Pannotation.Domain.Entities.OtherFiles;
using Pannotation.Domain.Entities.Payment;
using Pannotation.Domain.Entities.Subscription;
using System.Threading.Tasks;

namespace Pannotation.DAL
{
    public class DataContext : IdentityDbContext<ApplicationUser, ApplicationRole, int, IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            Database.SetCommandTimeout(500);
        }

        public virtual DbSet<UserToken> UserTokens { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<ApplicationUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<File> Files { get; set; }

        public virtual DbSet<Songsheet> Songsheets { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Instrument> Instruments { get; set; }
        public virtual DbSet<SongsheetGenre> SongsheetGenres { get; set; }
        public virtual DbSet<SongsheetInstrument> SongsheetInstruments { get; set; }
        public virtual DbSet<OrderSongsheet> OrderSongsheets { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
        
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<Subscription> Subscriptions { get; set; }

        public virtual DbSet<OtherFile> OtherFiles { get; set; }

        #region Queries

        #endregion

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        #region Fluent API

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<SongsheetGenre>(genre =>
            {
                genre.HasKey(sg => new { sg.SongsheetId, sg.GenreId });
            });

            modelBuilder.Entity<SongsheetInstrument>(instrument =>
            {
                instrument.HasKey(si => new { si.SongsheetId, si.InstrumentId });
            });

            modelBuilder.Entity<OrderSongsheet>(songsheet =>
            {
                songsheet.HasKey(os => new { os.OrderId, os.SongsheetId });
            });

            modelBuilder.Entity<CartItem>(cart =>
            {
                cart.HasKey(c => new { c.UserId, c.SongsheetId });
            });
        }

        #endregion
    }
}
