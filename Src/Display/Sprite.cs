using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace P.Particles
{
    public class Sprite : Container
    {
        public Material Material { get; set; } = new Material();

        public Texture2D Texture { set { TextureRegion = new TextureRegion2D(value); } }
        public TextureRegion2D TextureRegion { get; set; }

        public Color Tint { get; set; } = Color.White;

        public float Width
        {
            get
            {
                return (int)(Math.Abs(_scale.X) * TextureRegion.Size.X);
            }
            set
            {
                int s = Math.Sign(_scale.X);
                s = s == 0 ? 1 : s;
                _scale.X = s * value / TextureRegion.Size.X;
            }
        }

        public float Height
        {
            get
            {
                return (int)(Math.Abs(_scale.Y) * TextureRegion.Size.Y);
            }
            set
            {
                int s = Math.Sign(_scale.Y);
                s = s == 0 ? 1 : s;
                _scale.Y = s * value / TextureRegion.Size.Y;
            }
        }

        public Vector2 Anchor = new Vector2();

        public Sprite(Texture2D texture) : base()
        {
            TextureRegion = new TextureRegion2D(texture);
        }

        public Sprite(TextureRegion2D region) : base()
        {
            TextureRegion = region;
        }

        public Sprite() : base()
        {

        }

        public void SetAnchor(Vector2 anchor)
        {
            Anchor = anchor;
        }

        public void SetAnchor(float x, float y)
        {
            Anchor.X = x;
            Anchor.Y = y;
        }
        public void SetAnchor(float x)
        {
            Anchor.X = x;
            Anchor.Y = x;
        }
    }
}