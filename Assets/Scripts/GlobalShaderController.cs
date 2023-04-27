using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DayNight.Shaders
{
    [Serializable]
    public class ShaderValue
    {
        #region Fields

        [SerializeField]
        private string propertyName = string.Empty;
        [SerializeField]
        private GlobalShaderValueType propertyType;
        [SerializeField]
        private AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField]
        private List<float> shaderValues = null;
        [SerializeField]
        private List<Color> shaderColors = null;

        #endregion

        public void SetBlendValue(float blendValue, int fromIndex, int toIndex)
        {
            float blendSample = animationCurve.Evaluate(blendValue);

            switch (propertyType)
            {
                case GlobalShaderValueType.Color:
                    Color targetColor = Color.Lerp(shaderColors[fromIndex], shaderColors[toIndex], blendSample);
                    Shader.SetGlobalColor(propertyName, targetColor);
                    break;
                case GlobalShaderValueType.Float:
                    float targetValue = Mathf.Lerp(shaderValues[fromIndex], shaderValues[toIndex], blendSample);
                    Shader.SetGlobalFloat(propertyName, targetValue);
                    break;
            }
        }

        public enum GlobalShaderValueType
        {
            Float,
            Color
        }
    }

    public class GlobalShaderController : DayNightController<int>
    {
        #region Fields

        [Header("Settings")]
        [SerializeField]
        private List<ShaderValue> shaderValues = null;

        #endregion

        public void Load()
        { }

        public override void SetBlendValue(float blendValue)
        {
            GetSunriseSunsetInfo(blendValue, out float localBlendValue, out int fromIndex, out int toIndex);

            if (shaderValues != null)
            {
                for (int i = 0; i < shaderValues.Count; i++)
                {
                    shaderValues[i].SetBlendValue(localBlendValue, fromIndex, toIndex);
                }
            }
        }
    }
}
