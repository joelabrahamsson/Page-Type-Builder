using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using PageTypeBuilder.Discovery;
using Xunit;

namespace PageTypeBuilder.Tests.Discovery
{
    public class TabLocatorTests
    {
        [Fact]
        public void GivenNonAbstractDescendantOfTabInApplicationDomain_GetDefinedTabs_ReturnsListOfTabsWithInstanceOfThatClass()
        {
            Type typeThatShouldBeLocated = typeof(TestTab);
            TabLocator tabLocator = new TabLocator();

            List<Tab> definedTabs = tabLocator.GetDefinedTabs();

            Assert.Equal<int>(1, definedTabs.Count(tab => tab.GetType() == typeThatShouldBeLocated));
        }
    }
}