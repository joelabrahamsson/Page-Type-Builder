using System;

namespace PageTypeBuilder.Tests
{
    internal class TestValueUtility
    {
        public static string CreateRandomString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
