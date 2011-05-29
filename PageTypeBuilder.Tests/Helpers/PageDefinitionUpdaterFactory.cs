using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageDefinitionUpdaterFactory
    {
        public static PageDefinitionUpdater Create()
        {
            return new PageDefinitionUpdater(
                new PageDefinitionFactory(), 
                new TabFactory());
        }
    }
}
