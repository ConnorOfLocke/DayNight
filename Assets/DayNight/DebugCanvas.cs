using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace CRAB.DayNight
{
    public class DebugCanvas : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI timeText = null;
        [SerializeField]
        private TextMeshProUGUI dayText = null;
        [SerializeField]
        private Image dayNightImage = null;
        [SerializeField]
        private Image seasonImage = null;

        private TimeManager _timeManager = null;
        private TimeManager TimeManager
        {
            get
            {
                if (_timeManager == null)
                {
                    _timeManager = TimeManager.Instance;
                }
                return _timeManager;
            }
        }

        private void Update()
        {
            timeText.text = $"{TimeManager.GetElapsedTimeString()}";
            dayText.text = $"Day {Mathf.Floor(TimeManager.DaysElapsed)}";

            dayNightImage.sprite = TimeManager.GetDayNightSprite();
            seasonImage.sprite = TimeManager.GetCurrentSeasonSprite();
        }
    }
}
