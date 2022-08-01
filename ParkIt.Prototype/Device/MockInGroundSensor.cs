using ParkIt.Prototype.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ParkIt.Prototype.Sensor.Device
{
    public class MockInGroundSensor : IMockSensorDevice
    {

        private HttpClient _httpClient;
        private GroundSensor _sensor;

        public MockInGroundSensor(GroundSensor Sensor, string Endpoint)
        {
            _sensor = Sensor;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(Endpoint),
                Timeout = TimeSpan.FromSeconds(3)
            };
        }

        /// <summary>
        /// Start the sensor
        /// </summary>
        /// <returns></returns>
        public async Task Start(CancellationToken Token = default)
        {

            _sensor.Weight = Random.Shared.Next(10, 100);

            while (Token.IsCancellationRequested == false)
            {

                try
                {

                    var Response = await _httpClient.SendAsync(new HttpRequestMessage()
                    {
                        Method = HttpMethod.Post,
                        Content = new StringContent(JsonSerializer.Serialize(_sensor.CreateUpdatePacket() as object)),
                    });

                    await Task.Delay(TimeSpan.FromSeconds(Random.Shared.Next(15, 30)), Token);

                }
                catch (Exception ex)
                {

                    if (Token.IsCancellationRequested)
                        return;

                    Console.WriteLine($"Exception: { ex.Message }");

                    throw;
                }

                // Switch the weight state
                _sensor.Weight = _sensor.Weight < 50 ? Random.Shared.Next(150, 250) : Random.Shared.Next(10, 30);


            }

        }

    }
}
