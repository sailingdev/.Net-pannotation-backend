using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pannotation.Common.Exceptions;
using Pannotation.DAL.Abstract;
using Pannotation.Domain.Entities.Order;
using Pannotation.Domain.Entities.Payment;
using Pannotation.Models.Enums;
using Pannotation.Models.InternalModels;
using Pannotation.Models.RequestModels.Payment;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Order;
using Pannotation.Models.ResponseModels.Songsheet;
using Pannotation.Services.Interfaces;
using Pannotation.Services.Interfaces.External;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Pannotation.Services.Services
{
    public class CartService : ICartService
    {
        private IUnitOfWork _unitOfWork;
        private IFACService _facService;
        private IOrderService _orderService;
        private IMapper _mapper;

        public CartService(IUnitOfWork unitOfWork, IFACService facService, IOrderService orderService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _facService = facService;
            _orderService = orderService;
            _mapper = mapper;
        }

        public CartSongsheetResponseModel AddProductToCart(int songsheetId, int userId)
        {
            var user = _unitOfWork.UserRepository.Find(x => x.Id == userId);

            if (user == null)
                throw new CustomException(HttpStatusCode.NotFound, "userId", "User is not found");

            if (!_unitOfWork.SongsheetRepository.Any(x => x.IsActive && x.Id == songsheetId))
                throw new CustomException(HttpStatusCode.NotFound, "songsheetId", "Songsheet is not found");

            if (_unitOfWork.CartItemRepository.Any(x => x.SongsheetId == songsheetId && x.UserId == userId))
                throw new CustomException(HttpStatusCode.BadRequest, "", "Songsheet has already been added to the cart");

            var cartItem = new CartItem
            {
                User = user,
                SongsheetId = songsheetId
            };

            _unitOfWork.CartItemRepository.Insert(cartItem);
            _unitOfWork.SaveChanges();

            var songsheet = _unitOfWork.SongsheetRepository.Get(x => x.IsActive && x.Id == songsheetId)
                .Include(x => x.Image)
                .Include(x => x.Instruments)
                    .ThenInclude(x => x.Instrument)
                .Include(x => x.Genres)
                    .ThenInclude(x => x.Genre)
                .FirstOrDefault();

            var response = _mapper.Map<CartSongsheetResponseModel>(songsheet);
            return response;
        }

        public CartResponseModel GetCart(int userId)
        {
            var user = _unitOfWork.UserRepository.Find(x => x.Id == userId);

            if (user == null)
                throw new CustomException(HttpStatusCode.NotFound, "userId", "User is not found");

            var cartItems = _unitOfWork.CartItemRepository.Get(x => x.UserId == userId)
                .Select(x => x.Songsheet)
                .Include(x => x.Image)
                .Include(x => x.Instruments)
                    .ThenInclude(x => x.Instrument)
                .Include(x => x.Genres)
                    .ThenInclude(x => x.Genre)
                .ToList();

            var response = new CartResponseModel
            {
                TotalAmount = cartItems.Sum(x => x.Price),
                Items = _mapper.Map<List<CartSongsheetResponseModel>>(cartItems)
            };

            return response;
        }

        public CartResponseModel DeleteProductFromCart(int songsheetId, int userId)
        {
            var user = _unitOfWork.UserRepository.Find(x => x.Id == userId);

            if (user == null)
                throw new CustomException(HttpStatusCode.NotFound, "userId", "User is not found");

            if (!_unitOfWork.SongsheetRepository.Any(x => x.Id == songsheetId))
                throw new CustomException(HttpStatusCode.NotFound, "songsheetId", "Songsheet is not found");

            var cartItem = _unitOfWork.CartItemRepository.Find(x => x.SongsheetId == songsheetId && x.UserId == userId);

            if (cartItem == null)
                throw new CustomException(HttpStatusCode.NotFound, "", "Songsheet is not found in the cart");

            _unitOfWork.CartItemRepository.Delete(cartItem);
            _unitOfWork.SaveChanges();

            var amount = _unitOfWork.CartItemRepository.Get(x => x.UserId == userId).Sum(x => x.Songsheet.Price);
            return new CartResponseModel
            {
                TotalAmount = amount
            };
        }

        public async Task<string> PayCart(PaymentRequestModel model, int userId)
        {
            // 2000 - to make 2012 from 12
            var expDate = new DateTime(2000 + int.Parse(model.ExpirationDate.Substring(3)), int.Parse(model.ExpirationDate.Substring(0, 2)), 1);
            if (expDate.Date < new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1).Date)
                throw new CustomException(HttpStatusCode.BadRequest, "Expiration Date", "Expiration date is not valid");

            var user = _unitOfWork.UserRepository.Get(x => x.IsActive && !x.IsDeleted && x.Id == userId)
                .Include(x => x.Profile)
                .FirstOrDefault();

            if (user == null)
                throw new CustomException(HttpStatusCode.NotFound, "", "User is not found");

            var cartItems = _unitOfWork.CartItemRepository.Get(x => x.UserId == userId)
                .Include(x => x.Songsheet)
                    .ThenInclude(x => x.Image)
                .Include(x => x.Songsheet)
                    .ThenInclude(x => x.Genres)
                        .ThenInclude(x => x.Genre)
                .Include(x => x.Songsheet)
                    .ThenInclude(x => x.Instruments)
                        .ThenInclude(x => x.Instrument)
                .ToList();

            var songsheetsFromCart = cartItems.Select(x => x.Songsheet).ToList();

            if (!cartItems.Any())
                throw new CustomException(HttpStatusCode.BadRequest, "", "Cart is empty");

            var amount = songsheetsFromCart.Sum(x => x.Price);

            // Create order
            var order = _unitOfWork.OrderRepository.Get(x => x.UserId == userId && x.Amount == amount && x.Songsheets.Count == songsheetsFromCart.Count
                && !x.Songsheets.Select(w => w.SongsheetId).Except(songsheetsFromCart.Select(w => w.Id)).Any() && !x.Transactions.Any(w => w.TransactionStatus == TransactionStatus.Successfull))
                .Include(x => x.Transactions)
                .FirstOrDefault();

            if (order == null)
            {
                order = new Order
                {
                    Amount = amount,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                    Songsheets = songsheetsFromCart
                        .Select(x => new OrderSongsheet
                        {
                            SongsheetId = x.Id,
                            PriceAtPurchaseMoment = x.Price
                        })
                        .ToList()
                };

                _unitOfWork.OrderRepository.Insert(order);
            }

            var transaction = new Transaction
            {
                Amount = amount,
                CardholderName = $"{model.FirstName} {model.LastName}",
                PaymentOrderId = $"panno-{Guid.NewGuid().ToString()}",
                CardMask = model.Number.Substring(model.Number.Length - 4).PadLeft(model.Number.Length, 'X'),
                CardType = model.CardType,
                CreatedAt = DateTime.UtcNow,
                ExpirationDate = model.ExpirationDate,
                TransactionStatus = null,
                CountryName = model.CountryName
            };

            // make payment
            var paymentResult = await _facService.Authorize3DSPayment(
                new PaymentData
                {
                    Amount = amount,
                    CVV = model.CVV,
                    ExpiryDate = expDate.ToString("MMy"),
                    Number = model.Number,
                    OrderId = transaction.PaymentOrderId
                },
                new BillingDetails
                {
                    BillToAddress = model.Address,
                    BillToCity = model.City,
                    BillToCountry = model.CountryCode,
                    BillToFirstName = model.FirstName,
                    BillToLastName = model.LastName,
                    BillToState = model.State,
                    BillToZipPostCode = model.Zip
                });

            order.Transactions.Add(transaction);
            _unitOfWork.SaveChanges();

            if (paymentResult.ResponseCode != "0")
                throw new CustomException(HttpStatusCode.FailedDependency, "", paymentResult.ResponseCodeDescription);

            return paymentResult.HTMLFormData.Replace("\r\n", "").Replace("\"", "'");
        }

        public OrderDetailsResponseModel PayFreeCart(int userId)
        {
            var user = _unitOfWork.UserRepository.Get(x => x.IsActive && !x.IsDeleted && x.Id == userId)
                .Include(x => x.Profile)
                .FirstOrDefault();

            if (user == null)
                throw new CustomException(HttpStatusCode.NotFound, "", "User is not found");

            var cartItems = _unitOfWork.CartItemRepository.Get(x => x.UserId == userId)
                .Include(x => x.Songsheet)
                .ToList();

            var songsheetsFromCart = cartItems.Select(x => x.Songsheet).ToList();

            if (!cartItems.Any())
                throw new CustomException(HttpStatusCode.BadRequest, "", "Cart is empty");

            if (songsheetsFromCart.Sum(x => x.Price) != 0M)
                throw new CustomException(HttpStatusCode.BadRequest, "", "Selected items are not free");

            // Create order
            var order = _unitOfWork.OrderRepository.Get(x => x.UserId == userId && x.Amount == 0M && x.Songsheets.Count == songsheetsFromCart.Count
                && !x.Songsheets.Select(w => w.SongsheetId).Except(songsheetsFromCart.Select(w => w.Id)).Any() && !x.Transactions.Any(w => w.TransactionStatus == TransactionStatus.Successfull))
                .Include(x => x.Transactions)
                .FirstOrDefault();

            if (order == null)
            {
                order = new Order
                {
                    Amount = 0M,
                    CreatedAt = DateTime.UtcNow,
                    UserId = userId,
                    Songsheets = songsheetsFromCart
                        .Select(x => new OrderSongsheet
                        {
                            SongsheetId = x.Id,
                            PriceAtPurchaseMoment = x.Price
                        })
                        .ToList()
                };

                _unitOfWork.OrderRepository.Insert(order);
            }

            order.Transactions.Add(new Transaction
            {
                Amount = 0M,
                CreatedAt = DateTime.UtcNow,
                TransactionStatus = TransactionStatus.Successfull
            });

            foreach (var cartItem in cartItems)
                _unitOfWork.CartItemRepository.Delete(cartItem);

            _unitOfWork.SaveChanges();

            var response = _orderService.GetUserOrderDetails(order.Id);
            return response;
        }
    }
}
