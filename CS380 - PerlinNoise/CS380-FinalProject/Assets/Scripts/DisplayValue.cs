using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DisplayValue : MonoBehaviour
{
    
    public string label;
    [SerializeField]
    private TMP_Text _title;
    [SerializeField]
    private Slider _slider;
    public void UpdateValue()
    {
        _title.text = label + _slider.value;
    }
}
