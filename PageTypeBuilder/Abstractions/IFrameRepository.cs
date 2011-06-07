using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public interface IFrameRepository
    {
        Frame Load(int id);
    }
}
