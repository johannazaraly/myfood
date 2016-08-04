using myfoodapp.Business;
using myfoodapp.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;
using Microsoft.Data.Entity;

namespace myfoodapp.Model
{
    public class DatabaseModel
    {
        //private static readonly AsyncLock asyncLock = new AsyncLock();

        public DatabaseModel()
        {
        }

        public async Task<List<Measure>> GetLastDayMesures(SensorTypeEnum sensorType)
        {
            using (var db = new LocalDataContext())
            {
                return await (from m in db.Measures.Where(m => m.sensor.Id == (int)sensorType).OrderByDescending(m => m.captureDate)
                              select m).Take(24 * 6).ToListAsync();
            }          
        }

        public async Task<List<Measure>> GetLastWeeksMesures(SensorTypeEnum sensorType)
        {
            using (var db = new LocalDataContext())
            {
                return await (from m in db.Measures.Where(m => m.sensor.Id == (int)sensorType).OrderByDescending(m => m.captureDate)
                              select m).Take(7 * 24 * 6).ToListAsync();
            }
        }

        public async Task<List<Measure>> GetLastTwoMonthsMesures(SensorTypeEnum sensorType)
        {
            using (var db = new LocalDataContext())
            {
                return await (from m in db.Measures.Where(m => m.sensor.Id == (int)sensorType)
                              group m by new { m.captureDate.Year, m.captureDate.DayOfYear} into groupedDay
                              select new Measure
                              {
                                  captureDate = groupedDay.FirstOrDefault().captureDate,
                                  value = groupedDay.Average(x => x.value),
                              }).Take(7 * 30 * 2).ToListAsync();

            }
        }

        public bool AddMesure(DateTime currentDate, Decimal capturedValue, SensorTypeEnum sensorType)
        {
            using (var db = new LocalDataContext())
            {
                try
                {
                    var currentSensor = db.SensorTypes.Where(s => s.Id == (int)sensorType).FirstOrDefault();

                    db.Measures.Add(new Measure() { value = capturedValue, captureDate = currentDate, sensor = currentSensor });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            return true;
        }

    }
}