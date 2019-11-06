using System.Collections.Generic;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public interface ICatalogService
    {
        Catalog GetById(long id);
        List<Catalog> GetAllCatalogs(long? catalogId);
        bool ContainsSubCatalogs(long id);
        void SaveCatalog(Catalog catalog);
        void RemoveCatalog(long catalogId);
    }
}