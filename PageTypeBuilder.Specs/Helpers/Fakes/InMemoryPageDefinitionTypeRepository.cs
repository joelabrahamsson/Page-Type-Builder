using System;
using System.Collections.Generic;
using System.Linq;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryPageDefinitionTypeRepository : IPageDefinitionTypeRepository
    {
        private List<PageDefinitionType> pageDefinitions;

        public InMemoryPageDefinitionTypeRepository()
        {
            pageDefinitions = new List<PageDefinitionType>
            {
                new PageDefinitionType(0, PropertyDataType.Boolean,
                                        "PropertyBoolean", "EPiServer.Core.PropertyBoolean",
                                        "EPiServer"),

                new PageDefinitionType(1, PropertyDataType.Number,
                                        "PropertyNumber", "EPiServer.Core.PropertyNumber",
                                        "EPiServer"),

                new PageDefinitionType(2, PropertyDataType.FloatNumber,
                                        "PropertyFloatNumber", "EPiServer.Core.PropertyFloatNumber",
                                        "EPiServer"),

                new PageDefinitionType(3, PropertyDataType.PageType,
                                        "PropertyPageType", "EPiServer.Core.PropertyPageType",
                                        "EPiServer"),

                new PageDefinitionType(4, PropertyDataType.PageReference,
                                        "PropertyPageReference", "EPiServer.Core.PropertyPageReference",
                                        "EPiServer"),

                new PageDefinitionType(5, PropertyDataType.Date,
                                        "PropertyDate", "EPiServer.Core.PropertyDate",
                                        "EPiServer"),

                new PageDefinitionType(6, PropertyDataType.String,
                                        "PropertyString", "EPiServer.Core.PropertyString",
                                        "EPiServer"),

                new PageDefinitionType(7, PropertyDataType.LongString,
                                        "PropertyLongString", "EPiServer.Core.PropertyLongString",
                                        "EPiServer"),

                new PageDefinitionType(8, PropertyDataType.Category,
                                        "PropertyCategory", "EPiServer.Core.PropertyCategory",
                                        "EPiServer"),

                new PageDefinitionType(100, PropertyDataType.LongString,
                                        "PropertyXHtmlString",
                                        "EPiServer.SpecializedProperties.PropertyXhtmlString",
                                        "EPiServer")
            };
        }

        public PageDefinitionType GetPageDefinitionType(int id)
        {
            return pageDefinitions.FirstOrDefault(def => def.ID == id);
        }

        public PageDefinitionType GetPageDefinitionType(string typeName, string assemblyName)
        {
            return pageDefinitions.FirstOrDefault(def => def.TypeName == typeName && def.AssemblyName == assemblyName);
        }

        public PageDefinitionType GetPageDefinitionType<T>()
        {
            string typeName = typeof (T).FullName;
            string assemblyName = typeof (T).Assembly.GetName().Name;
            return GetPageDefinitionType(typeName, assemblyName);
        }
    }
}
