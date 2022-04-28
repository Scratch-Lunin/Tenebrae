using System.Collections.Generic;
using System.Reflection;
using Terraria.ModLoader;

namespace Tenebrae.Utilities
{
    public static class ModUtils
    {
        public static List<ModSystem> GetModSystems(this Mod mod) => ((Dictionary<Mod, List<ModSystem>>)SystemsByModInfo?.GetValue(null))[mod] ?? new();

        // ...

        private static readonly FieldInfo SystemsByModInfo = typeof(SystemLoader).GetField("SystemsByMod", BindingFlags.NonPublic | BindingFlags.Static);
    }
}