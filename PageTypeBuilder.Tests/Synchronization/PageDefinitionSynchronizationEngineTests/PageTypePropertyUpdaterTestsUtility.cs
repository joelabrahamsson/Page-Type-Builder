using System;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Tests.Synchronization.PageDefinitionSynchronizationEngineTests
{
    public class PageTypePropertyUpdaterTestsUtility
    {
        public static PageTypePropertyDefinition CreatePageTypePropertyDefinition()
        {
            string name = TestValueUtility.CreateRandomString();
            Type type = typeof(string);
            IPageType pageType = new NativePageType();
            PageTypePropertyAttribute attribute = new PageTypePropertyAttribute();
            return new PageTypePropertyDefinition(name, type, pageType, attribute);
        }
    }
}