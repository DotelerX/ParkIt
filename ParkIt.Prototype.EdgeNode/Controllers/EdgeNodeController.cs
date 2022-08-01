using Microsoft.AspNetCore.Mvc;
using ParkIt.Prototype.Common.Data;
using ParkIt.Prototype.EdgeNode.Models;
using System.Text.Json;

namespace ParkIt.Prototype.EdgeNode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EdgeNodeController : ControllerBase
    {
        
        private readonly ILogger<EdgeNodeController> _logger;
        private readonly IEdgeNodeContext nodeContext;

        public EdgeNodeController(ILogger<EdgeNodeController> Logger, IEdgeNodeContext Context)
        {
            _logger = Logger;
            nodeContext = Context;
        }

        [HttpPost("OverheadSensorEndpoint")]
        public async Task<StatusCodeResult> OverheadSensorEndpoint()
        {

            var text = await Request.GetRawBodyStringAsync();

            var NewSensor = JsonSerializer.Deserialize<OverheadSensorPacket>(text);

            this.nodeContext.AddSensorData(NewSensor);
            return Ok();

        }

        [HttpPost("GroundSensorEndpoint")]
        public async Task<StatusCodeResult> GroundSensorEndpoint()
        {

            var text = await Request.GetRawBodyStringAsync();

            var UpdateContent = JsonSerializer.Deserialize<GroundSensorPacket>(text);
            this.nodeContext.AddSensorData(UpdateContent);

            return Ok();

        }

        [HttpGet("Section")]
        public async Task<ISectionCapacity> SectionEndpoint(string Section)
        {

            return new SectionCapacity() {
                AvailableCapacity = this.nodeContext.Sensors.Where(x => x.Value.Section == Section && x.Value.IsEmpty).Count(),
                Section = Section
            };

        }

    }
}