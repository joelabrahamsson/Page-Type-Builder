using System;
using System.Text.RegularExpressions;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Migrations
{
    public abstract class Migration : IMigration
    {

        public Migration()
            : this(new PageTypeRepository(), new PageDefinitionRepository(), new TabDefinitionRepository())
        {}

        public Migration(
            IPageTypeRepository pageTypeRepository, 
            IPageDefinitionRepository pageDefinitionRepository,
            ITabDefinitionRepository tabDefinitionRepository)
        {
            PageTypeRepository = pageTypeRepository;
            PageDefinitionRepository = pageDefinitionRepository;
            TabDefinitionRepository = tabDefinitionRepository;
        }

        public abstract void Execute();

        protected internal IPageTypeRepository PageTypeRepository { get; private set; }
        protected internal IPageDefinitionRepository PageDefinitionRepository { get; private set; }
        protected internal ITabDefinitionRepository TabDefinitionRepository { get; private set; }

        protected PageTypeAction PageType(string name)
        {
            var pageType = PageTypeRepository.Load(name);
            return new PageTypeAction(pageType, PageTypeRepository);
        }
    }
}
