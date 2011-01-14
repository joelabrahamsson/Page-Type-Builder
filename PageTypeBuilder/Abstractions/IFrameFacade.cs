using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface IFrameFacade
    {
        Frame Load(int id);
    }
}
