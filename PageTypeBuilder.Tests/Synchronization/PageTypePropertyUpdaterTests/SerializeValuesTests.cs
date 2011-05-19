using EPiServer.DataAbstraction;
using EPiServer.Editor;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Helpers;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypePropertyUpdaterTests
{
    public class SerializeValuesTests
    {
        [Fact]
        public void GivenPageDefinitionWithSpecifiedLongStringSetting_UpdatePageDefinition_ReturnsDifferentStringThanWhenCalledWithPageDefinitionWithNoLongStringSettings()
        {
            PageTypePropertyUpdater pageTypePropertyUpdater = PageTypePropertyUpdaterFactory.Create();
            PageDefinition pageDefinition = new PageDefinition();
            string valuesWithOutClearAllLongStringSettings = pageTypePropertyUpdater.SerializeValues(pageDefinition);
            pageDefinition.LongStringSettings = EditorToolOption.Bold;

            string returnedValue = pageTypePropertyUpdater.SerializeValues(pageDefinition);

            Assert.NotEqual<string>(valuesWithOutClearAllLongStringSettings, returnedValue);
        }
    }
}
