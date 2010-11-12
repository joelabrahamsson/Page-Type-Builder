using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeUpdaterTests
{
    public static class PageTypeUpdaterTestsUtility
    {
        public static PageTypeDefinition CreateBasicPageTypeDefinition()
        {
            return new PageTypeDefinition
                       {
                           Type = typeof(object),
                           Attribute = new PageTypeAttribute()
                       };
        }
    }
}