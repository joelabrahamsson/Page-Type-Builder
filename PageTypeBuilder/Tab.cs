using EPiServer.Security;

namespace PageTypeBuilder
{
    public abstract class Tab
    {
        public abstract string Name { get; }

        public abstract AccessLevel RequiredAccess { get; }

        public abstract int SortIndex { get; }
    }
}
