using System.Web.Caching;

using Autofac;
using EPiServer;
using EPiServer.Core.PropertySettings;
using EPiServer.Events;
using EPiServer.Events.Clients;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Reflection;
using PageTypeBuilder.Synchronization;
using PageTypeBuilder.Synchronization.Validation;
using InitializationModule=EPiServer.Web.InitializationModule;

namespace PageTypeBuilder
{
    [ModuleDependency(typeof(InitializationModule))]
    public class Initializer : IInitializableModule
    {
        private IContainer container;

        public void Initialize(InitializationEngine context)
        {
            var containerBuilder = new ContainerBuilder();
            var defaultBootstrapper = new DefaultBootstrapper();
            defaultBootstrapper.Configure(containerBuilder);
            container = containerBuilder.Build();
            
            PageTypeSynchronizer synchronizer = container.Resolve<PageTypeSynchronizer>();
            synchronizer.SynchronizePageTypes();

            DataFactory.Instance.LoadedPage += DataFactory_LoadedPage;
            DataFactory.Instance.LoadedChildren += DataFactory_LoadedChildren;
            DataFactory.Instance.LoadedDefaultPageData += DataFactory_LoadedPage;
            Event.Get(CacheManager.RemoveFromCacheEventId).Raised += CacheManager_ItemRemoved;
        }

        public void Preload(string[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.LoadedPage -= DataFactory_LoadedPage;
            DataFactory.Instance.LoadedChildren -= DataFactory_LoadedChildren;
            DataFactory.Instance.LoadedDefaultPageData -= DataFactory_LoadedPage;
            Event.Get(CacheManager.RemoveFromCacheEventId).Raised -= CacheManager_ItemRemoved;
        }

        static void DataFactory_LoadedPage(object sender, PageEventArgs e)
        {
            if(e.Page == null)
                return;

            e.Page = PageTypeResolver.Instance.ConvertToTyped(e.Page);
        }

        static void DataFactory_LoadedChildren(object sender, ChildrenEventArgs e)
        {
            for (int i = 0; i < e.Children.Count; i++)
            {
                e.Children[i] = PageTypeResolver.Instance.ConvertToTyped(e.Children[i]);
            }
        }

        void CacheManager_ItemRemoved(object sender, EventNotificationEventArgs e)
        {
            var key = e.Param as string;
            if (key == "EP:PageType" && e.RaiserId != CacheManager.LocalCacheManagerRaiserId && container != null)
            {
                var synchronizer = container.Resolve<PageTypeSynchronizer>();
                synchronizer.SynchronizePageTypes(true);
            }
        }

        private PageTypeBuilderConfiguration Configuration
        {
            get
            {
                return PageTypeBuilderConfiguration.GetConfiguration();
            }
        }
    }
}