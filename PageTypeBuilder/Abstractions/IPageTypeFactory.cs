using System;
using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface IPageTypeFactory
    {
        PageType Load(string name);
        PageType Load(Guid guid);
        PageType Load(int id);
        void Save(PageType pageTypeToSave);
    }
}
