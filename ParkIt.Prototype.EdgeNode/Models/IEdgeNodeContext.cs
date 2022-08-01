using ParkIt.Prototype.Common;
using ParkIt.Prototype.Common.Data;
using System.Collections.Concurrent;

namespace ParkIt.Prototype.EdgeNode.Models
{
    public interface IEdgeNodeContext 
    {

        public IDictionary<string, ISensor> Sensors { get; }

        public void AddSensorData(ISensorPacket sensorData);

        public ISensorPacket GetSensorData(string Key);

        public event EventHandler<ISensorPacket> SensorDataAdded;

    }

    public class EdgeNodeContext : IEdgeNodeContext
    {
        
        public IDictionary<string, ISensor> Sensors { get; } = new ConcurrentDictionary<string, ISensor>();
        private ConcurrentDictionary<string, ISensorPacket> SensorUpdates { get; } = new ConcurrentDictionary<string, ISensorPacket>();

        public event EventHandler<ISensorPacket> SensorDataAdded;

        public EdgeNodeContext(IEnumerable<ISensor> Sensors)
        {
            foreach (var Sensor in Sensors)
                this.Sensors.TryAdd(Sensor.Key, Sensor);
        }

        public void AddSensorData(ISensorPacket sensorData)
        {
            SensorUpdates[sensorData.Key] = sensorData;

            if (sensorData is GroundSensorPacket GroundSensorUpdate)
                (Sensors[GroundSensorUpdate.Key] as GroundSensor).Weight = GroundSensorUpdate.Weight;
            else if (sensorData is OverheadSensorPacket OverheadSensorUpdate)
                (Sensors[OverheadSensorUpdate.Key] as OverheadSensor).Distance = OverheadSensorUpdate.Distance;

            SensorDataAdded?.Invoke(this, sensorData);
        }

        public ISensorPacket GetSensorData(string Key)
        {
            
            if (SensorUpdates.TryGetValue(Key, out var Data))
                return Data;

            throw new ArgumentException("Key does not exist in dictionary");

        }

    }
}
