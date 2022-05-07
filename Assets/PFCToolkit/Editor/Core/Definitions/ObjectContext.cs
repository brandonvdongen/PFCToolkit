using PFCToolkit.Lib;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace PFCToolkit.Core
{
    public class ObjectContext
    {

        public bool isValid = true;
        public GameObject Root;
        public Animator Animator;
        public HashSet<AnimationClip> Animations { get { return Root.GetAnimatorControllers().GetAllAnimationClips(); } }
        public HashSet<Material> Materials
        {
            get
            {
                HashSet<Material> materials = new HashSet<Material>();
                materials.UnionWith(Root.GetRenderers().GetMaterials());
                materials.UnionWith(Root.GetAnimatorControllers().GetAllAnimationClips().GetMaterials());
                return materials;
            }
        }
        public HashSet<Renderer> Renderers { get { return Root.GetRenderers(); } }



#if VRC_SDK_VRCSDK3
        public VRCAvatarDescriptor Descriptor;
        public bool isAvatar { get { return Descriptor != null; } }
#endif

        public List<LogNote> notes = new List<LogNote>();

        public ObjectContext(GameObject gameObject)
        {
            Root = gameObject;
            Animator = gameObject.GetComponent<Animator>();
#if VRC_SDK_VRCSDK3
            Descriptor = gameObject.GetComponent<VRCAvatarDescriptor>();
#endif
        }
    }
}