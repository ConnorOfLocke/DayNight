using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using Utils;

namespace DayNight.Lighting
{
    public class LightingVolumeController : DayNightController<Volume>
    {
        #region Fields

        [Header("Debug")]
        [SerializeField, ReadOnly]
        private float lastBlendValue = 0.0f;
        [SerializeField, ReadOnly]
        private float subBlendValue = 0.0f;

        #endregion

        public void Awake()
        {
            ResetVolumeWeight();
        }

        public override void SetBlendValue(float blendValue)
        {
            GetSunriseSunsetInfo(blendValue, out float localBlendValue, out Volume fromVolume, out Volume toVolume);

            BlendVolumes(localBlendValue, fromVolume, toVolume);

            lastBlendValue = blendValue;
            subBlendValue = localBlendValue;
        }

        private void ResetVolumeWeight()
        {
            if (day != null) day.weight = 0;
            if (night != null) night.weight = 0;
            if (sunrise != null) sunrise.weight = 0;
            if (sunset != null) sunset.weight = 0;
        }

        private void BlendVolumes(float blendValue, Volume fromVolume, Volume toVolume)
        {
            ResetVolumeWeight();

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
