using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIControl : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI text;
    private string maxHP;
    void Start()
    {
        slider.maxValue = GameManager.Instance.GetMaxHP();
        slider.value = slider.maxValue;
        maxHP = "/" + GameManager.Instance.GetMaxHP();
        text.text = (int) slider.value + maxHP;
    }

    void Update()
    {   
        int newValue = GameManager.Instance.GetHP();
        if (Mathf.Abs(slider.value - newValue) > 0.01f)
        {
            slider.value = Mathf.Lerp(slider.value, newValue, Time.deltaTime * 2f);
            text.text = (int) slider.value + maxHP;
        }
        
    }

}
