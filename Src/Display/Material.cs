using System;
using Microsoft.Xna.Framework.Graphics;

//from nez with some minor modifications - https://github.com/prime31/Nez/blob/30c1932db6dc8604b1e812e22d500aa5529f87b8/Nez.Portable/Graphics/Material.cs
namespace P.Particles
{
    /// <summary>
    /// convenience subclass with a single property that casts the Effect for cleaner configuration
    /// </summary>
    public class Material<T> : Material, IDisposable where T : Effect
    {
        public new T effect
        {
            get => (T)base.effect;
            set => base.effect = value;
        }

        public Material()
        { }

        public Material(T effect) : base(effect)
        { }
    }

    public class Material : IComparable<Material>, IDisposable
    {
        //implementing depth stencil for alpha masking - https://medium.com/@sfranks/i-originally-wrote-this-for-the-best-way-to-mask-2d-sprites-in-xna-game-development-stack-949cf7bd7421
        public static DepthStencilState alphaMaskWrite = new DepthStencilState
        {
            StencilEnable = true,
            StencilFunction = CompareFunction.Always,
            StencilPass = StencilOperation.Replace,
            ReferenceStencil = 1,
            DepthBufferEnable = false,
        };

        public static DepthStencilState alphaMaskRead = new DepthStencilState
        {
            StencilEnable = true,
            StencilFunction = CompareFunction.LessEqual,
            StencilPass = StencilOperation.Keep,
            ReferenceStencil = 1,
            DepthBufferEnable = false,
        };

        /// <summary>
        /// BlendState used by the Batcher for the current renderable
        /// </summary>
        public BlendState blendState = BlendState.AlphaBlend;

        /// <summary>
        /// DepthStencilState used by the Batcher for the current renderable
        /// </summary>
        public DepthStencilState depthStencilState = DepthStencilState.None;

        /// <summary>
        /// Effect used by the Batcher for the current renderable
        /// </summary>
        public Effect effect;


        #region Static common states

        // BlendStates can be made to work with transparency by adding the following:
        // - AlphaSourceBlend = Blend.SourceAlpha, 
        // - AlphaDestinationBlend = Blend.InverseSourceAlpha 

        public static Material stencilWrite(int stencilRef = 1)
        {
            return new Material
            {
                depthStencilState = new DepthStencilState
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Always,
                    StencilPass = StencilOperation.Replace,
                    ReferenceStencil = stencilRef,
                    DepthBufferEnable = false,
                }
            };
        }

        public static Material stencilRead(int stencilRef = 1)
        {
            return new Material
            {
                depthStencilState = new DepthStencilState
                {
                    StencilEnable = true,
                    StencilFunction = CompareFunction.Equal,
                    StencilPass = StencilOperation.Keep,
                    ReferenceStencil = stencilRef,
                    DepthBufferEnable = false
                }
            };
        }

        public static Material blendDarken()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.Min,
                    AlphaSourceBlend = Blend.One,
                    AlphaDestinationBlend = Blend.One,
                    AlphaBlendFunction = BlendFunction.Min
                }
            };
        }

        public static Material blendLighten()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.Max,
                    AlphaSourceBlend = Blend.One,
                    AlphaDestinationBlend = Blend.One,
                    AlphaBlendFunction = BlendFunction.Max
                }
            };
        }

        public static Material blendScreen()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.InverseDestinationColor,
                    ColorDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.Add
                }
            };
        }

        public static Material blendMultiply()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.DestinationColor,
                    ColorDestinationBlend = Blend.Zero,
                    ColorBlendFunction = BlendFunction.Add,
                    AlphaSourceBlend = Blend.DestinationAlpha,
                    AlphaDestinationBlend = Blend.Zero,
                    AlphaBlendFunction = BlendFunction.Add

                }
            };
        }


        /// <summary>
        /// blend equation is sourceColor * sourceBlend + destinationColor * destinationBlend so this works out to sourceColor * destinationColor * 2
        /// and results in colors < 0.5 darkening and colors > 0.5 lightening the base
        /// </summary>
        public static Material blendMultiply2x()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.DestinationColor,
                    ColorDestinationBlend = Blend.SourceColor,
                    ColorBlendFunction = BlendFunction.Add
                }
            };
        }

        public static Material blendLinearDodge()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.Add
                }
            };
        }

        public static Material blendLinearBurn()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.One,
                    ColorDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.ReverseSubtract
                }
            };
        }

        public static Material blendDifference()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.InverseDestinationColor,
                    ColorDestinationBlend = Blend.InverseSourceColor,
                    ColorBlendFunction = BlendFunction.Add
                }
            };
        }

        public static Material blendSubtractive()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.One,
                    ColorBlendFunction = BlendFunction.ReverseSubtract,
                    AlphaSourceBlend = Blend.SourceAlpha,
                    AlphaDestinationBlend = Blend.One,
                    AlphaBlendFunction = BlendFunction.ReverseSubtract
                }
            };
        }

        public static Material blendAdditive()
        {
            return new Material
            {
                blendState = new BlendState
                {
                    ColorSourceBlend = Blend.SourceAlpha,
                    ColorDestinationBlend = Blend.One,
                    AlphaSourceBlend = Blend.SourceAlpha,
                    AlphaDestinationBlend = Blend.One
                }
            };
        }

        #endregion


        public Material()
        { }

        public Material(Effect effect)
        {
            this.effect = effect;
        }

        public Material(BlendState blendState, Effect effect = null)
        {
            this.blendState = blendState;
            this.effect = effect;
        }

        public Material(DepthStencilState depthStencilState, Effect effect = null)
        {
            this.depthStencilState = depthStencilState;
            this.effect = effect;
        }

        ~Material()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            // dispose of our state only if they are not using the shared instances
            if (blendState != null && blendState != BlendState.AlphaBlend)
            {
                blendState.Dispose();
                blendState = null;
            }

            if (depthStencilState != null && depthStencilState != DepthStencilState.None)
            {
                depthStencilState.Dispose();
                depthStencilState = null;
            }

            if (effect != null)
            {
                effect.Dispose();
                effect = null;
            }
        }

        /// <summary>
        /// very basic here. We only check if the pointers are the same
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="other">Other.</param>
        public int CompareTo(Material other)
        {
            if (object.ReferenceEquals(other, null))
                return 1;

            if (object.ReferenceEquals(this, other))
                return 0;

            return -1;
        }

        /// <summary>
        /// clones the Material. Note that the Effect is not cloned. It is the same instance as the original Material.
        /// </summary>
        public Material clone()
        {
            return new Material
            {
                blendState = blendState,
                depthStencilState = depthStencilState,
                effect = effect
            };
        }
    }
}
