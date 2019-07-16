using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace P.Particles
{
    public class TextureRegion2D
    {
        public string Name { get; protected set; }
        public Texture2D BaseTexture { get; protected set; }
        public Rectangle Frame { get; protected set; }
        public Rectangle SourceRectangle { get; protected set; }
        public Rectangle Trim { get; protected set; }
        public Vector2 Size { get; protected set; }
        public bool IsRotated { get; protected set; }
        public bool IsTrimmed { get; protected set; }

        public TextureRegion2D(Texture2D texture, int x, int y, int width, int height)
            : this(null, texture, new Rectangle(x, y, width, height), new Rectangle(x, y, width, height), new Vector2(width, height))
        {
        }

        public TextureRegion2D(Texture2D texture, Rectangle frame, Rectangle sourceRect, Vector2 size)
            : this(null, texture, frame, sourceRect, size)
        {
        }

        public TextureRegion2D(string name, Texture2D texture, Rectangle frame, Rectangle sourceRect, Vector2 size)
            : this(name, texture, frame, sourceRect, size, false, false)
        {
        }

        public TextureRegion2D(Texture2D texture, Rectangle frame, Rectangle sourceRect, Vector2 size, bool isRotated = false)
            : this(null, texture, frame, sourceRect, size, isRotated)
        {
        }

        public TextureRegion2D(string name, Texture2D texture, Rectangle frame, Rectangle sourceRect, Vector2 size, bool isRotated = false)
            : this(name, texture, frame, sourceRect, size, isRotated, false)
        {
        }

        public TextureRegion2D(Texture2D texture, Rectangle frame, Rectangle sourceRect, Vector2 size, bool isRotated = false, bool isTrimmed = false)
            : this(null, texture, frame, sourceRect, size, isRotated, isTrimmed)
        {
        }

        public TextureRegion2D(Texture2D texture)
            : this(null, texture, new Rectangle(0, 0, texture.Width, texture.Height), new Rectangle(0, 0, texture.Width, texture.Height), new Vector2(texture.Width, texture.Height))
        {
        }

        public TextureRegion2D(string name, Texture2D texture, Rectangle frame, Rectangle sourceRect, Vector2 size, bool isRotated = false, bool isTrimmed = false)
        {
            Name = name;
            BaseTexture = texture;
            Frame = frame;
            SourceRectangle = sourceRect;
            Size = size;
            IsRotated = isRotated;
            IsTrimmed = isTrimmed;

            if (isTrimmed)
            {
                Trim = new Rectangle(sourceRect.X, sourceRect.Y, frame.Width, frame.Height);
            }
            else
            {
                Trim = new Rectangle(0, 0, 0, 0);
            }
        }

        public override string ToString()
        {
            return $"{Name ?? string.Empty} {SourceRectangle}";
        }
    }
}
