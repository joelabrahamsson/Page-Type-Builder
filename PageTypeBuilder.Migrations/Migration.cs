namespace PageTypeBuilder.Migrations
{
    public abstract class Migration : IMigration
    {

        public Migration()
            : this(new MigrationContext())
        {}

        public Migration(IMigrationContext context)
        {
            Context = context;
        }

        public abstract void Execute();

        protected IMigrationContext Context { get; private set; }

        protected PageTypeAction PageType(string name)
        {
            var pageType = Context.PageTypeRepository.Load(name);
            return new PageTypeAction(pageType, Context);
        }
    }
}
