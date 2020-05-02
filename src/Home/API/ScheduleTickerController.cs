﻿using Home.Services;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IO = System.IO;

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
            var output = Markdown.ToHtml(await IO.File.ReadAllTextAsync(tickerScheduler.LogPath));
            return Content(output, "text/html");
        }
    }
}