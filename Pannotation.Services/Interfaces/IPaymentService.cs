using Pannotation.Models.RequestModels.Payment;
using System.Threading.Tasks;

namespace Pannotation.Services.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// Get merchant signature
        /// </summary>
        /// <returns>Signature</returns>
        string GetSignature();

        /// <summary>
        /// Process 3ds response
        /// </summary>
        /// <param name="model">3ds response</param>
        Task<int> Process3DSResponse(Returned3DS model);

        /// <summary>
        /// FAC Recurring Response from FAC
        /// </summary>
        /// <param name="model">recurring payment response</param>
        Task ProcessRecurringResponse(ReturnedRecurringPayment model);
    }
}
