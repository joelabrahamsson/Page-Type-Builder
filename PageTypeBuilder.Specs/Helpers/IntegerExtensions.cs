using System;
using System.Text;

namespace PageTypeBuilder.Specs.Helpers
{
    public static class IntegerExtensions
    {
        private static string alphanumericChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        public static string CharactersLongAlphanumericString(this int number)
        {
            var random = new Random();
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < number; i++)
                result.Append(alphanumericChars[(int)(random.NextDouble() * alphanumericChars.Length)]);

            return result.ToString();
        }

        public static TimeSpan Minutes(this int number)
        {
            return new TimeSpan(0, 0, number, 0);
        }
    }
}
