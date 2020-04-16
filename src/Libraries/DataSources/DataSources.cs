using System;
using System.Linq;
using System.Reflection;
using IO = System.IO;

namespace Conesoft.DataSources
{
    public class DataSourcesImplementation : IDataSources
    {
        readonly Type rootType;

        public DataSourcesImplementation()
        {
            rootType = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.Name == "Program" && t.GetMethod("Main") != null).FirstOrDefault();
        }

        public string LocalDirectory => Directory(rootType.Namespace);

        public string SharedDirectory => Directory("shared");

        string Directory(string name) => IO.Directory.Exists($@"{AppContext.BaseDirectory}\..\data\{name}")
            ? $@"{AppContext.BaseDirectory}\..\data\{name}"
            : $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Webseiten\data\{name}"
            ;

        string LocalConfiguration => $@"{AppContext.BaseDirectory}\..\{rootType.Namespace}.json";

        internal static string Configuration => (Current as DataSourcesImplementation).LocalConfiguration;

        public static IDataSources Current => new DataSourcesImplementation();
    }
}
