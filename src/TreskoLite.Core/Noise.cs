using System;

namespace PNScripts.TreskoLite
{
    /// <summary>
    /// Smooth, seeded 1D gradient noise in -1..1 (Perlin style, quintic fade). The same seed
    /// always gives the same values, on every platform, so shakes can be replayed and tested.
    /// </summary>
    public sealed class Noise
    {
        private readonly float[] _gradients = new float[256];
        private readonly int[] _perm = new int[512];

        public int Seed { get; }

        public Noise(int seed)
        {
            Seed = seed;
            var rng = new SplitMix(unchecked((ulong)seed * 0x9E3779B97F4A7C15UL + 0x1234567UL));
            var p = new int[256];
            for (int i = 0; i < 256; i++)
            {
                p[i] = i;
                _gradients[i] = (float)(rng.NextDouble() * 2.0 - 1.0);
            }
            for (int i = 255; i > 0; i--)
            {
                int j = (int)(rng.NextULong() % (ulong)(i + 1));
                (p[i], p[j]) = (p[j], p[i]);
            }
            for (int i = 0; i < 512; i++) _perm[i] = p[i & 255];
        }

        /// <summary>Noise at x; about one bump per unit. Result is in -1..1.</summary>
        public float Sample(float x)
        {
            int xi = (int)MathF.Floor(x);
            float f = x - xi;
            float g0 = _gradients[_perm[xi & 255]];
            float g1 = _gradients[_perm[(xi + 1) & 255]];
            float v0 = g0 * f;
            float v1 = g1 * (f - 1f);
            float t = f * f * f * (f * (f * 6f - 15f) + 10f);
            // 1D gradient noise peaks at 0.5 in magnitude; scale to fill -1..1.
            return Math.Clamp((v0 + (v1 - v0) * t) * 2f, -1f, 1f);
        }

        private struct SplitMix
        {
            private ulong _s;
            public SplitMix(ulong s) { _s = s; }

            public ulong NextULong()
            {
                ulong z = (_s += 0x9E3779B97F4A7C15UL);
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return z ^ (z >> 31);
            }

            public double NextDouble() => (NextULong() >> 11) * (1.0 / 9007199254740992.0);
        }
    }
}
