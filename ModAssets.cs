using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace Tenebrae
{
    public static class ModAssets
    {
        public const string Path = nameof(Tenebrae) + "/Assets/";

        public const string ItemsPath = Path + "Textures/Items/";
        public const string BuffsPath = Path + "Textures/Buffs/";
        public const string ProjectilesPath = Path + "Textures/Projectiles/";
        public const string DustsPath = Path + "Textures/Dusts/";
        public const string ParticlesPath = Path + "Textures/Particles/";
        public const string MiscPath = Path + "Textures/Misc/";
        public const string InvisiblePath = Path + "Textures/Misc/Invisible";

        public const string EffectsPath = Path + "Effects/";

        // ...

        public static Asset<Texture2D> GetExtraTexture(int type, AssetRequestMode mode = AssetRequestMode.AsyncLoad) => ModContent.Request<Texture2D>(MiscPath + "Extra_" + type, mode);
        public static Asset<Effect> GetEffect(string name, AssetRequestMode mode = AssetRequestMode.AsyncLoad) => ModContent.Request<Effect>(EffectsPath + name, mode);
    }
}