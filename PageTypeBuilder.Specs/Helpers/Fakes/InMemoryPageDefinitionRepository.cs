using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryPageDefinitionRepository : IPageDefinitionRepository
    {
        private int nextId = 1;
        private List<PageDefinition> pageDefinitions;

        public InMemoryPageDefinitionRepository()
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

        public PageDefinitionCollection List(int pageTypeId)
        {
            var result = new PageDefinitionCollection();
            result.AddRange(MapRecordsToExposedObjects(pageDefinitions.Where(pd => pd.PageTypeID == pageTypeId)));
            return result;
        }

        public void Delete(PageDefinition pageDefinition)
        {
            if(pageDefinition == null)
            {
                throw new ArgumentNullException("pageDefinition cannot be null.");
            }

            var record = pageDefinitions.Find(d => d.ID == pageDefinition.ID);
            pageDefinitions.Remove(record);
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


        public PageDefinition Load(int id)
        {
            return List().Where(x => x.ID == id).FirstOrDefault();
        }
    }
}
