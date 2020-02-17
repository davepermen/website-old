using Conesoft.DataSources;
using Fitness.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

namespace Fitness.Pages
{
    public class LivetileModel : PageModel
    {
        private int amount;

        public void OnGet([FromServices] IDataSources dataSources)
        {
            var trainingData = new TrainingData(dataSources, "davepermen", "pushups", DateTime.Now.Year);
            amount = trainingData.Sum;

            Response.ContentType = "text/xml";
        }

        public int Amount => amount;
    }
}