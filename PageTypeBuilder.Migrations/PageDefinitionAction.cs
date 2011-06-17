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
        IPageDefinitionRepository pageDefinitionRepository;
        IPageDefinitionTypeRepository pageDefinitionTypeRepository;

        public PageDefinitionAction(
            PageDefinition pageDefinition, 
            IPageDefinitionRepository pageDefinitionRepository,
            IPageDefinitionTypeRepository pageDefinitionTypeRepository)
        {
            this.pageDefinition = pageDefinition;
            this.pageDefinitionRepository = pageDefinitionRepository;
            this.pageDefinitionTypeRepository = pageDefinitionTypeRepository;
        }

        public void Delete()
        {
            if(pageDefinition.IsNull())
            {
                return;
            }

            pageDefinitionRepository.Delete(pageDefinition);
        }

        public void Rename(string newName)
        {
            if(pageDefinition.IsNull())
            {
                return;
            }

            pageDefinition.Name = newName;
            pageDefinitionRepository.Save(pageDefinition);
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
            pageDefinitionRepository.Save(pageDefinition);
        }

        PageDefinitionType GetPageDefinitionType<T>()
        {
            var type = typeof (T);
            string typeName = type.FullName;
            string assemblyName = type.Assembly.GetName().Name;
            return pageDefinitionTypeRepository.GetPageDefinitionType(typeName, assemblyName);
        }
    }
}
