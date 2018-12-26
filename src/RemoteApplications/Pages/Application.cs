using System;

namespace RemoteApplications.Pages
{
    public class Application
    {
        public string Name { get; set; }
        public string RdpName => Name + ".rdp";
        public Icon[] Icons { get; set; }
        public DateTime PublishDate { get; set; }
    }
}