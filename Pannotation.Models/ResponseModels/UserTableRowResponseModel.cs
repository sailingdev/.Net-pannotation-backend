using System;
using System.Collections.Generic;
using System.Text;

namespace Pannotation.Models.ResponseModels
{
    public class UserTableRowResponseModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsSubscribed { get; set; }

        public string Country { get; set; }
                
        public bool? IsComposer { get; set; }

        public bool IsBlocked { get; set; }
    }
}
