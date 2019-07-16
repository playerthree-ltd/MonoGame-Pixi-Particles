using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;

namespace P.Particles
{
    public struct RandNumber
    {
        public float min;
        public float max;

        public RandNumber(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public struct BasicTweenable<T>
    {
        public T start;
        public T end;
    }

    public class EmitterConfig
    {
        public BasicTweenable<float>? alpha;
        public BasicTweenable<float>? speed;
        public float? minimumSpeedMultiplier;
        public float? maxSpeed;
        public Vector2? acceleration;
        public BasicTweenable<float>? scale;
        public float? minimumScaleMultiplier;
        public BasicTweenable<string>? color;
        public RandNumber? startRotation;
        public bool? noRotation;
        public RandNumber? rotationSpeed;
        public float? rotationAcceleration;
        public RandNumber lifetime;
        public string blendMode;
        public int? particlesPerWave;
        public string spawnType;
        public Rectangle? spawnRect;
        public Circle? spawnCircle;
        public float? particleSpacing;
        public float? angleStart;
        public float frequency;
        public float? spawnChance;
        public float? emitterLifetime;
        public int? maxParticles;
        public bool addAtBack;
        public Vector2 pos;
        public bool emit;

        public EmitterConfig(JToken configData)
        {
            var obj = (JObject)configData;

            alpha = new BasicTweenable<float>();
            if (alpha is BasicTweenable<float> bt0)
            {
                bt0.start = (float)configData["alpha"]["start"];
                bt0.end = (float)configData["alpha"]["end"];
                alpha = bt0;
            }
            speed = new BasicTweenable<float>();
            if (speed is BasicTweenable<float> bt1)
            {
                bt1.start = (float)configData["speed"]["start"];
                bt1.end = (float)configData["speed"]["end"];
                speed = bt1;
            }

            minimumSpeedMultiplier = (float)configData["speed"]["minimumSpeedMultiplier"];
            maxSpeed = (float)configData["maxSpeed"];

            acceleration = new Vector2((float)configData["acceleration"]["x"], (float)configData["acceleration"]["y"]);

            scale = new BasicTweenable<float>();
            if (scale is BasicTweenable<float> bt2)
            {
                bt2.start = (float)configData["scale"]["start"];
                bt2.end = (float)configData["scale"]["end"];
                scale = bt2;
            }
            minimumScaleMultiplier = (float)configData["scale"]["minimumScaleMultiplier"];

            color = new BasicTweenable<string>();
            if (color is BasicTweenable<string> bt3)
            {
                bt3.start = (string)configData["color"]["start"];
                bt3.end = (string)configData["color"]["end"];
                color = bt3;
            }

            startRotation = new RandNumber((float)configData["startRotation"]["min"], (float)configData["startRotation"]["max"]);

            noRotation = (bool)configData["noRotation"];

            rotationSpeed = new RandNumber((float)configData["rotationSpeed"]["min"], (float)configData["rotationSpeed"]["max"]);

            if (obj.ContainsKey("rotationAcceleration"))
            {
                rotationAcceleration = (float)configData["rotationAcceleration"];
            }

            lifetime = new RandNumber((float)configData["lifetime"]["min"], (float)configData["lifetime"]["max"]);

            blendMode = (string)configData["blendMode"];

            frequency = (float)configData["frequency"];
            emitterLifetime = (float)configData["emitterLifetime"];

            maxParticles = (int)configData["maxParticles"];

            pos = new Vector2((float)configData["pos"]["x"], (float)configData["pos"]["y"]);

            spawnType = (string)configData["spawnType"];

            if (spawnType == "rect")
            {
                spawnRect = new Rectangle(
                    (int)configData["spawnRect"]["x"],
                    (int)configData["spawnRect"]["y"],
                    (int)configData["spawnRect"]["w"],
                    (int)configData["spawnRect"]["h"]
                );
            }
            else if (spawnType == "circle")
            {
                spawnCircle = new Circle(
                    (int)configData["spawnCircle"]["x"],
                    (int)configData["spawnCircle"]["y"],
                    (int)configData["spawnCircle"]["r"]
                 );
            }
            else if (spawnType == "ring")
            {
                spawnCircle = new Circle(
                    (int)configData["spawnCircle"]["x"],
                    (int)configData["spawnCircle"]["y"],
                    (int)configData["spawnCircle"]["r"],
                    (int)configData["spawnCircle"]["minR"]
                );
            }
            else if (spawnType == "burst")
            {
                particlesPerWave = (int)configData["particlesPerWave"];
                particleSpacing = (int)configData["particleSpacing"];
                angleStart = (int)configData["angleStart"];
            }
        }
    }
}
