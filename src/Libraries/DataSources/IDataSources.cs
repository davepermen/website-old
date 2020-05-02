using Conesoft.Files;

namespace Conesoft.DataSources
{
    public interface IDataSources
    {
        string LocalDirectory { get; }
        string SharedDirectory { get; }

        Directory Local { get; }
        Directory Shared { get; }
    }
}