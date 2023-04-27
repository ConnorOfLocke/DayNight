using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace CRAB.Art
{
    public class LightingVolumeController : LightingController
    {
        [Header("References")]
        [SerializeField]
        private List<Volume> volumes = new List<Volume>();

        [Header("Debug")]
        [SerializeField]
        private int lastFromIndex = 0;
        [SerializeField]
        private int lastToIndex = 0;
        [SerializeField]
        private float lastBlendValue = 0.0f;
        [SerializeField]
        private float subBlendValue = 0.0f;

        public void Awake()
        {
            for (int i = 0; i < volumes.Count; i++)
            {
                volumes[i].weight = 0;
            }
        }

        public override void SetBlendValue(float blendValue)
        {
            float localBlendValue = 0.0f;
            int fromIndex = 0;
            int toIndex = 0;

            GetSunriseSunsetInfo(blendValue, out localBlendValue, out fromIndex, out toIndex);

            BlendVolumes(localBlendValue, fromIndex, toIndex);

            lastFromIndex = fromIndex;
            lastToIndex = toIndex;
            lastBlendValue = blendValue;
            subBlendValue = localBlendValue;
        }

        private void BlendVolumes(float blendValue, int fromIndex, int toIndex)
        {
            for (int i = 0; i < volumes.Count; i++)
            {
                if (volumes[i] != null)
                {
                    volumes[i].weight = 0;
                }
            }

            Volume fromVolume = fromIndex >= 0 && fromIndex < volumes.Count ? volumes[fromIndex] : null;
            Volume toVolume = toIndex >= 0 && toIndex < volumes.Count ? volumes[toIndex] : null;

            if (fromVolume != toVolume)
            {
                if (fromVolume != null)
                {
                    fromVolume.weight = 1.0f - blendValue;
                }

                if (toVolume != null)
                {
                    toVolume.weight = blendValue;
                }
            }
            else
            {
                if (fromVolume != null)
                {
                    fromVolume.weight = 1.0f;
                }
            }
        }
    }
}
