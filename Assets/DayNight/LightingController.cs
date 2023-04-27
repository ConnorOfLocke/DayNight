using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CRAB.DayNight;

namespace CRAB.Art
{
    public abstract class LightingController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        protected float blendOffset = 0.0f;
        [SerializeField]
        protected int nightIndex = 0;
        [SerializeField]
        protected int sunriseIndex = 0;
        [SerializeField]
        protected int dayIndex = 0;
        [SerializeField]
        protected int sunsetIndex = 0;

        [SerializeField]
        private bool debugBlending = false;
        [SerializeField, Range(0, 1)]
        private float manualBlendValue = 0;

        private TimeConfig debugTimeConfig = null;

        protected void OnEnable()
        {
            TimeManager.OnTick += HandleOnTick;
        }

        protected void OnDisable()
        {
            TimeManager.OnTick -= HandleOnTick;
        }

        private void HandleOnTick()
        {
            float value = (TimeManager.Instance.DayRatio + blendOffset) % 1.0f;
            value = (value < 0) ? 1.0f - value : value;

            SetBlendValue(value);
        }

        protected void OnValidate()
        {
#if UNITY_EDITOR
            if (debugBlending)
            {
                Debug.Log($"Blending {gameObject.name} to {manualBlendValue}");
                SetBlendValue(manualBlendValue);
            }
#endif
        }


        public abstract void SetBlendValue(float blendValue);

        protected void GetSunriseSunsetInfo(float value, out float outValue, out int fromIndex, out int toIndex)
        {
            float normalisedtransitionTime, sunriseValue, sunsetValue;
            if (debugBlending)
            {
                if (debugTimeConfig == null)
                {
                    debugTimeConfig = Resources.Load<TimeConfig>("Configs/TimeConfig");
                }

                if (debugTimeConfig == null)
                {
                    Debug.LogWarning("Could not find the TimeConfig under resources Configs/TimeConfig");
                    fromIndex = toIndex = 0;
                    outValue = 0.0f;
                    return;
                }

                normalisedtransitionTime = TimeManager.GetRayRatioFromSimulatedSeconds(debugTimeConfig.DayNightTransitionTime, debugTimeConfig);
                sunriseValue = debugTimeConfig.SunriseTime;
                sunsetValue = debugTimeConfig.SunsetTime;

            }
            else
            {
                normalisedtransitionTime = TimeManager.Instance.DayNightTransitionTimeDayRatio;
                sunriseValue = TimeManager.Instance.SunriseTime;
                sunsetValue = TimeManager.Instance.SunsetTime;
            }

            //Sun rise
            if (value >= sunriseValue && value <= sunriseValue + normalisedtransitionTime)
            {
                //night -> sunrise
                if (value < sunriseValue + 0.5f * normalisedtransitionTime)
                {
                    fromIndex = nightIndex;
                    toIndex = sunriseIndex;
                    outValue = Mathf.InverseLerp(sunriseValue, sunriseValue + 0.5f * normalisedtransitionTime, value);
                }
                //sunrise -> day
                else
                {
                    fromIndex = sunriseIndex;
                    toIndex = dayIndex;
                    outValue = Mathf.InverseLerp(sunriseValue + 0.5f * normalisedtransitionTime, sunriseValue + normalisedtransitionTime, value);
                }
            }
            //sun set
            else if (value >= sunsetValue && value <= sunsetValue + normalisedtransitionTime)
            {
                //day -> sun set
                if (value < sunsetValue + 0.5f * normalisedtransitionTime)
                {
                    fromIndex = dayIndex;
                    toIndex = sunsetIndex;
                    outValue = Mathf.InverseLerp(sunsetValue, sunsetValue + 0.5f * normalisedtransitionTime, value);
                }
                //sunset -> night
                else
                {
                    fromIndex = sunsetIndex;
                    toIndex = nightIndex;
                    outValue = Mathf.InverseLerp(sunsetValue + 0.5f * normalisedtransitionTime, sunsetValue + normalisedtransitionTime, value);
                }
            }
            //night
            else if (value < sunriseValue || value > sunsetValue + normalisedtransitionTime)
            {
                fromIndex = nightIndex;
                toIndex = nightIndex;
                outValue = 0.0f;
            }
            //day
            else// if (value > sunriseValue + normalisedtransitionTime || value < sunsetValue)
            {
                fromIndex = dayIndex;
                toIndex = dayIndex;
                outValue = 0.0f;
            }
        }
    }
}
