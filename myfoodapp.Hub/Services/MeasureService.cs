using myfoodapp.Hub.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace myfoodapp.Hub.Services
{
    public class MeasureService : IDisposable
    {
        private ApplicationDbContext entities;


        public MeasureService(ApplicationDbContext entities)
        {
            this.entities = entities;
        }

        public IList<MeasureViewModel> GetAll()
        {
            IList<MeasureViewModel> result = new List<MeasureViewModel>();

            result = entities.Measures.Select(meas => new MeasureViewModel
            {
                Id = meas.Id,
                captureDate = meas.captureDate,
                value = meas.value,
                sensorId = meas.sensor.Id,
                sensor = new SensorTypeViewModel()
                {
                    Id = meas.sensor.Id,
                    name = meas.sensor.name
                }

            }).ToList();

            return result;
        }

        public IEnumerable<MeasureViewModel> Read()
        {
            return GetAll();
        }

        public void Create(MeasureViewModel measure)
        {
            var entity = new Measure();

            entity.Id = measure.Id;
            entity.captureDate = measure.captureDate;
            entity.value = measure.value;

            if (entity.sensor == null)
            {
                entity.sensor = new SensorType();
                entity.sensor.Id = measure.sensorId;
            }

            entities.Measures.Add(entity);
            entities.SaveChanges();

            measure.Id = entity.Id;
        }

        public void Update(MeasureViewModel measure)
        {
            Measure target = new Measure();
            target = entities.Measures.Where(m => m.Id == measure.Id).Include(m => m.sensor).FirstOrDefault();

            if (target != null)
            {
                target.captureDate = measure.captureDate;
                target.value = measure.value;

                SensorType currentSensorType = new SensorType();
                currentSensorType = entities.SensorTypes.Where(m => m.Id == measure.sensorId).FirstOrDefault();

                target.sensor = currentSensorType;
            }      

                entities.SaveChanges();  
        }

        public void Destroy(MeasureViewModel message)
        {
                var entity = new Measure();

                entity.Id = message.Id;

                entities.Measures.Attach(entity);
                entities.Measures.Remove(entity);

                entities.SaveChanges();
        }

        public MeasureViewModel One(Func<MeasureViewModel, bool> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public void Dispose()
        {
            entities.Dispose();
        }
    }
}