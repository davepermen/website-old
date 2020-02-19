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
        public async Task<IActionResult> PostPostFinanceUpdateAsync([FromForm] string value, [FromServices] IDataSources dataSources)
        {
            IO.Directory.CreateDirectory(IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "PostFinance"));
            await IO.File.WriteAllTextAsync(IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "PostFinance", "AccountBalance.txt"), value);

            using var stream = IO.File.OpenWrite(IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "PostFinance", "AccountBalance-Body.txt"));
            await Request.Body.CopyToAsync(stream);
            stream.Close();

            return Ok();
        }
    }
}