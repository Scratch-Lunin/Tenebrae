using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace Tenebrae.Common.AdditiveDrawing
{
    public class AdditiveDrawSystem : ModSystem
    {
        private static readonly Dictionary<bool, List<AdditiveDrawData>> DataCache = new();

        // ...

        public override void Load()
        {
            DataCache.Add(false, new List<AdditiveDrawData>());
            DataCache.Add(true, new List<AdditiveDrawData>());
        }

        public override void Unload()
        {
            ClearDataCache();
            DataCache.Clear();
        }

        public override void OnWorldUnload()
        {
            ClearDataCache();
        }

        // ...

        public static bool Any() => Any(false) || Any(true);
        public static bool Any(bool behindEntities) => DataCache[behindEntities].Any();

        public static void AddToDataCache(AdditiveDrawData data)
        {
            DataCache[data.DrawBehindEntities].Add(data);
        }

        public static void ClearDataCache()
        {
            ClearDataCache(false);
            ClearDataCache(true);
        }

        public static void ClearDataCache(bool behindEntities)
        {
            DataCache[behindEntities].Clear();
        }

        public static void Draw(bool behindEntities)
        {
            var spriteBatch = Main.spriteBatch;

            foreach (var data in DataCache[behindEntities])
            {
                data.Draw(spriteBatch);
            }

            ClearDataCache(behindEntities);
        }
    }
}
