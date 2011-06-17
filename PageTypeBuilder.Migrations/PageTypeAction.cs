using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Helpers;

namespace PageTypeBuilder.Migrations
{
    public class PageTypeAction
    {
        IPageType pageType;
        IPageTypeRepository pageTypeRepository;
        IPageDefinitionRepository pageDefinitionRepository;
        IPageDefinitionTypeRepository pageDefinitionTypeRepository;

        public PageTypeAction(
            IPageType pageType, 
            IPageTypeRepository pageTypeRepository, 
            IPageDefinitionRepository pageDefinitionRepository,
            IPageDefinitionTypeRepository pageDefinitionTypeRepository)
        {
            this.pageType = pageType;
            this.pageTypeRepository = pageTypeRepository;
            this.pageDefinitionRepository = pageDefinitionRepository;
            this.pageDefinitionTypeRepository = pageDefinitionTypeRepository;
        }

        public void Delete()
        {
            if(pageType.IsNull())
            {
                return;
            }

            pageTypeRepository.Delete(pageType);
        }

        public void Rename(string newName)
        {
            if (pageType.IsNull())
            {
                return;
            }

            pageType.Name = newName;
            pageTypeRepository.Save(pageType);
        }

        public PageDefinitionAction PageDefinition(string name)
        {
            PageDefinition pageDefinition = null;
            if(pageType.IsNotNull())
            {
                pageDefinition = pageType.Definitions.Find(d => d.Name == name);    
            }

            return new PageDefinitionAction(pageDefinition, pageDefinitionRepository, pageDefinitionTypeRepository);
        }
    }
}
