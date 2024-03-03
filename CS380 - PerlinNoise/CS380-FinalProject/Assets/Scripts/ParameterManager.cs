
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ParameterManager : MonoBehaviour
{
    [SerializeField]
    Slider _noisePeriodSlider;
    [SerializeField]
    Slider _seaLevelSlider;
    [SerializeField]
    TMP_InputField _seedInput;

    public float GetNoisePeriod() { return _noisePeriodSlider.value; }
    public float GetSeaLevel() { return (float)_seaLevelSlider.value / _seaLevelSlider.maxValue; }
    public int GetSeed()
    {
        if (_seedInput.text.Length == 0)
        {
            TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
            return (int)(t.TotalMilliseconds % int.MaxValue);
        }
        return int.Parse(_seedInput.text);
    }
    
}
