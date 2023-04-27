using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using CRAB.DayNight;

namespace CRAB.Art
{
    public class DirectionalLightingController : LightingController
    {
        [Header("References")]
        [SerializeField]
        private Light mainLight = null;

        [SerializeField]
        private List<Light> refLights = new List<Light>();

        [Header("General Values")]
        [SerializeField]
        private bool blendEnabled = true;

        [Header("Transforms")]
        [SerializeField]
        private bool blendRotation = true;

        [Header("Shape")]
        [SerializeField]
        private bool blendAngularDiameter = true;

        [SerializeField]
        private CelestialBodyBlendSettings celestialBodyBlendSettings = null;
        [SerializeField]
        private EmmisionBlendSettings emmisionBlendSettings = null;
        [SerializeField]
        private ShadowBlendSettings shadowBlendSettings = null;

        [Header("Debug")]
        [SerializeField]
        private int lastFromIndex = 0;
        [SerializeField]
        private int lastToIndex = 0;
        [SerializeField]
        private float localBlendValue = 0.0f;

        private HDAdditionalLightData mainLightData = null;
        private List<HDAdditionalLightData> refLightData = null;

        public void Load()
        {
            Initialise();
        }

        private void Initialise()
        {
            if (refLightData == null)
            {
                refLightData = new List<HDAdditionalLightData>();
                foreach (Light light in refLights)
                {
                    refLightData.Add(light.GetComponent<HDAdditionalLightData>());
                }
            }
            if (mainLightData == null)
            {
                mainLightData = mainLight.GetComponent<HDAdditionalLightData>();
            }
        }


        public override void SetBlendValue(float blendValue)
        {
            Initialise();

            GetSunriseSunsetInfo(blendValue, out float localBlendValue, out int fromIndex, out int toIndex);

            if (blendEnabled)
            {
                //Blend Rotation
                if (blendRotation)
                {
                    mainLight.transform.rotation = Quaternion.Slerp(refLights[fromIndex].transform.rotation.normalized, refLights[toIndex].transform.rotation.normalized, localBlendValue);
                }
                //Shape
                if (blendAngularDiameter)
                {
                    mainLightData.angularDiameter = Mathf.Lerp(refLightData[fromIndex].angularDiameter, refLightData[toIndex].angularDiameter, localBlendValue);
                }

                celestialBodyBlendSettings.Blend(mainLightData, refLightData[fromIndex], refLightData[toIndex], localBlendValue);
                emmisionBlendSettings.Blend(mainLight, mainLightData, refLights[fromIndex], refLightData[fromIndex], refLights[toIndex], refLightData[toIndex], localBlendValue);
                shadowBlendSettings.Blend(mainLightData, refLightData[fromIndex], refLightData[toIndex], localBlendValue);

                mainLightData.UpdateAllLightValues();
            }

            lastFromIndex = fromIndex;
            lastToIndex = toIndex;
            this.localBlendValue = localBlendValue;
        }

        [Serializable]
        public class CelestialBodyBlendSettings
        {
            [SerializeField]
            private bool blendFlareSize = true;
            [SerializeField]
            private bool blendFlareFalloff = true;
            [SerializeField]
            private bool blendFlareTint = true;
            [SerializeField]
            private bool blendSurfaceTint = true;
            [SerializeField]
            private bool blendDistance = true;

            public void Blend(HDAdditionalLightData mainLightData, HDAdditionalLightData fromData, HDAdditionalLightData toData, float blendValue)
            {
                if (blendFlareSize)
                {
                    mainLightData.flareSize = Mathf.Lerp(fromData.flareSize, toData.flareSize, blendValue);
                }
                if (blendFlareFalloff)
                {
                    mainLightData.flareFalloff = Mathf.Lerp(fromData.flareFalloff, toData.flareFalloff, blendValue);
                }
                if (blendFlareTint)
                {
                    mainLightData.flareTint = Color.Lerp(fromData.flareTint, toData.flareTint, blendValue);
                }
                if (blendSurfaceTint)
                {
                    mainLightData.surfaceTint = Color.Lerp(fromData.surfaceTint, toData.surfaceTint, blendValue);
                }
                if (blendDistance)
                {
                    mainLightData.distance = Mathf.Lerp(fromData.distance, toData.distance, blendValue);
                }
            }
        }

        [Serializable]
        public class EmmisionBlendSettings
        {
            [SerializeField]
            private bool blendColor = true;
            [SerializeField]
            private bool blendColorTemperature = true;
            [SerializeField]
            private bool blendIntensity = true;
            [SerializeField]
            private bool blendIndirectMultiplier = true;

            public void Blend(Light mainLight, HDAdditionalLightData mainLightData, Light fromLight, HDAdditionalLightData fromData, Light toLight, HDAdditionalLightData toData, float blendValue)
            {
                if (blendColor)
                {
                    mainLightData.color = Color.Lerp(fromData.color, toData.color, blendValue);
                }
                if (blendColorTemperature)
                {
                    mainLight.colorTemperature = Mathf.Round(Mathf.Lerp(fromLight.colorTemperature, toLight.colorTemperature, blendValue));
                }
                if (blendIntensity)
                {
                    mainLightData.intensity = Mathf.Round(Mathf.Lerp(fromLight.intensity, toLight.intensity, blendValue));
                }
                if (blendIndirectMultiplier)
                {
                    //Shows as Indirect Multiplier
                    mainLight.bounceIntensity = Mathf.Lerp(fromLight.bounceIntensity, toLight.bounceIntensity, blendValue);
                }
            }
        }

        [Serializable]
        public class ShadowBlendSettings
        {
            [SerializeField]
            private bool blendSlopeBias = true;
            [SerializeField]
            private bool blendNormalBias = true;
            [SerializeField]
            private bool blendShadowDimmer = true;
            [SerializeField]
            private bool blendShadowTint = true;
            [SerializeField]
            private bool blendBlockerSampleCount = true;
            [SerializeField]
            private bool blendFilterSampleCount = true;
            [SerializeField]
            private bool blendMinimumBlurIntensity = true;

            public void Blend(HDAdditionalLightData mainLightData, HDAdditionalLightData fromData, HDAdditionalLightData toData, float blendValue)
            {
                if (blendSlopeBias)
                {
                    mainLightData.slopeBias = Mathf.Lerp(fromData.slopeBias, toData.slopeBias, blendValue);
                }
                if (blendNormalBias)
                {
                    mainLightData.normalBias = Mathf.Lerp(fromData.normalBias, toData.normalBias, blendValue);
                }
                if (blendShadowDimmer)
                {
                    mainLightData.shadowDimmer = Mathf.Lerp(fromData.shadowDimmer, toData.shadowDimmer, blendValue);
                }
                if (blendShadowTint)
                {
                    mainLightData.shadowTint = Color.Lerp(fromData.shadowTint, toData.shadowTint, blendValue);
                }
                if (blendBlockerSampleCount)
                {
                    mainLightData.blockerSampleCount = Mathf.RoundToInt(Mathf.Lerp(fromData.blockerSampleCount, toData.blockerSampleCount, blendValue));
                }
                if (blendFilterSampleCount)
                {
                    mainLightData.filterSampleCount = Mathf.RoundToInt(Mathf.Lerp(fromData.filterSampleCount, toData.filterSampleCount, blendValue));
                }
                if (blendMinimumBlurIntensity)
                {
                    //Shows as Minimum blur intensity
                    mainLightData.minFilterSize = Mathf.Lerp(fromData.minFilterSize, toData.minFilterSize, blendValue);
                }
            }
        }
    }
}
