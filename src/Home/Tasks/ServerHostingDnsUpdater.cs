using Conesoft.DataSources;
using Conesoft.Files;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Home.Tasks
{
    public class ServerHostingDnsUpdater : IScheduledTask
    {
        private readonly IConfigurationSection configuration;
        private readonly Conesoft.DNSimple.Client dnsimple;
        private readonly Conesoft.Ipify.Client ipify;
        private readonly File file;

        public TimeSpan? Every => TimeSpan.FromMinutes(1);
        public TimeSpan? DailyAt => null;

        public ServerHostingDnsUpdater(IDataSources dataSources, IConfiguration configuration, Conesoft.DNSimple.Client dnsimple, Conesoft.Ipify.Client ipify)
        {
            this.configuration = configuration.GetSection("hosting");
            this.dnsimple = dnsimple;
            this.ipify = ipify;
            this.file = dataSources.Local / "FromSources" / "Ipify" / File.Name("Ip", "txt");
        }

        public async Task UpdateDnsRecord(IPAddress address)
        {
            dnsimple.UseToken(configuration["dnsimple-token"]);

            var account = (await dnsimple.GetAccounts()).First();

            var zone = await account.GetZone(configuration["domain"]);

            var record = await zone.GetRecord(Conesoft.DNSimple.RecordType.A);

            await record.UpdateContent(address.ToString());
        }

        public async Task Run()
        {
            try
            {
                IPAddress lastIp = IPAddress.None;
                if(file.Exists)
                {
                    lastIp = IPAddress.Parse(await file.ReadText());
                }

                var currentIp = await ipify.GetPublicIPAddress();

                if (currentIp.ToString() != lastIp.ToString()) // urgh, value comparison, the cheap way
                {
                    await UpdateDnsRecord(currentIp);
                    await file.WriteText(currentIp.ToString());
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
