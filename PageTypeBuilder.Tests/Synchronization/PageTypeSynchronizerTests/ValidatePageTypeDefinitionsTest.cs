using System.Collections.Generic;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;
using PageTypeBuilder.Tests.Helpers;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class ValidatePageTypeDefinitionsTest
    {
        [Fact]
        public void GivenDefinition_ValidatePageTypeDefinitions_CallsValidatorsValidatePageTypeDefinitionsMethod()
        {
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeSynchronizer synchronizer = CreateSynchronizer();
            MockRepository fakes = new MockRepository();
            PageTypeDefinitionValidator definitionValidator = fakes.Stub<PageTypeDefinitionValidator>((PageDefinitionTypeMapper)null);
            definitionValidator.Stub(validator => validator.ValidatePageTypeDefinitions(definitions));
            definitionValidator.Replay();
            synchronizer.PageTypeDefinitionValidator = definitionValidator;

            synchronizer.ValidatePageTypeDefinitions(definitions);

            definitionValidator.AssertWasCalled(validator => validator.ValidatePageTypeDefinitions(definitions));
        }

        private PageTypeSynchronizer CreateSynchronizer()
        {
            return PageTypeSynchronizerFactory.Create();
        }
    }
}