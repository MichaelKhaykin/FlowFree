using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowFree
{
    public static class Extensions
    {
        public static float ToRadians(this float f)
        {
            return MathHelper.ToRadians(f);
        }

        public static Vector2 ToVector2(this float f)
        {
            return new Vector2(f);
        }
    }
}
