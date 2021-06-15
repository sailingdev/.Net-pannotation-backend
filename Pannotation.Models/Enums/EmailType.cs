using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Pannotation.Models.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum EmailType
    {
        SuccessfulRegistration,
        ConfrimEmail,
        ResetPassword,
        NewPassword,
        BlockUser,
        UnblockUser,
        ContactUs,
        SuccessfulSubscription
    }
}
