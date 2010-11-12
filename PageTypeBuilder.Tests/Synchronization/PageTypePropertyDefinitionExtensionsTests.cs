using EPiServer.DataAbstraction;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization
{
    public class PageTypePropertyDefinitionExtensionsTests
    {
        [Fact]
        public void GivenPageTypePropertyDefinitionWithNoEditCaption_GetEditCaptionOrName_ReturnsName()
        {
            string propertyName = TestValueUtility.CreateRandomString();
            PageTypePropertyDefinition definition = 
                new PageTypePropertyDefinition(propertyName, typeof(string), new PageType(), new PageTypePropertyAttribute());

            string returnedEditCaption = definition.GetEditCaptionOrName();

            Assert.Equal<string>(propertyName, returnedEditCaption);
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithEditCaption_GetEditCaptionOrName_ReturnsEditCaptionFromAttribute()
        {
            PageTypePropertyDefinition definition =
                new PageTypePropertyDefinition(
                    TestValueUtility.CreateRandomString(), typeof(string), new PageType(), new PageTypePropertyAttribute());
            string editCaption = TestValueUtility.CreateRandomString();
            definition.PageTypePropertyAttribute.EditCaption = editCaption;

            string returnedEditCaption = definition.GetEditCaptionOrName();

            Assert.Equal<string>(editCaption, returnedEditCaption);
        }
    }
}
