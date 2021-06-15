using Pannotation.Models.Enums;
using Pannotation.Models.InternalModels;
using Pannotation.Models.RequestModels;
using Pannotation.Models.ResponseModels;
using Pannotation.Models.ResponseModels.Order;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces
{
    public interface IOrderService
    {
        /// <summary>
        /// Get users orders songsheets
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="model">Pagination model</param>
        /// <returns>List of songsheets</returns>
        Task<PaginationResponseModel<OrderSongsheetResponseModel>> GetOrderSongsheets(int userId, PaginationBaseRequestModel model);

        /// <summary>
        /// Get orders list
        /// </summary>
        /// <param name="model">Pagination request model</param>
        /// <returns>List of orders</returns>
        PaginationResponseModel<OrderItemListResponseModel> GetOrders(OrderListRequestModel model);

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns></returns>
        AdminOrderDetailsResponseModel GetOrderDetails(int orderId);

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <returns></returns>
        OrderDetailsResponseModel GetUserOrderDetails(int orderId);

        /// <summary>
        /// Export orders list to csv file
        /// </summary>
        /// <param name="model">Request model</param>
        /// <returns>Csv file as bytes array</returns>
        byte[] ExportOrders(ExportListRequestModel<OrderListKey> model);

        /// <summary>
        /// Get files from order as a single file or zip archive
        /// </summary>
        /// <param name="orderId">Order id</param>
        /// <param name="userId">User id</param>
        /// <returns>Model with file or zip archive</returns>
        Task<SongsheetsFiles> DownloadSongsheets(int orderId, int userId);
    }
}
