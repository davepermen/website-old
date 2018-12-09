using Microsoft.AspNetCore.Mvc;
using System;

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
            System.IO.Directory.CreateDirectory($@"{Program.DataRoot}\{training}");
            System.IO.Directory.CreateDirectory($@"{Program.DataRoot}\{training}\{user}");

            var filename = $@"{Program.DataRoot}\{training}\{user}\{DateTime.UtcNow:yyyy-MM-dd}.txt";
            var current = System.IO.File.Exists(filename) ? int.Parse(System.IO.File.ReadAllText(filename)) : 0;
            System.IO.File.WriteAllText(filename, $"{current + pushups}");
        }
    }
}