using PFCToolkit.Base;
using UnityEditor;
using UnityEngine;

namespace PFCToolkit.DrawingTools {
    [ExecuteInEditMode]
    public static class PFCDrawTools {

        public static BoolPreferenceHandler CapMaxSize = new BoolPreferenceHandler("Cap Max Handle Size", "DrawingTools.CapMaxSize");
        public static FloatPreferenceHandler BoneThickness = new FloatPreferenceHandler("Bone Thickness", "DrawingTools.BoneThickness");




        public static void DrawBone(Vector3 Start, Vector3 End, Color color, bool endBone) {

            Vector3 direction = (End - Start).normalized;

            Vector3 midpoint = Vector3.Lerp(Start, End, .25f);

            Quaternion Rotation = Quaternion.FromToRotation(Vector3.forward, direction);

            float HeadSize = Vector3.Distance(Start, End) * BoneThickness.cachedValue / 3;
            float TailSize = Vector3.Distance(Start, End) * BoneThickness.cachedValue / 4;

            Vector3 v1 = midpoint + Rotation * (Vector3.up + Vector3.right) * HeadSize;
            Vector3 v2 = midpoint + Rotation * (Vector3.down + Vector3.right) * HeadSize;
            Vector3 v3 = midpoint + Rotation * (Vector3.down + Vector3.left) * HeadSize;
            Vector3 v4 = midpoint + Rotation * (Vector3.up + Vector3.left) * HeadSize;

            Handles.color = color;
            Handles.DrawPolyLine(new Vector3[] { v1, v2, v3, v4, v1, Start, v2, End, v3, Start, v4, End, v1 });

            if (CapMaxSize.cachedValue == true) {
                Handles.CubeHandleCap(0, Start, Rotation, Mathf.Min(HeadSize, 0.1f), EventType.Repaint);
                if (endBone) Handles.CubeHandleCap(0, End, Rotation, Mathf.Min(TailSize, 0.1f), EventType.Repaint);
            } else {
                Handles.CubeHandleCap(0, Start, Rotation, HeadSize, EventType.Repaint);
                if (endBone) Handles.CubeHandleCap(0, End, Rotation, TailSize, EventType.Repaint);
            }

        }

        public static void GenerateSkeletalMesh(BoneMeshData[] BoneData) {

        }
        public struct BoneMeshData {
            public Vector3 Start;
            public Vector3 End;
            public Color color;
        }

        public static void DrawEmptyLine(Vector3 Start, Vector3 End, Color color, bool endBone) {
            Handles.color = color;
            Handles.DrawDottedLine(Start, End, 1f);
            Handles.CubeHandleCap(0, Start, Quaternion.identity, 0.005f, EventType.Repaint);
            if (endBone) Handles.CubeHandleCap(0, End, Quaternion.identity, 0.004f, EventType.Repaint);
        }
    }
}