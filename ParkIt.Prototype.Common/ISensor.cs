using Newtonsoft.Json;
using ParkIt.Prototype.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ParkIt.Prototype.Common
{

    public interface ISensor
    {

        [JsonPropertyName("Key")]
        public string Key { get; }

        [JsonPropertyName("Section")]
        public string Section { get; }

        [System.Text.Json.Serialization.JsonIgnore]
        public bool IsEmpty { get; }

        public ISensorPacket CreateUpdatePacket();

        public static IReadOnlyList<ISensor> ReadFromJson(string Json)
        {
            
            dynamic dymJson = JsonConvert.DeserializeObject(Json);

            var Sensors = new List<ISensor>();

            foreach (var item in dymJson)
            {

                if (item.SensorType == "Overhead")

                    Sensors.Add(new OverheadSensor()
                    {
                        Key = (string)item.Key,
                        Section = (string)item.Section,
                    });
                else
                    Sensors.Add(new GroundSensor()
                    {
                        Key = (string)item.Key,
                        Section = (string)item.Section,
                    });
            }

            return Sensors;

        }

    }

    public class GroundSensor : ISensor
    {

        [JsonPropertyName("Key")]
        public string Key { get; set; }

        [JsonPropertyName("Section")]
        public string Section { get; set; }

        [JsonPropertyName("Weight")]
        public float Weight { get; set; }

        public bool IsEmpty => Weight < 50;

        public ISensorPacket CreateUpdatePacket()
        {
            return new GroundSensorPacket()
            {
                Key = this.Key,
                Weight = this.Weight
            };
        }
    }

    public class OverheadSensor : ISensor
    {

        [JsonPropertyName("Key")]
        public string Key { get; set; }

        [JsonPropertyName("Section")]
        public string Section { get; set; }

        [JsonPropertyName("Distance")]
        public float Distance { get; set; }

        public bool IsEmpty => Distance < 2;

        public ISensorPacket CreateUpdatePacket()
        {
            return new OverheadSensorPacket()
            {
                Key = this.Key,
                Distance = this.Distance
            };
        }

    }

}
