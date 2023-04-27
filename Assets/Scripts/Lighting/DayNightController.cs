using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace DayNight
{
    public abstract class DayNightController<T> : BaseDayNightController
    {
        #region Fields

        [Header("Blended Objects")]
        [SerializeField]
        protected T day;
        [SerializeField]
        protected T night;
        [SerializeField]
        protected T sunrise;
        [SerializeField]
        protected T sunset;

        #endregion

        protected void GetSunriseSunsetInfo(float value, out float outValue, out T from, out T to)
        {
            if (!TryGetTransitionTimes(out float normalisedtransitionTime, out float sunriseValue, out float sunsetValue))
            {
                from = to = default(T);
                outValue = 0.0f;
                return;
            }

            //Sun rise
            if (value >= sunriseValue && value <= sunriseValue + normalisedtransitionTime)
            {
                //night -> sunrise
                if (value < sunriseValue + 0.5f * normalisedtransitionTime)
                {
                    from = night;
                    to = sunrise;
                    outValue = Mathf.InverseLerp(sunriseValue, sunriseValue + 0.5f * normalisedtransitionTime, value);
                }
                //sunrise -> day
                else
                {
                    from = sunrise;
                    to = day;
                    outValue = Mathf.InverseLerp(sunriseValue + 0.5f * normalisedtransitionTime, sunriseValue + normalisedtransitionTime, value);
                }
            }
            //sun set
            else if (value >= sunsetValue && value <= sunsetValue + normalisedtransitionTime)
            {
                //day -> sun set
                if (value < sunsetValue + 0.5f * normalisedtransitionTime)
                {
                    from = day;
                    to = sunset;
                    outValue = Mathf.InverseLerp(sunsetValue, sunsetValue + 0.5f * normalisedtransitionTime, value);
                }
                //sunset -> night
                else
                {
                    from = sunset;
                    to = night;
                    outValue = Mathf.InverseLerp(sunsetValue + 0.5f * normalisedtransitionTime, sunsetValue + normalisedtransitionTime, value);
                }
            }
            //night
            else if (value < sunriseValue || value > sunsetValue + normalisedtransitionTime)
            {
                from = night;
                to = night;
                outValue = 0.0f;
            }
            //day
            else// if (value > sunriseValue + normalisedtransitionTime || value < sunsetValue)
            {
                from = day;
                to = day;
                outValue = 0.0f;
            }
        }
    }

    public abstract class DayNightController<T1, T2> : DayNightController<T1>
    {
        #region Fields

        [SerializeField]
        protected T2 dayData;
        [SerializeField]
        protected T2 nightData;
        [SerializeField]
        protected T2 sunriseData;
        [SerializeField]
        protected T2 sunsetData;

        #endregion

        private T2 MatchObjectToData(T1 blend)
        {
            if (blend == null)
            {
                return default(T2);
            }

            if (EqualityComparer<T1>.Default.Equals(blend, day)) return dayData;
            if (EqualityComparer<T1>.Default.Equals(blend, night)) return nightData;
            if (EqualityComparer<T1>.Default.Equals(blend, sunrise)) return sunriseData;
            if (EqualityComparer<T1>.Default.Equals(blend, sunset)) return sunsetData;

            return default(T2);
        }

        protected void GetSunriseSunsetInfo(float value, out float outValue, out T1 from, out T2 fromData, out T1 to, out T2 toData)
        {
            GetSunriseSunsetInfo(value, out outValue, out from, out to);
            fromData = MatchObjectToData(from);
            toData = MatchObjectToData(to);
        }
    }
}
