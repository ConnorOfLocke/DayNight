using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DayNight
{
    [CreateAssetMenu(menuName = "Config/" + nameof(TimeConfig))]
    public class TimeConfig : ScriptableObject
    {
        [Header("Settings")]
        [SerializeField]
        public float TimeScale = 1.0f;
        [SerializeField]
        public float TimeBetweenTicks = 0.25f;
        [SerializeField]
        public float SecondsPerDay = 300.0f;
        [SerializeField]
        public int DaysPerSeason = 10;
        [SerializeField]
        public float StartTimeOffset = 100.0f;

        [Header("Day/Night")]
        [SerializeField]
        public float DayNightTransitionTime = 30.0f;
        [SerializeField, Range(0, 1)]
        public float SunriseTime = 0.25f;
        [SerializeField, Range(0, 1)]
        public float SunsetTime = 0.75f;

        [Header("Daily Icon")]
        [SerializeField]
        public Sprite DayIcon = null;
        [SerializeField]
        public Sprite NightIcon = null;

        [Header("Season Icons")]
        [SerializeField]
        public Sprite SummerIcon = null;
        [SerializeField]
        public Sprite AutumnIcon = null;
        [SerializeField]
        public Sprite WinterIcon = null;
        [SerializeField]
        public Sprite SpringIcon = null;

        public Sprite GetSeasonIcon(SeasonType seasonType)
        {
            switch (seasonType)
            {
                case SeasonType.Summer:
                    return SummerIcon;
                case SeasonType.Autumn:
                    return AutumnIcon;
                case SeasonType.Winter:
                    return WinterIcon;
                case SeasonType.Spring:
                    return SpringIcon;
            }
            return null;
        }

    }
}
