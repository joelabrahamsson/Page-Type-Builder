using System;

namespace PageTypeBuilder.Specs.Helpers
{
    public static class Random
    {
        public static string String()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
