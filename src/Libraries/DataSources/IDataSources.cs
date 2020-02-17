namespace Conesoft.DataSources
{
    public interface IDataSources
    {
        string LocalDirectory { get; }
        string SharedDirectory { get; }
    }
}