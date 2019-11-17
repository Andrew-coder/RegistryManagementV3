using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RegistryManagementV3.Models.Domain;
using RegistryManagementV3.Models.Repository;

namespace RegistryManagementV3.Models
{
    public interface IUnitOfWork
    {
        ResourceRepository ResourceRepository { get; }
        CatalogRepository CatalogRepository { get; }
        UserGroupRepository UserGroupRepository { get; }
        UserRepository UserRepository { get; }
        TagRepository TagRepository { get; }
        void Save();
    }
}
