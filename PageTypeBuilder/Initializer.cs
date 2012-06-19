namespace PageTypeBuilder
{
    using Autofac;
    using EPiServer;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using Configuration;
    using Synchronization;
    using InitializationModule = EPiServer.Web.InitializationModule;

    [ModuleDependency(typeof(InitializationModule))]
    public class Initializer : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            var containerBuilder = new ContainerBuilder();
            var defaultBootstrapper = new DefaultBootstrapper();
            defaultBootstrapper.Configure(containerBuilder);
            var container = containerBuilder.Build();

            PageTypeSynchronizer synchronizer = container.Resolve<PageTypeSynchronizer>();
            TimingsLogger.Clear();

            using (new TimingsLogger("Total time to synchronize"))
            {
                synchronizer.SynchronizePageTypes();
            }

            DataFactory.Instance.LoadedPage += DataFactory_LoadedPage;
            DataFactory.Instance.LoadedChildren += DataFactory_LoadedChildren;
            DataFactory.Instance.LoadedDefaultPageData += DataFactory_LoadedPage;
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
        }

        static void DataFactory_LoadedPage(object sender, PageEventArgs e)
        {
            if (e.Page == null)
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

        private PageTypeBuilderConfiguration Configuration
        {
            get
            {
                return PageTypeBuilderConfiguration.GetConfiguration();
            }
        }
    }
}