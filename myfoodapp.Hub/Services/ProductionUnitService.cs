using myfoodapp.Hub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace myfoodapp.Hub.Services
{
    public class ProductionUnitService : IDisposable
    {
        private ApplicationDbContext entities;


        public ProductionUnitService(ApplicationDbContext entities)
        {
            this.entities = entities;
        }

        public IList<ProductionUnitViewModel> GetAll()
        {
            IList<ProductionUnitViewModel> result = new List<ProductionUnitViewModel>();

            result = entities.ProductionUnits.Select(pu => new ProductionUnitViewModel
            {
                Id = pu.Id,
                startDate = pu.startDate,
                locationLatitude = pu.locationLatitude,
                locationLongitude = pu.locationLongitude,
                version = pu.version,
                info = pu.info,
                options = pu.options,
                reference = pu.reference,

                productionUnitTypeId = pu.productionUnitType.Id,
                productionUnitType = new ProductionUnitTypeViewModel()
                {
                    Id = pu.productionUnitType.Id,
                    name = pu.productionUnitType.name
                },
                ownerId = pu.owner.Id,
                owner = new OwnerViewModel()
                {
                    Id = pu.owner.Id,
                    pioneerCitizenName = pu.owner.pioneerCitizenName,
                    pioneerCitizenNumber = pu.owner.pioneerCitizenNumber
                }

            }).ToList();

            return result;
        }

        public IEnumerable<ProductionUnitViewModel> Read()
        {
            return GetAll();
        }

        public void Create(ProductionUnitViewModel productionUnit)
        {
            var entity = new ProductionUnit();

            entity.Id = productionUnit.Id;
            entity.startDate = productionUnit.startDate;
            entity.locationLatitude = productionUnit.locationLatitude;
            entity.locationLongitude = productionUnit.locationLongitude;
            entity.version = productionUnit.version;
            entity.info = productionUnit.info;
            entity.options = productionUnit.options;
            entity.reference = productionUnit.reference;

            if (entity.productionUnitType == null)
            {
                entity.productionUnitType = new ProductionUnitType();
                entity.productionUnitType.Id = productionUnit.productionUnitTypeId;
            }

            if (entity.owner == null)
            {
                entity.owner = new ProductionUnitOwner();
                entity.owner.Id = productionUnit.ownerId;
            }

            entities.ProductionUnits.Add(entity);
            entities.SaveChanges();

            productionUnit.Id = entity.Id;
        }

        public void Update(ProductionUnitViewModel productionUnit)
        {
            ProductionUnit target = new ProductionUnit();
            target = entities.ProductionUnits.Where(p => p.Id == productionUnit.Id).Include(m => m.productionUnitType)
                                                                                   .Include(m => m.owner).FirstOrDefault();

            if (target != null)
            {
                target.startDate = productionUnit.startDate;
                target.locationLatitude = productionUnit.locationLatitude;
                target.locationLongitude = productionUnit.locationLongitude;
                target.version = productionUnit.version;
                target.info = productionUnit.info;
                target.options = productionUnit.options;
                target.reference = productionUnit.reference;

                ProductionUnitType currentProductionUnitType = new ProductionUnitType();
                currentProductionUnitType = entities.ProductionUnitTypes.Where(p => p.Id == productionUnit.productionUnitTypeId).FirstOrDefault();

                target.productionUnitType = currentProductionUnitType;

                ProductionUnitOwner currentProductionUnitOwner = new ProductionUnitOwner();
                currentProductionUnitOwner = entities.ProductionUnitOwner.Where(p => p.Id == productionUnit.ownerId).FirstOrDefault();

                target.owner = currentProductionUnitOwner;
            }      

                entities.SaveChanges();  
        }

        public void Destroy(ProductionUnitViewModel message)
        {
                var entity = new Measure();

                entity.Id = message.Id;

                entities.Measures.Attach(entity);
                entities.Measures.Remove(entity);

                entities.SaveChanges();
        }

        public ProductionUnitViewModel One(Func<ProductionUnitViewModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}