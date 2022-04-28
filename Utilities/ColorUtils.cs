using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tenebrae.Utilities
{
    public static class ColorUtils
    {
        public static Color MultipleLerp(this List<Color> colors, float percent)
        {
            return MultipleLerp(percent, colors.ToArray());
        }

        public static Color MultipleLerp(float percent, params Color[] values)
        {
            if (percent >= 1) return values.Last();

            percent = Math.Max(percent, 0);
            float num = 1f / (values.Length - 1);
            int index = Math.Max(0, (int)(percent / num));

            return Color.Lerp(values[index], values[index + 1], (percent - num * index) / num);
        }
    }
}