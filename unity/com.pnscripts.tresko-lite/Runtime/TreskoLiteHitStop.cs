using UnityEngine;

namespace PNScripts.TreskoLite
{
    /// <summary>
    /// Hit-stop: freezes or slows Time.timeScale for a short real-time duration and then puts
    /// back the scale the game had (your slow motion survives). Overlapping freezes extend,
    /// long chains are capped. One per scene is enough; Instance finds it.
    /// </summary>
    [AddComponentMenu("PN Scripts/Tresko Lite/Hit Stop")]
    [DisallowMultipleComponent]
    public sealed class TreskoLiteHitStop : MonoBehaviour
    {
        [Tooltip("Longest total freeze in one chain of hits (seconds of real time).")]
        public float maxTotal = 0.35f;
        [Tooltip("Pause after a capped chain before freezing again.")]
        public float cooldown = 0.1f;

        private readonly HitStop _stop = new HitStop();

        public static TreskoLiteHitStop Instance { get; private set; }
        public bool IsFrozen => _stop.IsActive;

        private void OnEnable()
        {
            if (Instance == null) Instance = this;
        }

        private void OnDisable()
        {
            if (_stop.IsActive) Time.timeScale = _stop.Release();
            if (Instance == this) Instance = null;
        }

        /// <summary>Freezes for seconds of real time; scale 0 stops, 0.1 slows to a crawl.</summary>
        public void Freeze(float seconds, float scale = 0f)
        {
            _stop.MaxTotal = maxTotal;
            _stop.Cooldown = cooldown;
            _stop.Freeze(seconds, scale, Time.timeScale);
            if (_stop.IsActive) Time.timeScale = scale;
        }

        public void Release()
        {
            if (_stop.IsActive) Time.timeScale = _stop.Release();
        }

        private void Update()
        {
            float before = Time.timeScale;
            float now = _stop.Step(Time.unscaledDeltaTime, before);
            if (!Mathf.Approximately(now, before)) Time.timeScale = now;
        }
    }
}
