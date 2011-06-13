using System.Linq;
using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Migrations
{
    public static class PageTypeExtensions
    {
        public static PageDefinition GetPageDefinition(this PageType pageType, string name)
        {
            return pageType.Definitions
                .Where(pageDefinition => pageDefinition.Name.Equals(name))
                .FirstOrDefault();
        }
    }
}
