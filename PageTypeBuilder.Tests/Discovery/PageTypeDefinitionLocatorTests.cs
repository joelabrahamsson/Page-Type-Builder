using System.Collections.Generic;
using System.Linq;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Tests.Helpers;
using Xunit;

namespace PageTypeBuilder.Tests.Discovery
{
    public class GetPageTypeDefinitionsTests
    {
        [Fact]
        public void GetPageTypeDefinitions_ReturnsListOfNonAbstractTypesWithAttributeInApplicationDomain()
        {
            PageTypeDefinitionLocator definitionLocator = PageTypeDefinitionLocatorFactory.Create();

            IEnumerable<PageTypeDefinition> definitions = definitionLocator.GetPageTypeDefinitions();

            Assert.Equal<int>(6, definitions.Count());
        }

        [Fact]
        public void GetPageTypeDefinition_IncludesTypesWithAttributeThatIsSubtypeOfPageTypeAttribute()
        {
            PageTypeDefinitionLocator definitionLocator = PageTypeDefinitionLocatorFactory.Create();

            IEnumerable<PageTypeDefinition> definitions = definitionLocator.GetPageTypeDefinitions();

            Assert.Equal<int>(1, definitions.Where(definition => definition.Attribute != null &&  definition.Attribute.GetType() == typeof(CustomPageTypeAttribute)).Count());
        }
    }
}