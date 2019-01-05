using Conesoft;
using Fitness.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using IO = System.IO;

namespace Fitness.Pages
{
    public class AddModel : PageModel
    {
        readonly IDataSources dataSources;

        public AddModel(IDataSources dataSources)
        {
            this.dataSources = dataSources;
        }

        readonly int year = DateTime.Now.Year;

        public IActionResult OnPost(int amount, string training)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity.Name;

                var trainingData = new TrainingData(dataSources, user, training, year);
                trainingData.Add(amount);
                UpdateLiveTile(trainingData);

                if (Request.Form.ContainsKey("redirectto"))
                {
                    return Redirect(Request.Form["redirectto"]);
                }
                return Page();
            }
            else
            {
                return Unauthorized();
            }
        }

        private void UpdateLiveTile(TrainingData trainingData)
        {
            if (User.Identity.IsAuthenticated && User.Identity.Name == "davepermen")
            {
                var template = IO.File.ReadAllText("wwwroot\\livetile.xml.template");
                var content = template.Replace("{{amount}}", trainingData.Sum.ToString());
                IO.File.WriteAllText("wwwroot\\livetile.xml", content);
            }
        }
    }
}