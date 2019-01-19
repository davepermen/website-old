using System;
using System.Linq;
using System.Reflection;
using IO = System.IO;

namespace Conesoft
{
    public class DataSources : IDataSources
    {
        readonly Type rootType;

        public DataSources()
        {
            rootType = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.Name == "Program" && t.GetMethod("Main") != null).FirstOrDefault();
        }

        public string LocalDirectory => Directory(rootType.Namespace);

        public string SharedDirectory => Directory("shared");

        public string Directory(string name) => IO.Directory.Exists($@"{AppContext.BaseDirectory}\..\data\{name}")
            ? $@"{AppContext.BaseDirectory}\..\data\{name}"
            : $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Webseiten\data\{name}"
            ;

        public string LocalDatabase(string name) => $@"{LocalDirectory}\{name}.sqlite";

        public string SharedDatabase(string name) => $@"{SharedDirectory}\{name}.sqlite";

        public string SharedUserDatabase => SharedDatabase("users");

        string LocalConfiguration => $@"{AppContext.BaseDirectory}\..\{rootType.Namespace}.json";

        internal static string Configuration => new DataSources().LocalConfiguration;
    }
}
