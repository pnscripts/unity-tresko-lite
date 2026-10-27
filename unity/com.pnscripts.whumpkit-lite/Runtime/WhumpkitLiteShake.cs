using UnityEngine;

namespace PNScripts.WhumpkitLite
{
    /// <summary>
    /// Trauma camera shake for any Transform (usually the camera, or a child "shaker" object
    /// under a camera rig). The offset is added in LateUpdate and taken back the next frame, so
    /// your camera controller, Cinemachine or animation keeps full control of the real position.
    /// Runs on unscaled time by default, so it keeps shaking during hit-stop.
    /// </summary>
    [AddComponentMenu("PN Scripts/Whumpkit Lite/Shake")]
    [DisallowMultipleComponent]
    public sealed class WhumpkitLiteShake : MonoBehaviour
    {
        [Tooltip("Same seed, same motion.")]
        public int seed = 1;
        [Tooltip("Trauma lost per second.")]
        public float decay = 1.4f;
        [Tooltip("Shake strength = trauma ^ power.")]
        public float power = 2f;
        [Tooltip("Noise bumps per second.")]
        public float frequency = 18f;
        [Tooltip("Largest local position offset at full trauma (units).")]
        public Vector3 maxOffset = new Vector3(0.4f, 0.4f, 0f);
        [Tooltip("Largest local rotation at full trauma (degrees): pitch, yaw, roll.")]
        public Vector3 maxRotation = new Vector3(0f, 0f, 2.5f);
        [Tooltip("Multiplier for an accessibility setting (0 turns shake off).")]
        [Range(0f, 1f)] public float intensity = 1f;
        public bool useUnscaledTime = true;

        private Shake _shake;
        private Vector3 _appliedPos;
        private Quaternion _appliedRot = Quaternion.identity;

        public float Trauma => _shake != null ? _shake.Trauma : 0f;

        private void Awake() => Build();

        private void Build()
        {
            _shake = new Shake(seed);
            Sync();
        }

        private void Sync()
        {
            _shake.Decay = decay;
            _shake.Power = power;
            _shake.Frequency = frequency;
            _shake.MaxX = maxOffset.x; _shake.MaxY = maxOffset.y; _shake.MaxZ = maxOffset.z;
            _shake.MaxPitch = maxRotation.x; _shake.MaxYaw = maxRotation.y; _shake.MaxRoll = maxRotation.z;
            _shake.Scale = intensity;
        }

        /// <summary>Adds trauma (0..1). 0.2 for a light hit, 0.5 for a heavy one, 1 for an explosion.</summary>
        public void AddTrauma(float amount)
        {
            if (_shake == null) Build();
            _shake.AddTrauma(amount);
        }

        public void Stop()
        {
            if (_shake != null) _shake.Stop();
            Remove();
        }

        private void LateUpdate()
        {
            Remove();
            if (_shake == null) return;
            Sync();
            float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            ShakeOffset o = _shake.Step(dt);
            if (o.Magnitude == 0f && o.Pitch == 0f && o.Yaw == 0f && o.Roll == 0f) return;
            _appliedPos = new Vector3(o.X, o.Y, o.Z);
            _appliedRot = Quaternion.Euler(o.Pitch, o.Yaw, o.Roll);
            transform.localPosition += _appliedPos;
            transform.localRotation = transform.localRotation * _appliedRot;
        }

        private void Remove()
        {
            if (_appliedPos != Vector3.zero) transform.localPosition -= _appliedPos;
            if (_appliedRot != Quaternion.identity) transform.localRotation = transform.localRotation * Quaternion.Inverse(_appliedRot);
            _appliedPos = Vector3.zero;
            _appliedRot = Quaternion.identity;
        }

        private void OnDisable() => Remove();
    }
}
