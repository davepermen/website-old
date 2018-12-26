using System;
using System.Linq;
using System.Reflection;
using IO = System.IO;

namespace DataSources
{
    public class Root
    {
        readonly Type rootType;

        public Root()
        {
            rootType = Assembly.GetEntryAssembly().ExportedTypes.Where(t => t.Name == "Program" && t.GetMethod("Main") != null).FirstOrDefault();
        }

        public string LocalDirectory => Directory(rootType.Namespace);

        public string SharedDirectory => Directory("shared");

        public string Directory(string name) => IO.Directory.Exists($@"{IO.Directory.GetCurrentDirectory()}\..\data\{name}")
            ? $@"{IO.Directory.GetCurrentDirectory()}\..\data\{name}"
            : $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Webseiten\data\{name}"
            ;

        public string LocalDatabase(string name) => $@"{LocalDirectory}\{name}.sqlite";

        public string SharedDatabase(string name) => $@"{SharedDirectory}\{name}.sqlite";

        public string SharedUserDatabase => SharedDatabase("users");
    }
}
