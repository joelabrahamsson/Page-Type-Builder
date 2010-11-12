using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization
{
    public static class PageTypePropertyDefinitionExtensions
    {
        public static string GetEditCaptionOrName(this PageTypePropertyDefinition pageTypePropertyDefinition)
        {
            string editCaption = pageTypePropertyDefinition.PageTypePropertyAttribute.EditCaption;
            
            if(string.IsNullOrEmpty(editCaption))
                editCaption = pageTypePropertyDefinition.Name;

            return editCaption;
        }
    }
}
