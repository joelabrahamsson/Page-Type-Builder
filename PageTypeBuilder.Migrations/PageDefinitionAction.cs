using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using log4net;
using PageTypeBuilder.Helpers;

namespace PageTypeBuilder.Migrations
{
    public class PageDefinitionAction
    {
        PageDefinition pageDefinition;
        IMigrationContext context;
        private static readonly ILog log = LogManager.GetLogger(typeof(PageDefinitionAction));

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

            log.DebugFormat("Deleting page definition named {0} from page type with id {1}", 
                pageDefinition.Name,
                pageDefinition.PageTypeID);
            context.PageDefinitionRepository.Delete(pageDefinition);
        }

        public void Rename(string newName)
        {
            if(pageDefinition.IsNull())
            {
                return;
            }

            log.DebugFormat(
                "Renaming page definition associated with page type with id {0}"
                + " from {1} to {2}.",
                pageDefinition.PageTypeID,
                pageDefinition.Name,
                newName);
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
                log.WarnFormat(
                    "Tried to change type of page definition named {0} associated with page type with id "
                    + "{1} but was unable to find a page definition type for the new type ({2}).",
                    pageDefinition.Name,
                    pageDefinition.PageTypeID,
                    typeof(T).Name);
                return;
            }

            log.DebugFormat(
                "Changing type of page definition named {0} associated with page type with id {1}"
                + " from type {2} to type {3}.",
                pageDefinition.Name,
                pageDefinition.PageTypeID,
                pageDefinition.Type.Name,
                newType.Name);
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
