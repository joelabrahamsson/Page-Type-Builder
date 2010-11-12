using System;
using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Configuration;
using Rhino.Mocks;
using Xunit;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class AddPageTypesToResolverTests
    {
        [Fact]
        public void GivenPageTypeDefinition_AddPageTypesToResolver_AddsToResolver()
        {
            PageTypeResolver resolver = new PageTypeResolver();
            PageTypeSynchronizer synchronizer = new PageTypeSynchronizer(new PageTypeDefinitionLocator(), new PageTypeBuilderConfiguration());
            synchronizer.PageTypeResolver = resolver;
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition definition = new PageTypeDefinition
                                                {
                                                    Type = typeof(string),
                                                    Attribute = new PageTypeAttribute()
                                                };
            definitions.Add(definition);
            PageType pageType = new PageType();
            pageType.ID = 1;
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = fakes.Stub<PageTypeUpdater>(new List<PageTypeDefinition>());
            pageTypeUpdater.Stub(updater => updater.GetExistingPageType(definition)).Return(pageType);
            pageTypeUpdater.Replay();
            synchronizer.PageTypeUpdater = pageTypeUpdater;

            synchronizer.AddPageTypesToResolver(definitions);

            Assert.Equal<Type>(definition.Type, resolver.GetPageTypeType(pageType.ID));
        }
    }
}
