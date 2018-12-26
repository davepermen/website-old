//using Conesoft;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Configuration;
//using System;
//using IO = System.IO;

//namespace Fitness
//{
//    public class HomeController : Controller
//    {
//        private readonly string training = "pushups";
//        private readonly string user = "davepermen";
//        private readonly IDataSources dataSources;

//        public HomeController(IDataSources dataSources, IConfiguration configuration)
//        {
//            this.dataSources = dataSources;
//            this.user = configuration.GetValue<string>("user");
//        }

//        [HttpGet]
//        public IActionResult Index() => View("Index", new HomeControllerModel(training, user, dataSources));
        
//        [HttpGet("/add")]
//        public IActionResult Add() => View("Add", new HomeControllerModel(training, user, dataSources));

//        [HttpPost("/add")]
//        public IActionResult Add(int pushups)
//        {
//            Log(pushups);
//            return Redirect("/add");
//        }

//        private void Log(int pushups)
//        {
//            IO.Directory.CreateDirectory($@"{dataSources.LocalDirectory}\{training}");
//            IO.Directory.CreateDirectory($@"{dataSources.LocalDirectory}\{training}\{user}");

//            var filename = $@"{dataSources.LocalDirectory}\{training}\{user}\{DateTime.UtcNow:yyyy-MM-dd}.txt";
//            var current = IO.File.Exists(filename) ? int.Parse(IO.File.ReadAllText(filename)) : 0;
//            IO.File.WriteAllText(filename, $"{current + pushups}");

//            UpdateLiveTile(new HomeControllerModel(training, user, dataSources));
//        }

//        private void UpdateLiveTile(HomeControllerModel model)
//        {
//            var template = IO.File.ReadAllText("wwwroot\\livetile.xml.template");
//            var content = template.Replace("{{amount}}", model.Pushups.ToString());
//            IO.File.WriteAllText("wwwroot\\livetile.xml", content);
//        }
//    }
//}