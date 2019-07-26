using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using P.Particles;
using System.IO;

namespace MonoGame_PIXI_Particles
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Container particleContainer;
        Texture2D particleTexture1;
        Texture2D particleTexture2;
        Emitter particleEmitter;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            particleTexture1 = Content.Load<Texture2D>("particle");
            particleTexture2 = Content.Load<Texture2D>("fire");

            particleContainer = new Container();

            var config = new EmitterConfig(LoadJSON("./Content/emitter.json"));

            particleEmitter = new Emitter(particleContainer, new TextureRegion2D[] {
                new TextureRegion2D(particleTexture1),
                new TextureRegion2D(particleTexture2)},
                config);

            var x = Window.ClientBounds.Width * 0.5f;
            var y = Window.ClientBounds.Height - 20f;

            particleEmitter.UpdateOwnerPos(x, y);
            particleEmitter.Emit = true;
        }

        protected JToken LoadJSON(string uri)
        {
            // read JSON directly from a file
            JToken json;
            using (StreamReader file = File.OpenText(uri))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                json = JToken.ReadFrom(reader);
            }

            return json;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            particleEmitter.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            var particles = particleContainer.Children;

            spriteBatch.Begin();
            for (int i = 0; i < particles.Count; i++)
            {
                var particle = (Sprite)particles[i];

                if (GraphicsDevice.BlendState != particle.Material.blendState)
                    GraphicsDevice.BlendState = particle.Material.blendState;

                Vector2 origin = new Vector2(particle.TextureRegion.Size.X * particle.Anchor.X - particle.TextureRegion.Trim.X,
                    particle.TextureRegion.Size.Y * particle.Anchor.Y - particle.TextureRegion.Trim.Y);

                spriteBatch.Draw(
                   particle.TextureRegion.BaseTexture,
                   particle.Position,
                   particle.TextureRegion.Frame,
                   particle.Tint * (particle.Alpha * particleContainer.Alpha),
                   particle.Rotation,
                   origin,
                   particle.Scale,
                   SpriteEffects.None,
                   0
                );
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
