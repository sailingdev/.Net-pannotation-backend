using Pannotation.Domain.Entities;
using Pannotation.Domain.Entities.Identity;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Base;
using Pannotation.Models.ResponseModels.Songsheet;
using System;
using System.Linq;
using Profile = Pannotation.Domain.Entities.Identity.Profile;
using Pannotation.Common.Extensions;
using Pannotation.Domain.Entities.Order;
using Pannotation.Models.ResponseModels.Order;
using Pannotation.Models.Enums;
using Pannotation.Domain.Entities.Payment;
using Pannotation.Models.ResponseModels.Payment;
using Microsoft.Extensions.Configuration;
using Pannotation.Models.Export;
using Pannotation.Models.RequestModels.Payment;
using Pannotation.Models.ResponseModels.Subscription;
using Pannotation.Domain.Entities.Subscription;
using Pannotation.Domain.Entities.OtherFiles;
using Pannotation.Models.ResponseModels.OtherFiles;
using Pannotation.Models.RequestModels.OtherFiles;

namespace Pannotation.Services.StartApp
{
    public class AutoMapperProfileConfiguration : AutoMapper.Profile
    {
        private IConfiguration _configuration;

        public AutoMapperProfileConfiguration(IConfiguration configuration)
        : this("MyProfile")
        {
            _configuration = configuration;
        }

        protected AutoMapperProfileConfiguration(string profileName)
        : base(profileName)
        {
            CreateMap<UserProfileRequestModel, Profile>()
                .ForMember(t => t.Id, opt => opt.Ignore())
                .ForMember(t => t.User, opt => opt.Ignore())
                .ForMember(t => t.UserId, opt => opt.Ignore());

            CreateMap<ApplicationUser, UserProfileResponseModel>()
                .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.Profile.Avatar))
                .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.Profile.FirstName))
                .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.Profile.LastName))
                .ForMember(t => t.PhoneNumber, opt => opt.MapFrom(x => x.Profile.PhoneNumber))
                .ForMember(t => t.MobileNumber, opt => opt.MapFrom(x => x.Profile.MobileNumber))
                .ForMember(t => t.Country, opt => opt.MapFrom(x => x.Profile.Country))
                .ForMember(t => t.State, opt => opt.MapFrom(x => x.Profile.State))
                .ForMember(t => t.City, opt => opt.MapFrom(x => x.Profile.City))
                .ForMember(t => t.Address, opt => opt.MapFrom(x => x.Profile.Address))
                .ForMember(t => t.Zip, opt => opt.MapFrom(x => x.Profile.Zip))
                .ForMember(t => t.IsComposer, opt => opt.MapFrom(x => x.Profile.IsComposer))
                .ForMember(t => t.IdCode, opt => opt.MapFrom(x => x.Profile.IdCode))
                .ForMember(t => t.IsBlocked, opt => opt.MapFrom(x => !x.IsActive));

            CreateMap<Profile, UserProfileBaseResponseModel>()
                .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.Avatar));

            CreateMap<Profile, UserProfileResponseModel>()
                .ForMember(t => t.Avatar, opt => opt.MapFrom(x => x.Avatar))
                .ForMember(t => t.Email, opt => opt.MapFrom(x => x.User != null ? x.User.Email : ""))
                .ForMember(t => t.IsBlocked, opt => opt.MapFrom(x => x.User != null ? !x.User.IsActive : false));

            CreateMap<Image, ImageResponseModel>();

            CreateMap<File, FileResponseModel>();

            CreateMap<CreateSongsheetRequestModel, Songsheet>()
                .ForMember(t => t.IsActive, opt => opt.MapFrom(x => true))
                .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => DateTime.UtcNow))
                .ForMember(t => t.YouTubeLink, opt => opt.MapFrom(x => x.YouTubeLink.Replace("watch?v=", "embed/")))
                .ForMember(t => t.Instruments, opt => opt.Ignore())
                .ForMember(t => t.Genres, opt => opt.Ignore());

            CreateMap<Songsheet, TopSongsheetResponseModel>()
                .ForMember(t => t.Instruments, opt => opt.MapFrom(x => x.Instruments.Select(i => i.Instrument.Name)))
                .ForMember(t => t.Genres, opt => opt.MapFrom(x => x.Genres.Select(g => g.Genre.Name)));

            CreateMap<Songsheet, SongsheetListItemResponseModel>()
                .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
                .ForMember(t => t.Instruments, opt => opt.MapFrom(x => x.Instruments.Select(i => i.Instrument.Name)))
                .ForMember(t => t.Genres, opt => opt.MapFrom(x => x.Genres.Select(g => g.Genre.Name)));

            CreateMap<Songsheet, SongsheetDetailsBaseResponseModel>();

            CreateMap<Songsheet, SongsheetDetailsResponseModel>()
                .ForMember(t => t.Instruments, opt => opt.MapFrom(x => x.Instruments.Select(i => i.Instrument.Name)))
                .ForMember(t => t.Genres, opt => opt.MapFrom(x => x.Genres.Select(g => g.Genre.Name)));

            CreateMap<Songsheet, SongsheetResponseModel>()
                .ForMember(t => t.Instruments, opt => opt.MapFrom(x => x.Instruments.ToDictionary(i => i.InstrumentId, y => y.Instrument.Name)))
                .ForMember(t => t.Genres, opt => opt.MapFrom(x => x.Genres.ToDictionary(i => i.GenreId, y => y.Genre.Name)));

            CreateMap<Order, OrderItemListResponseModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(y => y.Id))
                .ForMember(x => x.BuyerId, opt => opt.MapFrom(y => y.UserId))
                .ForMember(x => x.FirstName, opt => opt.MapFrom(y => y.User.Profile.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(y => y.User.Profile.LastName))
                .ForMember(x => x.Email, opt => opt.MapFrom(y => y.User.Email))
                .ForMember(x => x.Date, opt => opt.MapFrom(y => y.CreatedAt.ToISO()))
                .ForMember(x => x.Songsheets, opt => opt.MapFrom(y => y.Songsheets.Select(x => x.Songsheet.Name)));

            CreateMap<Order, AdminOrderDetailsResponseModel>()
                .ForMember(t => t.BuyerId, opt => opt.MapFrom(x => x.UserId))
                .ForMember(t => t.Date, opt => opt.MapFrom(x => x.CreatedAt.ToISO()))
                .ForMember(t => t.Email, opt => opt.MapFrom(x => x.User.Email))
                .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.User.Profile.FirstName))
                .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.User.Profile.LastName))
                .ForMember(t => t.PhoneNumber, opt => opt.MapFrom(x => x.User.Profile.PhoneNumber))
                .ForMember(t => t.MobileNumber, opt => opt.MapFrom(x => x.User.Profile.MobileNumber))
                .ForMember(t => t.Songsheets, opt => opt.MapFrom(x => x.Songsheets))
                .ForMember(t => t.TransactionInfo, opt => opt.MapFrom(x => x.Transactions.OrderByDescending(y => y.CreatedAt).FirstOrDefault()));

            CreateMap<OrderSongsheet, SongsheetShortResponseModel>()
                .ForMember(t => t.Price, opt => opt.MapFrom(x => x.PriceAtPurchaseMoment))
                .ForMember(t => t.Id, opt => opt.MapFrom(x => x.SongsheetId))
                .ForMember(t => t.Name, opt => opt.MapFrom(x => x.Songsheet.Name));

            CreateMap<Transaction, TransactionInfoResponseModel>()
                .ForMember(t => t.CardMask, opt => opt.MapFrom(x => x.CardMask.Substring(x.CardMask.Length - 4)))
                .ForMember(t => t.PaymentProcessor, opt => opt.MapFrom(x => _configuration["FAC:PaymentProcessor"]));

            CreateMap<Order, OrderExportModel>()
                .ForMember(t => t.BuyerName, opt => opt.MapFrom(x => $"{x.User.Profile.FirstName} {x.User.Profile.LastName}"))
                .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt.ToString("dd-MM-yyyy")))
                .ForMember(t => t.OrderStatus, opt => opt.MapFrom(x => x.Transactions.Any(y => y.TransactionStatus == TransactionStatus.Successfull) ? "Paid" : "Unpaid"))
                .ForMember(t => t.CardType, opt => opt.MapFrom(x => x.Transactions.Any() ? x.Transactions.OrderByDescending(y => y.CreatedAt).FirstOrDefault().CardType : null))
                .ForMember(t => t.CardMask, opt => opt.MapFrom(x => x.Transactions.Any() ? new string(x.Transactions.OrderByDescending(y => y.CreatedAt).FirstOrDefault().CardMask.TakeLast(4).ToArray()) : null))
                .ForMember(t => t.Songsheets, opt => opt.MapFrom(x => x.Songsheets));

            CreateMap<OrderSongsheet, SongsheetExportModel>()
                .ForMember(t => t.Id, opt => opt.MapFrom(x => x.SongsheetId))
                .ForMember(t => t.Arranger, opt => opt.MapFrom(x => x.Songsheet.Arranger))
                .ForMember(t => t.ArtistName, opt => opt.MapFrom(x => x.Songsheet.ArtistName))
                .ForMember(t => t.CreatedAt, opt => opt.MapFrom(x => x.Songsheet.CreatedAt))
                .ForMember(t => t.Description, opt => opt.MapFrom(x => x.Songsheet.Description))
                .ForMember(t => t.Name, opt => opt.MapFrom(x => x.Songsheet.Name))
                .ForMember(t => t.Price, opt => opt.MapFrom(x => x.PriceAtPurchaseMoment))
                .ForMember(t => t.Producer, opt => opt.MapFrom(x => x.Songsheet.Producer))
                .ForMember(t => t.YouTubeLink, opt => opt.MapFrom(x => x.Songsheet.YouTubeLink))
                .ForMember(t => t.Genres, opt => opt.MapFrom(x => x.Songsheet.Genres.Select(y => y.Genre.Name)))
                .ForMember(t => t.Instruments, opt => opt.MapFrom(x => x.Songsheet.Instruments.Select(y => y.Instrument.Name)));

            CreateMap<Songsheet, CartSongsheetResponseModel>()
                .ForMember(t => t.Genres, opt => opt.MapFrom(x => x.Genres != null ? x.Genres.Select(y => y.Genre.Name) : null))
                .ForMember(t => t.Instruments, opt => opt.MapFrom(x => x.Instruments != null ? x.Instruments.Select(y => y.Instrument.Name) : null));

            CreateMap<AddressRequestModel, AddressResponseModel>();

            CreateMap<Subscription, SubscriptionTableRowResponseModel>()
                .ForMember(t => t.Email, opt => opt.MapFrom(x => x.User.Email))
                .ForMember(t => t.FirstName, opt => opt.MapFrom(x => x.User.Profile.FirstName))
                .ForMember(t => t.LastName, opt => opt.MapFrom(x => x.User.Profile.LastName))
                .ForMember(t => t.Country, opt => opt.MapFrom(x => x.Transactions.Any() ? x.Transactions.OrderByDescending(w => w.CreatedAt).First().CountryName : null))
                .ForMember(t => t.PurchaseDate, opt => opt.MapFrom(x => x.PurchasedAt));

            CreateMap<Subscription, SubscriptionDetailsResponseModel>()
                .IncludeBase<Subscription, SubscriptionTableRowResponseModel>()
                .ForMember(t => t.PhoneNumber, opt => opt.MapFrom(x => x.User.Profile.PhoneNumber))
                .ForMember(t => t.MobileNumber, opt => opt.MapFrom(x => x.User.Profile.MobileNumber))
                .ForMember(t => t.IdCode, opt => opt.MapFrom(x => x.User.Profile.IsComposer ? x.User.Profile.IdCode : null))
                .ForMember(t => t.TransactionInfo, opt => opt.MapFrom(x => x.Transactions.OrderByDescending(w => w.CreatedAt).FirstOrDefault(w => w.TransactionStatus == TransactionStatus.Successfull)));

            CreateMap<Subscription, SubscriptionExportModel>()
                .ForMember(t => t.BuyerName, opt => opt.MapFrom(x => $"{x.User.Profile.FirstName} {x.User.Profile.LastName}"))
                .ForMember(t => t.PurchasedAt, opt => opt.MapFrom(x => x.PurchasedAt.HasValue ? x.PurchasedAt.Value.ToString("dd-MM-yyyy") : null))
                .ForMember(t => t.ExpiredAt, opt => opt.MapFrom(x => x.NextPaymentDate.HasValue ? x.NextPaymentDate.Value.ToString("dd-MM-yyyy") : null))
                .ForMember(t => t.SubscriptionStatus, opt => opt.MapFrom(x => x.NextPaymentDate.HasValue ? "Active" : "Expired"))
                .ForMember(t => t.PurchaseStatus, opt => opt.MapFrom(x => x.Transactions.Any(y => y.TransactionStatus == TransactionStatus.Successfull) ? "Paid" : "Unpaid"))
                .AfterMap((src, dest) =>
                {
                    var transaction = src.Transactions.OrderByDescending(y => y.CreatedAt).FirstOrDefault();
                    dest.CardType = transaction?.CardType;
                    dest.CardMask = (transaction != null) ? new string(transaction.CardMask.TakeLast(4).ToArray()) : null;
                    dest.Amount = transaction?.Amount;
                });

            CreateMap<PaymentRequestModel, AddressResponseModel>()
                .ForMember(t => t.Country, opt => opt.MapFrom(x => x.CountryCode));

            CreateMap<OtherFile, OtherFileBaseResponseModel>();

            CreateMap<OtherFile, OtherFileResponseModel>();

            CreateMap<EditOtherFileRequestModel, OtherFile>();
        }
    }
}
