namespace PageTypeBuilder.Tests.Synchronization
{
    using Abstractions;
    using PageTypeBuilder.Discovery;
    using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
    using Xunit;

    public class PageTypePropertyDefinitionExtensionsTests
    {
        [Fact]
        public void GivenPageTypePropertyDefinitionWithNoEditCaption_GetEditCaptionOrName_ReturnsName()
        {
            string propertyName = TestValueUtility.CreateRandomString();
            PageTypePropertyDefinition definition =
                new PageTypePropertyDefinition(propertyName, typeof(string), new NativePageType(), new PageTypePropertyAttribute(), null);

            string returnedEditCaption = definition.GetEditCaptionOrName(false);

            Assert.Equal<string>(propertyName, returnedEditCaption);
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithEditCaption_GetEditCaptionOrName_ReturnsEditCaptionFromAttribute()
        {
            PageTypePropertyDefinition definition =
                new PageTypePropertyDefinition(TestValueUtility.CreateRandomString(), typeof(string), new NativePageType(), new PageTypePropertyAttribute(), null);
            string editCaption = TestValueUtility.CreateRandomString();
            definition.PageTypePropertyAttribute.EditCaption = editCaption;

            string returnedEditCaption = definition.GetEditCaptionOrName(false);

            Assert.Equal<string>(editCaption, returnedEditCaption);
        }
    }
}
