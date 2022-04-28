using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace Tenebrae.Common.Particles
{
    public class ParticleSystem : ModSystem
    {
        public static ParticleSystem Instance { get => ModContent.GetInstance<ParticleSystem>(); }

        internal static Dictionary<bool, List<Particle>> particles = new();
        internal static Dictionary<int, Particle> particleInstances = new();

        public override void Load()
        {
            particles.Add(false, new List<Particle>());
            particles.Add(true, new List<Particle>());
        }

        public override void Unload()
        {
            particleInstances.Clear();
            particleInstances = null;

            ClearParticles();
            particles.Clear();
            particles = null;
        }

        public override void PostUpdateEverything()
        {
            foreach (var list in particles.Values)
            {
                foreach (var particle in list.ToArray())
                {
                    particle.Update();
                }
            }
        }

        public override void OnWorldUnload()
        {
            ClearParticles();
        }

        // ...

        public static bool Any() => Any(true) || Any(false);
        public static bool Any(bool additive) => particles[additive].Any();

        public static Particle GetParticleInstance(int type)
        {
            if (!particleInstances.ContainsKey(type)) return null;
            return particleInstances[type];
        }

        public static int ParticleType<T>() where T : Particle
        {
            T t = ModContent.GetInstance<T>();
            if (t == null) return -1;
            return t.Type;
        }

        public static void Draw(bool additive)
        {
            foreach (var particle in particles[additive])
            {
                particle.Draw();
            }
        }

        public static void ClearParticles()
        {
            foreach (var list in particles.Values)
            {
                foreach (var particle in list.ToArray())
                {
                    particle.Kill();
                }

                list.Clear();
            }
        }

        internal static void AddParticle(Particle particle)
        {
            var list = particles[particle.Additive];
            if (!list.Contains(particle))
            {
                list.Add(particle);
            }
        }

        internal static void RemoveParticle(Particle particle)
        {
            particles[particle.Additive].Remove(particle);
        }
    }
}