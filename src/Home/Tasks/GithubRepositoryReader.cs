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

        public TimeSpan Every => TimeSpan.FromMinutes(5);

        public GithubRepositoryReader(IHttpClientFactory httpClientFactory, IDataSources dataSources)
        {
            this.client = httpClientFactory.CreateClient();
            this.dataSources = dataSources;
        }

        async Task<string> GetRepository(string organisationOrUsername)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("davepermen.net");
            var result = await client.GetAsync($"https://api.github.com/users/{organisationOrUsername}/repos?sort=pushed");
            return result.IsSuccessStatusCode ? await result.Content.ReadAsStringAsync() : null;
        }

        public async Task Run()
        {
            var path = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "Github Repositories");
            IO.Directory.CreateDirectory(path);

            var repositories = new[] { "davepermen", "conesoft" };

            foreach (var repository in repositories)
            {
                var repositoryPath = IO.Path.Combine(path, $"{repository}.json");
                var result = await GetRepository(repository);
                if (result != null)
                {
                    await IO.File.WriteAllTextAsync(repositoryPath, result);
                }
            }

            try
            {
                var repositoryPath = IO.Path.Combine(path, $"rate_limit.json");
                var result = await client.GetAsync($"https://api.github.com/rate_limit");
                await IO.File.WriteAllTextAsync(repositoryPath, await result.Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
            }
        }
    }
}
