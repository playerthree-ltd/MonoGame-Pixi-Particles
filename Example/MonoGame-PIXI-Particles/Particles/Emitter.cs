using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace P.Particles
{
    public class Emitter
    {
        private Vector2 helperPoint = new Vector2();

        private TextureRegion2D[] particleImages;

        private PropertyNode<float> startAlpha;

        private PropertyNode<float> startSpeed;

        private float minimumSpeedMultiplier;

        private Vector2 acceleration;

        private float maxSpeed;

        private PropertyNode<float> startScale;

        private float minimumScaleMultiplier;

        private PropertyNode<Color> startColor;

        private float minLifetime;

        private float maxLifetime;

        private float minStartRotation;

        private float maxStartRotation;

        private bool noRotation;

        private float minRotationSpeed;

        private float maxRotationSpeed;

        private float rotationAcceleration;

        private BlendState particleBlendMode;

        protected float frequency;

        /**
         * Chance that a particle will be spawned on each opportunity to spawn one.
         * 0 is 0%, 1 is 100%.
         */
        private float spawnChance;

        private int maxParticles;

        private float emitterLifetime;//total life

        private Vector2 spawnPos;

        /**
         * How the particles will be spawned. Valid types are "point", "rectangle",
         * "circle", "burst", "ring".
         */
        private string spawnType;

        /**
        * A reference to the emitter function specific to the spawn type.
        */
        private Action<Particle, float, float, float> spawnFunc;

        public Rectangle spawnRect;

        //todo polygonal chain

        public Circle spawnCircle;

        private float particlesPerWave;

        private float particleSpacing;

        private float angleStart;

        public float rotation;

        private Vector2 ownerPos;

        protected Vector2 prevEmitterPos;

        protected bool prevPosIsValid;

        protected bool posChanged;

        protected Container parent;

        public bool addAtBack;

        private int particleCount;

        protected bool _emit;

        protected float spawnTimer;

        protected float emitterLife;//remaining life

        protected Particle activeParticlesFirst;

        protected Particle activeParticlesLast;

        protected Particle poolFirst;

        protected EmitterConfig origConfig;

        protected TextureRegion2D[] origArt;

        protected bool autoUpdate;

        protected bool destroyWhenComplete;

        protected Action completeCallback;

        public Emitter(Container particleParent, TextureRegion2D[] particleImages, EmitterConfig config)
        {
            //properties for individual particles
            this.particleImages = null;
            startAlpha = null;
            startSpeed = null;
            minimumSpeedMultiplier = 1;
            acceleration = new Vector2();
            maxSpeed = float.NaN;
            startScale = null;
            minimumScaleMultiplier = 1;
            startColor = null;
            minLifetime = 0f;
            maxLifetime = 0f;
            minStartRotation = 0;
            maxStartRotation = 0;
            noRotation = false;
            minRotationSpeed = 0;
            maxRotationSpeed = 0;
            particleBlendMode = BlendState.AlphaBlend;
            /// this.customEase = null;
            /// this.extraData = null;
            //properties for spawning particles
            frequency = 1;
            spawnChance = 1;
            maxParticles = 1000;
            emitterLifetime = -1;
            // this.spawnPos = null;
            spawnType = null;
            spawnFunc = null;
            // this.spawnRect = null;
            // this.spawnCircle = null;
            //  this.spawnPolygonalChain = null;
            particlesPerWave = 1;
            particleSpacing = 0;
            angleStart = 0;
            //emitter properties
            rotation = 0;
            // this.ownerPos = null;
            //  this.prevEmitterPos = null;
            prevPosIsValid = false;
            posChanged = false;
            parent = null;
            addAtBack = false;
            particleCount = 0;
            _emit = false;
            spawnTimer = 0;
            emitterLife = -1;
            activeParticlesFirst = null;
            activeParticlesLast = null;
            poolFirst = null;
            origConfig = null;
            origArt = null;
            autoUpdate = false;
            destroyWhenComplete = false;
            completeCallback = null;

            //set the initial parent
            parent = particleParent;

            if (particleImages != null && config != null)
                Init(particleImages, config);
        }

        public void Init(TextureRegion2D[] art, EmitterConfig config)
        {
            Cleanup();

            //store the original config and particle images, in case we need to re-initialize
            //when the particle constructor is changed
            origConfig = config;
            origArt = art;
            particleImages = art;
            ///////////////////////////
            // Particle Properties   //
            ///////////////////////////
            //set up the alpha
            if (config.alpha.HasValue)
            {
                startAlpha = PropertyNode<float>.CreateList((BasicTweenable<float>)config.alpha);
            }
            else
            {
                startAlpha = new PropertyNode<float>(1f, 0f);
            }
            //set up the speed
            if (config.speed.HasValue)
            {
                startSpeed = PropertyNode<float>.CreateList((BasicTweenable<float>)config.speed);
                minimumSpeedMultiplier = (config.minimumSpeedMultiplier.HasValue) ? (float)config.minimumSpeedMultiplier : 1f;
            }
            else
            {
                minimumSpeedMultiplier = 1f;
                startSpeed = new PropertyNode<float>(0f, 0f);
            }
            //set up acceleration
            var acceleration = config.acceleration;
            if (acceleration.HasValue)
            {
                //make sure we disable speed interpolation
                startSpeed.next = null;
                this.acceleration = (Vector2)acceleration;
                maxSpeed = (float)(config.maxSpeed.HasValue ? config.maxSpeed : float.NaN);
            }
            else
            {
                this.acceleration = new Vector2();
            }
            //set up the scale
            if (config.scale.HasValue)
            {
                startScale = PropertyNode<float>.CreateList((BasicTweenable<float>)config.scale);
                minimumScaleMultiplier = (config.minimumScaleMultiplier.HasValue) ? (float)config.minimumScaleMultiplier : 1f;
            }
            else
            {
                startScale = new PropertyNode<float>(1f, 0f);
                minimumScaleMultiplier = 1f;
            }
            //set up the color
            if (config.color.HasValue)
            {
                startColor = PropertyNode<Color>.CreateList((BasicTweenable<string>)config.color);
            }
            else
            {
                startColor = new PropertyNode<Color>(Color.White, 0);
            }
            //set up the start rotation
            if (config.startRotation.HasValue)
            {
                var startRot = (RandNumber)config.startRotation;
                minStartRotation = startRot.min;
                maxStartRotation = startRot.max;
            }
            else
            {
                minStartRotation = maxStartRotation = 0f;
            }
            if (config.noRotation.HasValue && (minStartRotation != 0f || maxStartRotation != 0f))
            {
                noRotation = (bool)config.noRotation;
            }
            else
            {
                noRotation = false;
            }
            //set up the rotation speed
            if (config.rotationSpeed.HasValue)
            {
                var rotSpeed = (RandNumber)config.rotationSpeed;
                minRotationSpeed = rotSpeed.min;
                maxRotationSpeed = rotSpeed.max;
            }
            else
            {
                minRotationSpeed = maxRotationSpeed = 0;
            }

            rotationAcceleration = config.rotationAcceleration.HasValue ? (float)config.rotationAcceleration : 0f;
            //set up the lifetime
            minLifetime = config.lifetime.min;
            maxLifetime = config.lifetime.max;
            //get the blend mode
            particleBlendMode = ParticleUtils.GetBlendMode(config.blendMode);
          
            //////////////////////////
            // Emitter Properties   //
            //////////////////////////
            //reset spawn type specific settings
            particlesPerWave = 1;
            if (config.particlesPerWave.HasValue && config.particlesPerWave > 1)
            {
                particlesPerWave = (int)config.particlesPerWave;
            }

            particleSpacing = 0;
            angleStart = 0;
            Circle spawnCircle;
            //determine the spawn function to use
            switch (config.spawnType)
            {
                case "rect":
                    spawnType = "rect";
                    spawnFunc = SpawnRect;
                    var spawnRect = (Rectangle)config.spawnRect;
                    this.spawnRect = spawnRect;
                    break;
                case "circle":
                    spawnType = "circle";
                    spawnFunc = SpawnCircle;
                    spawnCircle = (Circle)config.spawnCircle;
                    this.spawnCircle = spawnCircle;
                    break;
                case "ring":
                    spawnType = "ring";
                    spawnFunc = SpawnRing;
                    spawnCircle = (Circle)config.spawnCircle;
                    this.spawnCircle = spawnCircle;
                    break;
                case "burst":
                    spawnType = "burst";
                    spawnFunc = SpawnBurst;
                    particleSpacing = (float)config.particleSpacing;
                    angleStart = config.angleStart.HasValue ? (float)config.angleStart : 0f;
                    break;
                case "point":
                    spawnType = "point";
                    spawnFunc = SpawnPoint;
                    break;
                default:
                    spawnType = "point";
                    spawnFunc = SpawnPoint;
                    break;
            }
            //set the spawning frequency
            frequency = config.frequency;
            spawnChance = (config.spawnChance.HasValue && config.spawnChance > 0) ? (float)config.spawnChance : 1f;
            //set the emitter lifetime
            emitterLifetime = config.emitterLifetime.HasValue ? (float)config.emitterLifetime : -1f;
            //set the max particles
            maxParticles = config.maxParticles > 0 ? (int)config.maxParticles : 1000;
            //determine if we should add the particle at the back of the list or not
            addAtBack = config.addAtBack;
            //reset the emitter position and rotation variables
            rotation = 0;
            ownerPos = new Vector2();
            spawnPos = config.pos;
            prevEmitterPos = new Vector2(spawnPos.X, spawnPos.Y);
            //previous emitter position is invalid and should not be used for interpolation
            prevPosIsValid = false;
            //start emitting
            spawnTimer = 0;
            Emit = config.emit;
        }

        public void Recycle(Particle particle)
        {
            if (particle.next != null)
                particle.next.prev = particle.prev;
            if (particle.prev != null)
                particle.prev.next = particle.next;
            if (particle == activeParticlesLast)
                activeParticlesLast = particle.prev;
            if (particle == activeParticlesFirst)
                activeParticlesFirst = particle.next;
            //add to pool
            particle.prev = null;
            particle.next = poolFirst;
            poolFirst = particle;
            //remove child from display, or make it invisible if it is in a ParticleContainer (we don't have particle containers in this c# port)
            if (particle.parent != null)
                particle.parent.RemoveChild(particle);
            //decrease count
            --particleCount;
        }

        /// <summary>
        /// Sets the rotation of the emitter to a new value.
        /// </summary>
        /// <param name="newRot">The new rotation, in degrees.</param>
        public void Rotate(float newRot)
        {
            if (rotation == newRot) return;
            //caclulate the difference in rotation for rotating spawnPos
            var diff = newRot - rotation;
            rotation = newRot;
            //rotate spawnPos
            spawnPos = ParticleUtils.RotatePoint(diff, spawnPos);
            //mark the position as having changed
            posChanged = true;
        }

        /// <summary>
        /// Changes the spawn position of the emitter.
        /// </summary>
        /// <param name="x">The new x value of the spawn position for the emitter.</param>
        /// <param name="y">The new y value of the spawn position for the emitter.</param>
        public void UpdateSpawnPos(float x, float y)
        {
            posChanged = true;
            spawnPos.X = x;
            spawnPos.Y = y;
        }

        /// <summary>
        /// Changes the position of the emitter's owner. You should call this if you are adding
        /// particles to the world container that your emitter's owner is moving around in.
        /// </summary>
        /// <param name="x">The new x value of the emitter's owner.</param>
        /// <param name="y">The new y value of the emitter's owner.</param>
        public void UpdateOwnerPos(float x, float y)
        {
            posChanged = true;
            ownerPos.X = x;
            ownerPos.Y = y;
        }

        /// <summary>
        /// Prevents emitter position interpolation in the next update.
        /// This should be used if you made a major position change of your emitter's owner
        /// that was not normal movement.
        /// </summary>
        public void ResetPositionTracking()
        {
            prevPosIsValid = false;
        }

        /// <summary>
        /// If particles should be emitted during update() calls. Setting this to false
        /// stops new particles from being created, but allows existing ones to die out.
        /// </summary>
        public bool Emit { get { return _emit; } set { _emit = value; emitterLife = emitterLifetime; } }

        /// <summary>
        /// Starts emitting particles, sets autoUpdate to true, and sets up the Emitter to destroy itself
        /// when particle emission is complete.
        /// </summary>
        /// <param name="callback">Callback for when emission is complete (all particles have died off)</param>
        public void PlayOnceAndDestroy(Action callback = null)
        {
            Emit = true;
            destroyWhenComplete = true;
            completeCallback = callback;
        }

        /// <summary>
        /// Starts emitting particles and optionally calls a callback when particle emission is complete.
        /// </summary>
        /// <param name="callback">Callback for when emission is complete (all particles have died off)</param>
        public void PlayOnce(Action callback = null)
        {
            Emit = true;
            completeCallback = callback;
        }

        /// <summary>
        /// Updates all particles spawned by this emitter and emits new ones.
        /// </summary>
        /// <param name="delta">Time elapsed since the previous frame, in __seconds__</param>
        public void Update(float delta)
        {
            //if we don't have a parent to add particles to, then don't do anything.
            //this also works as a isDestroyed check
            if (parent == null)
                return;

            //update existing particles
            Particle particle, next;
            int i;
            for (particle = activeParticlesFirst; particle != null; particle = next)
            {
                next = particle.next;
                particle.Update(delta);
            }
            float prevX = 0f, prevY = 0f;
            //if the previous position is valid, store these for later interpolation
            if (prevPosIsValid)
            {
                prevX = prevEmitterPos.X;
                prevY = prevEmitterPos.Y;
            }
            //store current position of the emitter as local variables
            var curX = ownerPos.X + spawnPos.X;
            var curY = ownerPos.Y + spawnPos.Y;
            //spawn new particles
            if (_emit)
            {
                //decrease spawn timer
                spawnTimer -= delta < 0 ? 0 : delta;
                //while _spawnTimer < 0, we have particles to spawn
                while (spawnTimer <= 0)
                {
                    //determine if the emitter should stop spawning
                    if (emitterLife > 0)
                    {
                        emitterLife -= frequency;
                        if (emitterLife <= 0)
                        {
                            spawnTimer = 0;
                            emitterLife = 0;
                            Emit = false;
                            break;
                        }
                    }
                    //determine if we have hit the particle limit
                    if (particleCount >= maxParticles)
                    {
                        spawnTimer += frequency;
                        continue;
                    }
                    //determine the particle lifetime
                    float lifetime;
                    if (minLifetime == maxLifetime)
                        lifetime = minLifetime;
                    else
                        lifetime = Mathf.Random() * (maxLifetime - minLifetime) + minLifetime;
                    //only make the particle if it wouldn't immediately destroy itself
                    if (-spawnTimer < lifetime)
                    {
                        //If the position has changed and this isn't the first spawn,
                        //interpolate the spawn position
                        float emitPosX, emitPosY;
                        if (prevPosIsValid && posChanged)
                        {
                            //1 - _spawnTimer / delta, but _spawnTimer is negative
                            var lerp = 1 + spawnTimer / delta;
                            emitPosX = (curX - prevX) * lerp + prevX;
                            emitPosY = (curY - prevY) * lerp + prevY;
                        }
                        else//otherwise just set to the spawn position
                        {
                            emitPosX = curX;
                            emitPosY = curY;
                        }
                        //create enough particles to fill the wave (non-burst types have a wave of 1)
                        i = 0;
                        for (int len = (int)Math.Min(particlesPerWave, maxParticles - particleCount); i < len; ++i)
                        {
                            //see if we actually spawn one
                            if (spawnChance < 1 && Mathf.Random() >= spawnChance)
                                continue;
                            //create particle
                            Particle p;
                            if (poolFirst != null)
                            {
                                p = poolFirst;
                                poolFirst = poolFirst.next;
                                p.next = null;
                            }
                            else
                            {
                                p = new Particle(this);
                            }

                            //set a random texture if we have more than one
                            if (particleImages.Length > 1)
                            {
                                p.ApplyArt(particleImages[Mathf.FloorToInt(Mathf.Random() * particleImages.Length)]);
                            }
                            else
                            {
                                //if they are actually the same texture, a standard particle
                                //will quit early from the texture setting in setTexture().
                                p.ApplyArt(particleImages[0]);
                            }
                            //set up the start and end values
                            p.alphaList.Reset(startAlpha);
                            if (minimumSpeedMultiplier != 1)
                            {
                                p.speedMultiplier = Mathf.Random() * (1 - minimumSpeedMultiplier) + minimumSpeedMultiplier;
                            }
                            p.speedList.Reset(startSpeed);
                            p.acceleration.X = acceleration.X;
                            p.acceleration.Y = acceleration.Y;
                            p.maxSpeed = maxSpeed;
                            if (minimumScaleMultiplier != 1)
                            {
                                p.scaleMultiplier = Mathf.Random() * (1 - minimumScaleMultiplier) + minimumScaleMultiplier;
                            }
                            p.scaleList.Reset(startScale);
                            p.colorList.Reset(startColor);
                            //randomize the rotation speed
                            if (minRotationSpeed == maxRotationSpeed)
                                p.rotationSpeed = minRotationSpeed;
                            else
                                p.rotationSpeed = Mathf.Random() * (maxRotationSpeed - minRotationSpeed) + minRotationSpeed;
                            p.rotationAcceleration = rotationAcceleration;
                            p.noRotation = noRotation;
                            //set up the lifetime
                            p.maxLife = lifetime;
                            //set the blend mode
                            p.Material.blendState = particleBlendMode;

                            //set the custom ease, if any
                            // p.ease = this.customEase;
                            //set the extra data, if any
                            //p.extraData = this.extraData;
                            //call the proper function to handle rotation and position of particle
                            spawnFunc(p, emitPosX, emitPosY, i);

                            //initialize particle
                            p.Init();
                            //update the particle by the time passed, so the particles are spread out properly
                            p.Update(-spawnTimer);//we want a positive delta, because a negative delta messes things up
                                                       //add the particle to the display list
                            if (p.parent == null)
                            {
                                if (addAtBack)
                                    parent.AddChildAt(p, 0);
                                else
                                    parent.AddChild(p);
                            }
                            else
                            {
                                //kind of hacky, but performance friendly
                                //shuffle children to correct place
                                var children = parent.Children;
                                //avoid using splice if possible
                                if (children[0] == p)
                                    children.RemoveAt(0);
                                else if (children[children.Count - 1] == p)
                                    children.RemoveAt(children.Count - 1);
                                else
                                {
                                    children.Remove(p);
                                    //var index = children.IndexOf(p);
                                    // children.splice(index, 1);
                                }
                                if (addAtBack)
                                    children.Insert(0, p);
                                else
                                    children.Add(p);
                            }
                            //add particle to list of active particles
                            if (activeParticlesLast != null)
                            {
                                activeParticlesLast.next = p;
                                p.prev = activeParticlesLast;
                                activeParticlesLast = p;
                            }
                            else
                            {
                                activeParticlesLast = activeParticlesFirst = p;
                            }
                            ++particleCount;
                        }
                    }
                    //increase timer and continue on to any other particles that need to be created
                    spawnTimer += frequency;
                }
            }
            //if the position changed before this update, then keep track of that
            if (posChanged)
            {
                prevEmitterPos.X = curX;
                prevEmitterPos.Y = curY;
                prevPosIsValid = true;
                posChanged = false;
            }

            //if we are all done and should destroy ourselves, take care of that
            if (!_emit && activeParticlesFirst == null)
            {
                if (completeCallback != null)
                {
                    completeCallback.Invoke();
                }
                if (destroyWhenComplete)
                {
                    Destroy();
                }
            }
        }

        protected void SpawnPoint(Particle p, float emitPosX, float emitPosY, float i = -1f)
        {
            //set the initial rotation/direction of the particle based on
            //starting particle angle and rotation of emitter
            if (minStartRotation == maxStartRotation)
                p.Rotation = minStartRotation + rotation;
            else
                p.Rotation = Mathf.Random() * (maxStartRotation - minStartRotation) + minStartRotation + rotation;
            //drop the particle at the emitter's position
            p.X = emitPosX;
            p.Y = emitPosY;
        }

        protected void SpawnRect(Particle p, float emitPosX, float emitPosY, float i = -1f)
        {
            //set the initial rotation/direction of the particle based on starting
            //particle angle and rotation of emitter
            if (minStartRotation == maxStartRotation)
                p.Rotation = minStartRotation + rotation;
            else
                p.Rotation = Mathf.Random() * (maxStartRotation - minStartRotation) + minStartRotation + rotation;
            //place the particle at a random point in the rectangle
            helperPoint.X = Mathf.Random() * spawnRect.Width + spawnRect.X;
            helperPoint.Y = Mathf.Random() * spawnRect.Height + spawnRect.Y;
            if (rotation != 0)
                helperPoint = ParticleUtils.RotatePoint(rotation, helperPoint);
            p.X = emitPosX + helperPoint.X;
            p.Y = emitPosY + helperPoint.Y;
        }

        protected void SpawnCircle(Particle p, float emitPosX, float emitPosY, float i = -1f)
        {
            //set the initial rotation/direction of the particle based on starting
            //particle angle and rotation of emitter
            if (minStartRotation == maxStartRotation)
                p.Rotation = minStartRotation + rotation;
            else
                p.Rotation = Mathf.Random() * (maxStartRotation - minStartRotation) +
                            minStartRotation + rotation;
            //place the particle at a random radius in the circle
            helperPoint.X = Mathf.Random() * spawnCircle.radius;
            helperPoint.Y = 0;
            //rotate the point to a random angle in the circle
            helperPoint = ParticleUtils.RotatePoint(Mathf.Random() * 360, helperPoint);
            //offset by the circle's center
            helperPoint.X += spawnCircle.x;
            helperPoint.Y += spawnCircle.y;
            //rotate the point by the emitter's rotation
            if (rotation != 0)
                ParticleUtils.RotatePoint(rotation, helperPoint);
            //set the position, offset by the emitter's position
            p.X = emitPosX + helperPoint.X;
            p.Y = emitPosY + helperPoint.Y;
        }

        protected void SpawnRing(Particle p, float emitPosX, float emitPosY, float i = -1f)
        {
            //var spawnCircle = this.spawnCircle;
            //set the initial rotation/direction of the particle based on starting
            //particle angle and rotation of emitter
            if (minStartRotation == maxStartRotation)
                p.Rotation = minStartRotation + rotation;
            else
                p.Rotation = Mathf.Random() * (maxStartRotation - minStartRotation) +
                            minStartRotation + rotation;
            //place the particle at a random radius in the ring
            if (spawnCircle.minRadius != spawnCircle.radius)
            {
                helperPoint.X = Mathf.Random() * (spawnCircle.radius - spawnCircle.minRadius) +
                                spawnCircle.minRadius;
            }
            else
            {
                helperPoint.X = spawnCircle.radius;
            }

            helperPoint.Y = 0;
            //rotate the point to a random angle in the circle
            var angle = Mathf.Random() * 360;
            p.Rotation += angle;
            helperPoint = ParticleUtils.RotatePoint(angle, helperPoint);
            //offset by the circle's center
            helperPoint.X += spawnCircle.x;
            helperPoint.Y += spawnCircle.y;
            //rotate the point by the emitter's rotation
            if (rotation != 0)
                helperPoint = ParticleUtils.RotatePoint(rotation, helperPoint);
            //set the position, offset by the emitter's position
            p.X = emitPosX + helperPoint.X;
            p.Y = emitPosY + helperPoint.Y;
        }

        protected void SpawnBurst(Particle p, float emitPosX, float emitPosY, float i = -1f)
        {
            //set the initial rotation/direction of the particle based on spawn
            //angle and rotation of emitter
            if (particleSpacing == 0)
                p.Rotation = Mathf.Random() * 360;
            else
                p.Rotation = angleStart + (particleSpacing * i) + rotation;
            //drop the particle at the emitter's position
            p.X = emitPosX;
            p.Y = emitPosY;
        }

        /// <summary>
        /// Kills all active particles immediately
        /// </summary>
        public void Cleanup()
        {
            Particle particle, next;
            for (particle = activeParticlesFirst; particle != null; particle = next)
            {
                next = particle.next;
                Recycle(particle);
                if (particle.parent != null)
                    particle.parent.RemoveChild(particle);
            }
            activeParticlesFirst = activeParticlesLast = null;
            particleCount = 0;
        }

        /// <summary>
        /// Destroys the emitter and all of its particles.
        /// </summary>
        public void Destroy()
        {
            //puts all active particles in the pool, and removes them from the particle parent
            Cleanup();
            //wipe the pool clean
            Particle next;
            for (var particle = poolFirst; particle != null; particle = next)
            {
                //store next value so we don't lose it in our destroy call
                next = particle.next;
                particle.Destroy();
            }
            poolFirst = null;
            parent = null;
            particleImages = null;
            //this.spawnPos = null;
            //this.ownerPos = null;
            startColor = null;
            startScale = null;
            startAlpha = null;
            startSpeed = null;
            completeCallback = null;
        }
    }
}
