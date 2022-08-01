using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ParkIt.Prototype.Common.Data
{
    public interface ISensorPacket
    {

        [JsonPropertyName("Key")]
        public string Key { get; }

    }

    public class GroundSensorPacket : ISensorPacket
    {

        [JsonPropertyName("Key")]
        public string Key { get; init; }

        [JsonPropertyName("Weight")]
        public float Weight { get; init; }

    }

    public class OverheadSensorPacket : ISensorPacket
    {

        [JsonPropertyName("Key")]
        public string Key { get; init; }

        [JsonPropertyName("Distance")]
        public float Distance { get; init; }

    }

}
