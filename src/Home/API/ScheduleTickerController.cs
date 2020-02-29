using Microsoft.AspNetCore.Mvc;

namespace Home.API
{
    [Route("api/scheduler")]
    [ApiController]
    public class ScheduleTickerController : ControllerBase
    {
        [HttpPost("tick")]
        public IActionResult PostTick()
        {
            return Ok("tock");
        }
    }
}