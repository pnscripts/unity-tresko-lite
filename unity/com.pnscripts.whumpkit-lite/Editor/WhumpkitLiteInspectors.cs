using UnityEditor;
using UnityEngine;

namespace PNScripts.WhumpkitLite.Editor
{
    /// <summary>Try buttons in Play mode, so shake and hit-stop can be tuned while the game runs.</summary>
    [CustomEditor(typeof(WhumpkitLiteShake))]
    public sealed class WhumpkitLiteShakeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var shake = (WhumpkitLiteShake)target;
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Trauma", shake.Trauma.ToString("0.00"));
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Light (0.25)")) shake.AddTrauma(0.25f);
                if (GUILayout.Button("Heavy (0.6)")) shake.AddTrauma(0.6f);
                if (GUILayout.Button("Max")) shake.AddTrauma(1f);
                EditorGUILayout.EndHorizontal();
            }
            if (Application.isPlaying) Repaint();
        }
    }

    [CustomEditor(typeof(WhumpkitLiteHitStop))]
    public sealed class WhumpkitLiteHitStopEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var stop = (WhumpkitLiteHitStop)target;
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Freeze 0.05 s")) stop.Freeze(0.05f);
                if (GUILayout.Button("Freeze 0.12 s")) stop.Freeze(0.12f);
                if (GUILayout.Button("Slow 0.3 s")) stop.Freeze(0.3f, 0.2f);
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
