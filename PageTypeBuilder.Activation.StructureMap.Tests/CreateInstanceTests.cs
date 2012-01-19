using System;
using Moq;
using StructureMap;
using Xunit;

namespace PageTypeBuilder.Activation.StructureMap.Tests
{
    public class CreateInstanceTests
    {
        [Fact]
        public void CreateInstance_UsesInjectedContainerToCreateInstanceOfRequestedType()
        {
            string expectedCtorParam = Guid.NewGuid().ToString();
            Mock<IContainer> fakeContainer = new Mock<IContainer>();
            fakeContainer.Setup(container => container.GetInstance(typeof(string))).Returns(expectedCtorParam);
            StructureMapTypedPageActivator activator = new StructureMapTypedPageActivator(fakeContainer.Object);

            PageTypeWithStringCtorParam pageData = (PageTypeWithStringCtorParam) activator.CreateInstance(typeof(PageTypeWithStringCtorParam));

            Assert.Equal<string>(expectedCtorParam, pageData.Injected);
        }

        [Fact]
        public void CreateInstance_UsesInjectedContainerToCreateInstanceOfPropertyGroupInRequestedType()
        {
            string expectedCtorParam = Guid.NewGuid().ToString();
            Mock<IContainer> fakeContainer = new Mock<IContainer>();
            fakeContainer.Setup(container => container.GetInstance(typeof(string))).Returns(expectedCtorParam);
            StructureMapTypedPageActivator activator = new StructureMapTypedPageActivator(fakeContainer.Object);

            PageTypeThatHasPropertyGroup pageData = (PageTypeThatHasPropertyGroup)activator.CreateAndPopulateTypedInstance(new PageTypeThatHasPropertyGroup(), typeof(PageTypeThatHasPropertyGroup));

            Assert.Equal<string>(expectedCtorParam, pageData.PropertyGroup.Injected);
        }

        [Fact]
        public void CreateInstance_ThrowsPageTypeBuilderExceptionForTypeWithNoPublicCtor()
        {
            StructureMapTypedPageActivator activator = new StructureMapTypedPageActivator(null);

            Exception thrownException = Record.Exception(() => activator.CreateInstance(typeof(PageTypeWithNoPublicConstructor)));

            Assert.NotNull(thrownException);
            Assert.IsType<PageTypeBuilderException>(thrownException);
        }

    }
}
