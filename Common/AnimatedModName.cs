using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour.HookGen;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace Tenebrae.Common
{
    // By: S-Pladison
    // Code is quite simple, but there are a lot of reflections, which is not very good
    public sealed class AnimatedModName : ILoadable
    {
        // Let's render texture that will contain our text (name of mod)
        private RenderTarget2D target;
        private UIText uiText;
        private Asset<Effect> effect;
        private Mod mod;

        private const float SPEED = 0.25f;
        private const float REPETITION_RATE = 10f;

        private static readonly Type UIModItemType = typeof(ModLoader).Assembly.GetType("Terraria.ModLoader.UI.UIModItem");
        private static readonly MethodInfo UIModItemOnInitializeMethod = UIModItemType?.GetMethod("OnInitialize", BindingFlags.Public | BindingFlags.Instance);
        private static readonly MethodInfo UIModItemDrawMethod = UIModItemType?.GetMethod("Draw", BindingFlags.Public | BindingFlags.Instance);

        private delegate void orig_OnInitialize(object self);
        private delegate void hook_OnInitialize(orig_OnInitialize orig, object self);
        private delegate void orig_Draw(object self, SpriteBatch sb);
        private delegate void hook_Draw(orig_Draw orig, object self, SpriteBatch sb);

        // ...

        void ILoadable.Load(Mod mod)
        {
            this.mod = mod;

            if (!Main.dedServ)
            {
                effect = ModContent.Request<Effect>(nameof(Tenebrae) + "/Assets/Effects/AnimatedModName", AssetRequestMode.ImmediateLoad);

                var backgroundTexture = ModContent.Request<Texture2D>(nameof(Tenebrae) + "/Assets/Textures/Misc/AnimatedModName", AssetRequestMode.ImmediateLoad).Value;
                effect.Value.Parameters["texture1"].SetValue(backgroundTexture);
            }

            // Stop if we have not found methods
            if (UIModItemOnInitializeMethod == null || UIModItemDrawMethod == null) return;

            HookEndpointManager.Add<hook_OnInitialize>(UIModItemOnInitializeMethod, ModifyOnInitialize);
            HookEndpointManager.Add<hook_Draw>(UIModItemDrawMethod, ModifyDrawMethod);
        }

        void ILoadable.Unload()
        {
            // Stop if we have not found methods
            if (UIModItemOnInitializeMethod == null || UIModItemDrawMethod == null) return;

            HookEndpointManager.Remove<hook_OnInitialize>(UIModItemOnInitializeMethod, ModifyOnInitialize);
            HookEndpointManager.Remove<hook_Draw>(UIModItemDrawMethod, ModifyDrawMethod);
        }

        // ...

        private static void ModifyOnInitialize(orig_OnInitialize orig, object self)
        {
            orig(self);

            // Trying to find UIText

            var type = UIModItemType;
            if (type == null) return;

            var modNameInfo = type.GetField("_modName", BindingFlags.NonPublic | BindingFlags.Instance);
            if (modNameInfo == null) return;

            var modName = ModContent.GetInstance<AnimatedModName>().mod.DisplayName;
            if (modNameInfo.GetValue(self) is UIText uiText && uiText.Text.Contains(modName))
            {
                ModContent.GetInstance<AnimatedModName>().uiText = uiText;
            }
        }

        private static void ModifyDrawMethod(orig_Draw orig, object self, SpriteBatch sB)
        {
            orig(self, sB);

            var instance = ModContent.GetInstance<AnimatedModName>();
            var uiText = instance.uiText;
            if (uiText == null) return;

            var modName = instance.mod.DisplayName;
            var position = uiText.GetDimensions().Position() + new Vector2(0, -2);
            var rasterizerState = sB.GraphicsDevice.RasterizerState;
            var scissorRectangle = sB.GraphicsDevice.ScissorRectangle;
            var effect = instance.effect.Value;

            sB.End();
            sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
            sB.GraphicsDevice.RasterizerState = rasterizerState;
            {
                instance.target ??= Render(modName);
            }
            sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
            {
                ChatManager.DrawColorCodedStringShadow(sB, FontAssets.MouseText.Value, TextToSnippets(modName), position, Color.Black, 0f, Vector2.Zero, Vector2.One, -1f, 1f);
            }
            sB.End();
            sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
            sB.GraphicsDevice.RasterizerState = rasterizerState;
            sB.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, effect, Main.UIScaleMatrix);
            {
                effect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * SPEED);
                effect.Parameters["scale"].SetValue(Main.screenWidth / 1920f * REPETITION_RATE);

                sB.Draw(instance.target, position, null, Color.White);
            }
            sB.End();
            sB.GraphicsDevice.ScissorRectangle = scissorRectangle;
            sB.GraphicsDevice.RasterizerState = rasterizerState;
            sB.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, rasterizerState, null, Main.UIScaleMatrix);
        }

        private static RenderTarget2D Render(string text)
        {
            var device = Main.graphics.GraphicsDevice;
            var sb = Main.spriteBatch;
            var renderTargetUsage = device.PresentationParameters.RenderTargetUsage;
            var targetSize = new Point(Math.Max(Main.screenWidth, 200), Math.Max(Main.screenHeight, 50));
            var target = new RenderTarget2D(device, targetSize.X, targetSize.Y);

            device.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            device.SetRenderTarget(target);
            device.Clear(Color.Transparent);

            sb.Begin();
            ChatManager.DrawColorCodedString(sb, FontAssets.MouseText.Value, TextToSnippets(text), Vector2.Zero, Color.White, 0f, Vector2.Zero, Vector2.One, out _, -1f, false);
            sb.End();

            device.SetRenderTargets(null);
            device.PresentationParameters.RenderTargetUsage = renderTargetUsage;

            return target;
        }

        private static TextSnippet[] TextToSnippets(string text)
        {
            TextSnippet[] snippets = ChatManager.ParseMessage(text ?? " ", Color.White).ToArray();
            ChatManager.ConvertNormalSnippets(snippets);
            return snippets;
        }
    }
}
