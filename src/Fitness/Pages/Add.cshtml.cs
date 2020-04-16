using Conesoft.DataSources;
using Fitness.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;

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
    }
}