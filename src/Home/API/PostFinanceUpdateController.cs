using Conesoft.DataSources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.API
{
    [Route("api/postfinance")]
    [ApiController]
    public class PostFinanceUpdateController : ControllerBase
    {
        [HttpPost("update")]
        public async Task<IActionResult> PostPostFinanceUpdateAsync([FromServices] IDataSources dataSources)
        {
            IO.Directory.CreateDirectory(IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "PostFinance"));

            using var stream = IO.File.OpenWrite(IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "PostFinance", "AccountBalance.txt"));
            await Request.Body.CopyToAsync(stream);
            stream.Close();

            return Ok();
        }

        [HttpGet("ping")]
        public Task<IActionResult> GetPing() => Task.FromResult<IActionResult>(Ok("ping"));
    }
}