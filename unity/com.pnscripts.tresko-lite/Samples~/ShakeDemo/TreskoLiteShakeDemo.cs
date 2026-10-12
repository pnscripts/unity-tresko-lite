using UnityEngine;

namespace PNScripts.TreskoLite.Samples
{
    /// <summary>
    /// Shake Demo for Tresko Lite: add this component to an empty GameObject in an empty scene and
    /// press Play. It makes a camera with a shaker child, a hit-stop and a spinning cube, hits on a
    /// timer and has on-screen buttons, so it needs no input system.
    /// </summary>
    public sealed class TreskoLiteShakeDemo : MonoBehaviour
    {
        public float hitEvery = 1.2f;

        private TreskoLiteShake _shake;
        private TreskoLiteHitStop _stop;
        private Transform _cube;
        private float _next;
        private bool _heavy;

        private void Start()
        {
            var rig = new GameObject("Camera Rig").transform;
            rig.position = new Vector3(0f, 1.5f, -6f);
            var cam = new GameObject("Main Camera", typeof(Camera)).GetComponent<Camera>();
            cam.tag = "MainCamera";
            cam.transform.SetParent(rig, false);
            cam.transform.localRotation = Quaternion.Euler(8f, 0f, 0f);
            _shake = cam.gameObject.AddComponent<TreskoLiteShake>();
            _stop = gameObject.AddComponent<TreskoLiteHitStop>();
            new GameObject("Light", typeof(Light)).GetComponent<Light>().type = LightType.Directional;
            _cube = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            _cube.position = new Vector3(0f, 1f, 0f);
            _next = Time.unscaledTime + hitEvery;
        }

        private void Update()
        {
            _cube.Rotate(0f, 90f * Time.deltaTime, 0f);
            if (Time.unscaledTime >= _next)
            {
                Hit(_heavy);
                _heavy = !_heavy;
                _next = Time.unscaledTime + hitEvery;
            }
        }

        private void Hit(bool heavy)
        {
            _shake.AddTrauma(heavy ? 0.7f : 0.3f);
            _stop.Freeze(heavy ? 0.12f : 0.05f);
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(16, 16, 260, 140));
            GUILayout.Label($"Trauma {_shake.Trauma:0.00}   time scale {Time.timeScale:0.00}");
            if (GUILayout.Button("Light hit")) Hit(false);
            if (GUILayout.Button("Heavy hit")) Hit(true);
            GUILayout.EndArea();
        }
    }
}
