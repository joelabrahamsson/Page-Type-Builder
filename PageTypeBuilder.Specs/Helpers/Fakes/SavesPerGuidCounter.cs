using System;
using System.Collections.Generic;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class SavesPerGuidCounter
    {
        private Dictionary<Guid, int> savesPerId = new Dictionary<Guid, int>();

        public int GetNumberOfSaves(Guid id)
        {
            if (savesPerId.ContainsKey(id))
                return savesPerId[id];

            return 0;
        }

        public void ResetNumberOfSaves()
        {
            savesPerId.Clear();
        }

        public void IncrementNumberOfSaves(Guid id)
        {
            if (!savesPerId.ContainsKey(id))
                savesPerId[id] = 0;

            savesPerId[id]++;
        }
    }
}
