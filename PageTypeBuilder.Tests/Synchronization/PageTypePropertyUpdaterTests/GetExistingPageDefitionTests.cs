using System;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypePropertyUpdaterTests
{
    public class GetExistingPageDefitionTests
    {
        [Fact]
        public void GivenPageTypeWithPropertyAndCorrespondingPropertyDefition_GetExistingPropertyDefinition_ReturnsPageDefinition()
        {
            PageTypePropertyUpdater utility = new PageTypePropertyUpdater();
            PageType pageType = new PageType();
            string name = TestValueUtility.CreateRandomString();
            Type type = typeof(string);
            PageTypePropertyAttribute attribute = new PageTypePropertyAttribute();
            PageTypePropertyDefinition pageTypePropertyDefinition = new PageTypePropertyDefinition(name, type, pageType, attribute);
            PageDefinition pageDefinition = new PageDefinition();
            pageDefinition.Name = name;
            pageType.Definitions.Add(pageDefinition);

            PageDefinition returnedPageDefinition = utility.GetExistingPageDefinition(pageType, pageTypePropertyDefinition);

            Assert.Equal<PageDefinition>(pageDefinition, returnedPageDefinition);
        }
    }
}