using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace P.Particles
{
    public class Container
    {
        public Container() { }

        public Container parent = null;

        public List<Container> Children { get; private set; } = new List<Container>();

        public bool Visible { get; set; } = true;
        public float Alpha { get; set; } = 1f;

        protected Vector2 _position = new Vector2();
        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
            }
        }

        public float X { get { return _position.X; } set { _position.X = value; } }
        public float Y { get { return _position.Y; } set { _position.Y = value; } }

        protected Vector2 _scale = new Vector2(1f, 1f);
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
            }
        }

        protected float _rotation = 0f;
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
            }
        }

        public void SetScale(Vector2 scale)
        {
            Scale = scale;
        }

        public void SetScale(float x, float y)
        {
            _scale.X = x;
            _scale.Y = y;
        }

        public void SetScale(float x)
        {
            _scale.X = x;
            _scale.Y = x;
        }

        public void AddChild(Container child)
        {
            if (child.parent != null)
            {
                child.parent.RemoveChild(child);
                child.parent = this;
            }

            child.parent = this;

            Children.Add(child);
        }

        public void AddChildAt(Container child, int index)
        {
            if (child.parent != null)
            {
                child.parent.RemoveChild(child);
                child.parent = this;
            }

            child.parent = this;

            Children.Insert(index, child);
        }

        public void RemoveChild(Container child)
        {
            child.parent = null;
            Children.Remove(child);
        }

        public void RemoveAllChildren()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                Children[i].parent = null;
                Children[i] = null;
            }
            Children.Clear();
        }

        public virtual void Destroy()
        {

        }
    }
}
