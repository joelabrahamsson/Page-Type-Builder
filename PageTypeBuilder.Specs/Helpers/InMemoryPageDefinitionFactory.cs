using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers
{
    public class InMemoryPageDefinitionFactory : IPageDefinitionFactory
    {
        private int nextId = 1;
        private List<PageDefinition> pageDefinitions;

        public InMemoryPageDefinitionFactory()
        {
            Mapper.Configuration.CreateMap<PageDefinition, PageDefinition>();
            pageDefinitions = new List<PageDefinition>();
        }

        public void Save(PageDefinition pageDefinition)
        {
            if (string.IsNullOrEmpty(pageDefinition.Name) || string.IsNullOrEmpty(pageDefinition.EditCaption))
            {
                throw new DataAbstractionException("Cannot save a type without name and caption");
            }

            if(pageDefinition.ID <= 0)
            {
                pageDefinition.ID = nextId++;
                var pageDefinitionRecord = new PageDefinition();
                Mapper.Map(pageDefinition, pageDefinitionRecord);
                pageDefinitions.Add(pageDefinitionRecord);
            }
            else
            {
                var pageDefinitionRecord = pageDefinitions.First(p => p.ID == pageDefinition.ID);
                Mapper.Map(pageDefinition, pageDefinitionRecord);
            }
        }

        public IEnumerable<PageDefinition> List(int pageTypeId)
        {
            return MapRecordsToExposedObjects(pageDefinitions.Where(pd => pd.PageTypeID == pageTypeId));
        }

        public IEnumerable<PageDefinition> List()
        {
            return MapRecordsToExposedObjects(pageDefinitions);
        }

        private IEnumerable<PageDefinition> MapRecordsToExposedObjects(IEnumerable<PageDefinition> internalRecords)
        {
            return internalRecords.Select(pd =>
                {
                    var exposedPageDefinition = new PageDefinition();
                    Mapper.Map(pd, exposedPageDefinition);
                    return exposedPageDefinition;
                });
        }
    }
}
