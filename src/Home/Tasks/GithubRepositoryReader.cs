using Conesoft.DataSources;
using Conesoft.Files;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Home.Tasks
{
    public class GithubRepositoryReader : IScheduledTask
    {
        private readonly HttpClient client;
        private readonly Directory path;

        public TimeSpan? Every => TimeSpan.FromMinutes(5);
        public TimeSpan? DailyAt => null;

        public GithubRepositoryReader(IHttpClientFactory httpClientFactory, IDataSources dataSources)
        {
            this.client = httpClientFactory.CreateClient();
            this.path = dataSources.Local / "FromSources" / "Github Repositories";
        }

        async Task<string?> GetRepository(string organisationOrUsername)
        {
            client.DefaultRequestHeaders.UserAgent.ParseAdd("davepermen.net");
            var result = await client.GetAsync($"https://api.github.com/users/{organisationOrUsername}/repos?sort=pushed");
            return result.IsSuccessStatusCode ? await result.Content.ReadAsStringAsync() : null;
        }

        public async Task Run()
        {
            var repositories = new[] { "davepermen", "conesoft" };

            foreach (var repository in repositories)
            {
                var repositoryFile = path / File.Name(repository, "json");
                var result = await GetRepository(repository);
                if (result != null)
                {
                    await repositoryFile.WriteText(result);
                }
            }

            try
            {
                var file = path / File.Name("rate_limit", "json");
                await file.WriteText(await client.GetStringAsync($"https://api.github.com/rate_limit"));
            }
            catch (Exception)
            {
            }
        }
    }
}
