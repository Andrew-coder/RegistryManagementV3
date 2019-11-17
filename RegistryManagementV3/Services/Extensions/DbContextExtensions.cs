using System.Linq;
using Microsoft.EntityFrameworkCore;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;

namespace RegistryManagementV3.Services.Extensions
{
    public static class DbContextExtensions
    {
        public static void DetachLocal<T>(this SecurityDbContext context, T t)
        {
            context.Entry(t).State = EntityState.Detached;
        }
    }
}