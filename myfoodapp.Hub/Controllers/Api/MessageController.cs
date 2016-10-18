using myfoodapp.Hub.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace myfoodapp.Hub.Controllers.Api
{
    public class MessagesController : ApiController
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]JObject data)
        {
            var content = data["content"].ToObject<string>();
            var device = data["device"].ToObject<string>();

            var db = new ApplicationDbContext();
            var date = DateTime.Now;

            try
            {
                var measureType = db.MessageTypes.Where(m => m.Id == 1).FirstOrDefault();

                db.Messages.Add(new Message() { content = content, device = device, date = date, messageType = measureType });
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                db.Logs.Add(Log.CreateErrorLog("Error on Message from Sigfox", ex));
                db.SaveChanges();
            }

            try
            {
                var productionUnit = db.ProductionUnits.Where(p => p.reference == device).FirstOrDefault();

                if(productionUnit == null)
                {
                    db.Logs.Add(Log.CreateLog(String.Format("Production Unit not found - {0}", device), Log.LogType.Warning));
                    db.SaveChanges();
                }

                var phContent = content.Substring(0, 4).Insert(3,".");
                var waterTempContent = content.Substring(4, 4).Insert(3, ".");
                var orpContent = content.Substring(8, 4).Insert(3, ".");
                var dissolvedOxyContent = content.Substring(12, 4).Insert(3, ".");
                var airTempContent = content.Substring(16, 4).Insert(3, ".");
                var airHumidityContent = content.Substring(20, 4).Insert(3, ".");

                var phSensor = db.SensorTypes.Where(s => s.Id == 1).FirstOrDefault();
                var waterTemperatureSensor = db.SensorTypes.Where(s => s.Id == 2).FirstOrDefault();
                var dissolvedOxySensor = db.SensorTypes.Where(s => s.Id == 3).FirstOrDefault();
                var ORPSensor = db.SensorTypes.Where(s => s.Id == 4).FirstOrDefault();
                var airTemperatureSensor = db.SensorTypes.Where(s => s.Id == 5).FirstOrDefault();
                var airHumidity = db.SensorTypes.Where(s => s.Id == 6).FirstOrDefault();

                if(!phContent.Contains("a"))
                {
                    decimal phValue = 0;
                    if(decimal.TryParse(phContent, out phValue))
                    {
                        db.Measures.Add(new Measure() { captureDate = date, productionUnit = productionUnit, sensor = phSensor, value = phValue });
                        db.SaveChanges();
                    }       
                }

                if (!waterTempContent.Contains("a"))
                {
                    decimal waterTempValue = 0;
                    if (decimal.TryParse(waterTempContent, out waterTempValue))
                    {
                        db.Measures.Add(new Measure() { captureDate = date, productionUnit = productionUnit, sensor = waterTemperatureSensor, value = waterTempValue });
                        db.SaveChanges();
                    }          
                }

                if (!orpContent.Contains("a"))
                {
                    decimal orpValue = 0;
                    if (decimal.TryParse(orpContent, out orpValue))
                    {
                        db.Measures.Add(new Measure() { captureDate = date, productionUnit = productionUnit, sensor = ORPSensor, value = orpValue });
                        db.SaveChanges();
                    }                       
                }

                if (!dissolvedOxyContent.Contains("a"))
                {
                    decimal dissolvedOxyvalue = 0;
                    if (decimal.TryParse(dissolvedOxyContent, out dissolvedOxyvalue))
                    {
                        db.Measures.Add(new Measure() { captureDate = date, productionUnit = productionUnit, sensor = dissolvedOxySensor, value = dissolvedOxyvalue });
                        db.SaveChanges();
                    }
                }

                if (!airTempContent.Contains("a"))
                {
                    decimal airTempValue = 0;
                    if (decimal.TryParse(airTempContent, out airTempValue))
                    {
                        db.Measures.Add(new Measure() { captureDate = date, productionUnit = productionUnit, sensor = airTemperatureSensor, value = airTempValue });
                        db.SaveChanges();
                    }
                }

                if (!airHumidityContent.Contains("a"))
                {
                    decimal airHumidityValue = 0;
                    if (decimal.TryParse(airHumidityContent, out airHumidityValue))
                    {
                        db.Measures.Add(new Measure() { captureDate = date, productionUnit = productionUnit, sensor = airHumidity, value = airHumidityValue });
                        db.SaveChanges();
                    }                     
                }
                    
            }
            catch (Exception ex)
            {
                db.Logs.Add(Log.CreateErrorLog("Error on Convert Measures from Sigfox", ex));
            }
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}