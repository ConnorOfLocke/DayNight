using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DayNight
{
    public abstract class BaseDayNightController : MonoBehaviour
    {
        #region Consts

        protected const string Config_Location = "Configs/TimeConfig";
        protected const string Error_Message = "Could not find the TimeConfig under Resources/" + Config_Location;

        #endregion

        #region Fields

        [Header("Settings")]
        [SerializeField]
        protected float blendOffset = 0.0f;

        [Header("Debug")]
        [SerializeField]
        protected bool debugBlending = false;
        [SerializeField, Range(0, 1)]
        protected float manualBlendValue = 0;

        #endregion

        protected TimeConfig debugTimeConfig = null;

        protected void OnEnable()
        {
            TimeManager.OnTick += HandleOnTick;
        }

        protected void OnDisable()
        {
            TimeManager.OnTick -= HandleOnTick;
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

        private void HandleOnTick()
        {
            float value = (TimeManager.Instance.DayRatio + blendOffset) % 1.0f;
            value = (value < 0) ? 1.0f - value : value;

            SetBlendValue(value);
        }

        protected bool TryGetTransitionTimes(out float DayNightTransitionTimeDayRatio, out float sunriseTime, out float sunsetTime)
        {
            if (debugBlending)
            {
                if (debugTimeConfig == null)
                {
                    debugTimeConfig = Resources.Load<TimeConfig>(Config_Location);
                    if (debugTimeConfig == null)
                    {
                        Debug.LogWarning(Error_Message);
                        DayNightTransitionTimeDayRatio = sunriseTime = sunsetTime = 0;
                        return false;
                    }
                }

                DayNightTransitionTimeDayRatio = TimeManager.GetRayRatioFromSimulatedSeconds(debugTimeConfig.DayNightTransitionTime, debugTimeConfig);
                sunriseTime = debugTimeConfig.SunriseTime;
                sunsetTime = debugTimeConfig.SunsetTime;

            }
            else
            {
                DayNightTransitionTimeDayRatio = TimeManager.Instance.DayNightTransitionTimeDayRatio;
                sunriseTime = TimeManager.Instance.SunriseTime;
                sunsetTime = TimeManager.Instance.SunsetTime;
            }

            return true;
        }

        public abstract void SetBlendValue(float blendValue);
    }
}
