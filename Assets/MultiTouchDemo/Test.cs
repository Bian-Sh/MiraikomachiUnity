using Homebrew;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [Foldout("组件",true)]
    [SerializeField] InputField InputField;
    [SerializeField] Slider slider;
    [SerializeField] Text text;
    [Foldout("预设")]
    [SerializeField] float preset =0.2f;
    [Foldout("事件")]
    [SerializeField] OnValueChanged OnSliderValueChanged = new OnValueChanged();
    [Serializable] public class OnValueChanged : UnityEvent<float> { }

    private float sliderValue = 0;
    void Start()
    {
        InputField.onEndEdit.AddListener(v =>
        {
            SendMessage(sliderValue);
        });

        slider.onValueChanged.AddListener(v =>
        {
            SendMessage(v);
        });
        slider.value = preset;
    }

    void SendMessage(float value)
    {
        sliderValue = value;
        float factor = 1;
        if (!string.IsNullOrEmpty(InputField.text))
        {
            float.TryParse(InputField.text, out factor);
        }
        text.text = (factor * value).ToString();
        OnSliderValueChanged.Invoke(factor * value);
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.isPlaying&&Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
