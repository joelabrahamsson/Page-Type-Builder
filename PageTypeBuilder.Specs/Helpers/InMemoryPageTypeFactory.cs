using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
            Mapper.Configuration.CreateMap<PageType, PageType>()
                .ForMember(x => x.FileName, m => m.Ignore())
                .ForMember(x => x.DefaultFrameID, m => m.Ignore())
                .ForMember(x => x.ACL, m => m.Ignore());
            nextId = 1;
            pageTypes = new List<PageType>();
        }

        public PageType Load(string name)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.Name == name);
            if(pageTypeRecord == default(PageType))
                return pageTypeRecord;

            var pageTypeToReturn = new PageType();
            Mapper.Map(pageTypeRecord, pageTypeToReturn);
            return pageTypeToReturn;
        }

        public PageType Load(Guid guid)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.GUID == guid);
            if (pageTypeRecord == default(PageType))
                return pageTypeRecord;

            var pageTypeToReturn = new PageType();
            Mapper.Map(pageTypeRecord, pageTypeToReturn);
            return pageTypeToReturn;
        }

        public PageType Load(int id)
        {
            var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.ID == id);
            if (pageTypeRecord == default(PageType))
                return pageTypeRecord;

            var pageTypeToReturn = new PageType();
            Mapper.Map(pageTypeRecord, pageTypeToReturn);
            return pageTypeToReturn;
        }

        public void Save(PageType pageTypeToSave)
        {
            if (pageTypeToSave.IsNew)
            {
                pageTypeToSave.ID = nextId;
                nextId++;
                var pageTypeRecord = new PageType();
                
                Mapper.Map(pageTypeToSave, pageTypeRecord);

                pageTypes.Add(pageTypeRecord);
            }
            else
            {
                var pageTypeRecord = pageTypes.FirstOrDefault(pageType => pageType.ID == pageTypeToSave.ID);
                Mapper.Map(pageTypeToSave, pageTypeRecord);
            }
        }

        public IEnumerable<PageType> List()
        {
            return pageTypes;
        }
    }
}
