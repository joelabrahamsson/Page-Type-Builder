using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core.PropertySettings;

namespace PageTypeBuilder.Specs.Helpers
{
    public static class PropertySettingsRepositoryExtensions
    {
        public static IEnumerable<PropertySettingsWrapper> GetGlobalWrappers<TSettings>(this IPropertySettingsRepository repository, string displayName)
        {
            return
                repository.GetGlobals(typeof(TSettings)).Where(
                    w => w.DisplayName.Equals(displayName));
        }
    }
}
