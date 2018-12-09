using Microsoft.AspNetCore.Mvc;
using System;
using IO = System.IO;

namespace Fitness
{
    public class HomeController : Controller
    {
        private readonly string training = "pushups";
        private readonly string user = "davepermen";
        [HttpGet]
        public IActionResult Index() => View("Index", new HomeControllerModel(training, user));
        
        [HttpGet("/add")]
        public IActionResult Add() => View("Add", new HomeControllerModel(training, user));

        [HttpPost("/add")]
        public IActionResult Add(int pushups)
        {
            Log(pushups);
            return Redirect("/add");
        }

        private void Log(int pushups)
        {
            IO.Directory.CreateDirectory($@"{Program.DataRoot}\{training}");
            IO.Directory.CreateDirectory($@"{Program.DataRoot}\{training}\{user}");

            var filename = $@"{Program.DataRoot}\{training}\{user}\{DateTime.UtcNow:yyyy-MM-dd}.txt";
            var current = IO.File.Exists(filename) ? int.Parse(IO.File.ReadAllText(filename)) : 0;
            IO.File.WriteAllText(filename, $"{current + pushups}");

            UpdateLiveTile(new HomeControllerModel(training, user));
        }

        private void UpdateLiveTile(HomeControllerModel model)
        {
            var template = IO.File.ReadAllText("wwwroot\\livetile.xml.template");
            var content = template.Replace("{{amount}}", model.Pushups.ToString());
            IO.File.WriteAllText("wwwroot\\livetile.xml", content);
        }
    }
}