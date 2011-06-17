using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;
using PageTypeBuilder.Helpers;

namespace PageTypeBuilder.Migrations
{
    public class PageDefinitionAction
    {
        PageDefinition pageDefinition;
        IMigrationContext context;

        public PageDefinitionAction(
            PageDefinition pageDefinition,
            IMigrationContext context)
        {
            this.pageDefinition = pageDefinition;
            this.context = context;
        }

        public void Delete()
        {
            if(pageDefinition.IsNull())
            {
                return;
            }

            context.PageDefinitionRepository.Delete(pageDefinition);
        }

        public void Rename(string newName)
        {
            if(pageDefinition.IsNull())
            {
                return;
            }

            pageDefinition.Name = newName;
            context.PageDefinitionRepository.Save(pageDefinition);
        }

        public void ChangeTypeTo<T>() where T : PropertyData
        {
            if(pageDefinition.IsNull())
            {
                return;
            }

            PageDefinitionType newType = GetPageDefinitionType<T>();

            if(newType.IsNull())
            {
                return;
            }

            pageDefinition.Type = newType;
            context.PageDefinitionRepository.Save(pageDefinition);
        }

        PageDefinitionType GetPageDefinitionType<T>()
        {
            var type = typeof (T);
            if(context.NativePageDefinitionsMap.TypeIsNativePropertyType(type))
            {
                var typeId = context.NativePageDefinitionsMap.GetNativeTypeID(type);
                return context.PageDefinitionTypeRepository.GetPageDefinitionType(typeId);
            }

            string typeName = type.FullName;
            string assemblyName = type.Assembly.GetName().Name;
            return context.PageDefinitionTypeRepository.GetPageDefinitionType(typeName, assemblyName);
        }
    }
}
