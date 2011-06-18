using log4net;
using PageTypeBuilder.Helpers;

namespace PageTypeBuilder.Migrations
{
    public abstract class Migration : IMigration
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Migration));

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

            if(pageType.IsNull())
            {
                log.WarnFormat(
                    "Tried to get page type named {0} for further action but did not find a page type by that name",
                    name);
            }


            return new PageTypeAction(pageType, Context);
        }
    }
}
