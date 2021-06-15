using Pannotation.Domain.Entities;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Domain.Entities.Logging;
using Pannotation.Domain.Entities.Order;
using Pannotation.Domain.Entities.OtherFiles;
using Pannotation.Domain.Entities.Payment;
using Pannotation.Domain.Entities.Subscription;
using System;
using System.Threading.Tasks;

namespace Pannotation.DAL.Abstract
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<ApplicationRole> RoleRepository { get; }
        IRepository<ApplicationUser> UserRepository { get; }
        IRepository<ApplicationUserRole> UserRoleRepository { get; }
        IRepository<Profile> ProfileRepository { get; }
        IRepository<UserToken> UserTokenRepository { get; }
        IRepository<Image> ImageRepository { get; }
        IRepository<File> FileRepository { get; }
        IRepository<EmailLog> EmailLogRepository { get; }

        IRepository<Songsheet> SongsheetRepository { get; }
        IRepository<Instrument> InstrumentRepository { get; }
        IRepository<Genre> GenreRepository { get; }
        IRepository<Order> OrderRepository { get; }
        IRepository<CartItem> CartItemRepository { get; }
        
        IRepository<Transaction> TransactionRepository { get; }
        IRepository<Subscription> SubscriptionRepository { get; }

        IRepository<OtherFile> OtherFileRepository { get; }
        IRepository<SongsheetGenre> SongsheetGenreRepository { get; }
        IRepository<SongsheetInstrument> SongsheetInstrumentRepository { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
