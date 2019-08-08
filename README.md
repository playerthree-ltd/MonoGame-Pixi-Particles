# MonoGame Pixi Particles

A particle system for the [MonoGame](https://github.com/MonoGame/MonoGame) library, ported to C# from [Pixi-Particles](https://github.com/pixijs/pixi-particles)

Original system made by [CloudKid](http://github.com/cloudkidstudio)

Also, see the [interactive particle editor](http://pixijs.github.io/pixi-particles-editor/) to design and preview custom particle emitters.

![Alt text](particle_screenshot.png?raw=true "Fire Example")

## Basic Usage

In load

```C#
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
```

In draw

```C#
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
```

## Documentation

Original docs should be fine for reference, just use PascalCase instead of camelCase where appropriate. 

http://pixijs.github.io/pixi-particles/docs/

## License

Copyright (c) 2019 [Playerthree](http://github.com/playerthree-ltd)

Released under the MIT License.
