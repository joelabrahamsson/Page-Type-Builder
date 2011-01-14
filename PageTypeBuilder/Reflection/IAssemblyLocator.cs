using System.Collections.Generic;
using System.Reflection;

namespace PageTypeBuilder.Reflection
{
    public interface IAssemblyLocator
    {
        IEnumerable<Assembly> GetAssemblies();
    }
}