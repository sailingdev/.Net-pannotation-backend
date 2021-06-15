using System;
using System.Linq;
using System.Security.Claims;

namespace Pannotation.Common.Extensions
{
    public static class ClaimsPrincipalExtentions
    {
        public static int GetUserId(this ClaimsPrincipal userClaims)
        {
            try
            {
                string claim = userClaims.Claims.FirstOrDefault(w => w.Type == ClaimTypes.NameIdentifier)?.Value;
                int id = Convert.ToInt32(claim);

                if (id <= 0)
                    throw new SystemException("User is not found");

                return id;
            }
            catch
            {
                throw new SystemException("User is not found");
            }
        }

    }
}
