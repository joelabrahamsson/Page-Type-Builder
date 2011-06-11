using System;
using System.CodeDom;
using EPiServer.Core.PropertySettings;
using EPiServer.Editor.TinyMCE;
using Refraction;

namespace PageTypeBuilder.Specs.Helpers
{
    public static class AssemblyDefinitionExtensions
    {
        public static CodeTypeDeclaration GlobalPropertySettingsClass(
            this AssemblyDefinition assembly,
            string className = "MySettings",
            string displayName = "Settings display name",
            string description = "Description",
            bool? isDefault = null,
            bool overwriteExistingSettings = true,
            string updateSettingsImplementation = "",
            string matchMethodBody = "return DisplayName.Equals(wrapper.DisplayName);")
        {
            var isDefaultString = isDefault.ToString().ToLower();
            if(isDefault == null)
            {
                isDefaultString = "null";
            }

            var type = assembly.Class(className)
                .Implementing<IUpdateGlobalPropertySettings<TinyMCESettings>>()
                .PublicMethod(x =>
                        x.Named("UpdateSettings")
                            .Parameter<TinyMCESettings>("settings")
                            .Body(updateSettingsImplementation))
                .Property<string>(x =>
                                  x.Named("DisplayName")
                                      .Returning(displayName))
                .Property<bool?>(x =>
                                 x.Named("IsDefault")
                                     .GetterBody("return {0};", isDefaultString))
                .Property<string>(x => 
                                  x.Named("Description")
                                      .Returning(description))
                .PublicMethod<bool>(x =>
                              x.Named("Match")
                                  .Parameter<PropertySettingsWrapper>("wrapper")
                                  .Body(matchMethodBody))
                .PublicMethod<int>(x =>
                             x.Named("GetSettingsHashCode")
                                 .Parameter<TinyMCESettings>("settings")
                                 .Body("return settings.Width;"))
                .Property<bool>(x =>
                                x.Named("OverWriteExistingSettings")
                                    .Returning(overwriteExistingSettings));

            return type;
        }

        public static CodeTypeDeclaration TinyMceSettingsAttribute(
            this AssemblyDefinition assembly,
            string updateSettingsImplementation,
            string matchMethodBody,
            string className = "TinyMceSettingsAttribute",
            bool overwriteExistingSettings = true)
        {
            var type = assembly.Class(className)
                .Inheriting<Attribute>()
                .Implementing<IUpdatePropertySettings<TinyMCESettings>>()
                .PublicMethod(x =>
                        x.Named("UpdateSettings")
                            .Parameter<TinyMCESettings>("settings")
                            .Body(updateSettingsImplementation))
                .PublicMethod<bool>(x =>
                              x.Named("MatchesUpdatedSettings")
                                  .Parameter<TinyMCESettings>("settings")
                                  .Body(matchMethodBody))
                .PublicMethod<int>(x =>
                             x.Named("GetSettingsHashCode")
                                 .Parameter<TinyMCESettings>("settings")
                                 .Body("return settings.Width;"))
                .Property<bool>(x =>
                                x.Named("OverWriteExistingSettings")
                                    .Returning(overwriteExistingSettings));

            return type;
        }
    }
}