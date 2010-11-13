using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers
{
    public class InMemoryPageTypeFactory : IPageTypeFactory
    {
        private int nextId;
        private List<PageType> pageTypes;

        public InMemoryPageTypeFactory()
        {
            nextId = 1;
            pageTypes = new List<PageType>();
        }

        public PageType Load(string name)
        {
            return pageTypes.FirstOrDefault(pageType => pageType.Name == name);
        }

        public PageType Load(Guid guid)
        {
            return pageTypes.FirstOrDefault(pageType => pageType.GUID == guid);
        }

        public PageType Load(int id)
        {
            return pageTypes.FirstOrDefault(pageType => pageType.ID == id);
        }

        public void Save(PageType pageTypeToSave)
        {
            if (pageTypeToSave.IsNew)
            {
                pageTypeToSave.ID = nextId;
                nextId = nextId++;
                pageTypes.Add(pageTypeToSave);
            }
        }
    }
}
