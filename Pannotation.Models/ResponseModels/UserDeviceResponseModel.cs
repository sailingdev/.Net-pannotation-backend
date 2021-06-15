using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Models.ResponseModels
{
    public class UserDeviceResponseModel
    {
        [JsonRequired]
        public int UserId { get; set; }

        [JsonRequired]
        public bool IsVerified { get; set; }

        [JsonRequired]
        public string DeviceToken { get; set; }

        [JsonRequired]
        public DateTime AddedAt { get; set; }
    }
}
