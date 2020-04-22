using Conesoft.DataSources;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IO = System.IO;

namespace Home.Tasks
{
    public class ServerHostingDnsUpdater : IScheduledTask
    {
        private readonly IConfigurationSection configuration;
        private readonly IDataSources dataSources;
        private readonly Conesoft.DNSimple.Client dnsimple;
        private readonly Conesoft.Ipify.Client ipify;
        public TimeSpan? Every => TimeSpan.FromMinutes(1);
        public TimeSpan? DailyAt => null;

        public ServerHostingDnsUpdater(IDataSources dataSources, IConfiguration configuration, Conesoft.DNSimple.Client dnsimple, Conesoft.Ipify.Client ipify)
        {
            this.configuration = configuration.GetSection("hosting");
            this.dataSources = dataSources;
            this.dnsimple = dnsimple;
            this.ipify = ipify;
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
            var path = IO.Path.Combine(dataSources.LocalDirectory, "FromSources", "Ipify", "Ip.txt");
            IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(path));
            try
            {
                IPAddress lastIp = default;
                if(IO.File.Exists(path))
                {
                    lastIp = IPAddress.Parse(await IO.File.ReadAllTextAsync(path));
                }

                var currentIp = await ipify.GetPublicIPAddress();

                if (currentIp.ToString() != lastIp.ToString()) // urgh, value comparison, the cheap way
                {
                    await UpdateDnsRecord(currentIp);
                    await IO.File.WriteAllTextAsync(path, currentIp.ToString());
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
