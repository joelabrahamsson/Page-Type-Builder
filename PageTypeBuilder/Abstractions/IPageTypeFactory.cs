using System;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageTypeFactory
    {
        IPageType Load(string name);
        IPageType Load(Guid guid);
        IPageType Load(int id);
        void Save(IPageType pageTypeToSave);
        IPageType CreateNew();
    }
}
