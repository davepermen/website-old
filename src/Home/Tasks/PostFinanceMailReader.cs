using Conesoft.DataSources;
using Conesoft.Files;
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

namespace Home.Tasks
{
    public class PostFinanceMailReader : IScheduledTask
    {
        public TimeSpan? Every => TimeSpan.FromMinutes(1);
        public TimeSpan? DailyAt => null;

        private readonly IConfigurationSection configuration;
        private readonly File file;

        public PostFinanceMailReader(IDataSources dataSources, IConfiguration configuration)
        {
            this.configuration = configuration.GetSection("postfinance-mail");
            this.file = dataSources.Local / "FromSources" / "PostFinance" / File.Name("AccountBalance", "txt");
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
                var messageSegment = body.Substring(body.IndexOf("Guten Tag"));
                messageSegment = messageSegment.Substring(0, messageSegment.IndexOf("Ihre PostFinance"));
                messageSegment = messageSegment.Replace("'", ""); // no thousands marks
                var values = Regex.Matches(messageSegment, @"([0-9]{1,}\.[0-9]{1,})").OfType<Match>().Select(m => decimal.Parse(m.Value));
                return (
                    change: messageSegment.Contains("Lastschrift") ? -values.First() : values.First(),
                    current: values.Last(),
                    at: message.Date.UtcDateTime
                );
            }).OrderByDescending(m => m.at).ToArray();
        }

        public async Task Run()
        {
            var results = CheckMails(configuration["server"], configuration["account"], configuration["app-password"], configuration["from"], configuration["sources"]);
            var now = DateTime.Now;

            await file.WriteLines(results.Select(v => $"{v.value};{v.change};{v.at:o}"));
        }
    }
}
