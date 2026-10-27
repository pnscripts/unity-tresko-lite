using System;

namespace PNScripts.WhumpkitLite
{
    /// <summary>Offsets of one shake frame: position in units, rotation in degrees.</summary>
    public struct ShakeOffset
    {
        public float X, Y, Z, Pitch, Yaw, Roll;

        public static readonly ShakeOffset Zero = default;

        public float Magnitude => MathF.Sqrt(X * X + Y * Y + Z * Z);
    }

    /// <summary>
    /// Trauma-based camera shake (the model popularised by Squirrel Eiserloh's GDC talk):
    /// hits add trauma (0..1), trauma falls linearly over time, and the shake strength is
    /// trauma raised to a power, so small hits barely move the camera and big ones really do.
    /// Offsets come from six independent noise channels, so the motion is smooth and seeded.
    /// </summary>
    public sealed class Shake
    {
        private readonly Noise _noise;
        private float _time;

        /// <summary>0..1. Use AddTrauma; values are clamped.</summary>
        public float Trauma { get; private set; }

        /// <summary>Trauma lost per second.</summary>
        public float Decay = 1.4f;
        /// <summary>Shake strength = trauma ^ Power (2 or 3 feel right).</summary>
        public float Power = 2f;
        /// <summary>Noise speed: bumps per second.</summary>
        public float Frequency = 18f;
        /// <summary>Largest position offset at full trauma, per axis (units).</summary>
        public float MaxX = 0.5f, MaxY = 0.5f, MaxZ = 0f;
        /// <summary>Largest rotation at full trauma, per axis (degrees).</summary>
        public float MaxPitch = 0f, MaxYaw = 0f, MaxRoll = 3f;
        /// <summary>Multiplies everything (hook a "screen shake" accessibility setting here).</summary>
        public float Scale = 1f;

        public Shake(int seed = 1)
        {
            _noise = new Noise(seed);
        }

        public float Strength => MathF.Pow(Trauma, Power) * Scale;
        public bool IsShaking => Trauma > 0f;

        public void AddTrauma(float amount)
        {
            Trauma = Math.Clamp(Trauma + amount, 0f, 1f);
        }

        /// <summary>Sets trauma to at least this much (does not stack).</summary>
        public void SetTraumaAtLeast(float amount)
        {
            Trauma = Math.Clamp(Math.Max(Trauma, amount), 0f, 1f);
        }

        public void Stop()
        {
            Trauma = 0f;
        }

        /// <summary>Advances by dt seconds and returns this frame's offsets.</summary>
        public ShakeOffset Step(float dt)
        {
            if (dt < 0f) dt = 0f;
            _time += dt;
            ShakeOffset o = Evaluate();
            Trauma = Math.Max(0f, Trauma - Decay * dt);
            return o;
        }

        /// <summary>Offsets for the current time and trauma, without advancing.</summary>
        public ShakeOffset Evaluate()
        {
            float s = Strength;
            if (s <= 0f) return ShakeOffset.Zero;
            float t = _time * Frequency;
            return new ShakeOffset
            {
                X = MaxX * s * _noise.Sample(t),
                Y = MaxY * s * _noise.Sample(t + 101.3f),
                Z = MaxZ * s * _noise.Sample(t + 203.7f),
                Pitch = MaxPitch * s * _noise.Sample(t + 307.1f),
                Yaw = MaxYaw * s * _noise.Sample(t + 409.9f),
                Roll = MaxRoll * s * _noise.Sample(t + 511.3f),
            };
        }
    }
}
