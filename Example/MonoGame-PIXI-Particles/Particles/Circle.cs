using Microsoft.Xna.Framework;

namespace P.Particles
{
    /// <summary>
    /// The Circle object is used to help draw graphics and can also be used to specify a hit area for displayObjects.
    /// </summary>
    public struct Circle
    {
        public float x;
        public float y;
        public float radius;
        public float minRadius;

        /// <summary>
        /// Initializes a new instance of the <see cref="Circle{float, float, float, float}"/> class.
        /// <param name="x">The X coordinate of the center of this circle, </param>
        /// <param name="y">The Y coordinate of the center of this circle, </param>
        /// <param name="radius">The radius of the circle, </param>
        /// <param name="minRadius">The minimum radius of the circle, </param>
        /// </summary>
        public Circle(float x = 0f, float y = 0f, float radius = 0f, float minRadius = 0f)
        {
            this.x = x;
            this.y = y;
            this.radius = radius;
            this.minRadius = minRadius;
        }

        /// <summary>
        /// Checks whether the x and y coordinates given are contained within this circle
        /// </summary>
        /// <param name="x">The X coordinate of the point to test</param>
        /// <param name="y">The Y coordinate of the point to test</param>
        /// <returns>Whether the x/y coordinates are within this Circle.</returns>
        public bool Contains(float x, float y)
        {
            if (radius <= 0)
            {
                return false;
            }
            var r2 = radius * radius;
            var dx = (this.x - x);
            var dy = (this.y - y);
            dx *= dx;
            dy *= dy;
            return (dx + dy <= r2);
        }

        /// <summary>
        /// Returns the framing rectangle of the circle as a Rectangle object
        /// </summary>
        /// <returns>the framing rectangle.</returns>
        public Rectangle GetBounds()
        {
            return new Rectangle((int)(x - radius), (int)(y - radius), (int)(radius * 2), (int)(radius * 2));
        }
    }
}

