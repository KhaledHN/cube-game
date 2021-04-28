using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace LlamaSoftware.Utilities
{
    [RequireComponent(typeof(Light))]
    public class LightLOD : MonoBehaviour
    {
        private new Light light;
        [SerializeField]
        [Tooltip("For static (uncontrolled by script) lights, this should be true. If you have interactable lights, you should also adjust this with light.enabed")]
        public bool LightShouldBeOn = true;
        [SerializeField]
        [Tooltip("The lower you set this, the faster the light will respond to player locations, and the higher the CPU usage")]
        [Range(0, 1f)]
        private float UpdateDelay = 0.1f;
        //Replace this with your "Player" object
        private List<LightLODCamera> LODCameras;
        [SerializeField]
        public List<LODAdjustment> ShadowQualityLods;

        [SerializeField]
        [Tooltip("For Debugging - If you check this, the light color will be changed to the debug color defined on each LOD quality")]
        private bool ShowLightColorAsDebugColor;
        [SerializeField]
        [Tooltip("For Debugging - displays how far player is from the light source")]
        private float DistanceFromPlayer;
        [SerializeField]
        [Tooltip("For Debugging - displays if the Light's Shadow Resolution is clamped to Quality Settings")]
        private bool IsClamped;
        [SerializeField]
        private int LOD;
        private Color CurrentDebugColor;
        private LightShadows DesiredLightShadowQuality;
        private LightShadowResolution InspectorShadowResolution;
        private bool InitiallyOn;

        private LightLODCamera FirstActiveCamera;
        private int index;
        private string WarningMessage = "Shadow Resolution is clamped to: {0}, but no Light LOD step matches this quality!";

        private void Start()
        {
            light = GetComponent<Light>();
            DesiredLightShadowQuality = light.shadows;
            InspectorShadowResolution = light.shadowResolution;
            InitiallyOn = light.enabled;

            //Replace this with your "Player" object
            LODCameras = new List<LightLODCamera>(GameObject.FindObjectsOfType<LightLODCamera>());

            StartCoroutine("AdjustLODQuality");
        }

#if UNITY_EDITOR
        private void Update()
        {
            FirstActiveCamera = FindFirstActiveCamera();
            if (FirstActiveCamera != null)
            {
                CurrentDebugColor = ShadowQualityLods[LOD].DebugColor;

                Debug.DrawLine(transform.position, LODCameras[index].transform.position, CurrentDebugColor);
            }
        }
#endif

        IEnumerator AdjustLODQuality()
        {
            float delay = UpdateDelay + UnityEngine.Random.value / 20f; //this randomization is to prevent all lights updating at the same time causing frame spikes
            int i = 0;
            int DesiredQuality;
            LODAdjustment ClampedLOD;
            WaitForSeconds Wait = new WaitForSeconds(delay);

            while (true)
            {
                FirstActiveCamera = FindFirstActiveCamera();

                if (FirstActiveCamera != null) // If first active camera is null, no cameras with LightLODCamera are active, so we will default to on, unless it should be on
                {
                    if (LightShouldBeOn)
                    {
                        DistanceFromPlayer = Vector3.Distance(transform.position, FirstActiveCamera.transform.position);
                        for (i = 0; i < ShadowQualityLods.Count; i++)
                        {
                            if ((DistanceFromPlayer > ShadowQualityLods[i].DistanceRange.x && DistanceFromPlayer <= ShadowQualityLods[i].DistanceRange.y) || i == ShadowQualityLods.Count - 1)
                            {
                                LOD = i;
                                if (ShadowQualityLods[i].CastNoShadows)
                                {
                                    light.shadows = LightShadows.None;
                                    if (ShowLightColorAsDebugColor)
                                    {
                                        light.color = ShadowQualityLods[i].DebugColor;
                                    }
                                }
                                else
                                {
                                    light.shadows = DesiredLightShadowQuality;
                                    light.enabled = true;
                                    //respect quality settings, do not go higher than what they have defined.
                                    if (QualitySettings.shadowResolution <= ShadowQualityLods[i].ShadowResolution)
                                    {
                                        IsClamped = true;

                                        DesiredQuality = (int)QualitySettings.shadowResolution;
                                        light.shadowResolution = (LightShadowResolution)DesiredQuality;

                                        if (ShowLightColorAsDebugColor)
                                        {
                                            ClampedLOD = FindMatchingShadowQualityIndex(QualitySettings.shadowResolution);
                                            if (ClampedLOD == null)
                                            {
                                                Debug.LogWarning(string.Format(WarningMessage, QualitySettings.shadowResolution.ToString()));
                                            }
                                            else
                                            {
                                                light.color = ClampedLOD.DebugColor;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        IsClamped = false;

                                        light.shadowResolution = (LightShadowResolution)ShadowQualityLods[i].ShadowResolution;

                                        if (ShowLightColorAsDebugColor)
                                        {
                                            light.color = ShadowQualityLods[i].DebugColor;
                                        }
                                    }
                                }

                                break;
                            }
                        }
                    }
                    else
                    {
                        light.enabled = false;
                        LOD = 0;
                    }
                }
                else if (InitiallyOn)
                {
                    light.enabled = true;
                    light.shadows = DesiredLightShadowQuality;
                    light.shadowResolution = InspectorShadowResolution;
                    LOD = 0;
                }

                yield return Wait;
            }
        }

        private LightLODCamera FindFirstActiveCamera()
        {
            for (index = 0; index < LODCameras.Count; index++)
            {
                if (LODCameras[index].Camera.enabled && LODCameras[index].gameObject.activeInHierarchy)
                {
                    return LODCameras[index];
                }
            }

            return null;
        }

        private LODAdjustment FindMatchingShadowQualityIndex(ShadowResolution Quality)
        {
            for (index = 0; index < ShadowQualityLods.Count; index++)
            {
                if (ShadowQualityLods[index].ShadowResolution.Equals(Quality))
                {
                    return ShadowQualityLods[index];
                }
            }

            return null;
        }

        [Serializable]
        public class LODAdjustment
        {
            public Vector2 DistanceRange;
            public ShadowResolution ShadowResolution;
            public bool CastNoShadows;
            public Color DebugColor;
        }
    }
}
