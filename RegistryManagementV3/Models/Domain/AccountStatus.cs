
using System.ComponentModel.DataAnnotations;

namespace RegistryManagementV3.Models.Domain
{
    public enum AccountStatus
    {
        [Display(Name = "в очікуванні підтвердження")]
        PendingApproval,
        
        [Display(Name = "підтверджений адміністратором")]
        Approved,
        
        [Display(Name = "акаунт заблоковано")]
        Locked
    }
}