using AutoMapper;
using Ionic.Zip;
using Microsoft.EntityFrameworkCore;
using Pannotation.Common.Exceptions;
using Pannotation.Common.Extensions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Order;
using Pannotation.Models.Enums;
using Pannotation.Models.Export;
using Pannotation.Models.InternalModels;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Order;
using Pannotation.Models.ResponseModels.Payment;
using Pannotation.Models.ResponseModels.Songsheet;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.Export;
using Pannotation.Services.Interfaces.External;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class OrderService : IOrderService
    {
        private IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private IExportProvider<List<OrderExportModel>> _exportProvider;
        private IS3Service _s3Service;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, IExportProvider<List<OrderExportModel>> exportProvider, IS3Service s3Service)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _exportProvider = exportProvider;
            _s3Service = s3Service;
        }

        public PaginationResponseModel<OrderItemListResponseModel> GetOrders(OrderListRequestModel model)
        {
            var modelQuery = model.Search?.Trim();
            var isQueryEmpty = string.IsNullOrEmpty(modelQuery);

            IQueryable<Order> orders = _unitOfWork.OrderRepository.Get(x => (!model.DateFrom.HasValue || x.CreatedAt.Date >= model.DateFrom) &&
                (!model.DateTo.HasValue || x.CreatedAt.Date <= model.DateTo)
                    && (isQueryEmpty || (EF.Functions.Like(x.User.Profile.FirstName, $"%{modelQuery}%")
                        || EF.Functions.Like(x.User.Profile.LastName, $"%{modelQuery}%")
                        || EF.Functions.Like(x.User.Email, $"%{modelQuery}%")
                        || x.Songsheets.Any(y => EF.Functions.Like(y.Songsheet.Name, $"%{modelQuery}%")))))
                .TagWith(nameof(GetOrders))
                .Include(x => x.Songsheets)
                    .ThenInclude(x => x.Songsheet)
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile);

            var count = orders.Count();
            if (model.Order != null)
            {
                var isAscending = model.Order.Direction == SortingDirection.Asc;

                switch (model.Order.Key)
                {
                    case OrderListKey.FirstName:
                        orders = isAscending ? orders.OrderBy(x => x.User.Profile.FirstName) : orders.OrderByDescending(x => x.User.Profile.FirstName);
                        break;
                    case OrderListKey.LastName:
                        orders = isAscending ? orders.OrderBy(x => x.User.Profile.LastName) : orders.OrderByDescending(x => x.User.Profile.LastName);
                        break;
                    case OrderListKey.Email:
                        orders = isAscending ? orders.OrderBy(x => x.User.Email) : orders.OrderByDescending(x => x.User.Email);
                        break;
                    case OrderListKey.Amount:
                        orders = isAscending ? orders.OrderBy(x => x.Amount) : orders.OrderByDescending(x => x.Amount);
                        break;
                    case OrderListKey.Date:
                        orders = isAscending ? orders.OrderBy(x => x.CreatedAt) : orders.OrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        orders = isAscending ? orders.OrderBy(x => x.Id) : orders.OrderByDescending(x => x.Id);
                        break;
                }
            }
            else
            {
                orders = orders.OrderBy(x => x.Id);
            }

            var response = _mapper.Map<List<OrderItemListResponseModel>>(orders.Skip(model.Offset).Take(model.Limit).ToList());

            return new PaginationResponseModel<OrderItemListResponseModel>(response, count);
        }

        public async Task<PaginationResponseModel<OrderSongsheetResponseModel>> GetOrderSongsheets(int userId, PaginationBaseRequestModel model)
        {
            var orders = _unitOfWork.OrderRepository.Get(x => x.UserId == userId && x.Transactions.Any(w => w.TransactionStatus == TransactionStatus.Successfull))
               .TagWith(nameof(GetOrderSongsheets))
               .Include(x => x.Songsheets)
                   .ThenInclude(x => x.Songsheet)
                        .ThenInclude(x => x.File)
               .OrderByDescending(x => x.CreatedAt);

            var count = orders.Sum(x => x.Songsheets.Count());

            var response = orders.SelectMany(x => x.Songsheets.Select(y => new OrderSongsheetResponseModel
            {
                Id = y.Songsheet.Id,
                Date = x.CreatedAt.ToISO(),
                Name = y.Songsheet.Name,
                Price = y.Songsheet.Price,
                ArtistName = y.Songsheet.ArtistName,
                Arranger = y.Songsheet.Arranger,
                File = _mapper.Map<FileResponseModel>(y.Songsheet.File)
            })).Skip(model.Offset).Take(model.Limit).ToList();

            return new PaginationResponseModel<OrderSongsheetResponseModel>(response, count);
        }

        public AdminOrderDetailsResponseModel GetOrderDetails(int orderId)
        {
            var order = _unitOfWork.OrderRepository.Get(x => x.Id == orderId)
                .TagWith(nameof(GetOrders))
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.Songsheets)
                    .ThenInclude(x => x.Songsheet)
                .Include(x => x.Transactions)
                .FirstOrDefault();

            if (order == null)
                throw new CustomException(HttpStatusCode.NotFound, "orderId", "Order is not found");

            var response = _mapper.Map<AdminOrderDetailsResponseModel>(order);
            return response;
        }


        public OrderDetailsResponseModel GetUserOrderDetails(int orderId)
        {
            var order = _unitOfWork.OrderRepository.Get(x => x.Id == orderId)
                .TagWith(nameof(GetOrders))
                .Include(x => x.Songsheets)
                    .ThenInclude(x => x.Songsheet)
                        .ThenInclude(x => x.Image)
                .Include(x => x.Songsheets)
                    .ThenInclude(x => x.Songsheet)
                        .ThenInclude(x => x.Genres)
                            .ThenInclude(x => x.Genre)
                  .Include(x => x.Songsheets)
                    .ThenInclude(x => x.Songsheet)
                        .ThenInclude(x => x.Instruments)
                            .ThenInclude(x => x.Instrument)
                .Include(x => x.Transactions)
                .FirstOrDefault();

            if (order == null)
                throw new CustomException(HttpStatusCode.NotFound, "orderId", "Order is not found");

            var songsheetsFromCart = order.Songsheets.Select(x => x.Songsheet).ToList();

            var result = new OrderDetailsResponseModel
            {
                OrderId = order.Id,
                OrderAmount = order.Amount,
                Songsheets = _mapper.Map<List<CartSongsheetResponseModel>>(songsheetsFromCart)
            };

            var transaction = order.Transactions.OrderByDescending(x => x.CreatedAt).FirstOrDefault();
            if (transaction != null)
            {
                result.Status = transaction.TransactionStatus;
                result.PaymentMethod = new PaymentMethodResponseModel
                {
                    CardMask = transaction.CardMask,
                    CardType = transaction.CardType
                };
            }

            return result;
        }

        public byte[] ExportOrders(ExportListRequestModel<OrderListKey> model)
        {
            var search = model.Search?.Trim();
            var isQueryEmpty = string.IsNullOrEmpty(search);

            IQueryable<Order> orders = _unitOfWork.OrderRepository.Get(x => (!model.DateFrom.HasValue || x.CreatedAt.Date >= model.DateFrom) &&
                (!model.DateTo.HasValue || x.CreatedAt.Date <= model.DateTo)
                    && (isQueryEmpty || (EF.Functions.Like(x.User.Profile.FirstName, $"%{search}%")
                        || EF.Functions.Like(x.User.Profile.LastName, $"%{search}%")
                        || EF.Functions.Like(x.User.Email, $"%{search}%")
                        || x.Songsheets.Any(y => EF.Functions.Like(y.Songsheet.Name, $"%{search}%")))))
                .TagWith(nameof(GetOrders))
                .Include(x => x.Songsheets)
                    .ThenInclude(x => x.Songsheet)
                        .ThenInclude(x => x.Genres)
                            .ThenInclude(x => x.Genre)
                 .Include(x => x.Songsheets)
                    .ThenInclude(x => x.Songsheet)
                        .ThenInclude(x => x.Instruments)
                            .ThenInclude(x => x.Instrument)
                .Include(x => x.User)
                    .ThenInclude(x => x.Profile)
                .Include(x => x.Transactions);

            if (model.Order != null)
            {
                var isAscending = model.Order.Direction == SortingDirection.Asc;

                switch (model.Order.Key)
                {
                    case OrderListKey.FirstName:
                        orders = isAscending ? orders.OrderBy(x => x.User.Profile.FirstName) : orders.OrderByDescending(x => x.User.Profile.FirstName);
                        break;
                    case OrderListKey.LastName:
                        orders = isAscending ? orders.OrderBy(x => x.User.Profile.LastName) : orders.OrderByDescending(x => x.User.Profile.LastName);
                        break;
                    case OrderListKey.Email:
                        orders = isAscending ? orders.OrderBy(x => x.User.Email) : orders.OrderByDescending(x => x.User.Email);
                        break;
                    case OrderListKey.Amount:
                        orders = isAscending ? orders.OrderBy(x => x.Amount) : orders.OrderByDescending(x => x.Amount);
                        break;
                    case OrderListKey.Date:
                        orders = isAscending ? orders.OrderBy(x => x.CreatedAt) : orders.OrderByDescending(x => x.CreatedAt);
                        break;
                    default:
                        orders = isAscending ? orders.OrderBy(x => x.Id) : orders.OrderByDescending(x => x.Id);
                        break;
                }
            }

            var exportedList = _mapper.Map<List<OrderExportModel>>(orders.ToList());
            var response = _exportProvider.Export(exportedList, ExportType.Csv);
            return response;
        }

        public async Task<SongsheetsFiles> DownloadSongsheets(int orderId, int userId)
        {
            var order = _unitOfWork.OrderRepository.Get(x => x.Id == orderId && x.UserId == userId)
                .Include(x => x.User)
                .Include(x => x.Transactions)
                .Include(x => x.Songsheets)
                    .ThenInclude(x => x.Songsheet)
                        .ThenInclude(x => x.File)
                .FirstOrDefault();

            if (order == null)
                throw new CustomException(HttpStatusCode.NotFound, "", "Order is not found");

            if (!order.Transactions.Any(x => x.TransactionStatus == TransactionStatus.Successfull))
                throw new CustomException(HttpStatusCode.BadRequest, "", "You should pay for songsheet first");

            if (!order.Songsheets.Any())
                throw new CustomException(HttpStatusCode.BadRequest, "", "There is no songsheets on the order");

            var songsheetsFileNames = order.Songsheets.Select(x => Path.GetFileName(x.Songsheet.File.Path));

            var response = new SongsheetsFiles();

            if (songsheetsFileNames.Count() == 1)
            {
                var fileName = songsheetsFileNames.First();
                response.Content = await _s3Service.ReadFile(fileName);
                response.ContentType = ContentType.Pdf;
                response.SingleSongsheetName = fileName;
                return response;
            }
            else
            {
                using (var archiveStream = new MemoryStream())
                {
                    using (var archive = new ZipOutputStream(archiveStream))
                    {
                        foreach (var fileName in songsheetsFileNames)
                        {
                            archive.PutNextEntry(fileName);
                            var bytes = await _s3Service.ReadFile(fileName);
                            archive.Write(bytes, 0, bytes.Length);
                        }
                    }

                    response.Content = archiveStream.ToArray();
                    response.ContentType = ContentType.Zip;
                    return response;
                }
            }
        }
    }
}
