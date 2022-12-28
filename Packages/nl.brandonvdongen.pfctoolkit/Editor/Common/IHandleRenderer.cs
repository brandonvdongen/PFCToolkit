using UnityEngine;

namespace PFCToolkit.Base {

    public interface IHandleRenderer {
        Vector3 start { get; }
        Vector3 end { get; }
        Color color { get; set; }


    }
    public class HandleVector : IHandleRenderer {
        public Vector3 startPos;
        public Vector3 endPos;

        public Vector3 start => startPos;
        public Vector3 end => endPos;
        public Color color { get; set; }


    }
    public class HandleTransform : IHandleRenderer {
        public Transform startTransform;
        public Transform endTransform;

        public Vector3 start => startTransform.position;
        public Vector3 end => endTransform.position;
        public Color color { get; set; }
    }

    public class HandleTransformRay : IHandleRenderer {

        public Transform startTransform;
        public Vector3 Direction;
        public float Length = 0.05f;

        public Vector3 start => startTransform.position;
        public Vector3 end => startTransform.position + ((startTransform.rotation * Direction).normalized * Length);
        public Color color { get; set; }
    }
}