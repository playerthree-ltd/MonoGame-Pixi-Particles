using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace P.Particles
{
    public class Particle : Sprite
    {
        /// <summary>
        /// The emitter that controls this particle.
        /// </summary>
        public Emitter emitter;

        /// <summary>
        /// The velocity of the particle. Speed may change, but the angle also
        /// contained in velocity is constant.
        /// </summary>
        public Vector2 velocity;

        /// <summary>
        /// The maximum lifetime of this particle, in seconds.
        /// </summary>
        public float maxLife;

        /// <summary>
        /// The current age of the particle, in seconds.
        /// </summary>
        public float age;

        public BlendState blendMode;

        //todo - simple ease
        //todo - extra data

        /// <summary>
        /// The alpha of the particle throughout its life.
        /// </summary>
        public PropertyList<float> alphaList;

        /// <summary>
        /// The speed of the particle throughout its life.
        /// </summary>
        public PropertyList<float> speedList;

        /// <summary>
        /// A multiplier from 0-1 applied to the speed of the particle at all times.
        /// </summary>
        public float speedMultiplier;

        /// <summary>
        /// Acceleration to apply to the particle.
        /// </summary>
        public Vector2 acceleration;

        /// <summary>
        /// The maximum speed allowed for accelerating particles. Negative values, values of 0 or NaN
        /// will disable the maximum speed.
        /// </summary>
        public float maxSpeed;

        /// <summary>
        /// Speed at which the particle rotates, in radians per second.
        /// </summary>
        public float rotationSpeed;

        /// <summary>
        /// Acceleration of rotation (angular acceleration) to apply to the particle.
        /// </summary>
        public float rotationAcceleration;

        /// <summary>
        /// If particle rotation is locked, preventing rotation from occurring due
        /// to directional changes.
        /// </summary>
        public bool noRotation;

        /// <summary>
        /// The scale of the particle throughout its life.
        /// </summary>
        public PropertyList<float> scaleList;

        /// <summary>
        /// A multiplier from 0-1 applied to the scale of the particle at all times.
        /// </summary>
        public float scaleMultiplier;

        /// <summary>
        /// The tint of the particle throughout its life.
        /// </summary>
        public PropertyList<Color> colorList;

        //todo -  particle init
        //todo - update ref
        //todo - destroy ref
        //todo - sprite destroy ref

        /// <summary>
        /// If alpha should be interpolated at all.
        /// </summary>
        protected bool doAlpha;

        /// <summary>
        /// If scale should be interpolated at all.
        /// </summary>
        protected bool doScale;

        /// <summary>
        /// If speed should be interpolated at all.
        /// </summary>
        protected bool doSpeed;

        /// <summary>
        /// If acceleration should be handled at all. _doSpeed is mutually exclusive with this, and _doSpeed gets priority.
        /// </summary>
        protected bool doAcceleration;

        /// <summary>
        /// If color should be interpolated at all.
        /// </summary>
        protected bool doColor;

        /// <summary>
        /// If normal movement should be handled. Subclasses wishing to override movement can set this to false in init().
        /// </summary>
        protected bool doNormalMovement;

        /// <summary>
        /// One divided by the max life of the particle, saved for slightly faster math.
        /// </summary>
        private float oneOverLife;

        /// <summary>
        /// Reference to the next particle in the list.
        /// </summary>
        public Particle next;

        /// <summary>
        /// Reference to the previous particle in the list.
        /// </summary>
        public Particle prev;

        /// <param name="emitter">Emitter that controls this particle.</param>
        public Particle(Emitter emitter) : base(/*TextureRegion2D.Empty*/)
        {
            //start off the sprite with a blank texture, since we are going to replace it
            //later when the particle is initialized.

            this.emitter = emitter;
            //paricles should be centred
            Anchor = new Vector2(0.5f);
            velocity = new Vector2();
            rotationSpeed = 0f;
            rotationAcceleration = 0f;
            maxLife = 0f;
            age = 0f;
            //ease = null;
            //...
            alphaList = new PropertyList<float>();
            speedList = new PropertyList<float>();
            speedMultiplier = 1f;
            acceleration = new Vector2();
            maxSpeed = float.NaN;
            scaleList = new PropertyList<float>();
            scaleMultiplier = 1f;
            colorList = new PropertyList<Color>();
            doAlpha = false;
            doScale = false;
            doSpeed = false;
            doAcceleration = false;
            doColor = false;
            doNormalMovement = false;
            oneOverLife = 0f;
            next = null;
            prev = null;
        }

        /// <summary>
        /// Initializes the particle for use, based on the properties that have to
        /// have been set already on the particle.
        /// </summary>
        public void Init()
        {
            //reset the age
            age = 0f;
            //set up the velocity based on the start speed and rotation
            velocity.X = speedList.current.value * speedMultiplier;
            velocity.Y = 0;
            velocity = ParticleUtils.RotatePoint(Rotation, velocity);
            if (noRotation)
            {
                Rotation = 0;
            }
            else
            {
                //convert rotation to Radians from Degrees
                Rotation *= ParticleUtils.DEG_TO_RADS;
            }
            //convert rotation speed to Radians from Degrees
            rotationSpeed *= ParticleUtils.DEG_TO_RADS;
            rotationAcceleration *= ParticleUtils.DEG_TO_RADS;

            //set alpha to inital alpha
            Alpha = alphaList.current.value;
            //set scale to initial scale
            Scale = new Vector2(scaleList.current.value, scaleList.current.value);
            //figure out what we need to interpolate
            doAlpha = (alphaList.current.next != null);
            doSpeed = (speedList.current.next != null);
            doScale = (scaleList.current.next != null);
            doColor = (colorList.current.next != null);
            doAcceleration = acceleration.X != 0 || acceleration.Y != 0;
            //_doNormalMovement can be cancelled by subclasses
            doNormalMovement = doSpeed || speedList.current.value != 0 || doAcceleration;
            //save our lerp helper
            oneOverLife = 1 / maxLife;
            //set the inital color
            var color = colorList.current.value;
            Tint = new Color(color.R, color.G, color.B);
            //ensure visibility
            Visible = true;
        }

        /// <summary>
        /// Sets the texture for the particle. This can be overridden to allow
        /// for an animated particle.
        /// </summary>
        /// <param name="texture">The texture to set.</param>
        public virtual void ApplyArt(TextureRegion2D texture)
        {
            TextureRegion = texture;
        }

        /// <summary>
        /// Updates the particle.
        /// </summary>
        /// <param name="delta">Time elapsed since the previous frame, in __seconds__.</param>
        /// <returns>The standard interpolation multiplier (0-1) used for all 
        /// relevant particle properties.A value of -1 means the particle died of old age instead.</returns>
        public float Update(float delta)
        {
            //increase age
            age += delta;
            //recycle particle if it is too old
            if (age >= maxLife || age < 0)
            {
                Kill();
                return -1f;
            }

            //determine our interpolation value
            var lerp = age * oneOverLife;//lifetime / maxLife;

            //interpolate alpha
            if (doAlpha/* && alphaList.interpolateValue != null*/)
            {
                Alpha = alphaList.interpolateValue(alphaList, lerp);
            }

            //interpolate scale
            if (doScale/* && scaleList.interpolateValue != null*/)
            {
                var scale = scaleList.interpolateValue(scaleList, lerp) * scaleMultiplier;
                Scale = new Vector2(scale);
            }
            //handle movement
            if (doNormalMovement)
            {
                float deltaX;
                float deltaY;
                //interpolate speed
                if (doSpeed/* && speedList.interpolateValue != null*/)
                {
                    var speed = speedList.interpolateValue(speedList, lerp) * speedMultiplier;
                    // ParticleUtils.normalize(this.velocity);
                    velocity.Normalize();
                    //ParticleUtils.scaleBy(this.velocity, speed);
                    velocity *= speed;
                    deltaX = velocity.X * delta;
                    deltaY = velocity.Y * delta;
                }
                else if (doAcceleration)
                {
                    var oldVX = velocity.X;
                    var oldVY = velocity.Y;
                    velocity.X += acceleration.X * delta;
                    velocity.Y += acceleration.Y * delta;
                    if (!float.IsNaN(maxSpeed) && maxSpeed != 0)
                    {
                        var currentSpeed = velocity.Length();
                        //if we are going faster than we should, clamp at the max speed
                        //DO NOT recalculate vector length
                        if (currentSpeed > maxSpeed)
                        {
                            velocity *= maxSpeed / currentSpeed;
                        }
                    }
                    // calculate position delta by the midpoint between our old velocity and our new velocity
                    deltaX = (oldVX + velocity.X) / 2 * delta;
                    deltaY = (oldVY + velocity.Y) / 2 * delta;
                }
                else
                {
                    deltaX = this.velocity.X * delta;
                    deltaY = this.velocity.Y * delta;
                }
                //adjust position based on velocity
                X += deltaX;
                Y += deltaY;
            }
            //interpolate color
            if (doColor)
            {
                Tint = colorList.InterpolateColorSimple(colorList, lerp);//todo - unsure how this is to be ported
            }
            //update rotation
            if (rotationAcceleration != 0)
            {
                var newRotationSpeed = rotationSpeed + rotationAcceleration * delta;

                Rotation += (rotationSpeed + newRotationSpeed) / 2 * delta;
                rotationSpeed = newRotationSpeed;
            }
            else if (this.rotationSpeed != 0)
            {
                Rotation += rotationSpeed * delta;
            }
            else if (acceleration != null && !noRotation)
            {
                Rotation = (float)Math.Atan2(velocity.Y, velocity.X);// + Math.PI / 2;
            }
            return lerp;
        }

        /// <summary>
        /// Kills the particle, removing it from the display list
        /// and telling the emitter to recycle it.
        /// </summary>
        public void Kill()
        {
            emitter.Recycle(this);
        }

        public override void Destroy()
        {
            if (parent != null)
                parent.RemoveChild(this);
            base.Destroy();
            emitter = null;
            colorList = null;
            scaleList = null;
            alphaList = null;
            speedList = null;
            next = null;
            prev = null;
        }
    }
}
