using System.Collections.Generic;
using System.Linq;
using EPiServer.DataAbstraction;
using PageTypeBuilder.Abstractions;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class InMemoryFrameRepository : IFrameRepository
    {
        private List<Frame> frames;

        public InMemoryFrameRepository()
        {
            frames = new List<Frame>
                {
                    new Frame(1, "target=\"_blank\"", "Open the link in a new window", true),
                    new Frame(2, "target=\"_top\"", "Open the link in the whole window", true)
                };
        }

        public Frame Load(int id)
        {
            return frames.FirstOrDefault(frame => frame.ID == id);
        }
    }
}
