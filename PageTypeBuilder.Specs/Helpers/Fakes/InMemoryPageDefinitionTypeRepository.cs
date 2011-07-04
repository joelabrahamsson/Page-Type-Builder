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
                                        "Boolean", null, null /*"EPiServer.Core.PropertyBoolean",
                                        "EPiServer"*/),

                new PageDefinitionType(1, PropertyDataType.Number,
                                        "Number", null, null/*"EPiServer.Core.PropertyNumber",
                                        "EPiServer"*/),

                new PageDefinitionType(2, PropertyDataType.FloatNumber,
                                        "FloatNumber", null, null /*"EPiServer.Core.PropertyFloatNumber",
                                        "EPiServer"*/),

                new PageDefinitionType(3, PropertyDataType.PageType,
                                        "PageType", null, null /*"EPiServer.Core.PropertyPageType",
                                        "EPiServer"*/),

                new PageDefinitionType(4, PropertyDataType.PageReference,
                                        "PageReference", null, null/*"EPiServer.Core.PropertyPageReference",
                                        "EPiServer"*/),

                new PageDefinitionType(5, PropertyDataType.Date,
                                        "Date",  null, null/*"EPiServer.Core.PropertyDate",
                                        "EPiServer"*/),

                new PageDefinitionType(6, PropertyDataType.String, "String", "EPiServer.Core.PropertyString", "EPiServer"),

                new PageDefinitionType(7, PropertyDataType.LongString,
                                        "LongString", null, null /*"EPiServer.Core.PropertyLongString",
                                        "EPiServer"*/),

                new PageDefinitionType(8, PropertyDataType.Category,
                                        "Category", null, null /*"EPiServer.Core.PropertyCategory",
                                        "EPiServer"*/),

                new PageDefinitionType(11, PropertyDataType.String, "Url", "EPiServer.SpecializedProperties.PropertyUrl",
                    "EPiServer"),

                new PageDefinitionType(100, PropertyDataType.LongString,
                                        "XhtmlString",
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

        public PageDefinitionType GetPageDefinitionType(Type type)
        {
            string typeName = type.FullName;
            string assemblyName = type.Assembly.GetName().Name;
            return GetPageDefinitionType(typeName, assemblyName);
        }

        public PageDefinitionType GetPageDefinitionType<T>()
        {
            return GetPageDefinitionType(typeof(T));
        }
    }
}
