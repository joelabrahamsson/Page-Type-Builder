using System;
using System.Text.RegularExpressions;
using EPiServer.DataAbstraction;

namespace PageTypeBuilder.Migrations
{
    public abstract class Migration
    {
        public abstract void Execute();

        protected PageType GetPageType(string name)
        {
            return PageType.Load(name);
        }

        private static Regex numberPattern = new Regex("[0-9]*$", RegexOptions.Compiled);
        internal int Number()
        {
            var typeName = GetType().Name;
            return int.Parse(numberPattern.Match(typeName).Captures[0].Value);
        }
    }
}
