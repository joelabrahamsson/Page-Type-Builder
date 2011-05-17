using System;
using System.Text;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.SpecializedProperties;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Tests.Synchronization.PageTypePropertyUpdaterTests;
using Rhino.Mocks;
using Xunit;
using Xunit.Extensions;

namespace PageTypeBuilder.Tests.Synchronization
{
    public class PageDefinitionTypeMapperTests
    {
        [Theory]
        [InlineData(typeof(PropertyBoolean), 0)]
        [InlineData(typeof(PropertyNumber), 1)]
        [InlineData(typeof(PropertyFloatNumber), 2)]
        [InlineData(typeof(PropertyPageType), 3)]
        [InlineData(typeof(PropertyPageReference), 4)]
        [InlineData(typeof(PropertyDate), 5)]
        [InlineData(typeof(PropertyString), 6)]
        [InlineData(typeof(PropertyLongString), 7)]
        [InlineData(typeof(PropertyCategory), 8)]
        public void NativePropertyTypes_ReturnsArrayWithPropertyTypeAtIndex(Type propertyType, int index)
        {
            PageDefinitionTypeMapper mapper = new PageDefinitionTypeMapper(null);

            Type[] nativePropertyTypes = mapper.NativePropertyTypes;

            Assert.Equal<Type>(propertyType, nativePropertyTypes[index]);
        }

        [Fact]
        public void GivenPageTypePropertyDefinition_GetPageDefinitionType_CallsGetPropertyType()
        {
            PageTypePropertyDefinition definition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            MockRepository fakes = new MockRepository();
            PageDefinitionTypeFactory fakeFactory = fakes.Stub<PageDefinitionTypeFactory>();
            PageDefinitionTypeMapper pageDefinitionTypeMapper = fakes.PartialMock<PageDefinitionTypeMapper>(fakeFactory);
            Type type = typeof(string);
            pageDefinitionTypeMapper.Stub(mapper => mapper.GetPropertyType(definition.PropertyType, definition.PageTypePropertyAttribute)).Return(type);
            fakes.ReplayAll();

            pageDefinitionTypeMapper.GetPageDefinitionType(definition);

            pageDefinitionTypeMapper.AssertWasCalled(mapper => mapper.GetPropertyType(definition.PropertyType, definition.PageTypePropertyAttribute));
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithTypeSet_GetPropertyType_ReturnsThatType()
        {
            PageDefinitionTypeMapper mapper = new PageDefinitionTypeMapper(null);
            PageTypePropertyDefinition definition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            definition.PageTypePropertyAttribute.Type = typeof(PropertyImageUrl);

            Type returnedType = mapper.GetPropertyType(definition.PropertyType, definition.PageTypePropertyAttribute);

            Assert.Equal<Type>(definition.PageTypePropertyAttribute.Type, returnedType);
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithNoTypeSet_GetPropertyType_GetsDefaultPropertyTypeForDefinitionsPropertyType()
        {
            PageTypePropertyDefinition definition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            definition.PageTypePropertyAttribute.Type = null;
            MockRepository fakes = new MockRepository();
            PageDefinitionTypeMapper pageDefinitionTypeMapper = fakes.PartialMock<PageDefinitionTypeMapper>((PageDefinitionTypeFactory)null);
            Type defaultType = typeof(string);
            pageDefinitionTypeMapper.Stub(utility => utility.GetDefaultPropertyType(definition.PropertyType)).Return(defaultType);
            pageDefinitionTypeMapper.Replay();

            Type returnedType = pageDefinitionTypeMapper.GetDefaultPropertyType(definition.PropertyType);

            pageDefinitionTypeMapper.AssertWasCalled(utility => utility.GetDefaultPropertyType(definition.PropertyType));
            Assert.Equal<Type>(defaultType, returnedType);
        }

        [Theory]
        [InlineData(typeof(string), typeof(PropertyXhtmlString))]
        [InlineData(typeof(int), typeof(PropertyNumber))]
        [InlineData(typeof(int?), typeof(PropertyNumber))]
        [InlineData(typeof(bool), typeof(PropertyBoolean))]
        [InlineData(typeof(DateTime), typeof(PropertyDate))]
        [InlineData(typeof(DateTime?), typeof(PropertyDate))]
        [InlineData(typeof(float), typeof(PropertyFloatNumber))]
        [InlineData(typeof(float?), typeof(PropertyFloatNumber))]
        [InlineData(typeof(PageReference), typeof(PropertyPageReference))]
        [InlineData(typeof(PageType), typeof(PropertyPageType))]
        [InlineData(typeof(LinkItemCollection), typeof(PropertyLinkCollection))]
        public void GivenType_GetDefaultPropertyType_ReturnsDefaultPropertyType(Type nativeType, Type expectedDefaultType)
        {
            PageDefinitionTypeMapper mapper = new PageDefinitionTypeMapper(null);

            Type returnedType = mapper.GetDefaultPropertyType(nativeType);

            Assert.Equal<Type>(expectedDefaultType, returnedType);
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithNoTypeAndNonMappedPropertyType_GetPageDefinitionType_ThrowsException()
        {
            PageDefinitionTypeMapper mapper = new PageDefinitionTypeMapper(null);
            Type unmappedType = typeof(StringBuilder);
            PageTypePropertyDefinition definition = new PageTypePropertyDefinition(
                TestValueUtility.CreateRandomString(), unmappedType, new NativePageType(), new PageTypePropertyAttribute());

            Exception exception = Record.Exception(() => { mapper.GetPageDefinitionType(definition); });

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenNativeType_GetNativeTypeID_ReturnsCorrespondingIndexFromNativePropertyTypes()
        {
            PageDefinitionTypeMapper mapper = new PageDefinitionTypeMapper(null);

            int nativeTypeID = mapper.GetNativeTypeID(typeof(PropertyString));

            Assert.Equal<Type>(mapper.NativePropertyTypes[nativeTypeID], typeof(PropertyString));
        }

        [Fact]
        public void GivenNonNativeType_GetNativeTypeID_ThrowsException()
        {
            PageDefinitionTypeMapper mapper = new PageDefinitionTypeMapper(null);
            Type nonNativeType = typeof(string);

            Exception exception = Record.Exception(() => { mapper.GetNativeTypeID(nonNativeType); });

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithNativeType_GetPageDefinitionType_ReturnsCorrectPageDefinitionType()
        {
            PageTypePropertyDefinition definition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            definition.PageTypePropertyAttribute.Type = typeof(PropertyString);
            MockRepository fakes = new MockRepository();
            PageDefinitionTypeFactory fakeFactory = fakes.Stub<PageDefinitionTypeFactory>();
            PageDefinitionTypeMapper mapper = new PageDefinitionTypeMapper(fakeFactory);
            int nativeTypeID = mapper.GetNativeTypeID(definition.PageTypePropertyAttribute.Type);
            PageDefinitionType pageDefinitionTypeFromFactory = new PageDefinitionType(1, PropertyDataType.String, TestValueUtility.CreateRandomString());
            fakeFactory.Stub(factory => factory.GetPageDefinitionType(nativeTypeID)).Return(pageDefinitionTypeFromFactory);
            fakeFactory.Replay();

            PageDefinitionType returnedPageDefinitionType = mapper.GetPageDefinitionType(definition);

            Assert.Equal<PageDefinitionType>(pageDefinitionTypeFromFactory, returnedPageDefinitionType);
        }

        [Fact]
        public void GivenPageTypePropertyDefinitionWithNonNativeType_GetPageDefinitionType_ReturnsCorrectPageDefinitionType()
        {
            PageTypePropertyDefinition definition = PageTypePropertyUpdaterTestsUtility.CreatePageTypePropertyDefinition();
            definition.PageTypePropertyAttribute.Type = typeof(PropertyXhtmlString);
            MockRepository fakes = new MockRepository();
            PageDefinitionTypeFactory fakeFactory = fakes.Stub<PageDefinitionTypeFactory>();
            PageDefinitionTypeMapper mapper = new PageDefinitionTypeMapper(fakeFactory);
            PageDefinitionType pageDefinitionTypeFromFactory = new PageDefinitionType(1, PropertyDataType.String, TestValueUtility.CreateRandomString());
            string typeName = definition.PageTypePropertyAttribute.Type.FullName;
            string assemblyName = definition.PageTypePropertyAttribute.Type.Assembly.GetName().Name;
            fakeFactory.Stub(factory => factory.GetPageDefinitionType(typeName, assemblyName)).Return(pageDefinitionTypeFromFactory);
            fakeFactory.Replay();

            PageDefinitionType returnedPageDefinitionType = mapper.GetPageDefinitionType(definition);

            Assert.Equal<PageDefinitionType>(pageDefinitionTypeFromFactory, returnedPageDefinitionType);
        }
    }
}