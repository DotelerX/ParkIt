using ParkIt.Prototype.Common.Data;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace ParkIt.Prototype.EdgeNode.Models
{

    public class BackgroundUploadService : IHostedService
    {

        private readonly IEdgeNodeContext context;
        private readonly IHttpClientFactory httpFactory;
        private readonly ILogger<BackgroundUploadService> logger;
        private readonly CancellationTokenSource cancelToken = new CancellationTokenSource();
        private readonly ConcurrentDictionary<string, ISensorPacket> sensorDataToUpload = new ConcurrentDictionary<string, ISensorPacket>();

        public BackgroundUploadService(IEdgeNodeContext Context, IHttpClientFactory HttpFactory, ILogger<BackgroundUploadService> Logger)
        {
            context = Context;
            httpFactory = HttpFactory;
            logger = Logger;
            context.SensorDataAdded += Context_SensorAdded;
        }

        private void Context_SensorAdded(object? sender, ISensorPacket e)
        {
            this.sensorDataToUpload[e.Key] = e;
        }

        public async Task StartAsync(CancellationToken Token = default)
        {

            Token.Register(() => cancelToken.Cancel());

            var Client = httpFactory.CreateClient("thinger.io");

            while (cancelToken.Token.IsCancellationRequested == false)
            {

                try
                {

                    var DataToUpload = sensorDataToUpload.Values;

                    sensorDataToUpload.Clear();

                    var debug = JsonSerializer.Serialize(DataToUpload.Select(x => x as object));

                    if (DataToUpload.Any())
                    {

                        var message = new HttpRequestMessage()
                        {
                            Method = HttpMethod.Post,
                            Content = new StringContent(JsonSerializer.Serialize(DataToUpload.Select(x => x as object)), Encoding.UTF8, "application/json"),
                        };

                        var Response = await Client.SendAsync(message, Token);

                        if (Response.IsSuccessStatusCode == false)
                        {

                            foreach (var item in DataToUpload)
                                sensorDataToUpload.TryAdd(item.Key, item);

                            logger.LogCritical("Failed to send updates");

                        }

                        

                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), cancelToken.Token);

                }
                catch (Exception ex)
                {

                    if (cancelToken.Token.IsCancellationRequested)
                        return;

                    logger.LogError(ex, ex.Message);

                    throw;

                }

            }

            Console.WriteLine("Upload service stopping");

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            cancelToken.Cancel();
        }
    }
}
