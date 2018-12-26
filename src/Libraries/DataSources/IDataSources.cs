namespace Conesoft
{
    public interface IDataSources
    {
        string LocalDirectory { get; }
        string SharedDirectory { get; }
        string SharedUserDatabase { get; }

        string Directory(string name);
        string LocalDatabase(string name);
        string SharedDatabase(string name);
    }
}