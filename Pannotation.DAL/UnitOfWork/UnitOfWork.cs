using Microsoft.Extensions.DependencyInjection;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Domain.Entities.Logging;
using Pannotation.Domain.Entities.Order;
using Pannotation.Domain.Entities.OtherFiles;
using Pannotation.Domain.Entities.Payment;
using Pannotation.Domain.Entities.Subscription;
using System;
using System.Threading.Tasks;

namespace Pannotation.DAL.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private IRepository<ApplicationRole> _roleRepository;
        private IRepository<ApplicationUser> _userRepository;
        private IRepository<ApplicationUserRole> _userRoleRepository;
        private IRepository<Profile> _profileRepository;
        private IRepository<UserToken> _userTokenRepository;
        private IRepository<Image> _imageRepository;
        private IRepository<File> _fileRepository;
        private IRepository<EmailLog> _emailLogRepository;
        private IRepository<Songsheet> _songsheetRepository;
        private IRepository<Instrument> _instrumentRepository;
        private IRepository<Genre> _genreRepository;
        private IRepository<Order> _orderRepository;
        private IRepository<Transaction> _transactionRepository;
        private IRepository<CartItem> _cartItemRepository;
        private IRepository<Subscription> _subscriptionRepository;
        private IRepository<OtherFile> _otherFileRepository;
        private IRepository<SongsheetGenre> _songsheetGenreRepository;
        private IRepository<SongsheetInstrument> _songsheetInstrumentRepository;

        private readonly IServiceProvider _serviceProvider;
        private readonly IDataContext _context;

        public UnitOfWork(IServiceProvider serviceProvider, IDataContext context)
        {
            _serviceProvider = serviceProvider;
            _context = context;
        }

        #region Repository Getters


        public IRepository<ApplicationRole> RoleRepository
        {
            get
            {
                if (_roleRepository == null)
                    _roleRepository = _serviceProvider.GetRequiredService<IRepository<ApplicationRole>>();
                return _roleRepository;
            }
        }

        public IRepository<ApplicationUser> UserRepository
        {
            get
            {
                if (_userRepository == null)
                    _userRepository = _serviceProvider.GetRequiredService<IRepository<ApplicationUser>>();
                return _userRepository;
            }
        }

        public IRepository<ApplicationUserRole> UserRoleRepository
        {
            get
            {
                if (_userRoleRepository == null)
                    _userRoleRepository = _serviceProvider.GetRequiredService<IRepository<ApplicationUserRole>>();
                return _userRoleRepository;
            }
        }

        public IRepository<Profile> ProfileRepository
        {
            get
            {
                if (_profileRepository == null)
                    _profileRepository = _serviceProvider.GetRequiredService<IRepository<Profile>>();
                return _profileRepository;
            }
        }

        public IRepository<UserToken> UserTokenRepository
        {
            get
            {
                if (_userTokenRepository == null)
                    _userTokenRepository = _serviceProvider.GetRequiredService<IRepository<UserToken>>();
                return _userTokenRepository;
            }
        }

        public IRepository<Image> ImageRepository
        {
            get
            {
                if (_imageRepository == null)
                    _imageRepository = _serviceProvider.GetRequiredService<IRepository<Image>>();
                return _imageRepository;
            }
        }

        public IRepository<File> FileRepository
        {
            get
            {
                if (_fileRepository == null)
                    _fileRepository = _serviceProvider.GetRequiredService<IRepository<File>>();
                return _fileRepository;
            }
        }

        public IRepository<EmailLog> EmailLogRepository
        {
            get
            {
                if (_emailLogRepository == null)
                    _emailLogRepository = _serviceProvider.GetRequiredService<IRepository<EmailLog>>();
                return _emailLogRepository;
            }
        }

        public IRepository<Order> OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                    _orderRepository = _serviceProvider.GetRequiredService<IRepository<Order>>();
                return _orderRepository;
            }
        }

        public IRepository<Instrument> InstrumentRepository
        {
            get
            {
                if (_instrumentRepository == null)
                    _instrumentRepository = _serviceProvider.GetRequiredService<IRepository<Instrument>>();
                return _instrumentRepository;
            }
        }

        public IRepository<Genre> GenreRepository
        {
            get
            {
                if (_genreRepository == null)
                    _genreRepository = _serviceProvider.GetRequiredService<IRepository<Genre>>();
                return _genreRepository;
            }
        }

        public IRepository<Songsheet> SongsheetRepository
        {
            get
            {
                if (_songsheetRepository == null)
                    _songsheetRepository = _serviceProvider.GetRequiredService<IRepository<Songsheet>>();
                return _songsheetRepository;
            }
        }

        public IRepository<Transaction> TransactionRepository
        {
            get
            {
                if (_transactionRepository == null)
                    _transactionRepository = _serviceProvider.GetRequiredService<IRepository<Transaction>>();
                return _transactionRepository;
            }
        }

        public IRepository<CartItem> CartItemRepository
        {
            get
            {
                if (_cartItemRepository == null)
                    _cartItemRepository = _serviceProvider.GetRequiredService<IRepository<CartItem>>();
                return _cartItemRepository;
            }
        }

        public IRepository<Subscription> SubscriptionRepository
        {
            get
            {
                if (_subscriptionRepository == null)
                    _subscriptionRepository = _serviceProvider.GetRequiredService<IRepository<Subscription>>();
                return _subscriptionRepository;
            }
        }

        public IRepository<OtherFile> OtherFileRepository
        {
            get
            {
                if (_otherFileRepository == null)
                    _otherFileRepository = _serviceProvider.GetRequiredService<IRepository<OtherFile>>();
                return _otherFileRepository;
            }
        }

        public IRepository<SongsheetGenre> SongsheetGenreRepository
        {
            get
            {
                if (_songsheetGenreRepository == null)
                    _songsheetGenreRepository = _serviceProvider.GetRequiredService<IRepository<SongsheetGenre>>();
                return _songsheetGenreRepository;
            }
        }

        public IRepository<SongsheetInstrument> SongsheetInstrumentRepository
        {
            get
            {
                if (_songsheetInstrumentRepository == null)
                    _songsheetInstrumentRepository = _serviceProvider.GetRequiredService<IRepository<SongsheetInstrument>>();
                return _songsheetInstrumentRepository;
            }
        }

        #endregion

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).                  
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
