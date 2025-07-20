using System.Collections.Generic;
using Verse;
using SharedDefs;

namespace ModSettingRegistry
{
    public static class Registry
    {
        private static readonly List<SettingDefinition> singleItems = new List<SettingDefinition>();
        private static readonly List<SettingGroup> groups = new List<SettingGroup>();
        private static readonly List<System.Action> exposers = new List<System.Action>();

        public static void RegisterItem(SettingDefinition def, System.Action exposer)
        {
            if (def != null)
                singleItems.Add(def);
            if (exposer != null)
                exposers.Add(exposer);
        }

        public static void RegisterGroup(SettingGroup group, System.Action exposer)
        {
            if (group != null)
                groups.Add(group);
            if (exposer != null)
                exposers.Add(exposer);
        }

        public static IEnumerable<SettingDefinition> AllItems => singleItems;
        public static IEnumerable<SettingGroup> AllGroups => groups;

        public static void ExposeAll()
        {
            foreach (var exposer in exposers)
                exposer();
        }
    }
}
