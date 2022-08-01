// See https://aka.ms/new-console-template for more information
using ParkIt.Prototype.Common;
using ParkIt.Prototype.Sensor.Device;

var SensorDevices = ISensor.ReadFromJson(File.ReadAllText("Sensors.json"));

Console.WriteLine("Starting...");

var CancelToken = new CancellationTokenSource();

var Sensors = new List<IMockSensorDevice>();

foreach (var device in SensorDevices)
{
    if (device is GroundSensor GroundSensor)
        Sensors.Add(new MockInGroundSensor(GroundSensor, "https://localhost:5005/EdgeNode/GroundSensorEndpoint"));
    else if (device is OverheadSensor OverheadSensor)
        Sensors.Add(new MockOverheadSensor(OverheadSensor, "https://localhost:5005/EdgeNode/OverheadSensorEndpoint"));

    _ = Sensors[^1].Start();

}

Console.WriteLine("Running!");

while (Console.KeyAvailable == false)
    await Task.Delay(100);

CancelToken.Cancel();

Console.WriteLine("Stopped.");
Console.ReadLine();


