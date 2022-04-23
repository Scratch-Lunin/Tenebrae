using Terraria.ModLoader;

namespace Tenebrae
{
    public partial class Tenebrae : Mod
    {
        public override void Load()
        {
            LoadHooks();
        }

        public override void Unload()
        {
            UnloadHooks();
        }
    }
}