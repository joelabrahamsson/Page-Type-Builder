using System;
using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Synchronization.Validation;
using PageTypeBuilder.Tests.Helpers;
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
            
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition>();
            PageTypeDefinition definition = new PageTypeDefinition
                                                {
                                                    Type = typeof(string),
                                                    Attribute = new PageTypeAttribute()
                                                };
            definitions.Add(definition);
            IPageType pageType = new NativePageType();
            pageType.ID = 1;
            MockRepository fakes = new MockRepository();
            IPageTypeLocator pageTypeLocator = fakes.Stub<IPageTypeLocator>();
            pageTypeLocator.Stub(locator => locator.GetExistingPageType(definition)).Return(pageType);
            pageTypeLocator.Replay();
            PageTypeResolver resolver = new PageTypeResolver();
            PageTypeSynchronizer synchronizer = PageTypeSynchronizerFactory.Create(pageTypeLocator);
            synchronizer.PageTypeResolver = resolver;

            synchronizer.AddPageTypesToResolver(definitions);

            Assert.Equal<Type>(definition.Type, resolver.GetPageTypeType(pageType.ID));
        }
    }
}
