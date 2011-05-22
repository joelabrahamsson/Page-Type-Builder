using System.Collections.Generic;

namespace PageTypeBuilder.Discovery
{
    public interface IGlobalPropertySettingsLocator
    {
        IEnumerable<GlobalPropertySettingsUpdater> GetGlobalPropertySettingsUpdaters();
    }
}