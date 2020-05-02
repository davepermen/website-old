using Home.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Home.API
{
    [Route("api/scheduler")]
    [ApiController]
    public class ScheduleTickerController : ControllerBase
    {
        [HttpPost("tick")]
        public async Task<IActionResult> PostTickAsync([FromServices] TickerScheduler tickerScheduler)
        {
            await tickerScheduler.Tick();
            return Ok("tock");
        }

        [HttpGet("log")]
        public async Task<IActionResult> GetLogAsync([FromServices] TickerScheduler tickerScheduler)
        {
            var output = Markdown.ToHtml(await tickerScheduler.Logfile.ReadText());
            return Content(output, "text/html");
        }

        [HttpPost("force-tick/{taskName}")]
        public async Task<IActionResult> ForcePostTickAsync([FromServices] TickerScheduler tickerScheduler, string taskName)
        {
            await tickerScheduler.ForceTick(taskName);
            return Ok("run " + taskName);
        }
    }
}