using Microsoft.Xna.Framework;
using System;

namespace P.Particles
{
    /**
    * Singly linked list container for keeping track of interpolated properties for particles.
    * Each Particle will have one of these for each interpolated property.
    */
    public class PropertyList<V>
    {
        /// <summary>
        /// The current property node in the linked list.
        /// </summary>
        public PropertyNode<V> current;

        /// <summary>
        /// The next property node in the linked list. Stored separately for slightly less variable
        /// access.
        /// </summary>
        public PropertyNode<V> next;

        /// <summary>
        /// If this list manages colors, which requires a different method for interpolation.
        /// </summary>
        public bool isColor;

        public Func<PropertyList<float>, float, float> interpolateValue;
        public Func<PropertyList<Color>, float, Color> interpolateColor;

        public PropertyList(bool isColor = false)
        {
            this.isColor = isColor;
        }

        public void Reset(PropertyNode<V> first)
        {
            current = first;
            next = first.next;

            bool isSimple = (next != null && next.time >= 1f);
            if (isSimple)
            {
                if (isColor)
                {
                    interpolateColor = InterpolateColorSimple;
                }
                else
                {
                    interpolateValue = InterpolateValueSimple;
                }
            }
            else if (first.isStepped)
            {
                if (isColor)
                {
                    interpolateColor = InterpolateColorStepped;
                }
                else
                {
                    interpolateValue = InterpolateValueStepped;
                }
            }
            else
            {
                if (isColor)
                {
                    interpolateColor = InterpolateColorComplex;
                }
                else
                {
                    interpolateValue = InterpolateValueComplex;
                }
            }
        }

        public float InterpolateValueSimple(PropertyList<float> list, float lerp)
        {
            return (list.next.value - list.current.value) * lerp + list.current.value;
        }

        public Color InterpolateColorSimple(PropertyList<Color> list, float lerp)
        {
            var curVal = list.current.value;
            var nextVal = list.next.value;

            int r = (int)((nextVal.R - curVal.R) * lerp + curVal.R);
            int g = (int)((nextVal.G - curVal.G) * lerp + curVal.G);
            int b = (int)((nextVal.B - curVal.B) * lerp + curVal.B);

            return new Color(r, g, b);
        }

        public float InterpolateValueComplex(PropertyList<float> list, float lerp)
        {
            //make sure we are on the right segment
            while (lerp > list.next.time)
            {
                list.current = list.next;
                list.next = list.next.next;
            }
            //convert the lerp value to the segment range
            lerp = (lerp - list.current.time) / (list.next.time - list.current.time);
            return (list.next.value - list.current.value) * lerp + list.current.value;
        }

        public Color InterpolateColorComplex(PropertyList<Color> list, float lerp)
        {
            //make sure we are on the right segment
            while (lerp > list.next.time)
            {
                list.current = list.next;
                list.next = list.next.next;
            }
            //convert the lerp value to the segment range
            lerp = (lerp - list.current.time) / (list.next.time - list.current.time);
            var curVal = list.current.value;
            var nextVal = list.next.value;
            var r = (nextVal.R - curVal.R) * lerp + curVal.R;
            var g = (nextVal.G - curVal.G) * lerp + curVal.G;
            var b = (nextVal.B - curVal.B) * lerp + curVal.B;
            return new Color(r, g, b);
        }

        public float InterpolateValueStepped(PropertyList<float> list, float lerp)
        {
            //make sure we are on the right segment
            while (lerp > list.next.time)
            {
                list.current = list.next;
                list.next = list.next.next;
            }
            return list.current.value;
        }

        public Color InterpolateColorStepped(PropertyList<Color> list, float lerp)
        {
            //make sure we are on the right segment
            while (lerp > list.next.time)
            {
                list.current = list.next;
                list.next = list.next.next;
            }
            return list.current.value;
        }
    }
}
