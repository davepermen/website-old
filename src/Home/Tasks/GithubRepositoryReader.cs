using Conesoft.DataSources;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Tasks
{
    public class GithubRepositoryReader : IScheduledTask
    {
        private readonly HttpClient client;
        private readonly IDataSources dataSources;

        public TimeSpan Every => TimeSpan.FromMinutes(1);

        public GithubRepositoryReader(IHttpClientFactory httpClientFactory, IDataSources dataSources)
        {
            this.client = httpClientFactory.CreateClient();
            this.dataSources = dataSources;
        }

        async Task<string> GetRepositories(string organisationOrUsername)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("davepermen.net");
            var result = await client.GetAsync($"https://api.github.com/users/{organisationOrUsername}/repos?sort=pushed");
            return await result.Content.ReadAsStringAsync();
        }

        public async Task Run()
        {
            var path = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "Github Repositories");
            IO.Directory.CreateDirectory(path);

            var repositories = new[] { "davepermen", "conesoft" };

            foreach(var repository in repositories)
            {
                var repositoryPath = IO.Path.Combine(path, $"{repository}.json");
                await IO.File.WriteAllTextAsync(repositoryPath, await GetRepositories(repository));
            }
        }
    }
}
