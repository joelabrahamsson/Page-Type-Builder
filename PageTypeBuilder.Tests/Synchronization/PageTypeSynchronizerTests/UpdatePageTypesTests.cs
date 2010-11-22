﻿using System.Collections.Generic;
using Moq;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using Rhino.Mocks;
using Xunit;

namespace PageTypeBuilder.Tests.Synchronization.PageTypeSynchronizerTests
{
    public class UpdatePageTypesTests
    {
        [Fact]
        public void GivenPageType_UpdatePageTypes_CallsPageTypeUpdaterUpdatePageType()
        {
            PageTypeSynchronizer synchronizer = new PageTypeSynchronizer(new PageTypeDefinitionLocator(), new PageTypeBuilderConfiguration(), new PageTypeFactory());
            MockRepository fakes = new MockRepository();
            PageTypeUpdater pageTypeUpdater = fakes.Stub<PageTypeUpdater>(new Mock<IPageTypeDefinitionLocator>().Object, new PageTypeFactory());
            PageTypeDefinition definition = new PageTypeDefinition();   
            pageTypeUpdater.Stub(updater => updater.UpdatePageType(definition));
            pageTypeUpdater.Replay();
            synchronizer.PageTypeUpdater = pageTypeUpdater;
            List<PageTypeDefinition> definitions = new List<PageTypeDefinition> { definition };

            synchronizer.UpdatePageTypes(definitions);

            pageTypeUpdater.AssertWasCalled(updater => updater.UpdatePageType(definition));
        }
    }
}
