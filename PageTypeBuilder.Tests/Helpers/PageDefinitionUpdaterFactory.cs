using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;

namespace PageTypeBuilder.Tests.Helpers
{
    public class PageDefinitionUpdaterFactory
    {
        public static PageDefinitionUpdater Create()
        {
            return new PageDefinitionUpdater(
                new PageDefinitionFactory(),
                new PageDefinitionTypeFactory(), 
                new TabFactory());
        }
    }
}
