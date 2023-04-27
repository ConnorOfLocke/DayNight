using System;
using System.Collections;
using UnityEngine;
using Utils;

namespace DayNight
{
    public class TimeManager : Singleton<TimeManager>
    {
        public const int Real_Seconds_Per_Day = 86400;

        [Header("References")]
        [SerializeField]
        private TimeConfig timeConfig = null;

        //Usually these would be made ReadOnly with a cool plugin called NaughtyAttributes
        [Header("Debug")]
        [SerializeField, ReadOnly]
        private bool isPaused = false;
        [SerializeField, ReadOnly]
        private float timeElapsed = 0.0f;
        [SerializeField, ReadOnly]
        private float dayRatio = 0.0f;

        public float DayNightTransitionTime => timeConfig.DayNightTransitionTime;
        public float DayNightTransitionTimeDayRatio => GetRayRatioFromSimulatedSeconds(DayNightTransitionTime, timeConfig);
        public float SunriseTime => timeConfig.SunriseTime;
        public float SunsetTime => timeConfig.SunsetTime;
        public float TimeElapsed => timeElapsed;
        public float DaysElapsed => timeElapsed / timeConfig.SecondsPerDay;
        public float DayRatio => (timeElapsed % timeConfig.SecondsPerDay) / timeConfig.SecondsPerDay;
        public bool IsDayTime => DayRatio > SunriseTime + 0.5f * DayNightTransitionTimeDayRatio &&
                                    DayRatio < SunsetTime + 0.5f * DayNightTransitionTimeDayRatio;
        public bool IsPaused => isPaused;

        public delegate void TimeManagerEvent();
        public static event TimeManagerEvent OnTick;

        private DateTime dateTimeElapsed = new DateTime();
        private float timeBetweenTicks = 0.0f;
        private float cachedTimescale = 1f;

        protected override void Initialise()
        {
            InitTime();
        }

        private void Update()
        {
            if (!isPaused)
            {
                float timeAdded = Time.deltaTime * timeConfig.TimeScale;
                timeElapsed += timeAdded;
                timeBetweenTicks += timeAdded;
                dateTimeElapsed = dateTimeElapsed.AddSeconds(SimulatedSecondsToRealSeconds(timeAdded, timeConfig));

                if (timeBetweenTicks >= timeConfig.TimeBetweenTicks)
                {
                    OnTick?.Invoke();
                }
            }

            dayRatio = DayRatio;
        }

        public void RestartTime()
        {
            InitTime();
        }

        private void InitTime()
        {
            timeElapsed = timeConfig.StartTimeOffset;

            dateTimeElapsed = new DateTime();
            dateTimeElapsed = dateTimeElapsed.AddSeconds(SimulatedSecondsToRealSeconds(timeConfig.StartTimeOffset, timeConfig));
            cachedTimescale = timeConfig.TimeScale;
        }

        public void SetPausedState(bool isPaused)
        {
            if (!this.isPaused)
            {
                if (Time.timeScale != 0)
                {
                    cachedTimescale = Time.timeScale;
                }
            }

            this.isPaused = isPaused;

            Time.timeScale = isPaused ? 0 : cachedTimescale;
        }

        public Sprite GetDayNightSprite()
        {
            return IsDayTime ? timeConfig.DayIcon : timeConfig.NightIcon;
        }

        public Sprite GetCurrentSeasonSprite()
        {
            return timeConfig.GetSeasonIcon(GetCurrentSeason());
        }

        public float GetSeasonRatio()
        {
            float days = DaysElapsed % (timeConfig.DaysPerSeason * 4);
            return days / (timeConfig.DaysPerSeason * 4);
        }

        public SeasonType GetCurrentSeason()
        {
            int days = Mathf.FloorToInt(DaysElapsed);
            switch ((days / timeConfig.DaysPerSeason) % 4)
            {
                case 0:
                    return SeasonType.Summer;
                case 1:
                    return SeasonType.Autumn;
                case 2:
                    return SeasonType.Winter;
                case 3:
                    return SeasonType.Spring;
            };

            return SeasonType.None;
        }

        public string GetElapsedTimeString()
        {
            return dateTimeElapsed.ToString("HH:mm");
        }

        public static float GetRayRatioFromSimulatedSeconds(float simSeconds, TimeConfig config)
        {
            return simSeconds / config.SecondsPerDay;
        }

        private static float SimulatedSecondsToRealSeconds(float simSeconds, TimeConfig config)
        {
            return simSeconds / config.SecondsPerDay * Real_Seconds_Per_Day;
        }
    }

    public enum SeasonType
    {
        None,
        Summer,
        Autumn,
        Winter,
        Spring
    }
}
