using System;
using System.Collections.Generic;
using EPiServer.Core;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.PageDefinitionSynchronization;
using PageTypeBuilder.Synchronization.Validation;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.Validation
{
    public class PageTypeDefinitionValidatorTests
    {
        [Fact]
        public void Constructor_SetsPropertiesValidtorProperty()
        {
            PageTypeDefinitionValidator pageTypeValidator = new PageTypeDefinitionValidator(null);

            Assert.NotNull(pageTypeValidator.PropertiesValidator);
        }

        [Fact]
        public void GivenListOfPageTypeDefinitions_ValidatePageTypeDefinitions_ValidatesPageTypesHaveGuidOrUniqueNamee()
        {
            MockRepository fakes = new MockRepository();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition definition = new PageTypeDefinition
            {
                Type = typeof(TypedPageData),
                Attribute = new PageTypeAttribute()
            };
            definitions.Add(definition);
            PageTypeDefinitionValidator definitionValidator = fakes.PartialMock<PageTypeDefinitionValidator>((PageDefinitionTypeMapper)null);
            definitionValidator.Stub(validator => validator.ValidatePageTypesHaveGuidOrUniqueName(definitions));
            definitionValidator.Replay();

            definitionValidator.ValidatePageTypeDefinitions(definitions);

            definitionValidator.AssertWasCalled(validator => validator.ValidatePageTypesHaveGuidOrUniqueName(definitions));
        }

        [Fact]
        public void GivenListOfPageTypeDefinitions_ValidatePageTypeDefinitions_ValidatesEachPageType()
        {
            MockRepository fakes = new MockRepository();
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition definition = new PageTypeDefinition
            {
                Type = typeof(TypedPageData),
                Attribute = new PageTypeAttribute()
            };
            definitions.Add(definition);
            PageTypeDefinitionValidator definitionValidator = fakes.PartialMock<PageTypeDefinitionValidator>((PageDefinitionTypeMapper)null);
            definitionValidator.Stub(validator => validator.ValidatePageTypeDefinition(definition, definitions));
            definitionValidator.Replay();

            definitionValidator.ValidatePageTypeDefinitions(definitions);

            definitionValidator.AssertWasCalled(validator => validator.ValidatePageTypeDefinition(definition, definitions));
        }

        [Fact]
        public void GivenDefinitionNameLongerThan50Chars_ValidatePageTypeDefinitions_ThrowsException()
        {
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition invalidTypeDefinition = new PageTypeDefinition
            {
                Type = typeof(TypedPageData),
                Attribute = new PageTypeAttribute() { Name = "123456789123456789123456789123456789123456789123456" }
            };
            definitions.Add(invalidTypeDefinition);
            PageTypeDefinitionValidator definitionValidator = new PageTypeDefinitionValidator(null);

            Exception exception =
                Record.Exception(
                    () => definitionValidator.ValidatePageTypeDefinitions(definitions));

            Assert.NotNull(exception);
            Type exceptionType = exception.GetType();
            Assert.Equal<Type>(typeof(PageTypeBuilderException), exceptionType);
        }

        [Fact]
        public void GivenDefinitionWithTypeThatDoesNotInheritFromTypedPageData_ValidatePageTypeDefinitions_ThrowsException()
        {
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition invalidTypeDefinition = new PageTypeDefinition
            {
                Type = typeof(PageData),
                Attribute = new PageTypeAttribute()
            };
            definitions.Add(invalidTypeDefinition);
            PageTypeDefinitionValidator definitionValidator = new PageTypeDefinitionValidator(null);

            Exception exception =
                Record.Exception(
                    () => definitionValidator.ValidatePageTypeDefinitions(definitions));

            Assert.NotNull(exception);
            Type exceptionType = exception.GetType();
            Assert.Equal<Type>(typeof(PageTypeBuilderException), exceptionType);
        }

        [Fact]
        public void ValidatePageTypeDefinition_ValidatesProperties()
        {
            PageTypeDefinition pageTypeDefinition = new PageTypeDefinition();
            MockRepository fakes = new MockRepository();
            PageTypeDefinitionValidator pageTypeValidator = fakes.PartialMock<PageTypeDefinitionValidator>((PageDefinitionTypeMapper)null);
            pageTypeValidator.Stub(validator => validator.ValidateInheritsFromBasePageType(pageTypeDefinition));
            pageTypeValidator.Stub(validator => validator.ValidateAvailablePageTypes(pageTypeDefinition, null));
            pageTypeValidator.Stub(validator => validator.ValidateNameLength(pageTypeDefinition));
            pageTypeValidator.Replay();
            pageTypeValidator.PropertiesValidator = fakes.Stub<PageTypeDefinitionPropertiesValidator>((PageDefinitionTypeMapper)null);
            pageTypeValidator.PropertiesValidator.Stub(validator => validator.ValidatePageTypeProperties(pageTypeDefinition));
            pageTypeValidator.PropertiesValidator.Replay();

            pageTypeValidator.ValidatePageTypeDefinition(pageTypeDefinition, null);

            pageTypeValidator.PropertiesValidator.AssertWasCalled(validator => validator.ValidatePageTypeProperties(pageTypeDefinition));
        }

        [Fact]
        public void GivenTypesThatInheritFromTypedPageData_ValidatePageTypeDefinitions_DoesNotThrowException()
        {
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition validTypeDefinition = new PageTypeDefinition
            {
                Type = typeof(TypedPageData),
                Attribute = new PageTypeAttribute()
            };
            definitions.Add(validTypeDefinition);
            PageTypeDefinitionValidator definitionValidator = new PageTypeDefinitionValidator(null);

            definitionValidator.ValidatePageTypeDefinitions(definitions);
        }

        [Fact]
        public void GivenTwoPageTypesWithSameNameAndNoGuid_ValidatePageTypeDefinitions_ThrowsException()
        {
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition validTypeDefinition = new PageTypeDefinition
            {
                Type = typeof(TypedPageData),
                Attribute = new PageTypeAttribute()
            };
            definitions.Add(validTypeDefinition);
            definitions.Add(validTypeDefinition);
            PageTypeDefinitionValidator definitionValidator = new PageTypeDefinitionValidator(null);

            Exception exception = Record.Exception(() => definitionValidator.ValidatePageTypeDefinitions(definitions));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenPageTypeDefinitionWithDuplicatesInAllowedPageTypes_ValidatePageTypeDefinition_ThrowsException()
        {
            PageTypeDefinition definition = new PageTypeDefinition
            {
                Type = typeof(TypedPageData),
                Attribute = new PageTypeAttribute
                                {
                                    AvailablePageTypes = new [] { typeof(TestPageType), typeof(TestPageType) }
                                }
            };
            PageTypeDefinitionValidator validator = new PageTypeDefinitionValidator(null);

            Exception exception = Record.Exception(() => validator.ValidatePageTypeDefinition(definition, null));

            Assert.NotNull(exception);
        }

        [Fact]
        public void GivenPageTypeDefinitionWithUndefinedTypeInAllowedPageTypes_ValidatePageTypeDefinition_ThrowsException()
        {
            PageTypeDefinition definition = new PageTypeDefinition
            {
                Type = typeof(TypedPageData),
                Attribute = new PageTypeAttribute
                {
                    AvailablePageTypes = new[] { typeof(TestPageType) }
                }
            };
            List<PageTypeDefinition> allPageTypes = new List<PageTypeDefinition>();
            PageTypeDefinitionValidator validator = new PageTypeDefinitionValidator(null);

            Exception exception = Record.Exception(() => validator.ValidatePageTypeDefinition(definition, allPageTypes));

            Assert.NotNull(exception);
        }
    }
}
