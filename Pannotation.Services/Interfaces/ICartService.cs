using Pannotation.Models.InternalModels;
using Pannotation.Models.RequestModels.Payment;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Order;
using Pannotation.Models.ResponseModels.Payment;
using Pannotation.Models.ResponseModels.Songsheet;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces
{
    public interface ICartService
    {
        /// <summary>
        /// Add product to the cart
        /// </summary>
        /// <param name="songsheetId">Songsheet id</param>
        /// <param name="userId">User id</param>
        /// <returns>Info about added songsheet</returns>
        CartSongsheetResponseModel AddProductToCart(int songsheetId, int userId);

        /// <summary>
        /// Get cart info
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Cart info with cart items</returns>
        CartResponseModel GetCart(int userId);

        /// <summary>
        /// Delete product from the cart
        /// </summary>
        /// <param name="songsheetId">Songsheet id</param>
        /// <param name="userId">User id</param>
        /// <returns>Cart info</returns>
        CartResponseModel DeleteProductFromCart(int songsheetId, int userId);

        /// <summary>
        /// Pay for purchases from cart
        /// </summary>
        /// <param name="model">Payment info model</param>
        /// <param name="userId">User id</param>
        /// <returns>Payment info</returns>
        Task<string> PayCart(PaymentRequestModel model, int userId);

        /// <summary>
        /// Get free files from cart
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Order details</returns>
        OrderDetailsResponseModel PayFreeCart(int userId);
    }
}
