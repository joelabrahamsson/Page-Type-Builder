using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Abstractions
{
    public class FrameFacade : IFrameFacade
    {
        public Frame Load(int id)
        {
            return Frame.Load(id);
        }

        public void Save(Frame frameToSave)
        {
            frameToSave.Save();
        }
    }
}
