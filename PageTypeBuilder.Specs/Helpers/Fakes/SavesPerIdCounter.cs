using System.Collections.Generic;

namespace PageTypeBuilder.Specs.Helpers.Fakes
{
    public class SavesPerIdCounter
    {
        private Dictionary<int, int> savesPerId = new Dictionary<int, int>();

        public int GetNumberOfSaves(int id)
        {
            if (savesPerId.ContainsKey(id))
                return savesPerId[id];

            return 0;
        }

        public void ResetNumberOfSaves()
        {
            savesPerId.Clear();
        }

        public void IncrementNumberOfSaves(int id)
        {
            if (!savesPerId.ContainsKey(id))
                savesPerId[id] = 0;

            savesPerId[id]++;
        }
    }
}
