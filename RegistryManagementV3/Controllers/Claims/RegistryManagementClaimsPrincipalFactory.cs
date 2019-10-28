using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Controllers.Claims
{
    public class RegistryManagementClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public RegistryManagementClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager
            , RoleManager<IdentityRole> roleManager
            , IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        { }
        
        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            var principal = await base.CreateAsync(user);

            if (user.AccountStatus != null)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("accountStatus", user.AccountStatus.ToString())
                });
            }

            if (user.UserGroup != null)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim("userGroup", user.UserGroup.Name),
                });
            }

            return principal;
        }
    }
}