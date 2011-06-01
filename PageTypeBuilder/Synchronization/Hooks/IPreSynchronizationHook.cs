using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PageTypeBuilder.Synchronization.Hooks
{
    public interface IPreSynchronizationHook
    {
        void PreSynchronization();
    }
}
