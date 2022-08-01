using ParkIt.Prototype.Common;
using ParkIt.Prototype.EdgeNode.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Configuration.AddJsonFile("Properties\\launchSettings.json");

var Sensors = ISensor.ReadFromJson(File.ReadAllText(Environment.CurrentDirectory + "\\Sensors.json"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEdgeNodeContext>(new EdgeNodeContext(Sensors));
builder.Services.AddSingleton<BackgroundUploadService>();
builder.Services.AddHttpClient("thinger.io", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration["thinger.io:ApiPath"]);
    httpClient.DefaultRequestHeaders.Authorization = 
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", builder.Configuration["thinger.io:BearerToken"]);
});

var app = builder.Build();
app.Urls.Clear();
app.Urls.Add(builder.Configuration["profiles:ParkIt.Prototype.EdgeNode:applicationUrl"]);

_ = app.Services.GetService<BackgroundUploadService>().StartAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
