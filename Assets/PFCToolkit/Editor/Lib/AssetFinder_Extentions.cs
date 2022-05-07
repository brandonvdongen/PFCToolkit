using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Avatars.Components;
#endif

namespace PFCToolkit.Lib
{
    public static class AssetFinder_Extentions
    {
        #region GetRenderers
        public static HashSet<Renderer> GetRenderers(this GameObject gameObject)
        {
            return new HashSet<Renderer>(gameObject.GetComponentsInChildren<Renderer>(true));
        }
        #endregion           

        #region GetMaterials
        //Renderer
        public static HashSet<Material> GetMaterials(this HashSet<Renderer> renderers)
        {
            HashSet<Material> materials = new HashSet<Material>();
            foreach (Renderer r in renderers)
            {
                foreach (Material material in r.sharedMaterials)
                {
                    materials.Add(material);
                }
            }
            return materials;
        }
        public static HashSet<Material> GetMaterials(this Renderer renderer) => GetMaterials(new HashSet<Renderer> { renderer });

        //AnimationClip
        public static HashSet<Material> GetMaterials(this HashSet<AnimationClip> clip)
        {
            HashSet<Material> materials = new HashSet<Material>();
            foreach (AnimationClip _clip in clip)
            {
                EditorCurveBinding[] bindings = AnimationUtility.GetObjectReferenceCurveBindings(_clip);
                foreach (EditorCurveBinding binding in bindings)
                {
                    ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(_clip, binding);

                    foreach (ObjectReferenceKeyframe keyframe in keyframes)
                    {
                        materials.Add(keyframe.value as Material);
                    }

                }
            }
            return materials;
        }
        public static HashSet<Material> GetMaterials(this AnimationClip clip) => GetMaterials(new HashSet<AnimationClip> { clip });

        #endregion

        #region GetTextures
        public static HashSet<Texture> GetTextures(this HashSet<Material> materials)
        {

            HashSet<Renderer> renderers = new HashSet<Renderer>();
            HashSet<Texture> textures = new HashSet<Texture>();
            foreach (Material material in materials)
            {
                foreach (string name in material.GetTexturePropertyNames())
                {
                    Texture texture = material.GetTexture(name);
                    if (texture != null)
                    {
                        textures.Add(texture);
                    }
                }
            }
            return textures;
        }
        public static HashSet<Texture> GetTextures(this Material material) => GetTextures(new HashSet<Material>() { material });
        #endregion

        #region GetControllers
        public static HashSet<AnimatorController> GetAnimatorControllers(this GameObject go)
        {
            HashSet<AnimatorController> controllers = new HashSet<AnimatorController>();
            HashSet<Animator> animators = new HashSet<Animator>(go.GetComponentsInChildren<Animator>());
            foreach (Animator animator in animators)
            {
                if (animator.runtimeAnimatorController != null)
                {
                    controllers.Add(animator.runtimeAnimatorController as AnimatorController);
                }
            }

#if VRC_SDK_VRCSDK3
            HashSet<VRCAvatarDescriptor> descriptors = new HashSet<VRCAvatarDescriptor>(go.GetComponentsInChildren<VRCAvatarDescriptor>());
            foreach (VRCAvatarDescriptor descriptor in descriptors)
            {
                foreach (VRCAvatarDescriptor.CustomAnimLayer layer in descriptor.baseAnimationLayers)
                {
                    if (layer.animatorController != null)
                    {
                        controllers.Add(layer.animatorController as AnimatorController);
                    }
                }
                foreach (VRCAvatarDescriptor.CustomAnimLayer layer in descriptor.specialAnimationLayers)
                {
                    if (layer.animatorController != null)
                    {
                        controllers.Add(layer.animatorController as AnimatorController);
                    }
                }
            }
#endif

            return controllers;
        }
        #endregion

        #region GetControllerLayers
        public static HashSet<AnimatorControllerLayer> GetControllerLayers(this HashSet<AnimatorController> controllers)
        {
            HashSet<AnimatorControllerLayer> layers = new HashSet<AnimatorControllerLayer>();
            foreach (AnimatorController controller in controllers)
            {
                layers.UnionWith(controller.layers);
            }
            return layers;
        }
        public static HashSet<AnimatorControllerLayer> GetControllerLayers(this AnimatorController controller) => GetControllerLayers(new HashSet<AnimatorController>() { controller });

        #endregion

        #region GetStateMachines
        public static HashSet<AnimatorStateMachine> GetStateMachines(this HashSet<AnimatorControllerLayer> layers)
        {
            HashSet<AnimatorStateMachine> stateMachines = new HashSet<AnimatorStateMachine>();
            foreach (AnimatorControllerLayer layer in layers)
            {
                stateMachines.Add(layer.stateMachine);
                foreach (ChildAnimatorStateMachine child in layer.stateMachine.stateMachines)
                {
                    stateMachines.Add(child.stateMachine);
                }

            }
            return stateMachines;
        }
        public static HashSet<AnimatorStateMachine> GetStateMachines(this AnimatorControllerLayer Layer) => GetStateMachines(new HashSet<AnimatorControllerLayer>() { Layer });

        #endregion

        #region GetChildStateMachines
        public static HashSet<AnimatorStateMachine> GetChildStateMachines(this HashSet<AnimatorStateMachine> statemachines)
        {
            HashSet<AnimatorStateMachine> childStateMachines = new HashSet<AnimatorStateMachine>();
            foreach (AnimatorStateMachine statemachine in statemachines)
            {
                foreach (ChildAnimatorStateMachine child in statemachine.stateMachines)
                {
                    childStateMachines.Add(child.stateMachine);
                }
            }
            return statemachines;
        }
        public static HashSet<AnimatorStateMachine> GetChildStateMachines(this AnimatorStateMachine statemachine) => GetChildStateMachines(new HashSet<AnimatorStateMachine>() { statemachine });

        #endregion

        #region GetAllStateMachines
        //AnimatorStateMachine
        public static HashSet<AnimatorStateMachine> GetAllStateMachines(this HashSet<AnimatorStateMachine> stateMachines)
        {
            HashSet<AnimatorStateMachine> ChildStateMachines = new HashSet<AnimatorStateMachine>();

            foreach (AnimatorStateMachine stateMachine in stateMachines)
            {
                foreach (ChildAnimatorStateMachine child in stateMachine.stateMachines)
                {
                    ChildStateMachines.Add(child.stateMachine);
                    ChildStateMachines.UnionWith(child.stateMachine.GetAllStateMachines());
                }
            }

            return ChildStateMachines;
        }
        public static HashSet<AnimatorStateMachine> GetAllStateMachines(this AnimatorStateMachine stateMachine) => GetAllStateMachines(new HashSet<AnimatorStateMachine>() { stateMachine });

        //AnimatorControllerLayer
        public static HashSet<AnimatorStateMachine> GetAllStateMachines(this HashSet<AnimatorControllerLayer> layers)
        {
            HashSet<AnimatorStateMachine> StateMachines = new HashSet<AnimatorStateMachine>();

            foreach (AnimatorControllerLayer layer in layers)
            {
                StateMachines.Add(layer.stateMachine);
                StateMachines.UnionWith(layer.stateMachine.GetAllStateMachines());
            }

            return StateMachines;
        }
        public static HashSet<AnimatorStateMachine> GetAllStateMachines(this AnimatorControllerLayer layer) => GetAllStateMachines(new HashSet<AnimatorControllerLayer>() { layer });
        #endregion

        #region GetAnimatorStates
        public static HashSet<AnimatorState> GetAnimatorStates(this HashSet<AnimatorStateMachine> stateMachines)
        {
            HashSet<AnimatorState> AnimatorStates = new HashSet<AnimatorState>();
            foreach (AnimatorStateMachine stateMachine in stateMachines)
            {
                foreach (ChildAnimatorState ChildAnimatorState in stateMachine.states)
                {
                    AnimatorStates.Add(ChildAnimatorState.state);
                }
            }
            return AnimatorStates;
        }
        public static HashSet<AnimatorState> GetAnimatorStates(this AnimatorStateMachine stateMachine) => GetAnimatorStates(new HashSet<AnimatorStateMachine>() { stateMachine });
        #endregion

        #region GetMotions
        //AnimatorState
        public static HashSet<Motion> GetMotion(this HashSet<AnimatorState> states)
        {
            HashSet<Motion> Motions = new HashSet<Motion>();
            foreach (AnimatorState state in states)
            {
                if (state.motion)
                {
                    Motions.Add(state.motion);
                }
            }
            return Motions;
        }
        public static HashSet<Motion> GetMotion(this AnimatorState stateMachine) => GetMotion(new HashSet<AnimatorState>() { stateMachine });

        //Blend Tree
        public static HashSet<Motion> GetMotion(this HashSet<BlendTree> trees)
        {
            HashSet<Motion> Motions = new HashSet<Motion>();
            foreach (BlendTree tree in trees)
            {
                foreach (ChildMotion childMotion in tree.children)
                {
                    if (childMotion.motion)
                    {
                        Motions.Add(childMotion.motion);
                    }
                }
            }
            return Motions;
        }
        public static HashSet<Motion> GetMotion(this BlendTree tree) => GetMotion(new HashSet<BlendTree> { tree });
        #endregion

        #region GetAllMotion
        public static HashSet<Motion> GetAllMotion(this HashSet<AnimatorState> states)
        {
            HashSet<Motion> Motions = new HashSet<Motion>();
            foreach (AnimatorState state in states)
            {

                if (state.motion)
                {
                    Motions.Add(state.motion);
                }

                if (state.motion is BlendTree)
                {
                    BlendTree tree = state.motion as BlendTree;
                    Motions.UnionWith(tree.GetAllMotion());
                }
            }
            return Motions;
        }
        public static HashSet<Motion> GetAllMotion(this AnimatorState stateMachine) => GetAllMotion(new HashSet<AnimatorState>() { stateMachine });

        //Blend Tree
        public static HashSet<Motion> GetAllMotion(this HashSet<BlendTree> trees)
        {
            HashSet<Motion> Motions = new HashSet<Motion>();
            foreach (BlendTree tree in trees)
            {
                foreach (Motion motion in tree.GetMotion())
                {
                    if (motion)
                    {
                        Motions.Add(motion);
                    }

                    if (motion is BlendTree)
                    {
                        BlendTree t = motion as BlendTree;
                        Motions.UnionWith(t.GetAllMotion());
                    }
                }

            }
            return Motions;
        }
        public static HashSet<Motion> GetAllMotion(this BlendTree tree) => GetAllMotion(new HashSet<BlendTree> { tree });
        #endregion

        #region GetAllAnimationClips
        //AnimatorStates
        public static HashSet<AnimationClip> GetAllAnimationClips(this HashSet<AnimatorState> states)
        {
            HashSet<AnimationClip> AnimationClips = new HashSet<AnimationClip>();
            foreach (Motion motion in states.GetAllMotion())
            {
                if (motion is AnimationClip)
                {
                    AnimationClips.Add(motion as AnimationClip);
                }
            }
            return AnimationClips;
        }
        public static HashSet<AnimationClip> GetAllAnimationClips(this AnimatorState state) => GetAllAnimationClips(new HashSet<AnimatorState>() { state });

        //Motions
        public static HashSet<AnimationClip> GetAllAnimationClips(this HashSet<Motion> motions)
        {
            HashSet<AnimationClip> AnimationClips = new HashSet<AnimationClip>();
            foreach (Motion motion in motions)
            {
                if (motion is AnimationClip)
                {
                    AnimationClips.Add(motion as AnimationClip);
                }
            }
            return AnimationClips;
        }
        public static HashSet<AnimationClip> GetAllAnimationClips(this Motion motion) => GetAllAnimationClips(new HashSet<Motion>() { motion });

        //AnimatorController
        public static HashSet<AnimationClip> GetAllAnimationClips(this HashSet<AnimatorController> controllers)
        {
            HashSet<AnimationClip> AnimationClips = new HashSet<AnimationClip>();
            foreach (AnimatorController controller in controllers)
            {
                AnimationClips.UnionWith(controller.animationClips);
            }
            return AnimationClips;
        }
        public static HashSet<AnimationClip> GetAllAnimationClips(this AnimatorController controller) => GetAllAnimationClips(new HashSet<AnimatorController>() { controller });


        #endregion
    }
}