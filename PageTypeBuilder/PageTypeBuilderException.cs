using System;

namespace PageTypeBuilder
{
    public class PageTypeBuilderException : ApplicationException
    {
        public PageTypeBuilderException()
        {
        }

        public PageTypeBuilderException(string message)
            : base(message)
        {
        }

        public PageTypeBuilderException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
