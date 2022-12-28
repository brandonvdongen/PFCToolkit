using PFCToolkit.DrawingTools;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PFCToolkit.Base {
    internal class HierarchyVisualizer {
        public static BoolPreferenceHandler SlowMode = new BoolPreferenceHandler("Preview Visualization (slow mode)", "ObjectVisualizer.SlowMode", false);

        public bool isWorking = false;
        public static int vertI = 0;
        private readonly Dictionary<Transform, IHandleRenderer> BoneHandles = new Dictionary<Transform, IHandleRenderer>();
        private readonly Dictionary<Transform, IHandleRenderer> EndBoneHandles = new Dictionary<Transform, IHandleRenderer>();
        private readonly Dictionary<Transform, IHandleRenderer> EmptyHandles = new Dictionary<Transform, IHandleRenderer>();
        private Dictionary<Transform, IHandleRenderer> EndEmptyHandles = new Dictionary<Transform, IHandleRenderer>();
        private readonly HashSet<Transform> AnimationBones = new HashSet<Transform>();
        private readonly HashSet<Transform> HumanoidBones = new HashSet<Transform>();

        public async void VisualizeHierarchy(GameObject target) {
            isWorking = true;
            BoneHandles.Clear();
            EndBoneHandles.Clear();
            EmptyHandles.Clear();
            EndEmptyHandles.Clear();
            AnimationBones.Clear();
            HumanoidBones.Clear();

            GetHumanoidBones(target, HumanoidBones);
            GetAnimationBones(target, AnimationBones);

            foreach (Transform transform in target.GetComponentsInChildren<Transform>()) {
                if (!isWorking) break;
                if (transform.parent == null || transform.parent == target.transform) continue;
                string Path = AnimationUtility.CalculateTransformPath(transform, target.transform);
                bool isHumanoid = HumanoidBones.Contains(transform);
                bool isAnimationBone = AnimationBones.Contains(transform);

                if ((isHumanoid || isAnimationBone) && !BoneHandles.ContainsKey(transform) && (HumanoidBones.Contains(transform.parent) || AnimationBones.Contains(transform.parent))) {
                    BoneHandles.Add(transform, new HandleTransform() { startTransform = transform.parent, endTransform = transform, color = isHumanoid ? Color.cyan : Color.gray });
                    if (transform.childCount <= 0 && !EndBoneHandles.ContainsKey(transform)) EndBoneHandles.Add(transform, new HandleTransformRay() { startTransform = transform, Direction = Vector3.up, color = Color.blue });

                } else if (!EmptyHandles.ContainsKey(transform)) {
                    EmptyHandles.Add(transform, new HandleTransform() { startTransform = transform.parent, endTransform = transform, color = Color.gray });
                    if (transform.childCount <= 0 && !EndEmptyHandles.ContainsKey(transform)) EndEmptyHandles.Add(transform, new HandleTransformRay() { startTransform = transform, Direction = Vector3.up, color = Color.blue });
                }
                if (SlowMode.cachedValue) {
                    await Task.Yield();
                    SceneView.lastActiveSceneView.Repaint();
                }
            }
            SceneView.lastActiveSceneView.Repaint();
        }

        private static void GetAnimationBones(GameObject target, HashSet<Transform> AnimationBones) {
            foreach (SkinnedMeshRenderer renderer in target.GetComponentsInChildren<SkinnedMeshRenderer>()) {
                if (renderer != null) {
                    foreach (Transform bone in renderer.bones) {
                        AnimationBones.Add(bone);
                    }
                }
            }
        }

        public static void DrawTest(GameObject target) {
            SkinnedMeshRenderer renderer = target.GetComponentInChildren<SkinnedMeshRenderer>();
            Vector3 vA = renderer.sharedMesh.vertices[vertI];
            Vector3 vB = renderer.sharedMesh.vertices[0];
            vertI++;
            if (vertI + 1 < renderer.sharedMesh.vertices.Length) vB = renderer.sharedMesh.vertices[vertI + 1];
            else vertI = 0;
            Debug.DrawLine(vA, vB, Color.yellow);
            SceneView.lastActiveSceneView.Repaint();
        }

        private static void GetHumanoidBones(GameObject target, HashSet<Transform> HumanoidBones) {
            foreach (Animator animator in target.GetComponentsInChildren<Animator>()) {
                foreach (HumanBodyBones HumanBone in Enum.GetValues(typeof(HumanBodyBones))) {
                    if (HumanBone is HumanBodyBones.LastBone) continue;
                    Transform bone = animator.GetBoneTransform(HumanBone);
                    if (bone != null) {
                        HumanoidBones.Add(bone);
                    }
                }
            }
        }

        public void ClearVisualization() {
            isWorking = false;
            BoneHandles.Clear();
            EmptyHandles.Clear();
            EndBoneHandles.Clear();
            EndEmptyHandles.Clear();
            SceneView.RepaintAll();
        }
        public void DrawHandles(SceneView _SceneView) {
            foreach (KeyValuePair<Transform, IHandleRenderer> entry in BoneHandles) {
                if (entry.Key == null) continue;
                IHandleRenderer boneHandle = entry.Value;
                PFCDrawTools.DrawBone(boneHandle.start, boneHandle.end, boneHandle.color, false);
            }
            foreach (KeyValuePair<Transform, IHandleRenderer> entry in EndBoneHandles) {
                if (entry.Key == null) continue;
                IHandleRenderer boneHandle = entry.Value;
                PFCDrawTools.DrawBone(boneHandle.start, boneHandle.end, boneHandle.color, true);

            }

            foreach (KeyValuePair<Transform, IHandleRenderer> entry in EmptyHandles) {
                if (entry.Key == null) continue;
                IHandleRenderer boneHandle = entry.Value;
                PFCDrawTools.DrawEmptyLine(boneHandle.start, boneHandle.end, boneHandle.color, false);
            }
            foreach (KeyValuePair<Transform, IHandleRenderer> entry in EndEmptyHandles) {
                if (entry.Key == null) continue;
                IHandleRenderer boneHandle = entry.Value;
                PFCDrawTools.DrawEmptyLine(boneHandle.start, boneHandle.end, boneHandle.color, true);
            }
        }
    }
}