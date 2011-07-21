using System;
using System.Collections.Generic;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageTypeRepository
    {
        IPageType Load(string name);
        IPageType Load(Guid guid);
        IPageType Load(int id);
        IEnumerable<IPageType> List();
        void Save(IPageType pageTypeToSave);
        IPageType CreateNew();
        void Delete(IPageType pageType);
    }
}
