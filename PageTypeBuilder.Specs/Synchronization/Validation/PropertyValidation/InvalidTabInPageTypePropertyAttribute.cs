//using System.Reflection;
//using EPiServer.Security;
//using Machine.Specifications;
//using PageTypeBuilder.Specs.Helpers;
//using PageTypeBuilder.Specs.Helpers.TypeBuildingDsl;

//namespace PageTypeBuilder.Specs.Synchronization.Validation.PropertyValidation.InvalidTabInPageTypePropertyAttribute
//{
//    [Subject("Synchronization")]
//    public class when_a_page_type_property_has_a_Tab_specified_in_the_attribute_that_does_not_inherit_from_Tab
//        : PropertyValidationSpecs
//    {
//        Establish context = () =>
//        {
//            PageTypePropertyAttribute propertyAttribute;
//            propertyAttribute = new PageTypePropertyAttribute();
//            propertyAttribute.Tab = typeof(string);

//            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
//            {
//                type.Name = pageTypeName;
//                type.AddProperty(prop =>
//                {
//                    prop.Name = propertyName;
//                    prop.Type = typeof(string);
//                    prop.AddAttributeTemplate(propertyAttribute);
//                });
//            });
//        };

//        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
//    }

//    [Subject("Synchronization")]
//    public class when_a_page_type_property_has_a_Tab_specified_in_the_attribute_that_inherits_from_Tab_but_is_abstract
//        : PropertyValidationSpecs
//    {
//        Establish context = () =>
//        {
//            //var tabClass = TabClassFactory.CreateTabClass(
//            //    "NameOfTheAbstractClass",
//            //    "TabName",
//            //    AccessLevel.Edit,
//            //    1,
//            //    TypeAttributes.Public | TypeAttributes.Abstract);
//            var moduleBuilder = ReflectionExtensions.CreateModuleWithReferenceToPageTypeBuilder("AssemblyWithTab");
//            var tabClass = moduleBuilder.CreateClass(type =>
//            {
//                type.Name = "NameOfTheTabClass";
//                type.ParentType = typeof(Tab);
//                type.TypeAttributes = TypeAttributes.Public | TypeAttributes.Abstract;
//            });
//            SyncContext.AssemblyLocator.Add(tabClass.Assembly);

//            PageTypePropertyAttribute propertyAttribute;
//            propertyAttribute = new PageTypePropertyAttribute();
//            propertyAttribute.Tab = tabClass;

//            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
//            {
//                type.Name = pageTypeName;
//                type.AddProperty(prop =>
//                {
//                    prop.Name = propertyName;
//                    prop.Type = typeof(string);
//                    prop.AddAttributeTemplate(propertyAttribute);
//                });
//            });
//        };

//        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
//    }

//    [Subject("Synchronization")]
//    public class when_a_page_type_property_has_a_Tab_specified_in_the_attribute_that_inherits_from_Tab_but_is_not_public
//        : PropertyValidationSpecs
//    {
//        Establish context = () =>
//        {
//            var tabClass = TabClassFactory.CreateTabClass(
//                "NameOfTheAbstractClass",
//                "TabName",
//                AccessLevel.Edit,
//                1,
//                TypeAttributes.NotPublic);
//            SyncContext.AssemblyLocator.Add(tabClass.Assembly);

//            PageTypePropertyAttribute propertyAttribute;
//            propertyAttribute = new PageTypePropertyAttribute();
//            propertyAttribute.Tab = tabClass;

//            SyncContext.CreateAndAddPageTypeClassToAppDomain(type =>
//            {
//                type.Name = pageTypeName;
//                type.AddProperty(prop =>
//                {
//                    prop.Name = propertyName;
//                    prop.Type = typeof(string);
//                    prop.AddAttributeTemplate(propertyAttribute);
//                });
//            });
//        };

//        [Ignore]
//        Behaves_like<InvalidPageTypePropertyBehavior> it_found_an_invalid_property;
//    }
//}
