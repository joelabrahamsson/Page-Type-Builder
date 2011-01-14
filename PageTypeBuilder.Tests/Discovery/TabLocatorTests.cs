using System;
using System.Collections.Generic;
using System.Linq;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Tests.Helpers;
using Xunit;

namespace PageTypeBuilder.Tests.Discovery
{
    public class TabLocatorTests
    {
        [Fact]
        public void GivenNonAbstractDescendantOfTabInApplicationDomain_GetDefinedTabs_ReturnsListOfTabsWithInstanceOfThatClass()
        {
            Type typeThatShouldBeLocated = typeof(TestTab);
            TabLocator tabLocator = TabLocatorFactory.Create();

            IEnumerable<Tab> definedTabs = tabLocator.GetDefinedTabs();

            Assert.Equal<int>(1, definedTabs.Count(tab => tab.GetType() == typeThatShouldBeLocated));
        }
    }
}