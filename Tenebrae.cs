using Terraria.ModLoader;

namespace Tenebrae
{
    public partial class Tenebrae : Mod
    {
        public static Tenebrae Instance { get; private set; }

        // ...

        public Tenebrae()
        {
            Instance = this;
        }

        public override void Load()
        {
            LoadHooks();
        }

        public override void Unload()
        {
            UnloadHooks();

            Instance = null;
        }
    }
}