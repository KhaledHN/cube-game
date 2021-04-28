using UnityEngine;

namespace LlamaSoftware.Utilities
{
    [RequireComponent(typeof(Camera))]
    public class LightLODCamera : MonoBehaviour
    {
        [HideInInspector]
        public Camera Camera;

        private void Awake()
        {
            Camera = GetComponent<Camera>();
        }
    }
}
