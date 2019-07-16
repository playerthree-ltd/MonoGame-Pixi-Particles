using Microsoft.Xna.Framework;

namespace P.Particles
{
    public struct ValueStep<T>
    {
        public T Value { get; set; }
        public float Time { get; set; }
    }

    public struct ValueList<T>
    {
        public ValueStep<T>[] List { get; set; }
        public bool IsStepped { get; set; }
    }

    public class PropertyNode<V>
    {
        /// <summary>
        /// Value for the node.
        /// </summary>
        public V value;

        /// <summary>
        /// Time value for the node. Between 0-1.
        /// </summary>
        public float time;

        /// <summary>
        /// The next node in line.
        /// </summary>
        public PropertyNode<V> next;

        /// <summary>
        /// If this is the first node in the list, controls if the entire list is stepped or not.
        /// </summary>
        public bool isStepped;

        //todo - ease

        public PropertyNode(V value, float time)
        {
            this.value = value;
            this.time = time;
            next = null;
            isStepped = false;
        }

        public static PropertyNode<float> CreateList(ValueList<float> data)
        {
            var array = data.List;
            PropertyNode<float> node;
            PropertyNode<float> first;

            var elementFirst = array[0];

            first = node = new PropertyNode<float>(elementFirst.Value, elementFirst.Time);
            //only set up subsequent nodes if there are a bunch or the 2nd one is different from the first
            if (array.Length > 2 || (array.Length == 2 && array[1].Value != elementFirst.Value))
            {
                for (int i = 1; i < array.Length; i++)
                {
                    var current = array[i];
                    node.next = new PropertyNode<float>(current.Value, current.Time);
                    node = node.next;
                }
            }
            first.isStepped = data.IsStepped;

            return first;
        }

        public static PropertyNode<Color> CreateList(ValueList<Color> data)
        {
            var array = data.List;
            PropertyNode<Color> node;
            PropertyNode<Color> first;

            var elementFirst = array[0];

            first = node = new PropertyNode<Color>(elementFirst.Value, elementFirst.Time);
            //only set up subsequent nodes if there are a bunch or the 2nd one is different from the first
            if (array.Length > 2 || (array.Length == 2 && array[1].Value != elementFirst.Value))
            {
                for (int i = 1; i < array.Length; i++)
                {
                    var current = array[i];
                    node.next = new PropertyNode<Color>(current.Value, current.Time);
                    node = node.next;
                }
            }
            first.isStepped = data.IsStepped;

            return first;
        }

        public static PropertyNode<string> CreateList(ValueList<string> data)
        {
            var array = data.List;
            PropertyNode<string> node;
            PropertyNode<string> first;

            var elementFirst = array[0];

            first = node = new PropertyNode<string>(elementFirst.Value, elementFirst.Time);
            //only set up subsequent nodes if there are a bunch or the 2nd one is different from the first
            if (array.Length > 2 || (array.Length == 2 && array[1].Value != elementFirst.Value))
            {
                for (int i = 1; i < array.Length; i++)
                {
                    var current = array[i];
                    node.next = new PropertyNode<string>(current.Value, current.Time);
                    node = node.next;
                }
            }
            first.isStepped = data.IsStepped;

            return first;
        }
        public static PropertyNode<float> CreateList(BasicTweenable<float> data)
        {
            var start = new PropertyNode<float>(data.start, 0);
            //only set up a next value if it is different from the starting value
            if (data.end != data.start)
            {
                start.next = new PropertyNode<float>(data.end, 1);
            }
            return start;
        }

        public static PropertyNode<Color> CreateList(BasicTweenable<string> data)
        {
            var start = new PropertyNode<Color>(ParticleUtils.HexToRGB(data.start), 0);
            //only set up a next value if it is different from the starting value
            if (data.end != data.start)
            {
                start.next = new PropertyNode<Color>(ParticleUtils.HexToRGB(data.end), 1);
            }
            return start;
        }
    }
}
