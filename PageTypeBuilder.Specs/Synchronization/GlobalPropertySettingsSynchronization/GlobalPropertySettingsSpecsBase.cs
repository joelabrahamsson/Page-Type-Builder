using System.Collections.Generic;
using EPiServer.Core.PropertySettings;
using PageTypeBuilder.Specs.Helpers;

namespace PageTypeBuilder.Specs.Synchronization.GlobalPropertySettingsSynchronization
{
    public abstract class GlobalPropertySettingsSpecsBase : SynchronizationSpecs
    {
        protected static IEnumerable<PropertySettingsWrapper> GetGlobalWrappersByDisplayName<TSettings>(string displayName)
        {
            return SyncContext.PropertySettingsRepository.GetGlobalWrappers<TSettings>(displayName);
        }
    }
}
