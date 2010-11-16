using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers
{
    public class InMemoryTabFactory : ITabFactory
    {
        private int nextId = 1;
        private List<TabDefinition> tabs;

        public InMemoryTabFactory()
        {
            Mapper.Configuration.CreateMap<TabDefinition, TabDefinition>();
            tabs = new List<TabDefinition>();
            tabs.Add(new TabDefinition());
        }

        public TabDefinition GetTabDefinition(string name)
        {
            var record = tabs.Where(t => t.Name == name);
            if (record == null)
                return null;

            var exposedTab = new TabDefinition();

            Mapper.Map(record, exposedTab);

            return exposedTab;
        }

        public void SaveTabDefinition(TabDefinition tabDefinition)
        {
            if(tabDefinition.ID <= 0)
            {
                tabDefinition.ID = nextId++;
                var record = new TabDefinition();
                Mapper.Map(tabDefinition, record);
                tabs.Add(record);
            }
        }

        public TabDefinitionCollection List()
        {
            var tabCollection = new TabDefinitionCollection();
            foreach (var tab in MapRecordsToExposedObjects(tabs))
            {
                tabCollection.Add(tab);
            }

            return tabCollection;
        }

        private IEnumerable<TabDefinition> MapRecordsToExposedObjects(IEnumerable<TabDefinition> internalRecords)
        {
            return internalRecords.Select(record =>
            {
                var exposed = new TabDefinition();
                Mapper.Map(record, exposed);
                return exposed;
            });
        }
    }
}
