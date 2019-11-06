using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Identity;

namespace RegistryManagementV3.Models.Domain
{
    public sealed class ApplicationUser : IdentityUser
    {
        public AccountStatus AccountStatus { get; set; }
        public UserGroup UserGroup { get; set; }

        public int SecurityLevel { get; set; }
    }
}