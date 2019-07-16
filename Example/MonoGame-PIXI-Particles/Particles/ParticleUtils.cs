using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace P.Particles
{
    public struct EaseSegment
    {
        public float cp;
        public float s;
        public float e;
    }

    /// <summary>
    /// Contains helper functions for particles and emitters to use.
    /// </summary>
    class ParticleUtils
    {
        public const float DEG_TO_RADS = (float)Math.PI / 180f;

        /// <summary>
        /// Rotates a point by a given angle.
        /// </summary>
        /// <param name="angle">The angle to rotate by in degrees</param>
        /// <param name="p">The point to rotate around 0,0.</param>
        /// <returns>The rotated point</returns>
        public static Vector2 RotatePoint(float angle, Vector2 p)
        {
            //if(!angle) return; //I don't understand the point of this line?
            angle *= DEG_TO_RADS;
            var s = Math.Sin(angle);
            var c = Math.Cos(angle);
            var xnew = p.X * c - p.Y * s;
            var ynew = p.X * s + p.Y * c;
            return new Vector2((float)xnew, (float)ynew);
        }

        /// <summary>
        /// Returns a blend mode from a string
        /// </summary>
        /// <param name="name">Blend mode key</param>
        /// <returns>The blend mode</returns>
        public static BlendState GetBlendMode(string name = null)
        {
            if (name == null)
                return BlendState.AlphaBlend;

            name = name.ToUpper();
            switch (name)
            {
                case "NORMAL":
                    return BlendState.AlphaBlend;
                case "ADD":
                    return BlendState.Additive;
            }

            return BlendState.AlphaBlend;
        }

        /// <summary>
        /// Converts hexidecimal string to XNA Color
        /// </summary>
        /// <param name="color">The hexdecicaml string to convert</param>
        /// <returns>Hex as color</returns>
        public static Color HexToRGB(string color)
        {
            if (color[0] == '#')
                color = color.Substring(1);
            else if (color.IndexOf("0x") == 0)
                color = color.Substring(2);

            string alpha = null;
            if (color.Length == 8)
            {
                alpha = color.Substring(0, 2);
                color = color.Substring(2);
            }
            var r = int.Parse(color.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier); //Red
            var g = int.Parse(color.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier); //Green
            var b = int.Parse(color.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier); //Blue
            int a = 255;
            if (alpha != null)
            {
                int.TryParse(color.Substring(0, 2), out a);
            }

            return new Color(r, g, b, a);
        }
    }
}
