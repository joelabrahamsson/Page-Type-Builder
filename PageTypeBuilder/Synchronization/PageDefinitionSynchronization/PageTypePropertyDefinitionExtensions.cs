using PageTypeBuilder.Discovery;

namespace PageTypeBuilder.Synchronization.PageDefinitionSynchronization
{
    public static class PageTypePropertyDefinitionExtensions
    {
        public static string GetEditCaptionOrName(this PageTypePropertyDefinition pageTypePropertyDefinition, bool propertyGroupOverride)
        {
            string editCaption = pageTypePropertyDefinition.PageTypePropertyAttribute.EditCaption;

            if (propertyGroupOverride && pageTypePropertyDefinition.PageTypePropertyGroupPropertyOverrideAttribute.EditCaptionSet)
                editCaption = pageTypePropertyDefinition.PageTypePropertyGroupPropertyOverrideAttribute.EditCaption;

            if (string.IsNullOrEmpty(editCaption))
                editCaption = pageTypePropertyDefinition.Name;

            return editCaption;
        }
    }
}
