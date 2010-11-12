using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Tests.Synchronization.TabDefinitionUpdaterTests
{
    public class TabDefinitionUpdaterTestsUtility
    {
        public static TabDefinition CreateTabDefinition(Tab tab)
        {
            TabDefinition tabDefinition = new TabDefinition();
            tabDefinition.Name = tab.Name;
            tabDefinition.RequiredAccess = tab.RequiredAccess;
            tabDefinition.SortIndex = tab.SortIndex;
            return tabDefinition;
        }
    }
}
