using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class FrameRepository : IFrameRepository
    {
        public Frame Load(int id)
        {
            return Frame.Load(id);
        }
    }
}
