using Conesoft.DataSources;
using Microsoft.AspNetCore.Mvc;
using System.IO;
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

            using var reader = new StreamReader(Request.Body);
            await IO.File.WriteAllTextAsync(
                IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "PostFinance", "AccountBalance.txt"),
                await reader.ReadToEndAsync()
                );

            return Ok();
        }
    }
}