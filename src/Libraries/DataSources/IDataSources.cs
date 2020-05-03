using Conesoft.Files;
using System;

namespace Conesoft.DataSources
{
    public interface IDataSources
    {
        [Obsolete("Use IDataSources.Local instead")]
        string LocalDirectory { get; }
        [Obsolete("Use IDataSources.Shared instead")]
        string SharedDirectory { get; }

        Directory Local { get; }
        Directory Shared { get; }
    }
}