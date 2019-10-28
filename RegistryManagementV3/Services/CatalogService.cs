using System.Collections.Generic;
using System.Linq;
using RegistryManagementV3.Models;
using RegistryManagementV3.Models.Domain;

namespace RegistryManagementV3.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly IUnitOfWork _uow;

        public CatalogService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public Catalog GetById(long id)
        {
            return _uow.CatalogRepository.GetById(id);
        }

        public List<Catalog> GetAllCatalogs(long? catalogId)
        {
            var catalogs = new List<Catalog>();
            if (catalogId.HasValue)
            {
                catalogs = _uow.CatalogRepository.GetAllChildCatalogs(catalogId.Value);
            }
            else
            {
                catalogs = _uow.CatalogRepository
                    .AllEntities
                    .Where(catalog => catalog.SuperCatalog == null)
                    .ToList();
            }
            return catalogs;
        }

        public List<Catalog> GetRootCatalogsForUserGroup(string groupName)
        {
            var userGroup = _uow.UserGroupRepository.FindUserGroupByName(groupName);
            return _uow.CatalogRepository.FindRootCatalogs(userGroup);
        }

        public List<Catalog> GetChildCatalogsByUserGroup(long? catalogId, string groupName)
        {
            var userGroup = _uow.UserGroupRepository.FindUserGroupByName(groupName);
            if (catalogId.HasValue)
            {
                return _uow.CatalogRepository.GetChildCatalogsByUserGroup(catalogId.Value, userGroup);
            }
            return GetRootCatalogsForUserGroup(groupName);
        }

        public bool ContainsSubCatalogs(long id)
        {
            return _uow.CatalogRepository.GetAllChildCatalogs(id).Any();
        }

        public void SaveCatalog(Catalog catalog)
        {
            _uow.CatalogRepository.Add(catalog);
            _uow.Save();
        }

        public void RemoveCatalog(long catalogId)
        {
            Catalog catalog = _uow.CatalogRepository.GetById(catalogId);
            _uow.CatalogRepository.Remove(catalog);
            _uow.Save();
        }
    }
}
