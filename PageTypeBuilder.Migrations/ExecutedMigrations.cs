using System;

namespace PageTypeBuilder.Migrations
{
    internal class ExecutedMigration
    {
        public ExecutedMigration(IMigration migration)
        {
            Number = migration.Number();
            Date = DateTime.Now;
        }

        public int Number { get; private set; }

        public DateTime Date { get; private set; }
    }
}
