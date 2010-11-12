using System.Linq;
using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Synchronization
{
    public static class TabDefinitionExtensions
    {
        public static bool IsEmpty(this TabDefinition tabDefinition)
        {
            return tabDefinition.ID == -1;
        }

        public static bool IsFirstTab(this TabDefinition tabDefinition)
        {
            return tabDefinition.Name == TabDefinition.List().First().Name;
        }
    }
}
