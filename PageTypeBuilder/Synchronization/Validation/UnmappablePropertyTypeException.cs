using System;

namespace PageTypeBuilder.Synchronization.Validation
{
    public class UnmappablePropertyTypeException : PageTypeBuilderException
    {
        public UnmappablePropertyTypeException()
        {
        }

        public UnmappablePropertyTypeException(string message) : base(message)
        {
        }

        public UnmappablePropertyTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
