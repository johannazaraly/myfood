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
        private LogModel logModel = LogModel.GetInstance;

        private static DatabaseModel instance;

        public static DatabaseModel GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseModel();
                }
                return instance;
            }
        }

        private LocalDataContext db;

        private DatabaseModel()
        {
            db = new LocalDataContext();
        }

        public async Task SetLastCalibrationDate(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                try
                {
                    var currentSensorType = await (from m in db.SensorTypes where m.Id == (int)sensorType select m).FirstOrDefaultAsync();

                    if (currentSensorType != null)
                    {
                        var currentDate = await GetLastMesureDate();
                        currentSensorType.lastCalibration = currentDate;
                        await db.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    logModel.AppendLog(Log.CreateErrorLog("Exception on Database - Set Last Calibration Date", ex));
                }

            }
        }

        public async Task<String> GetLastCalibrationDate(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                var currentDateTime = await (from m in db.SensorTypes where m.Id == (int)sensorType select m.lastCalibration).FirstOrDefaultAsync();

                if (currentDateTime.Value == null)
                    return "No yet calibrated";
                else
                    return currentDateTime.Value.ToString("d");
            }
        }

        public async Task<bool> DeleteAllMesures()
        {
            using (await asyncLock.LockAsync())
            {
                var task = Task.Run(async () =>
                {
                        try
                        {
                            if (db.Measures.Any())
                            {
                                db.RemoveRange(db.Measures);
                                await db.SaveChangesAsync();
                            }
                    }
                    catch (Exception ex)
                        {
                            logModel.AppendLog(Log.CreateErrorLog("Exception on Database", ex));
                        }
                });
                task.Wait();
            }
            return true;
        }

        public async Task<DateTime> GetLastMesureDate()
        {
            using (await asyncLock.LockAsync())
            {
                var currentDateTime = await (from m in db.Measures select m.captureDate).MaxAsync();

                if (currentDateTime == null)
                    return new DateTime(2016, 8, 10, 0, 0, 0);
                else
                    return currentDateTime;
            }
        }

        public async Task<Decimal> GetLastMesure(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                var rslt = await (from m in db.Measures.Where(m => m.sensor.Id == (int)sensorType).OrderByDescending(m => m.captureDate)
                                  select m).Take(1).FirstOrDefaultAsync();

                if (rslt != null)
                    return Math.Round(rslt.value,1);
                else
                    return 0;
            }
        }

        public async Task<Decimal> GetYesterdayMesure(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                var currentDateTime = await (from m in db.Measures select m.captureDate).MaxAsync();

                if (currentDateTime == null)
                    return 0;

                var rslt = await (from m in db.Measures
                                  .Where(m => m.sensor.Id == (int)sensorType 
                                  && m.captureDate > currentDateTime.AddDays(-1) 
                                  && m.captureDate < currentDateTime)
                                  .OrderBy(m => m.captureDate)
                                  select m).Take(1).FirstOrDefaultAsync();

                if (rslt != null)
                    return Math.Round(rslt.value, 1);
                else
                    return 0;
            }
        }

        public async Task<Decimal> GetLastDayMinMesure(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                var currentDateTime = await (from m in db.Measures select m.captureDate).MaxAsync();

                if (currentDateTime == null)
                    return 0;

                var rslt = await (from m in db.Measures
                                  .Where(m => m.sensor.Id == (int)sensorType
                                  && m.captureDate > currentDateTime.AddDays(-1))
                                  .OrderByDescending(m => m.captureDate)
                                  select m).MinAsync(m => m.value);

                return Math.Round(rslt, 1);

            }
        }

        public async Task<Decimal> GetLastDayMaxMesure(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                var currentDateTime = await (from m in db.Measures select m.captureDate).MaxAsync();

                if (currentDateTime == null)
                    return 0;

                var rslt = await (from m in db.Measures
                                  .Where(m => m.sensor.Id == (int)sensorType
                                  && m.captureDate > currentDateTime.AddDays(-1))
                                  .OrderByDescending(m => m.captureDate)
                                  select m).MaxAsync(m => m.value);

                return Math.Round(rslt, 1);
            }
        }

        public async Task<Decimal> GetLastDayAverageMesure(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                var currentDateTime = await (from m in db.Measures select m.captureDate).MaxAsync();

                if (currentDateTime == null)
                    return 0;

                var rslt = await (from m in db.Measures
                                  .Where(m => m.sensor.Id == (int)sensorType
                                  && m.captureDate > currentDateTime.AddDays(-1))
                                  .OrderByDescending(m => m.captureDate)
                                  select m).AverageAsync(m => m.value);

                return Math.Round(rslt, 1);

            }
        }

        public async Task<List<Measure>> GetLastDayMesures(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                return await (from m in db.Measures.Where(m => m.sensor.Id == (int)sensorType).OrderByDescending(m => m.captureDate)
                              select m).Take(24 * 6).ToListAsync();
            }          
        }

        public async Task<List<Measure>> GetLastWeeksMesures(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                return await(from m in db.Measures.Where(m => m.sensor.Id == (int)sensorType).OrderByDescending(m => m.captureDate)
                              select m).Take(7 * 24 * 6).ToListAsync();
            }
        }

        public async Task<List<Measure>> GetLastTwoMonthsMesures(SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
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

        public async Task<bool> AddMesure(DateTime currentDate, decimal capturedValue, SensorTypeEnum sensorType)
        {
            using (await asyncLock.LockAsync())
            {
                var task = Task.Run(async () =>
                {
                        try
                        {
                            var currentSensor = db.SensorTypes.Where(s => s.Id == (int)sensorType).FirstOrDefault();

                            db.Measures.Add(new Measure() { value = capturedValue, captureDate = currentDate, sensor = currentSensor });
                            await db.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            logModel.AppendLog(Log.CreateErrorLog("Exception on Database - Add Mesure", ex));
                        }
                });
                task.Wait();
            }

            return true;
        }

    }
}