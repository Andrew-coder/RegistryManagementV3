using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Controllers.Attributes
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
//        public AccountStatus AccountStatus { get; set; }
//
//        protected override bool AuthorizeCore(HttpContextBase httpContext)
//        {
//            if (!httpContext.User.Identity.IsAuthenticated)
//                return false;
//
//            var claimsIdentity = httpContext.User.Identity as ClaimsIdentity;
//            var accountStatusClaim = claimsIdentity.FindFirst("accountStatus");
//            
//            if (accountStatusClaim == null)
//                return false;
//
//            var accountStatus = accountStatusClaim.Value;
//
//            if (!AccountStatus.ToString().Equals(accountStatus))
//            {
//                return false;
//            }
//            
//            return base.AuthorizeCore(httpContext);
//        }
    }
}