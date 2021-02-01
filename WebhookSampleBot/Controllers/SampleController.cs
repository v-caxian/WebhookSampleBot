using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebhookSampleBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <returns>The response data.</returns>
        [HttpGet]
        public async Task<string> Get()
        {
            return await Task.FromResult($"{MethodBase.GetCurrentMethod().DeclaringType.FullName}: '{DateTime.Now:yyyy-MM-dd HH:mm:ss}'");
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
            await Task.Delay(delay);

            string content = Convert.ToString(data);
            Trace.TraceInformation("Sending payload: " + content);
            return await JsonDocument.ParseAsync(new MemoryStream(Encoding.UTF8.GetBytes(content)));
        }
    }
}
