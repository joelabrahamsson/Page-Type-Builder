using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageTypeBuilder.Migrations
{
    internal class ExecutedMigration
    {
        public ExecutedMigration(Migration migration)
        {
            Number = migration.Number();
            Date = DateTime.Now;
        }

        public int Number { get; private set; }

        public DateTime Date { get; private set; }
    }
}
