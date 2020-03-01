using Conesoft.DataSources;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Tasks
{
    public class PostFinanceMailReader : IScheduledTask
    {
        public TimeSpan Every => TimeSpan.FromMinutes(1);

        private readonly IDataSources dataSources;
        private readonly IConfigurationSection configuration;

        public PostFinanceMailReader(IDataSources dataSources, IConfiguration configuration)
        {
            this.dataSources = dataSources;
            this.configuration = configuration.GetSection("postfinance-mail");
        }

        private IEnumerable<(decimal change, decimal value, DateTime at)> CheckMails(string server, string user, string password, string from, string sources)
        {
            using var client = new ImapClient
            {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            client.Connect(server, 993, true);
            client.Authenticate(user, password);

            var all = client.GetFolder(sources);
            all.Open(FolderAccess.ReadOnly);

            return all.Search(SearchQuery.FromContains(from).And(SearchQuery.SentSince(DateTime.Today - TimeSpan.FromDays(30)))).Select(id =>
            {
                var message = all.GetMessage(id);
                var body = message.GetTextBody(TextFormat.Html);
                var values = Regex.Matches(body, @"([0-9]{1,}\.[0-9]{1,})").OfType<Match>().Select(m => decimal.Parse(m.Value));
                return (
                    change: body.Contains("Lastschrift") ? -values.First() : values.First(),
                    current: values.Last(),
                    at: message.Date.UtcDateTime
                );
            }).OrderByDescending(m => m.at).ToArray();
        }

        public async Task Run()
        {
            var results = CheckMails(configuration["server"], configuration["account"], configuration["app-password"], configuration["from"], configuration["sources"]);
            var now = DateTime.Now;

            var file = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "PostFinance", "AccountBalance.txt");
            await IO.File.WriteAllLinesAsync(file, results.Select(v => $"{v.value};{v.change};{v.at.ToString("o")}"));
        }
    }
}
