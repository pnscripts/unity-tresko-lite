using System;

namespace PNScripts.TreskoLite
{
    /// <summary>
    /// Hit-stop (freeze frames) as a time-scale clock driven by real time. Freeze(0.08) drops
    /// the time scale to zero for 80 ms of real time and then restores the scale the game had
    /// before, including your own slow motion. Overlapping freezes extend to the latest end and
    /// use the lowest scale; MaxTotal stops long chains of hits from freezing the game.
    /// </summary>
    public sealed class HitStop
    {
        /// <summary>Longest total freeze in one chain, seconds.</summary>
        public float MaxTotal = 0.35f;
        /// <summary>After a chain reached MaxTotal, new freezes wait this long (real seconds),
        /// so a constant stream of hits cannot freeze the game for good.</summary>
        public float Cooldown = 0.1f;

        private float _left;
        private float _chain;
        private float _scale = 1f;
        private float _sinceCappedEnd = float.MaxValue;
        private bool _capped;

        /// <summary>The scale to restore when the freeze ends.</summary>
        public float BaseScale { get; private set; } = 1f;
        public bool IsActive => _left > 1e-5f;
        public float Remaining => _left;

        /// <summary>Starts or extends a freeze. scale 0 = full stop, 0.1 = heavy slow motion.</summary>
        public void Freeze(float duration, float scale, float currentTimeScale)
        {
            if (duration <= 0f) return;
            if (!IsActive && _sinceCappedEnd < Cooldown) return;
            if (!IsActive)
            {
                BaseScale = currentTimeScale;
                _scale = Math.Clamp(scale, 0f, 1f);
                _chain = 0f;
                _left = 0f;
            }
            else
            {
                _scale = Math.Min(_scale, Math.Clamp(scale, 0f, 1f));
            }
            float allowed = Math.Max(0f, MaxTotal - _chain - _left);
            _left = Math.Max(_left, Math.Min(duration, _left + allowed));
            _capped = _chain + _left >= MaxTotal - 1e-6f;
        }

        /// <summary>
        /// Advances by real (unscaled) seconds; returns the time scale to use now. When the
        /// freeze ends it returns BaseScale once and then follows whatever the game sets.
        /// </summary>
        public float Step(float realDt, float currentTimeScale)
        {
            if (!IsActive)
            {
                if (_sinceCappedEnd < float.MaxValue) _sinceCappedEnd += realDt;
                return currentTimeScale;
            }
            float used = Math.Min(realDt, _left);
            _left -= used;
            _chain += used;
            if (IsActive) return _scale;
            _sinceCappedEnd = _capped ? realDt - used : float.MaxValue;
            return BaseScale;
        }

        /// <summary>Ends the freeze now; returns the scale to restore.</summary>
        public float Release()
        {
            _left = 0f;
            return BaseScale;
        }
    }
}
