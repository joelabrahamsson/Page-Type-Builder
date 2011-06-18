using EPiServer.DataAbstraction;
using log4net;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Helpers;

namespace PageTypeBuilder.Migrations
{
    public class PageTypeAction
    {
        IPageType pageType;
        IMigrationContext context;
        private static readonly ILog log = LogManager.GetLogger(typeof(PageTypeAction));

        public PageTypeAction(
            IPageType pageType,
            IMigrationContext context)
        {
            this.pageType = pageType;
            this.context = context;
        }

        public void Delete()
        {
            if(pageType.IsNull())
            {
                return;
            }

            log.DebugFormat("Deleting page type {0}.", pageType.Name);
            context.PageTypeRepository.Delete(pageType);
        }

        public void Rename(string newName)
        {
            if (pageType.IsNull())
            {
                return;
            }

            log.DebugFormat("Renaming page type {0} to {1}.", pageType.Name, newName);
            pageType.Name = newName;
            context.PageTypeRepository.Save(pageType);
        }

        public PageDefinitionAction PageDefinition(string name)
        {
            PageDefinition pageDefinition = null;
            if(pageType.IsNotNull())
            {
                pageDefinition = pageType.Definitions.Find(d => d.Name == name);    
                if(pageDefinition.IsNull())
                {
                    log.WarnFormat(
                        "Tried to retrieve page definition named {0} from page type {1} but the page type"
                        + " does not have a page definition by that name.",
                        name, 
                        pageDefinition);
                }
            }

            return new PageDefinitionAction(pageDefinition, context);
        }
    }
}
