using System;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Tests.Synchronization.PageTypePropertyUpdaterTests
{
    public class PageTypePropertyUpdaterTestsUtility
    {
        public static PageTypePropertyDefinition CreatePageTypePropertyDefinition()
        {
            string name = TestValueUtility.CreateRandomString();
            Type type = typeof(string);
            PageType pageType = new PageType();
            PageTypePropertyAttribute attribute = new PageTypePropertyAttribute();
            return new PageTypePropertyDefinition(name, type, pageType, attribute);
        }
    }
}