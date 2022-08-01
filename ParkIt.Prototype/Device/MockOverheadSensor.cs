using ParkIt.Prototype.Common;
using System.Text.Json;

namespace ParkIt.Prototype.Sensor.Device
{

    public interface IMockSensorDevice
    {

        public Task Start(CancellationToken Token = default);

    }

    public class MockOverheadSensor : IMockSensorDevice
    {

        private HttpClient _httpClient;
        private OverheadSensor _sensor;

        public MockOverheadSensor(OverheadSensor Sensor, string Endpoint)
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

            _sensor.Distance = Random.Shared.NextSingle() * 4f;

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

                    Console.WriteLine($"Exception: {ex.Message}");

                    throw;
                }

                // Switch the weight state
                _sensor.Distance = _sensor.Distance < 2 ? (Random.Shared.NextSingle() * 2f) + 2.5f : (Random.Shared.NextSingle() * 1.5f) + 0.4f;

            }

        }
    }
}
