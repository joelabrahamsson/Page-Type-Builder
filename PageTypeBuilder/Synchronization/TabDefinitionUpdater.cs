using System.Collections.Generic;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Synchronization
{
    public class TabDefinitionUpdater
    {
        public TabDefinitionUpdater()
        {
            TabFactory = new TabFactory();
        }

        public virtual void UpdateTabDefinitions(List<Tab> tabs)
        {
            foreach (Tab tab in tabs)
            {
                TabDefinition tabDefinition = GetTabDefinition(tab);

                if(TabDefinitionShouldBeUpdated(tabDefinition, tab))
                {
                    UpdateTabDefinition(tabDefinition, tab);
                    TabFactory.SaveTabDefinition(tabDefinition);
                }
            }
        }

        private TabDefinition GetTabDefinition(Tab tab)
        {
            TabDefinition tabDefinition = TabFactory.GetTabDefinition(tab.Name);
            if(tabDefinition == null)
                tabDefinition = new TabDefinition();
            return tabDefinition;
        }

        protected internal virtual void UpdateTabDefinition(TabDefinition tabDefinition, Tab tab)
        {
            tabDefinition.Name = tab.Name;
            tabDefinition.RequiredAccess = tab.RequiredAccess;
            tabDefinition.SortIndex = tab.SortIndex;
        }

        protected internal virtual bool TabDefinitionShouldBeUpdated(TabDefinition tabDefinition, Tab tab)
        {
            if(tabDefinition.Name != tab.Name)
                return true;

            if (tabDefinition.RequiredAccess != tab.RequiredAccess)
                return true;

            if (tabDefinition.SortIndex != tab.SortIndex)
                return true;
            
            return false;
        }

        public TabFactory TabFactory { get; set; }
    }
}
