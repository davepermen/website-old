using Home.Services;
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
    }
}