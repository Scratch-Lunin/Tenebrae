using Microsoft.Xna.Framework;
using Terraria;

namespace Tenebrae.Utilities
{
    public static class ProjectileUtils
    {
        public static void MoveTowards(this Projectile projectile, Vector2 target, float speed, float turnResistance)
        {
            var move = target - projectile.Center;
            var length = move.Length();

            if (length > speed) move *= speed / length;

            move = (projectile.velocity * turnResistance + move) / (turnResistance + 1f);
            length = move.Length();

            if (length > speed) move *= speed / length;

            projectile.velocity = move;
        }
    }
}