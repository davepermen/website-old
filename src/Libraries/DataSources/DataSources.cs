using Conesoft.Files;
using System;
using System.Linq;
using System.Reflection;

namespace Conesoft.DataSources
{
    public class DataSourcesImplementation : IDataSources
    {
        readonly Type rootType;

        public DataSourcesImplementation()
        {
            rootType = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.Name == "Program" && t.GetMethod("Main") != null).FirstOrDefault();
        }

        public string LocalDirectory => Local.Path;

        public string SharedDirectory => Shared.Path;

        Directory ServerRoot => Directory.From(AppContext.BaseDirectory) / ".." / "data";
        Directory DeveloperRoot => Directory.From(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) / "Webseiten" / "data";

        Directory Root => ServerRoot.Exists ? ServerRoot : DeveloperRoot;

        File ServerConfiguration => Directory.From(AppContext.BaseDirectory) / ".." / File.Name(rootType.Namespace, "json");

        public Directory Local => Root / rootType.Namespace;

        public Directory Shared => Root / "shared";

        internal static string Configuration => (Current as DataSourcesImplementation).ServerConfiguration.Path;

        public static IDataSources Current => new DataSourcesImplementation();
    }
}
