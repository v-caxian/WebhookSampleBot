using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebhookSampleBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly TelemetryClient _telemetry;

        public SampleController(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The response data.</returns>
        [HttpGet]
        public async Task<string> Get()
        {
            var content = $"{MethodBase.GetCurrentMethod().DeclaringType.FullName}: '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'";
            Trace.TraceInformation("Sending payload: " + content);
            _telemetry.TrackTrace($"Sending payload: {content}", new Dictionary<string, string> { ["data"] = content });
            return await Task.FromResult(content);
        }

        /// <summary>
        /// Posts the data.
        /// </summary>
        /// <param name="data">The request data.</param>
        /// <param name="delay">The delay in milliseconds.</param>
        /// <returns>The response data.</returns>
        [HttpPost]
        public async Task<JsonDocument> Post([FromBody] object data, int delay = 0)
        {
            Trace.TraceInformation("Received data. Delay:'{0}'", delay);
            _telemetry.TrackTrace($"Received data. Delay:'{delay}'");
            await Task.Delay(delay);

            string content = Convert.ToString(data);

            Trace.TraceInformation("Sending payload: " + content);
            var payload = await JsonDocument.ParseAsync(new MemoryStream(Encoding.UTF8.GetBytes(content)));
            _telemetry.TrackTrace($"Sending payload: {content}", new Dictionary<string, string> { [nameof(data)] = ToJsonString(payload) });
            return payload;
        }

        private static string ToJsonString(JsonDocument jdoc)
        {
            using (var stream = new MemoryStream())
            {
                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                jdoc.WriteTo(writer);
                writer.Flush();
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }
    }
}
