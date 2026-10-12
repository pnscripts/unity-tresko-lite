using System;
using System.Linq;
using PNScripts.TreskoLite;
using Xunit;

namespace TreskoLite.Tests
{
    public class NoiseTests
    {
        [Fact]
        public void SameSeedSameValuesDifferentSeedDifferent()
        {
            var a = new Noise(7); var b = new Noise(7); var c = new Noise(8);
            var xs = Enumerable.Range(0, 200).Select(i => i * 0.137f).ToArray();
            Assert.Equal(xs.Select(a.Sample), xs.Select(b.Sample));
            Assert.NotEqual(xs.Select(a.Sample), xs.Select(c.Sample));
        }

        [Fact]
        public void BoundedSmoothAndZeroAtIntegers()
        {
            var n = new Noise(3);
            float prev = n.Sample(0f), maxStep = 0f, maxAbs = 0f;
            for (int i = 1; i < 5000; i++)
            {
                float v = n.Sample(i * 0.01f);
                maxAbs = Math.Max(maxAbs, Math.Abs(v));
                maxStep = Math.Max(maxStep, Math.Abs(v - prev));
                prev = v;
            }
            Assert.InRange(maxAbs, 0.3f, 1f);
            Assert.True(maxStep < 0.06f, $"step {maxStep}");
            for (int i = -5; i < 5; i++) Assert.Equal(0f, n.Sample(i), 5);
        }
    }

    public class ShakeTests
    {
        [Fact]
        public void TraumaClampsDecaysLinearlyAndStops()
        {
            var s = new Shake(1) { Decay = 1f };
            s.AddTrauma(0.7f); s.AddTrauma(0.7f);
            Assert.Equal(1f, s.Trauma);
            for (int i = 0; i < 30; i++) s.Step(1f / 60f);
            Assert.Equal(0.5f, s.Trauma, 3);
            for (int i = 0; i < 40; i++) s.Step(1f / 60f);
            Assert.Equal(0f, s.Trauma);
            Assert.Equal(0f, s.Step(1f / 60f).Magnitude);
        }

        [Fact]
        public void StrengthFollowsThePowerAndOffsetsStayInLimits()
        {
            var s = new Shake(4) { Power = 2f, MaxX = 0.5f, MaxY = 0.25f, MaxRoll = 3f, Decay = 0f };
            s.AddTrauma(0.5f);
            Assert.Equal(0.25f, s.Strength, 5);
            for (int i = 0; i < 600; i++)
            {
                var o = s.Step(1f / 60f);
                Assert.InRange(o.X, -0.125f, 0.125f);
                Assert.InRange(o.Y, -0.0625f, 0.0625f);
                Assert.InRange(o.Roll, -0.75f, 0.75f);
                Assert.Equal(0f, o.Z);
            }
        }

        [Fact]
        public void SameSeedReplaysExactly()
        {
            var a = new Shake(42); var b = new Shake(42);
            a.AddTrauma(0.9f); b.AddTrauma(0.9f);
            for (int i = 0; i < 90; i++)
            {
                var oa = a.Step(1f / 60f); var ob = b.Step(1f / 60f);
                Assert.Equal(oa.X, ob.X); Assert.Equal(oa.Roll, ob.Roll);
            }
        }

        [Fact]
        public void ScaleZeroTurnsShakeOff()
        {
            var s = new Shake(2) { Scale = 0f };
            s.AddTrauma(1f);
            Assert.Equal(0f, s.Step(0.016f).Magnitude);
        }
    }

    public class HitStopTests
    {
        [Fact]
        public void FreezesThenRestoresTheEarlierScale()
        {
            var h = new HitStop();
            h.Freeze(0.1f, 0f, 0.5f);
            Assert.Equal(0f, h.Step(0.05f, 0f));
            Assert.Equal(0.5f, h.Step(0.06f, 0f));
            Assert.False(h.IsActive);
            Assert.Equal(0.8f, h.Step(0.016f, 0.8f));
        }

        [Fact]
        public void OverlapsExtendAndUseTheLowestScale()
        {
            var h = new HitStop();
            h.Freeze(0.1f, 0.2f, 1f);
            h.Step(0.05f, 0.2f);
            h.Freeze(0.12f, 0f, 0.2f);
            Assert.Equal(0.12f, h.Remaining, 4);
            Assert.Equal(0f, h.Step(0.01f, 0f));
            Assert.Equal(1f, h.BaseScale);
        }

        [Fact]
        public void ChainsAreCappedAndCoolDown()
        {
            var h = new HitStop { MaxTotal = 0.2f, Cooldown = 0.1f };
            var frozen = new System.Collections.Generic.List<bool>();
            for (int i = 0; i < 40; i++)
            {
                h.Freeze(0.08f, 0f, 1f);
                frozen.Add(h.IsActive);
                h.Step(0.02f, 0f);
            }
            int firstRun = frozen.TakeWhile(f => f).Count();
            Assert.Equal(10, firstRun);
            int gap = frozen.Skip(firstRun).TakeWhile(f => !f).Count();
            Assert.True(gap >= 5, $"gap {gap} steps");
            Assert.True(frozen.Skip(firstRun + gap).Any(f => f), "freezes again after the cooldown");
            Assert.Equal(1f, h.Release());
        }
    }
}
