namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization
{
    using System.Linq;
    using Helpers.TypeBuildingDsl;
    using Machine.Specifications;

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotted_with_property_group : SynchronizationSpecs
    {

        static string propertyName = "PropertyOne";
        static string pageTypeName = "PageTypeName";

        Establish context = () =>
        {
            var propertyGroupAttribute = new PageTypePropertyGroupAttribute
                                             {
                                                 EditCaptionPrefix = "Property One -",
                                                 StartSortOrderFrom = 100
                                             };

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(Image);
                    prop.AddAttributeTemplate(propertyGroupAttribute);
                });
            });
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_have_a_property_one_image_url_page_type_property =
            () =>
                SyncContext.PageDefinitionFactory.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).ShouldNotBeNull();

        It should_have_a_property_one_image_url_page_type_property_with_the_correct_edit_caption =
            () =>
                SyncContext.PageDefinitionFactory.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).EditCaption.ShouldEqual("Property One - Image Url");

        It should_have_a_property_one_image_url_page_type_property_with_the_correct_sort_order =
            () =>
                SyncContext.PageDefinitionFactory.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).FieldOrder.ShouldEqual(100);

        It should_have_a_property_one_image_alt_page_type_property =
            () =>
                SyncContext.PageDefinitionFactory.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageAlt")).ShouldNotBeNull();

        It should_have_a_property_one_image_alt_page_type_property_with_the_correct_edit_captio =
            () =>
                SyncContext.PageDefinitionFactory.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageAlt")).EditCaption.ShouldEqual("Property One - Image Alt");

        It should_have_a_property_one_image_alt_page_type_property_with_the_correct_sort_order =
            () =>
                SyncContext.PageDefinitionFactory.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageAlt")).FieldOrder.ShouldEqual(110);
    }

    public class Image : PageTypePropertyGroup
    {
        [PageTypeProperty(EditCaption = "Image Url", SortOrder = 0)]
        public virtual string ImageUrl { get; set; }

        [PageTypeProperty(EditCaption = "Alt text", SortOrder = 10)]
        public virtual string AltText { get; set; }
    }
}
