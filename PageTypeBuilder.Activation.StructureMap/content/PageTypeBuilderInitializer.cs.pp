using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using PageTypeBuilder;
using PageTypeBuilder.Activation.StructureMap;

namespace $rootnamespace$
{
    [InitializableModule]
    [ModuleDependency(typeof(Initializer))]
    public class PageTypeBuilderInitializer : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            PageTypeResolver.Instance.Activator = new StructureMapTypedPageActivator(StructureMap.ObjectFactory);
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
