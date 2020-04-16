﻿using Conesoft.DataSources;
using System;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Tasks
{
    public class SimpleTicker : IScheduledTask
    {
        private readonly IDataSources dataSources;

        public TimeSpan Every => TimeSpan.FromMinutes(1);

        public SimpleTicker(IDataSources dataSources)
        {
            this.dataSources = dataSources;
        }

        public async Task Run()
        {
            var now = DateTime.Now;
            await IO.File.WriteAllTextAsync(IO.Path.Combine(dataSources.LocalDirectory, "tick.txt"), now.ToLongDateString() + " " + now.ToLongTimeString());
        }
    }
}