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
        private static readonly AsyncLock asyncLock = new AsyncLock();

        public DatabaseModel()
        {
        }

        public async Task<List<Measure>> GetLastDayMesures(SensorTypeEnum sensorType)
        {
            using (var db = new LocalDataContext())
            {
                return await (from m in db.Measures.Where(m => m.sensor.Id == (int)sensorType).OrderByDescending(m => m.captureDate)
                              select m).Take(12 * 6).ToListAsync();
            }          
        }

        public async Task<List<Measure>> GetLastThreeWeeksMesures(SensorTypeEnum sensorType)
        {
            using (var db = new LocalDataContext())
            {
                return await (from m in db.Measures.Where(m => m.sensor.Id == (int)sensorType).OrderByDescending(m => m.captureDate).Take(12 * 6 * 7 * 5)
                              group m by new { m.captureDate.Year, m.captureDate.DayOfYear } into groupedDay
                              select new Measure
                              {
                                  captureDate = groupedDay.OrderByDescending(m => m.captureDate).FirstOrDefault().captureDate,
                                  value = groupedDay.Average(x => x.value),
                                  maximum = groupedDay.Max(x => x.value),
                                  minimum = groupedDay.Min(x => x.value),
                              }).ToListAsync();

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

    }
}