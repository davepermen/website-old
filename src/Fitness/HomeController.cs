using Microsoft.AspNetCore.Mvc;
using System;

namespace Fitness
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View("Index", new HomeControllerModel("pushups\\davepermen"));
        
        [HttpGet("/add")]
        public IActionResult Add() => View("Add", new HomeControllerModel("pushups\\davepermen"));

        [HttpPost("/add")]
        public IActionResult Add(int pushups)
        {
            Log(pushups);
            return Redirect("/add");
        }

        private void Log(int pushups)
        {
            System.IO.Directory.CreateDirectory("pushups");
            System.IO.Directory.CreateDirectory("pushups\\davepermen");

            var filename = $"pushups\\davepermen\\{DateTime.UtcNow:yyyy-MM-dd}.txt";
            var current = System.IO.File.Exists(filename) ? int.Parse(System.IO.File.ReadAllText(filename)) : 0;
            System.IO.File.WriteAllText(filename, $"{current + pushups}");
        }
    }
}