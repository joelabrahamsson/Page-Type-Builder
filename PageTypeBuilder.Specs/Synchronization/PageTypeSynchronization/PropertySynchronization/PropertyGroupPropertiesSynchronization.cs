using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using EPiServer.Security;
using PageTypeBuilder;
using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;
using Machine.Specifications;

namespace PageTypeBuilder.Specs.Synchronization.PageTypeSynchronization.PropertySynchronization
{
    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_property_group : SynchronizationSpecs
    {

        static string propertyName = "PropertyOne";
        static string pageTypeName = "PageTypeName";

        Establish context = () =>
        {
            var propertyGroupAttribute = new PageTypePropertyGroupAttribute
            {
                EditCaptionPrefix = "Property One - ",
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

            SyncContext.AssemblyLocator.Add(typeof(when_a_page_type_property_is_annotated_with_property_group).Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_have_a_property_one_image_url_page_type_property =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).ShouldNotBeNull();

        It should_have_a_property_one_image_url_page_type_property_with_the_correct_edit_caption =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).EditCaption.ShouldEqual("Property One - Image Url");

        It should_have_a_property_one_image_url_page_type_property_with_the_correct_sort_order =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).FieldOrder.ShouldEqual(100);

        It should_have_a_property_one_image_url_page_type_property_with_the_correct_tab =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).Tab.ID.ShouldEqual(-1);

        It should_have_a_property_one_image_alt_page_type_property =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-AltText")).ShouldNotBeNull();

        It should_have_a_property_one_image_alt_page_type_property_with_the_correct_edit_caption =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-AltText")).EditCaption.ShouldEqual("Property One - Alt text");

        It should_have_a_property_one_image_alt_page_type_property_with_the_correct_sort_order =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-AltText")).FieldOrder.ShouldEqual(110);

        It should_have_a_property_one_image_alt_page_type_property_with_the_correct_tab =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-AltText")).Tab.ID.ShouldEqual(-1);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_property_group_with_tab_defined : SynchronizationSpecs
    {
        static string propertyName = "PropertyOne";
        static string pageTypeName = "PageTypeName";

        Establish context = () =>
        {
            var propertyGroupAttribute = new PageTypePropertyGroupAttribute
            {
                EditCaptionPrefix = "Property One - ",
                StartSortOrderFrom = 100,
                Tab = typeof(FooterTab)
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

            SyncContext.AssemblyLocator.Add(typeof(when_a_page_type_property_is_annotated_with_property_group).Assembly);
        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_have_a_property_one_image_url_page_type_property_with_the_correct_tab =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).Tab.ID.ShouldEqual(SyncContext.TabDefinitionRepository.List().Where(current => !string.IsNullOrEmpty(current.Name) && current.Name.Equals("Footer")).First().ID);

        It should_have_a_property_one_image_alt_page_type_property_with_the_correct_tab =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-AltText")).Tab.ID.ShouldEqual(SyncContext.TabDefinitionRepository.List().Where(current => !string.IsNullOrEmpty(current.Name) && current.Name.Equals("Footer")).First().ID);
    }

    [Subject("Synchronization")]
    public class when_a_page_type_property_is_annotated_with_property_group_with_property_attribute_values_overridden : SynchronizationSpecs
    {
        static string propertyName = "PropertyOne";
        static string pageTypeName = "PageTypeName";

        Establish context = () =>
        {
            var propertyGroupAttribute = new PageTypePropertyGroupAttribute
            {
                EditCaptionPrefix = "Property One - ",
                StartSortOrderFrom = 100,
                Tab = typeof(FooterTab)
            };

            var pageTypePropertyOverrideAttribute = new PageTypePropertyGroupPropertyOverrideAttribute
            {
                PropertyName = "ImageUrl",
                EditCaption = "New Caption",
                DefaultValue = "NewDefaultValue",
                DefaultValueType = DefaultValueType.Value,
                DisplayInEditMode = false,
                HelpText = "NewHelpText",
                Required = true,
                Searchable = true,
                UniqueValuePerLanguage = true
            };

            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
            {
                type.Name = pageTypeName;
                type.AddProperty(prop =>
                {
                    prop.Name = propertyName;
                    prop.Type = typeof(Image);
                    prop.AddAttributeTemplate(propertyGroupAttribute);
                    prop.AddAttributeTemplate(pageTypePropertyOverrideAttribute);
                });
            });

            SyncContext.AssemblyLocator.Add(typeof(when_a_page_type_property_is_annotated_with_property_group_with_property_attribute_values_overridden).Assembly);

        };

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_have_a_property_one_image_url_page_type_property_with_an_updated_edit_caption =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).EditCaption.ShouldEqual("Property One - New Caption");

        It should_have_a_property_one_image_url_page_type_property_with_an_updated_default_value =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).DefaultValue.ShouldEqual("NewDefaultValue");

        It should_have_a_property_one_image_url_page_type_property_with_an_updated_default_value_type =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).DefaultValueType.ShouldEqual(DefaultValueType.Value);

        It should_have_a_property_one_image_url_page_type_property_with_an_updated_display_edit_ui =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).DisplayEditUI.ShouldEqual(false);

        It should_have_a_property_one_image_url_page_type_property_with_an_updated_help_text =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).HelpText.ShouldEqual("NewHelpText");

        It should_have_a_property_one_image_url_page_type_property_with_an_updated_required =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).Required.ShouldEqual(true);

        It should_have_a_property_one_image_url_page_type_property_with_an_updated_searchable =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).Searchable.ShouldEqual(true);

        It should_have_a_property_one_image_url_page_type_property_with_an_updated_unique_value_per_language =
            () =>
                SyncContext.PageDefinitionRepository.List().FirstOrDefault(p => string.Equals(p.Name, "PropertyOne-ImageUrl")).LanguageSpecific.ShouldEqual(true);
    }

    public class Image : PageTypePropertyGroup
    {
        [PageTypeProperty(EditCaption = "Image Url", SortOrder = 0)]
        public virtual string ImageUrl { get; set; }

        [PageTypeProperty(EditCaption = "Alt text", SortOrder = 10, Tab = typeof(FooterTab))]
        public virtual string AltText { get; set; }
    }

    public class FooterTab : Tab
    {
        public override string Name
        {
            get { return "Footer"; }
        }

        public override AccessLevel RequiredAccess
        {
            get { return AccessLevel.Edit; }
        }

        public override int SortIndex
        {
            get { return 1000; }
        }
    }

}
